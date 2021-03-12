import React from 'react';

import { useTranslation } from 'react-i18next';
import { SafeAreaView } from 'react-native-safe-area-context';

import { Container } from 'native-base';

import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { NavigationContainer } from '@react-navigation/native';

import { RootBottomTabParamList } from './Root.types';
import { RootBottomTabBar } from './RootBottomTabBar';

import { useBackendLogin } from '../../shared/hooks/useBackendLogin';
import { linking } from '../routes';
import { HomeScreen } from '../screens/HomeScreen';
import { LoginScreen } from '../screens/LoginScreen';
import { SettingsScreen } from '../screens/SettingsScreen';

const RootBottomTab = createBottomTabNavigator<RootBottomTabParamList>();

export const Root: React.FC = () => {
  const { isLoggedIn } = useBackendLogin();
  const { t } = useTranslation();

  // TODO: Consider applying safe area on screens instead
  // FIXME: Replace hard coded background colour with variable.
  return (
    <NavigationContainer linking={linking}>
      <SafeAreaView style={{ flex: 1, backgroundColor: '#3F51B5' }}>
        <Container>
          <RootBottomTab.Navigator
            key={`${isLoggedIn}`}
            tabBar={(props) =>
              isLoggedIn ? <RootBottomTabBar {...props} /> : null
            }
          >
            {isLoggedIn ? (
              <>
                <RootBottomTab.Screen
                  name="Home"
                  options={{
                    tabBarLabel: t('Home', { context: 'screen name' }),
                  }}
                  component={HomeScreen}
                />
                <RootBottomTab.Screen
                  name="Settings"
                  options={{ tabBarLabel: t('Settings') }}
                  component={SettingsScreen}
                />
              </>
            ) : (
              <RootBottomTab.Screen
                name="Login"
                options={{ tabBarLabel: t('Login') }}
                component={LoginScreen}
              />
            )}
          </RootBottomTab.Navigator>
        </Container>
      </SafeAreaView>
    </NavigationContainer>
  );
};
