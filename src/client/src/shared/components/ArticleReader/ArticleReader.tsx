import React from 'react';

import { useTranslation } from 'react-i18next';
import { findNodeHandle, ScrollView, View as RNView } from 'react-native';

import { last, noop } from 'lodash';
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
    articlePages,
    readingProgressPointingTo,
  } = useArticleReaderHandle();

  const [currentPage, setCurrentPage] = React.useState(
    readingProgressPointingTo.pageIndex,
  );

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

    const start = Date.now();
    const attemptScrolling = () => {
      viewToScrollToOnMountRef.current?.measureLayout(
        scrollViewNodeHandle,
        (left, top, width, height) => {
          // Sometimes the dimension is not available for unknown reason thus the retry.
          if (!left && !top && !width && !height && Date.now() - start < 1000) {
            attemptScrolling();
          }

          scrollViewRef.current?.scrollTo({
            y: top,
          });
        },
        noop,
      );
    };

    attemptScrolling();
  };

  const documentIndex = 0;

  const sentences = articlePages[currentPage];

  return (
    <Grid>
      <Row size={5}>
        <ScrollView
          ref={(ref) => {
            scrollViewRef.current = ref;
            tryScrollToTheTokenToScrollToOnMount();
          }}
        >
          {currentPage > 0 && (
            <Button
              onPress={() => {
                setCurrentPage((p) => p - 1);
                // TODO: Auto scrolling to end
              }}
            >
              <Text>{t('Previous page')}</Text>
            </Button>
          )}
          <View
            style={{
              padding: 4,
            }}
          >
            <View>
              {sentences.map((sentence, sentencesInPageIndex) => (
                <RNView
                  // Using index because the whole article never changes during lifecycle,
                  // besides there were some buggy documents with duplicated paragraph ID.
                  // eslint-disable-next-line react/no-array-index-key
                  key={`${currentPage}_${sentencesInPageIndex}`}
                  ref={
                    sentence.conlluPointer.paragraphIndex ===
                      article.readingProgress.conlluTokenPointer
                        .paragraphIndex &&
                    sentence.conlluPointer.sentenceIndex ===
                      article.readingProgress.conlluTokenPointer.sentenceIndex
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
                      return (
                        <RenderToken
                          // eslint-disable-next-line react/no-array-index-key
                          key={tokenIndex}
                          token={token}
                          documentIndex={documentIndex}
                          paragraphIndex={token.conlluPointer.paragraphIndex}
                          sentenceIndex={token.conlluPointer.sentenceIndex}
                          tokenIndex={tokenIndex}
                          readRatio={token.readRatio}
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
                    const lastSentenceInPage = last(sentences);
                    const lastTokenInPage = last(lastSentenceInPage?.tokens);

                    api().articles_UpsertReadingProgress({
                      id: article.id,
                      request: {
                        conlluTokenPointer: {
                          documentIndex,
                          paragraphIndex:
                            lastTokenInPage?.conlluPointer.paragraphIndex ?? 0,
                          sentenceIndex:
                            lastTokenInPage?.conlluPointer.sentenceIndex ?? 0,
                          tokenIndex: lastSentenceInPage
                            ? lastSentenceInPage.tokens.length &&
                              lastSentenceInPage.tokens.length - 1
                            : 0,
                        },
                        readRatio: lastTokenInPage?.readRatio ?? 0,
                      },
                    });

                    updateWordFamiliarityForTokens(
                      fromLevel,
                      3,
                      sentences.flatMap((s) => s.tokens).filter(isWord),
                    );
                  }}
                >
                  <Icon name={iconName} />
                </Button>
              ))}
            </View>
          </View>
          {currentPage <= articlePages.length - 1 && (
            <Button
              onPress={() => {
                setCurrentPage((p) => p + 1);
                scrollViewRef.current?.scrollTo({ y: 0 });
              }}
            >
              <Text>{t('Next page')}</Text>
            </Button>
          )}
        </ScrollView>
      </Row>
      <Row size={2}>
        <SelectedTokenDefinitionCard />
      </Row>
    </Grid>
  );
};
