import React from 'react';

import { useInfiniteQuery } from 'react-query';

import {
  Body,
  Content,
  Fab,
  Icon,
  Left,
  List,
  ListItem,
  Text,
  Thumbnail,
  View,
} from 'native-base';

import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { useLinkTo } from '@react-navigation/native';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { ContentLoading } from '../../shared/components/Loading';
import {
  QueryCacheKey,
  queryCacheKey,
} from '../../shared/hooks/useBackendReactQuery';
import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';
import { Routes, routeUrl } from '../routes';

export const HomeScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Home'>
> = () => {
  const linkTo = useLinkTo();

  const { isLoading, data, fetchNextPage } = useInfiniteQuery(
    // TODO: Confirm the cache is keyed correctly here
    queryCacheKey(QueryCacheKey.ArticleList, {
      page_Size: 50,
    }),
    ({ pageParam }) =>
      api.articles_List({
        page_Index: pageParam ?? 1,
        page_Size: 50,
      }),
    {
      getNextPageParam: (last) => last.page.next?.index,
      getPreviousPageParam: (first) => first.page.previous?.index,
    },
  );

  return (
    <>
      {isLoading ? (
        <ContentLoading />
      ) : (
        <Content>
          <List
            dataArray={data?.pages}
            keyExtractor={(x: Backend.ArticleListItemViewModel) => x.id}
            onEndReached={() => fetchNextPage()}
            onEndReachedThreshold={0.5}
            renderItem={({
              item,
            }: {
              item: Backend.ArticleListItemViewModel;
            }) => {
              return (
                <ListItem
                  thumbnail
                  onPress={() =>
                    linkTo(routeUrl(Routes.Article, { id: item.id }))
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
                  </Body>
                </ListItem>
              );
            }}
          />
        </Content>
      )}
      <View style={{ flex: 1 }}>
        <Fab
          position="bottomRight"
          onPress={() => linkTo(routeUrl(Routes.ArticleCreate))}
        >
          <Icon name="add-outline" />
        </Fab>
      </View>
    </>
  );
};
