import React from 'react';

import { useQuery } from 'react-query';

import { Content, Grid, Row, Text } from 'native-base';

import { StackScreenProps } from '@react-navigation/stack';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { ContentLoading } from '../../shared/components/Loading';
import {
  QueryCacheKey,
  queryCacheKey,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  const onTokenPress = (token: Backend.Token) => {
    // TODO: Show word definition and tags.
  };

  return (
    <Grid>
      <Row size={1}>{/* Render dictionary */}</Row>
      <Row size={3}>
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => (
                    <Text
                      key={token.id}
                      onPress={() => onTokenPress(token)}
                      style={{ fontSize: 28 }}
                    >
                      {token.form}
                    </Text>
                  ))}
                </Text>
              ))}
            </Text>
          ))}
        </Content>
      </Row>
    </Grid>
  );
};

export const ArticleScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'Article'>
> = ({ route }) => {
  const { id } = route.params;

  const { data } = useQuery(queryCacheKey(QueryCacheKey.Article, id), () =>
    api.articles_Get({ id }),
  );

  if (!data) {
    return <ContentLoading />;
  }

  return <ArticleReader article={data} />;
};
