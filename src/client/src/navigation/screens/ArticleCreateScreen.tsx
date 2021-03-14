import React from 'react';

import { useTranslation } from 'react-i18next';
import { useMutation, useQuery } from 'react-query';

import { Content, Text } from 'native-base';

import { StackNavigationProp } from '@react-navigation/stack';

import { api } from '../../integrations/backend/backend';
import { useAppSettingsContext } from '../../shared/contexts/AppSettingsContext';
import {
  QueryCacheKey,
  useMutateArticleCollectionsCreate,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

export const ArticleCreateScreen: React.FC<
  StackNavigationProp<ArticleStackParamList, 'Article'>
> = () => {
  const { t } = useTranslation();
  const { appSettings } = useAppSettingsContext();

  const { mutateAsync } = useMutateArticleCollectionsCreate();

  const { isLoading, refetch, data } = useQuery(
    [QueryCacheKey.ArticleCollections],
    () => api.articleCollections_List(),
    {
      onSuccess: async ({ items }) => {
        if (!items.length) {
          await mutateAsync({
            languageCode: appSettings.languageCodes.studying,
            name: t('Quick imports'),
            public: false,
          });

          refetch();
        }
      },
    },
  );

  return (
    <Content>
      <Text>{'// TODO: ArticleCreateScreen '}</Text>
    </Content>
  );
};
