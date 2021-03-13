import React from 'react';

import { useTranslation } from 'react-i18next';

import {
  Body,
  Button,
  Container,
  Content,
  Header,
  Left,
  Right,
  Title,
  Text,
} from 'native-base';

import { StackNavigationProp } from '@react-navigation/stack';

import { SafeArea } from '../../shared/components/SafeArea';
import { useBackendLogin } from '../../shared/hooks/useBackendLogin';
import { LoginStackParamList } from '../navigators/LoginNavigator.types';

export const LoginScreen: React.FC<
  StackNavigationProp<LoginStackParamList, 'Login'>
> = () => {
  const { t } = useTranslation();
  const { gotoLoginPage } = useBackendLogin();

  return (
    <SafeArea>
      <Container>
        <Header>
          <Left />
          <Body>
            <Title>{t('Login')}</Title>
          </Body>
          <Right />
        </Header>
        <Content centerContent>
          <Button onPress={gotoLoginPage}>
            <Text>{t('Login')}</Text>
          </Button>
        </Content>
      </Container>
    </SafeArea>
  );
};
