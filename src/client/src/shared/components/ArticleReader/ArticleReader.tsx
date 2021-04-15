import React from 'react';

import { findNodeHandle, ScrollView, View as RNView } from 'react-native';
import { useTranslation } from 'react-i18next';

import { noop, round } from 'lodash';
import { Button, Grid, Icon, Row, Text, View } from 'native-base';

import { api } from '@src/integrations/backend/backend';

import { useArticleReaderHandle } from './ArticleReaderRenderContext';
import { RenderToken } from './RenderToken';
import { SelectedTokenDefinitionCard } from './SelectedTokenDefinitionCard';
import { getSpacesAfter, isWord } from './TokenUtils';

export const ArticleReader: React.FC = () => {
  const { t } = useTranslation();
  const {
    updateWordFamiliarityForTokens,
    ttsSpeak,
    article,
  } = useArticleReaderHandle();

  const scrollViewRef = React.useRef<ScrollView | null>(null);
  const viewToScrollToOnMountRef = React.useRef<RNView | null>(null);

  const tryScrollToTheTokenToScrollToOnMount = () => {
    if (!scrollViewRef.current || !viewToScrollToOnMountRef.current) {
      return;
    }

    const scrollViewNodeHandle = findNodeHandle(scrollViewRef.current);

    if (!scrollViewNodeHandle) {
      return;
    }

    viewToScrollToOnMountRef.current.measureLayout(
      scrollViewNodeHandle,
      (left, top, width, height) => {
        scrollViewRef.current?.scrollTo({
          y: top,
        });
      },
      noop,
    );
  };

  const flattenedTokens = article.conlluDocument.paragraphs
    .flatMap((p) => p.sentences)
    .flatMap((s) => s.tokens);

  const markAllNewWordAs = (level: number) => {
    updateWordFamiliarityForTokens(0, level, flattenedTokens.filter(isWord));
  };

  const documentIndex = 0;
  const allTokensCount = flattenedTokens.length;
  let tokenCounter = 0;

  return (
    <Grid>
      <Row size={2}>
        <ScrollView
          ref={(ref) => {
            scrollViewRef.current = ref;
            tryScrollToTheTokenToScrollToOnMount();
          }}
        >
          {article.conlluDocument.paragraphs.map(
            (paragraph, paragraphIndex) => (
              <View
                key={paragraph.id}
                style={{
                  padding: 4,
                }}
              >
                <View>
                  {paragraph.sentences.map((sentence, sentenceIndex) => (
                    <RNView
                      key={sentence.id}
                      ref={
                        paragraphIndex ===
                          article.readingProgress.conlluTokenPointer
                            .paragraphIndex &&
                        sentenceIndex ===
                          article.readingProgress.conlluTokenPointer
                            .sentenceIndex
                          ? (ref) => {
                              viewToScrollToOnMountRef.current = ref;
                              tryScrollToTheTokenToScrollToOnMount();
                            }
                          : undefined
                      }
                    >
                      <Text>
                        <Button
                          transparent
                          onPress={() => {
                            ttsSpeak(
                              sentence.tokens
                                .map(
                                  (token) =>
                                    `${token.form}${getSpacesAfter(token)}`,
                                )
                                .join(''),
                            );
                          }}
                        >
                          <Icon name="volume-medium-outline" />
                        </Button>
                        {sentence.tokens.map((token, tokenIndex) => {
                          tokenCounter += 1;

                          return (
                            <RenderToken
                              key={token.id}
                              token={token}
                              articleId={article.id}
                              documentIndex={documentIndex}
                              paragraphIndex={paragraphIndex}
                              sentenceIndex={sentenceIndex}
                              tokenIndex={tokenIndex}
                              readRatio={round(
                                tokenCounter / allTokensCount,
                                2,
                              )}
                              isLastTokenInSentence={
                                tokenIndex === sentence.tokens.length - 1
                              }
                            />
                          );
                        })}
                      </Text>
                    </RNView>
                  ))}
                </View>
                <View
                  style={{
                    flexDirection: 'row',
                    marginTop: 4,
                    marginBottom: 8,
                    paddingBottom: 4,
                    borderBottomWidth: 1,
                  }}
                >
                  {[
                    { fromLevel: 0, iconName: 'checkmark-circle-outline' },
                    { fromLevel: 1, iconName: 'checkmark-done-circle-outline' },
                  ].map(({ fromLevel, iconName }) => (
                    <Button
                      key={iconName}
                      transparent
                      style={{ marginRight: 8 }}
                      onPress={() => {
                        const lastSentenceInParagraph = paragraph.sentences.slice(
                          -1,
                        )[0];

                        api().articles_UpsertReadingProgress({
                          id: article.id,
                          request: {
                            conlluTokenPointer: {
                              documentIndex,
                              paragraphIndex,
                              sentenceIndex:
                                paragraph.sentences.length &&
                                paragraph.sentences.length - 1,
                              tokenIndex: lastSentenceInParagraph
                                ? lastSentenceInParagraph.tokens.length &&
                                  lastSentenceInParagraph.tokens.length - 1
                                : 0,
                            },
                            // Taking the counter directly since we've already
                            // increased the counter when rendering tokens.
                            readRatio: round(tokenCounter / allTokensCount, 2),
                          },
                        });

                        updateWordFamiliarityForTokens(
                          fromLevel,
                          3,
                          paragraph.sentences
                            .flatMap((s) => s.tokens)
                            .filter(isWord),
                        );
                      }}
                    >
                      <Icon name={iconName} />
                    </Button>
                  ))}
                </View>
              </View>
            ),
          )}
          <Button onPress={() => markAllNewWordAs(3)}>
            <Text>{t('Mark all new words as known')}</Text>
          </Button>
        </ScrollView>
      </Row>
      <Row size={1}>
        <SelectedTokenDefinitionCard />
      </Row>
    </Grid>
  );
};
