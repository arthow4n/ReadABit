import React from 'react';

import { useTranslation } from 'react-i18next';

import { Container } from 'native-base';

import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { HomeBottomTabParamList } from './HomeNavigator.types';

import { BottomTabBar } from '../../shared/components/BottomTabBar';
import { SafeArea } from '../../shared/components/SafeArea';
import { HomeScreen } from '../screens/HomeScreen';
import { SettingsScreen } from '../screens/SettingsScreen';

const HomeBottomTab = createBottomTabNavigator<HomeBottomTabParamList>();

export const HomeNavigator: React.FC = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <Container>
        <HomeBottomTab.Navigator
          tabBar={(props) => <BottomTabBar {...props} />}
        >
          <HomeBottomTab.Screen
            name="Home"
            options={{ tabBarLabel: t('Home_screen name') }}
            component={HomeScreen}
          />
          {/* TODO: Fix the missing screens */}
          {/* <HomeBottomTab.Screen
              name="ArticleCollectionNavigator"
              component={ArticleCollectionNavigator}
            /> */}
          <HomeBottomTab.Screen
            name="Settings"
            options={{ tabBarLabel: t('Settings') }}
            component={SettingsScreen}
          />
        </HomeBottomTab.Navigator>
      </Container>
    </SafeArea>
  );
};
