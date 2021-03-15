import {
  MutationFunction,
  QueryClient,
  useMutation,
  useQueryClient,
} from 'react-query';

import { api } from '../../integrations/backend/backend';

// TODO: Find a better way to move useQuery calls here.

export enum QueryCacheKey {
  ArticleCollectionList = 'ArticleCollectionList',
  ArticleList = 'ArticleList',
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
