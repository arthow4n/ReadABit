import * as React from 'react';

import * as Font from 'expo-font';
import * as SplashScreen from 'expo-splash-screen';

import { Ionicons } from '@expo/vector-icons';

import { loadAuthToken } from '../../integrations/backend/backend';

const ignorePromiseErrors = (promises: Promise<void>[]) => {
  const catched = promises.map((x) => x.catch((e) => console.warn(e)));
  return Promise.all(catched);
};

export function useCachedResources() {
  const [isLoadingComplete, setLoadingComplete] = React.useState(false);

  const loadResources = async () => {
    SplashScreen.preventAutoHideAsync();

    await ignorePromiseErrors([
      Font.loadAsync({
        // eslint-disable-next-line global-require
        Roboto: require('native-base/Fonts/Roboto.ttf'),
        // eslint-disable-next-line global-require
        Roboto_medium: require('native-base/Fonts/Roboto_medium.ttf'),
        ...Ionicons.font,
      }),
      loadAuthToken(),
    ]);

    setLoadingComplete(true);
    SplashScreen.hideAsync();
  };

  React.useEffect(() => {
    loadResources();
  }, []);

  return isLoadingComplete;
}
