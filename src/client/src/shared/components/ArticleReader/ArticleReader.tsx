import React from 'react';

import { useTranslation } from 'react-i18next';

import { Button, Content, Grid, Row, Text } from 'native-base';

import { Backend } from '@src/integrations/backend/types';

import { useArticleReaderHandle } from './ArticleReaderRenderContext';
import { RenderToken } from './RenderToken';
import { SelectedTokenDefinitionCard } from './SelectedTokenDefinitionCard';
import { isWord } from './TokenUtils';

export const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  // TODO: Save article reading progress.

  const { t } = useTranslation();
  const { updateWordFamiliarityForTokens } = useArticleReaderHandle();

  const markAllNewAs = (level: number) => {
    const flattenedTokens = article.conlluDocument.paragraphs
      .flatMap((p) => p.sentences)
      .flatMap((s) => s.tokens)
      .filter(isWord);
    updateWordFamiliarityForTokens(0, level, flattenedTokens);
  };

  return (
    <Grid>
      <Row size={2}>
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => (
                    <RenderToken key={token.id} token={token} />
                  ))}
                </Text>
              ))}
            </Text>
          ))}
          <Button onPress={() => markAllNewAs(3)}>
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
