import React from 'react';

import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

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
  const { t } = useTranslation();

  // TODO: ArticleReader
  return null;
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
