import * as React from 'react';

import { useTranslation } from 'react-i18next';

import {
  Body,
  Button,
  Content,
  Header,
  Left,
  Right,
  Text,
  Title,
} from 'native-base';

import { useBackendLogin } from '../../shared/hooks/useBackendLogin';
import { LoginScreenProps } from '../navigators/Root.types';

export const LoginScreen: React.FC<LoginScreenProps> = () => {
  const { t } = useTranslation();
  const { gotoLoginPage } = useBackendLogin();
  return (
    <>
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
    </>
  );
};
