import * as React from 'react';

import { useTranslation } from 'react-i18next';
import {
  Menu,
  MenuTrigger,
  MenuOptions,
  MenuOption,
} from 'react-native-popup-menu';
import { Bar } from 'react-native-progress';
import {
  InfiniteData,
  useInfiniteQuery,
  useMutation,
  useQueryClient,
} from 'react-query';

import { produce } from 'immer';
import {
  List,
  ListItem,
  Left,
  Thumbnail,
  Body,
  View,
  Right,
  Icon,
  Text,
} from 'native-base';

import { useLinkTo } from '@react-navigation/native';
import { StackScreenProps } from '@react-navigation/stack';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { ContentLoading } from '@src/shared/components/Loading';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import {
  queryCacheKey,
  QueryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

import { ArticleCollectionStackParamList } from '../navigators/ArticleCollectionNavigator.types';
import { routeUrl, Routes } from '../routes';

export const ArticleCollectionListScreen: React.FC<
  StackScreenProps<ArticleCollectionStackParamList, 'ArticleCollectionList'>
> = () => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const queryClient = useQueryClient();
  const { appSettings } = useAppSettingsContext();

  const queryKey = queryCacheKey(QueryCacheKey.ArticleCollectionList, {
    filter_LanguageCode: appSettings.languageCodes.studying,
    page_Size: 50,
  });

  const { data, fetchNextPage } = useInfiniteQuery(
    // TODO: Confirm the cache is keyed correctly here
    queryKey,
    ({ pageParam }) =>
      api().articleCollections_List({
        filter_LanguageCode: appSettings.languageCodes.studying,
        page_Index: pageParam ?? 1,
        page_Size: 50,
      }),
    {
      getNextPageParam: (last) => last.page.next?.index,
      getPreviousPageParam: (first) => first.page.previous?.index,
    },
  );

  const articleCollectionDeleteHandle = useMutation(
    api().articleCollections_Delete,
  );

  const deleteArticle = async (articleCollectionId: string) => {
    await articleCollectionDeleteHandle.mutateAsync({
      id: articleCollectionId,
    });

    queryClient.setQueryData<
      InfiniteData<Backend.PaginatedOfArticleListItemViewModel>
    >(queryKey, (oldData) => {
      if (!oldData) {
        throw new Error();
      }
      return {
        pages: produce(oldData.pages, (draft) => {
          draft.forEach((paginated) => {
            // eslint-disable-next-line no-param-reassign
            paginated.items = paginated.items.filter(
              (articleCollection) =>
                articleCollection.id !== articleCollectionId,
            );
          });
        }),
        pageParams: oldData.pageParams,
      };
    });
  };

  return !data ? (
    <ContentLoading />
  ) : (
    <List
      dataArray={data.pages.flatMap((p) => p.items)}
      keyExtractor={(x: Backend.ArticleCollectionListItemViewModel) => x.id}
      onEndReached={() => fetchNextPage()}
      onEndReachedThreshold={0.5}
      renderItem={({
        item,
      }: {
        item: Backend.ArticleCollectionListItemViewModel;
      }) => {
        return (
          <ListItem
            key={item.id}
            thumbnail
            onPress={() =>
              linkTo(routeUrl(Routes.ArticleCollection, { id: item.id }))
            }
          >
            <Left>
              <Thumbnail
                square
                source={{ uri: 'https://placekitten.com/100/100' }}
              />
            </Left>
            <Body>
              <Text>{item.name}</Text>
              {/* TODO: Article collection reading progress */}
            </Body>
            <Right>
              <Menu>
                <MenuTrigger>
                  <Icon name="menu-outline" />
                </MenuTrigger>
                <MenuOptions>
                  <MenuOption onSelect={() => deleteArticle(item.id)}>
                    <Text style={{ color: '#FF0000' }}>{t('Delete')}</Text>
                  </MenuOption>
                </MenuOptions>
              </Menu>
            </Right>
          </ListItem>
        );
      }}
    />
  );
};
