import React from 'react';

import { useInfiniteQuery } from 'react-query';

import {
  Body,
  Fab,
  Grid,
  Icon,
  Left,
  List,
  ListItem,
  Row,
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

const HomeArticleList: React.FC = () => {
  const linkTo = useLinkTo();

  const { data, fetchNextPage } = useInfiniteQuery(
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
            </Body>
          </ListItem>
        );
      }}
    />
  );
};

export const HomeScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Home'>
> = () => {
  const linkTo = useLinkTo();

  return (
    <View style={{ flex: 1 }}>
      <Grid>
        <Row size={2}>{/* TODO: Daily goal, status, ...etc */}</Row>
        <Row size={2}>
          <HomeArticleList />
        </Row>
      </Grid>
      <Fab
        position="bottomRight"
        onPress={() => linkTo(routeUrl(Routes.ArticleCreate))}
      >
        <Icon name="add-outline" />
      </Fab>
    </View>
  );
};
