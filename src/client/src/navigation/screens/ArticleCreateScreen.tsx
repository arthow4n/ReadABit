import React from 'react';

import { useTranslation } from 'react-i18next';

import { Container, Content, Text } from 'native-base';

import { StackNavigationProp } from '@react-navigation/stack';

import { SafeArea } from '../../shared/components/SafeArea';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

export const ArticleCreateScreen: React.FC<
  StackNavigationProp<ArticleStackParamList, 'Article'>
> = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <Container>
        <Content>
          <Text>{'// TODO: ArticleCreateScreen '}</Text>
        </Content>
      </Container>
    </SafeArea>
  );
};
