import React from 'react';

import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';

import { RootStackParamList } from './RootNavigator.types';
import { ArticleNavigator } from './navigators/ArticleNavigator';
import { HomeNavigator } from './navigators/HomeNavigator';
import { LoginNavigator } from './navigators/LoginNavigator';
import { linking } from './routes';

const RootStack = createStackNavigator<RootStackParamList>();

export const RootNavigator: React.FC = () => {
  return (
    <NavigationContainer linking={linking}>
      <RootStack.Navigator headerMode="none" initialRouteName="HomeNavigator">
        <RootStack.Screen name="LoginNavigator" component={LoginNavigator} />
        <RootStack.Screen name="HomeNavigator" component={HomeNavigator} />
        <RootStack.Screen
          name="ArticleNavigator"
          component={ArticleNavigator}
        />
      </RootStack.Navigator>
    </NavigationContainer>
  );
};
