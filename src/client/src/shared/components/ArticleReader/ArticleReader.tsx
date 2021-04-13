import React from 'react';

import { useTranslation } from 'react-i18next';

import { round } from 'lodash';
import { Button, Content, Grid, Icon, Row, Text, View } from 'native-base';

import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';

import { useArticleReaderHandle } from './ArticleReaderRenderContext';
import { RenderToken } from './RenderToken';
import { SelectedTokenDefinitionCard } from './SelectedTokenDefinitionCard';
import { isWord } from './TokenUtils';

export const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  const { t } = useTranslation();
  const { updateWordFamiliarityForTokens } = useArticleReaderHandle();

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
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <View key={paragraph.id}>
              <View>
                {paragraph.sentences.map((sentence) => (
                  <Text key={sentence.id}>
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
                    style={{ marginRight: 8 }}
                    transparent
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
            </View>
          ))}
          <Button onPress={() => markAllNewWordAs(3)}>
            <Text>{t('Mark all new words as known')}</Text>
          </Button>
        </Content>
      </Row>
      <Row size={1}>
        <SelectedTokenDefinitionCard />
      </Row>
    </Grid>
  );
};
