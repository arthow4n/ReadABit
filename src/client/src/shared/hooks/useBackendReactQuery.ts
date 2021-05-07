import {
  MutationFunction,
  QueryClient,
  QueryKey,
  useMutation,
  useQueryClient,
} from 'react-query';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';

export enum QueryCacheKey {
  ArticleCollectionList = 'ArticleCollectionList',
  ArticleCollection = 'ArticleCollection',
  ArticleList = 'ArticleList',
  Article = 'Article',
  WordDefinitionList = 'WordDefinitionList',
  WordDefinition = 'WordDefinition',
  WordFamiliarityList = 'WordFamiliarityList',
  WordFamiliarityDailyGoalCheck = 'WordFamiliarityDailyGoalCheck',
  UserPreferenceData = 'UserPreferenceData',
  CultureInfoListAllSupportedTimeZones = 'CultureInfoListAllSupportedTimeZones',
}

// TODO: Extract language codes and collection IDs to individual cache keys preceding the filter
export function queryCacheKey(
  base: QueryCacheKey.ArticleCollectionList,
  filter: Parameters<Backend.IClient['articleCollections_List']>[0],
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.ArticleCollection,
  id: string,
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.ArticleList,
  filter: Parameters<Backend.IClient['articles_List']>[0],
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.Article,
  id: string,
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.CultureInfoListAllSupportedTimeZones,
): QueryKey;
export function queryCacheKey(base: QueryCacheKey.UserPreferenceData): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.WordDefinitionList,
  filter: Parameters<Backend.IClient['wordDefinitions_List']>[0],
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.WordDefinition,
  filter: Parameters<Backend.IClient['wordDefinitions_Get']>[0],
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.WordFamiliarityList,
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey.WordFamiliarityDailyGoalCheck,
): QueryKey;
export function queryCacheKey(
  base: QueryCacheKey,
  ...args: (object | string)[]
): QueryKey {
  return [base, ...args];
}

const useMutateAndInvalidate = <TData, TVariables>(
  mutateFn: MutationFunction<TData, TVariables>,
  invalidations: Parameters<QueryClient['invalidateQueries']>[0][],
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
      setTimeout(() => {
        invalidations.map((invalidation) =>
          queryClient.invalidateQueries(invalidation),
        );
      }, 0);

      return result;
    },
  };

  return proxied;
};

export const useMutateArticleCollectionsCreate = () => {
  return useMutateAndInvalidate(api().articleCollections_Create, [
    [QueryCacheKey.ArticleCollectionList],
  ]);
};

export const useMutateArticleCreate = () => {
  return useMutateAndInvalidate(api().articles_Create, [
    [QueryCacheKey.ArticleList],
  ]);
};

export const useMutateWordDefninitionCreate = () => {
  return useMutateAndInvalidate(api().wordDefinitions_Create, [
    [QueryCacheKey.WordDefinitionList],
  ]);
};

export const useMutateWordDefninitionUpdate = (
  getRequestParam: Parameters<Backend.IClient['wordDefinitions_List']>[0],
) => {
  return useMutateAndInvalidate(api().wordDefinitions_Update, [
    [QueryCacheKey.WordDefinition, getRequestParam],
  ]);
};

export const useMutateUserPreferenceUpdate = () => {
  return useMutateAndInvalidate(api().userPreferences_Upsert, [
    [QueryCacheKey.UserPreferenceData],
  ]);
};
