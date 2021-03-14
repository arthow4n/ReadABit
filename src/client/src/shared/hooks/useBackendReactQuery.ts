import {
  MutationFunction,
  QueryClient,
  useMutation,
  useQueryClient,
} from 'react-query';

import { api } from '../../integrations/backend/backend';

// TODO: Find a better way to move useQuery calls here.

export enum QueryCacheKey {
  ArticleCollections = 'ArticleCollections',
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
      invalidations.forEach((invalidation) => {
        queryClient.invalidateQueries(...invalidation);
      });
      return mutationHandle.mutateAsync(...args);
    },
  };

  return proxied;
};

export const useMutateArticleCollectionsCreate = () => {
  return useMutateAndInvalidate(api.articleCollections_Create, [
    [QueryCacheKey.ArticleCollections],
  ]);
};
