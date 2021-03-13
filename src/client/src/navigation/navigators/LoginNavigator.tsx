import React from 'react';

import { createStackNavigator } from '@react-navigation/stack';

import { LoginStackParamList } from './LoginNavigator.types';

import { LoginScreen } from '../screens/LoginScreen';

const LoginStack = createStackNavigator<LoginStackParamList>();

export const LoginNavigator: React.FC = () => {
  return (
    <LoginStack.Navigator>
      <LoginStack.Screen name="Login" component={LoginScreen} />
    </LoginStack.Navigator>
  );
};
