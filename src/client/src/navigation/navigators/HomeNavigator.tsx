import React from 'react';

import { useTranslation } from 'react-i18next';

import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { HomeBottomTabParamList } from './HomeNavigator.types';

import { BottomTabBar } from '../../shared/components/BottomTabBar';
import { SafeArea } from '../../shared/components/SafeArea';
import { HomeScreen } from '../screens/HomeScreen';

const HomeBottomTab = createBottomTabNavigator<HomeBottomTabParamList>();

export const HomeNavigator: React.FC = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <HomeBottomTab.Navigator tabBar={(props) => <BottomTabBar {...props} />}>
        <HomeBottomTab.Screen
          name="Home"
          options={{ tabBarLabel: t('Home_screen name') }}
          component={HomeScreen}
        />
        {/* TODO: Fix the missing screens */}
        {/* <HomeButtomTab.Screen
              name="ArticleCollectionNavigator"
              component={ArticleCollectionNavigator}
            /> */}
        {/* <HomeButtomTab.Screen name="Settings" component={SettingsScreen} /> */}
      </HomeBottomTab.Navigator>
    </SafeArea>
  );
};
