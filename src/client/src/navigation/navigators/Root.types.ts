import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';

enum RootBottomTabRouteNames {
  Login = 'Login',
  Home = 'Home',
  Settings = 'Settings',
}

export interface RootBottomTabParamList
  extends Record<RootBottomTabRouteNames, any> {
  [key: string]: any;
  Login: undefined;
  Home: undefined;
  Settings: undefined;
}

export function assertIsRootBottomTabRouteName(
  routeName: string,
): asserts routeName is RootBottomTabRouteNames {
  if (!(routeName in RootBottomTabRouteNames)) {
    throw new Error();
  }
}

export type LoginScreenProps = BottomTabNavigationProp<
  RootBottomTabParamList,
  'Login'
>;

export type HomeNavigatorScreenProps = BottomTabNavigationProp<
  RootBottomTabParamList,
  'Home'
>;

export type SettingsScreenProps = BottomTabNavigationProp<
  RootBottomTabParamList,
  'Settings'
>;
