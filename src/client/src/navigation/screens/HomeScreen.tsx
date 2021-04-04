import React from 'react';

import { Fab, Grid, Icon, Row, View } from 'native-base';

import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { useLinkTo } from '@react-navigation/native';
import { ArticleList } from '@src/shared/components/ArticleList';

import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';
import { Routes, routeUrl } from '../routes';

export const HomeScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Home'>
> = () => {
  const linkTo = useLinkTo();

  return (
    <View style={{ flex: 1 }}>
      <Grid>
        <Row size={2}>{/* TODO: Daily goal, status, ...etc */}</Row>
        <Row size={2}>
          <ArticleList />
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
