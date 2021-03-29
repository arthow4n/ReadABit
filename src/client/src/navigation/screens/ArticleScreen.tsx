import React from 'react';

import { useQuery } from 'react-query';

import { StackScreenProps } from '@react-navigation/stack';
import { api } from '@src/integrations/backend/backend';
import { ArticleStackParamList } from '@src/navigation/navigators/ArticleNavigator.types';
import { ArticleReader } from '@src/shared/components/ArticleReader/ArticleReader';
import { ContentLoading } from '@src/shared/components/Loading';
import { WordFamiliarityContextProvider } from '@src/shared/contexts/WordFamiliarityContext';
import {
  QueryCacheKey,
  queryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

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

  return (
    <WordFamiliarityContextProvider>
      <ArticleReader article={data} />
    </WordFamiliarityContextProvider>
  );
};
