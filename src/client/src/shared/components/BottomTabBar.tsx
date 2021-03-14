import React from 'react';

import { Footer, FooterTab, Button, Text } from 'native-base';

import {
  BottomTabBarOptions,
  BottomTabBarProps,
} from '@react-navigation/bottom-tabs';

const TabButton: React.FC<{
  active: boolean;
  label: string;
  onPress: () => void;
  onLongPress: () => void;
}> = ({ active, label, onPress, onLongPress }) => {
  return (
    <Button
      vertical
      active={active}
      onPress={onPress}
      onLongPress={onLongPress}
    >
      {/* TODO: <Icon name="" /> */}
      <Text>{label}</Text>
    </Button>
  );
};

export const BottomTabBar: React.FC<BottomTabBarProps<BottomTabBarOptions>> = ({
  state,
  descriptors,
  navigation,
}) => {
  const focusedOptions = descriptors[state.routes[state.index].key].options;

  if (focusedOptions.tabBarVisible === false) {
    return null;
  }

  return (
    <Footer>
      <FooterTab>
        {state.routes.map((route, index) => {
          const { options } = descriptors[route.key];
          const label = (() => {
            if (typeof options.tabBarLabel !== 'string') {
              throw new Error(
                `tabBarLabel should be string. Check <${route.name}.Screen options={{ tabBarLabel: ... }}>`,
              );
            }

            return options.tabBarLabel;
          })();

          const active = state.index === index;

          const onPress = () => {
            const event = navigation.emit({
              type: 'tabPress',
              target: route.key,
              canPreventDefault: true,
            });

            if (!active && !event.defaultPrevented) {
              navigation.navigate(route.name);
            }
          };

          const onLongPress = () => {
            navigation.emit({
              type: 'tabLongPress',
              target: route.key,
            });
          };

          return (
            <TabButton
              key={label}
              active={active}
              label={label}
              onPress={onPress}
              onLongPress={onLongPress}
            />
          );
        })}
      </FooterTab>
    </Footer>
  );
};
