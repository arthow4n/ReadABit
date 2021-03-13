import React from 'react';

import { useTranslation } from 'react-i18next';

import { Content, Fab, Icon, Text, View } from 'native-base';

import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { useLinkTo } from '@react-navigation/native';

import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';
import { Routes, routeUrl } from '../routes';

export const HomeScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Home'>
> = () => {
  const { t } = useTranslation();

  const linkTo = useLinkTo();

  return (
    <>
      <Content>
        <Text>{'// TODO: HomeScreen '}</Text>
      </Content>
      <View style={{ flex: 1 }}>
        <Fab
          position="bottomRight"
          onPress={() => linkTo(routeUrl(Routes.ArticleCreate))}
        >
          <Icon name="add-outline" />
        </Fab>
      </View>
    </>
  );
};
