import React from 'react';

import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';

import { RootStackParamList } from './RootNavigator.types';
import { ArticleNavigator } from './navigators/ArticleNavigator';
import { HomeNavigator } from './navigators/HomeNavigator';
import { LoginNavigator } from './navigators/LoginNavigator';
import { linking } from './routes';

import { useBackendLogin } from '../shared/hooks/useBackendLogin';

const RootStack = createStackNavigator<RootStackParamList>();

export const RootNavigator: React.FC = () => {
  const { isLoggedIn } = useBackendLogin();

  return (
    <NavigationContainer linking={linking}>
      <RootStack.Navigator headerMode="none">
        {isLoggedIn ? (
          <>
            <RootStack.Screen name="HomeNavigator" component={HomeNavigator} />
            <RootStack.Screen
              name="ArticleNavigator"
              component={ArticleNavigator}
            />
          </>
        ) : (
          <RootStack.Screen name="LoginNavigator" component={LoginNavigator} />
        )}
      </RootStack.Navigator>
    </NavigationContainer>
  );
};
