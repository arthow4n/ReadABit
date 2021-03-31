import React from 'react';

import { Content, Grid, Row, Text } from 'native-base';

import { Backend } from '@src/integrations/backend/types';

import { SelectedTokenDefinitionCard } from './SelectedTokenDefinitionCard';
import { WordToken } from './WordToken';

export const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  // TODO: Save article reading progress.

  return (
    <Grid>
      <Row size={3}>
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => (
                    <WordToken key={token.id} token={token} />
                  ))}
                </Text>
              ))}
            </Text>
          ))}
        </Content>
      </Row>
      <Row size={1}>
        <SelectedTokenDefinitionCard />
      </Row>
    </Grid>
  );
};
