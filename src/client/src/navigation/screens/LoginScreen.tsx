import React from 'react';

import { useTranslation } from 'react-i18next';

import { Button, Content, Text } from 'native-base';

import { StackNavigationProp } from '@react-navigation/stack';

import { useBackendLogin } from '../../shared/hooks/useBackendLogin';
import { LoginStackParamList } from '../navigators/LoginNavigator.types';

export const LoginScreen: React.FC<
  StackNavigationProp<LoginStackParamList, 'Login'>
> = () => {
  const { t } = useTranslation();
  const { gotoLoginPage } = useBackendLogin();

  return (
    <Content centerContent>
      <Button onPress={gotoLoginPage}>
        <Text>{t('Login')}</Text>
      </Button>
    </Content>
  );
};
