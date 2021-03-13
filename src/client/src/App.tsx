import React from 'react';

import { LogBox } from 'react-native';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { QueryClient, QueryClientProvider } from 'react-query';

import { registerRootComponent } from 'expo';
import { StatusBar } from 'expo-status-bar';

import { RootNavigator } from './navigation/RootNavigator';
import { useCachedResources } from './shared/hooks/useCachedResources';
import './translations/init';

LogBox.ignoreLogs([
  // Shuold be safe to ignore as long as the devs know the issue.
  // https://github.com/tannerlinsley/react-query/issues/1259
  'Setting a timer',
]);

const queryClient = new QueryClient();

const App: React.FC = () => {
  const isLoadingComplete = useCachedResources();

  if (!isLoadingComplete) {
    return null;
  }
  return (
    <SafeAreaProvider>
      <QueryClientProvider client={queryClient}>
        <RootNavigator />
      </QueryClientProvider>
      <StatusBar />
    </SafeAreaProvider>
  );
};

registerRootComponent(App);
