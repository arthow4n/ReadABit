import * as React from 'react';

import * as SplashScreen from 'expo-splash-screen';

import { tryLoadingAuthToken } from '../../integrations/backend/backend';

const ignorePromiseErrors = (promises: Promise<void>[]) => {
  const catched = promises.map((x) => x.catch((e) => console.warn(e)));
  return Promise.all(catched);
};

export function useCachedResources() {
  const [isLoadingComplete, setLoadingComplete] = React.useState(false);

  const loadResources = async () => {
    SplashScreen.preventAutoHideAsync();

    await ignorePromiseErrors([tryLoadingAuthToken()]);

    setLoadingComplete(true);
    SplashScreen.hideAsync();
  };

  React.useEffect(() => {
    loadResources();
  }, []);

  return isLoadingComplete;
}
