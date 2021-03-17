import React from 'react';

import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import { Content, Text } from 'native-base';

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
  article: Backend.Article;
}> = ({ article }) => {
  const onTokenPress = (token: Backend.Token) => {
    // TODO: Show word definition and tags.
  };

  return (
    <Content>
      {article.conlluDocument.paragraphs.map((paragraph) => (
        <Text key={paragraph.id}>
          {paragraph.sentences.map((sentence) => (
            <Text key={sentence.id}>
              {sentence.tokens.map((token) => (
                <Text key={token.id} onPress={() => onTokenPress(token)}>
                  {token.form}
                </Text>
              ))}
            </Text>
          ))}
        </Text>
      ))}
    </Content>
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
