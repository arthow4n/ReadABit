import React from 'react';

import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { HomeBottomTabParamList } from './HomeNavigator.types';

import { BottomTabBar } from '../../shared/components/BottomTabBar';
import { HomeScreen } from '../screens/HomeScreen';

const HomeBottomTab = createBottomTabNavigator<HomeBottomTabParamList>();

export const HomeNavigator: React.FC = () => {
  return (
    <HomeBottomTab.Navigator tabBar={(props) => <BottomTabBar {...props} />}>
      <HomeBottomTab.Screen name="Home" component={HomeScreen} />
      {/* TODO: Fix the missing screens */}
      {/* <HomeButtomTab.Screen
              name="ArticleCollectionNavigator"
              component={ArticleCollectionNavigator}
            /> */}
      {/* <HomeButtomTab.Screen name="Settings" component={SettingsScreen} /> */}
    </HomeBottomTab.Navigator>
  );
};
