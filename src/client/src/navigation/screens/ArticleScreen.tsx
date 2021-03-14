import React from 'react';

import { useTranslation } from 'react-i18next';

import { Content, Text } from 'native-base';

import { StackNavigationProp } from '@react-navigation/stack';

import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

export const ArticleScreen: React.FC<
  StackNavigationProp<ArticleStackParamList, 'Article'>
> = () => {
  const { t } = useTranslation();

  return (
    <Content>
      <Text>{'// TODO: ArticleScreen '}</Text>
    </Content>
  );
};
