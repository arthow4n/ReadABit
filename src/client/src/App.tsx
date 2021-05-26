import React from 'react';

import { AppRegistry, LogBox } from 'react-native';
import { MenuProvider } from 'react-native-popup-menu';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { QueryClient, QueryClientProvider } from 'react-query';

import { StatusBar } from 'expo-status-bar';

import { ReAuthRequiredException } from './integrations/backend/backend';
import { RootNavigator } from './navigation/RootNavigator';
import { AppSettingsContextProvider } from './shared/contexts/AppSettingsContext';
import { useCachedResources } from './shared/hooks/useCachedResources';
import './translations/init';

LogBox.ignoreLogs([
  // Shuold be safe to ignore as long as the devs know the issue.
  // https://github.com/tannerlinsley/react-query/issues/1259
  'Setting a timer',
]);

const defaultRetryLogic = (failureCount: number, error: unknown) =>
  failureCount < 3 && !(error instanceof ReAuthRequiredException);

const queryClient = new QueryClient({
  defaultOptions: {
    mutations: {
      retry: defaultRetryLogic,
    },
    queries: {
      retry: defaultRetryLogic,
    },
  },
});

const App: React.FC = () => {
  const isLoadingComplete = useCachedResources();

  if (!isLoadingComplete) {
    return null;
  }
  return (
    <SafeAreaProvider>
      <MenuProvider>
        <QueryClientProvider client={queryClient}>
          <AppSettingsContextProvider>
            <RootNavigator />
          </AppSettingsContextProvider>
        </QueryClientProvider>
        <StatusBar />
      </MenuProvider>
    </SafeAreaProvider>
  );
};

AppRegistry.registerComponent('main', () => App);
