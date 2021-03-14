import React from 'react';

import { useTranslation } from 'react-i18next';

import { Content, Text } from 'native-base';

import { StackScreenProps } from '@react-navigation/stack';

import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

export const ArticleScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'Article'>
> = ({ route }) => {
  const { t } = useTranslation();

  return (
    <Content>
      <Text>{'// TODO: ArticleScreen '}</Text>
      <Text>{`Route ID: ${route.params.id}`}</Text>
    </Content>
  );
};
