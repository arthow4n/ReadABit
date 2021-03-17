import {
  MutationFunction,
  QueryClient,
  QueryKey,
  useMutation,
  useQueryClient,
} from 'react-query';

import { api } from '../../integrations/backend/backend';

export enum QueryCacheKey {
  ArticleCollectionList = 'ArticleCollectionList',
  ArticleList = 'ArticleList',
  Article = 'Article',
}

export function queryCacheKey(
  base: QueryCacheKey.ArticleCollectionList,
  filter: {
    filter_LanguageCode: string;
    page_Index?: number | null;
    page_Size?: number | null;
  },
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.Article,
  id: string,
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey,
  ...args: (object | string)[]
): QueryKey {
  return [base, ...args];
}

const useMutateAndInvalidate = <TData, TVariables>(
  mutateFn: MutationFunction<TData, TVariables>,
  invalidations: Parameters<QueryClient['invalidateQueries']>[],
) => {
  const queryClient = useQueryClient();
  const mutationHandle = useMutation(mutateFn);

  const proxied: typeof mutationHandle = {
    ...mutationHandle,
    mutateAsync: async (...args) => {
      const result = await mutationHandle.mutateAsync(...args);

      // TODO: Fix invalidation
      // Invalidation should be run after the mutation
      // because it will trigger refetch in background
      // when the query is currently being rendered.
      // https://react-query.tanstack.com/guides/query-invalidation
      // Promise.all(
      //   invalidations.map((invalidation) =>
      //     queryClient.invalidateQueries(...invalidation),
      //   ),
      // );

      return result;
    },
  };

  return proxied;
};

export const useMutateArticleCollectionsCreate = () => {
  return useMutateAndInvalidate(api.articleCollections_Create, [
    [QueryCacheKey.ArticleCollectionList],
  ]);
};

export const useMutateArticleCreate = () => {
  return useMutateAndInvalidate(api.articles_Create, [
    [QueryCacheKey.ArticleList],
  ]);
};
