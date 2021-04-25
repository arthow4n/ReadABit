import React from 'react';

import { useTranslation } from 'react-i18next';
import {
  Menu,
  MenuOption,
  MenuOptions,
  MenuTrigger,
} from 'react-native-popup-menu';
import { Bar } from 'react-native-progress';
import {
  InfiniteData,
  useInfiniteQuery,
  useMutation,
  useQueryClient,
} from 'react-query';

import produce from 'immer';
import {
  Body,
  Icon,
  Left,
  List,
  ListItem,
  Right,
  Text,
  Thumbnail,
  View,
} from 'native-base';

import { useLinkTo } from '@react-navigation/native';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { Routes, routeUrl } from '@src/navigation/routes';
import { ContentLoading } from '@src/shared/components/Loading';
import {
  QueryCacheKey,
  queryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

export const ArticleList: React.FC<{
  articleCollectionId?: string;
}> = ({ articleCollectionId }) => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const queryClient = useQueryClient();

  // TODO: Support sorting
  // TODO: Support searching

  const queryKey = queryCacheKey(QueryCacheKey.ArticleList, {
    articleCollectionId,
    page_Size: 50,
  });

  const { data, fetchNextPage } = useInfiniteQuery(
    // TODO: Confirm the cache is keyed correctly here
    queryKey,
    ({ pageParam }) =>
      api().articles_List({
        articleCollectionId,
        page_Index: pageParam ?? 1,
        page_Size: 50,
      }),
    {
      getNextPageParam: (last) => last.page.next?.index,
      getPreviousPageParam: (first) => first.page.previous?.index,
    },
  );

  const articleDeleteHandle = useMutation(api().articles_Delete);

  const deleteArticle = async (articleId: string) => {
    await articleDeleteHandle.mutateAsync({ id: articleId });

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
              (article) => article.id !== articleId,
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
      keyExtractor={(x: Backend.ArticleListItemViewModel) => x.id}
      onEndReached={() => fetchNextPage()}
      onEndReachedThreshold={0.5}
      renderItem={({ item }: { item: Backend.ArticleListItemViewModel }) => {
        return (
          <ListItem
            key={item.id}
            thumbnail
            onPress={() => linkTo(routeUrl(Routes.Article, { id: item.id }))}
          >
            <Left>
              <Thumbnail
                square
                source={{ uri: 'https://placekitten.com/100/100' }}
              />
            </Left>
            <Body>
              <Text>{item.name}</Text>
              <View style={{ marginTop: 12 }}>
                <Bar
                  progress={item.readRadio}
                  color={item.readRadio >= 1 ? 'green' : 'blue'}
                />
              </View>
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
