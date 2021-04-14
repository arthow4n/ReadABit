import React from 'react';

import { findNodeHandle, ScrollView, View as RNView } from 'react-native';
import { useTranslation } from 'react-i18next';

import { noop, round } from 'lodash';
import { Button, Content, Grid, Icon, Row, Text, View } from 'native-base';

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

  const documentId = article.conlluDocument.id;
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
          {/* TODO: Flatten the child tree,
           so it's possible to scroll to at least sentence level.
           because `measureLayout` can't work correctly/directly
           with the `Text` in `RenderToken` */}
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <RNView
              key={paragraph.id}
              ref={
                paragraph.id ===
                article.readingProgress.conlluTokenPointer.paragraphId
                  ? (ref) => {
                      viewToScrollToOnMountRef.current = ref;
                      tryScrollToTheTokenToScrollToOnMount();
                    }
                  : undefined
              }
              style={{
                padding: 4,
              }}
            >
              <View>
                {paragraph.sentences.map((sentence) => (
                  <Text key={sentence.id}>
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
                    {sentence.tokens.map((token, index) => {
                      tokenCounter += 1;

                      return (
                        <RenderToken
                          key={token.id}
                          token={token}
                          articleId={article.id}
                          documentId={documentId}
                          paragraphId={paragraph.id}
                          sentenceId={sentence.id}
                          readRatio={round(tokenCounter / allTokensCount, 2)}
                          isLastTokenInSentence={
                            index === sentence.tokens.length - 1
                          }
                        />
                      );
                    })}
                  </Text>
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
                      const lastTokenInParagraph = lastSentenceInParagraph.tokens.slice(
                        -1,
                      )[0];

                      api().articles_UpsertReadingProgress({
                        id: article.id,
                        request: {
                          conlluTokenPointer: {
                            documentId,
                            paragraphId: paragraph.id,
                            sentenceId: lastSentenceInParagraph.id ?? '',
                            tokenId: lastTokenInParagraph.id ?? '',
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
            </RNView>
          ))}
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
