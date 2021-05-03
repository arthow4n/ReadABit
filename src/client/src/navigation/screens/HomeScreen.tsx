import React from 'react';

import { Fab, Grid, Icon, Row, View } from 'native-base';

import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { useLinkTo } from '@react-navigation/native';
import { Backend } from '@src/integrations/backend/types';
import { ArticleList } from '@src/shared/components/ArticleList';
import { HomeDailyGoal } from '@src/shared/components/HomeDailyGoal';

import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';
import { Routes, routeUrl } from '../routes';

export const HomeScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Home'>
> = () => {
  const linkTo = useLinkTo();

  return (
    <View style={{ flex: 1 }}>
      <Grid>
        <Row size={2}>
          {/* TODO: Total known words count or some other numbers */}
          <HomeDailyGoal />
        </Row>
        <Row size={2}>
          <ArticleList defaultSortBy={Backend.SortBy.LastAccessed} />
        </Row>
      </Grid>
      <Fab
        position="bottomRight"
        onPress={() =>
          linkTo(
            routeUrl(Routes.ArticleCreate, null, {
              preselectedArticleCollectionId: null,
            }),
          )
        }
      >
        <Icon name="add-outline" />
      </Fab>
    </View>
  );
};
