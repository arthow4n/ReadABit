import * as React from 'react';

import { Body, Button, Header, Icon, Left, Right, Title } from 'native-base';

import { StackHeaderProps } from '@react-navigation/stack';

export const StackNavigatorHeader: React.FC<StackHeaderProps> = ({
  scene,
  navigation,
}) => {
  const { options } = scene.descriptor;

  const title = (() => {
    if (typeof options.headerTitle !== 'string') {
      throw new Error(
        `headerTitle should be string. Check <${scene.route.name}.Screen options={{ headerTitle: ... }}>`,
      );
    }

    return options.headerTitle;
  })();

  return (
    <Header>
      <Left>
        {navigation.canGoBack() && (
          <Button transparent onPress={() => navigation.goBack()}>
            <Icon name="arrow-back" />
          </Button>
        )}
      </Left>
      <Body>
        <Title>{title}</Title>
      </Body>
      <Right />
    </Header>
  );
};
