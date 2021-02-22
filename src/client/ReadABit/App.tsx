import { StatusBar } from 'expo-status-bar';
import React from 'react';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { QueryClient, QueryClientProvider } from 'react-query';

import useCachedResources from './shared/hooks/useCachedResources';
import useColorScheme from './shared/hooks/useColorScheme';
import Navigation from './navigation';

const queryClient = new QueryClient()

export default function App() {
  const isLoadingComplete = useCachedResources();
  const colorScheme = useColorScheme();

  if (!isLoadingComplete) {
    return null;
  } else {
    return (
      <SafeAreaProvider>
        <QueryClientProvider client={queryClient}>
          <Navigation colorScheme={colorScheme} />
        </QueryClientProvider>
        <StatusBar />
      </SafeAreaProvider>
    );
  }
}
