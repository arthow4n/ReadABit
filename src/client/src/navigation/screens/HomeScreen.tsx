import React from 'react';

import { useTranslation } from 'react-i18next';

import { Content, Fab, Icon, Text, View } from 'native-base';

import { HomeNavigatorScreenProps } from '../navigators/Root.types';

export const HomeScreen: React.FC<HomeNavigatorScreenProps> = () => {
  const { t } = useTranslation();

  return (
    <>
      <Content>
        <Text>{t('Home_screen name')}</Text>
      </Content>
      <View style={{ flex: 1 }}>
        <Fab position="bottomRight" onPress={() => {}}>
          <Icon name="add-outline" />
        </Fab>
      </View>
    </>
  );
};
