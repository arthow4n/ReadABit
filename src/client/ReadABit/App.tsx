import React from 'react';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { QueryClient, QueryClientProvider } from 'react-query';

import { StatusBar } from 'expo-status-bar';

import { Navigation } from './navigation';
import { useCachedResources } from './shared/hooks/useCachedResources';
import { useColorScheme } from './shared/hooks/useColorScheme';

const queryClient = new QueryClient();

// eslint-disable-next-line
export default function App() {
  const isLoadingComplete = useCachedResources();
  const colorScheme = useColorScheme();

  if (!isLoadingComplete) {
    return null;
  }
  return (
    <SafeAreaProvider>
      <QueryClientProvider client={queryClient}>
        <Navigation colorScheme={colorScheme} />
      </QueryClientProvider>
      <StatusBar />
    </SafeAreaProvider>
  );
}
