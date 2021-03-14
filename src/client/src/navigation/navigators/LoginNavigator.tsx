import React from 'react';

import { useTranslation } from 'react-i18next';

import { Container } from 'native-base';

import { createStackNavigator } from '@react-navigation/stack';

import { LoginStackParamList } from './LoginNavigator.types';

import { SafeArea } from '../../shared/components/SafeArea';
import { StackNavigatorHeader } from '../../shared/components/StackNavigatorHeader';
import { LoginScreen } from '../screens/LoginScreen';

const LoginStack = createStackNavigator<LoginStackParamList>();

export const LoginNavigator: React.FC = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <Container>
        <LoginStack.Navigator
          headerMode="screen"
          screenOptions={{
            header: (props) => <StackNavigatorHeader {...props} />,
          }}
        >
          <LoginStack.Screen
            name="Login"
            component={LoginScreen}
            options={{ headerTitle: t('Login') }}
          />
        </LoginStack.Navigator>
      </Container>
    </SafeArea>
  );
};
