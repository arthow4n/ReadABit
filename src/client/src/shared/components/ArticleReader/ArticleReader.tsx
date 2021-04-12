import React from 'react';

import { useTranslation } from 'react-i18next';

import { round } from 'lodash';
import { Button, Content, Grid, Row, Text } from 'native-base';

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
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => {
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
                      />
                    );
                  })}
                </Text>
              ))}
            </Text>
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
