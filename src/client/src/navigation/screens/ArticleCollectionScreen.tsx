import * as React from 'react';

import { useQuery } from 'react-query';

import { Fab, Icon, View } from 'native-base';

import { useLinkTo, useNavigation } from '@react-navigation/native';
import { StackScreenProps } from '@react-navigation/stack';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { ArticleList } from '@src/shared/components/ArticleList';
import {
  QueryCacheKey,
  queryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

import { ArticleCollectionStackParamList } from '../navigators/ArticleCollectionNavigator.types';
import { routeUrl, Routes } from '../routes';

export const ArticleCollectionScreen: React.FC<
  StackScreenProps<ArticleCollectionStackParamList, 'ArticleCollection'>
> = ({ route }) => {
  const linkTo = useLinkTo();
  const navigation = useNavigation();
  const articleCollectionId = route.params.id;

  useQuery(
    queryCacheKey(QueryCacheKey.ArticleCollection, articleCollectionId),
    () => api().articleCollections_Get({ id: articleCollectionId }),
    {
      onSuccess: (ac) => {
        navigation.setOptions({
          headerTitle: ac.name,
        });
      },
    },
  );

  return (
    <View style={{ flex: 1 }}>
      <View style={{ flex: 1 }}>
        <ArticleList
          articleCollectionId={articleCollectionId}
          defaultSortBy={Backend.SortBy.OrderInCollection}
        />
      </View>
      <Fab
        position="bottomRight"
        onPress={() =>
          linkTo(
            routeUrl(Routes.ArticleCreate, null, {
              preselectedArticleCollectionId: articleCollectionId,
            }),
          )
        }
      >
        <Icon name="add-outline" />
      </Fab>
    </View>
  );
};
