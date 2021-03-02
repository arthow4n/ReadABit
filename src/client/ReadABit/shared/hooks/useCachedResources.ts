import { Ionicons } from "@expo/vector-icons";
import * as Font from "expo-font";
import * as SplashScreen from "expo-splash-screen";
import * as React from "react";
import { tryLoadingAuthToken } from "../../integrations/backend/backend";

const ignorePromiseErrors = (promises: Promise<void>[]) => {
  const catched = promises.map((x) => x.catch((e) => console.warn(e)));
  return Promise.all(catched);
};

export default function useCachedResources() {
  const [isLoadingComplete, setLoadingComplete] = React.useState(false);

  // TODO: Load auth tokens on app load.

  // Load any resources or data that we need prior to rendering the app

  const loadResources = async () => {
    SplashScreen.preventAutoHideAsync();

    await ignorePromiseErrors([
      Font.loadAsync({
        ...Ionicons.font,
        "space-mono": require("../../assets/fonts/SpaceMono-Regular.ttf"),
      }),
      tryLoadingAuthToken(),
    ]);

    setLoadingComplete(true);
    SplashScreen.hideAsync();
  };

  React.useEffect(() => {
    loadResources();
  }, []);

  return isLoadingComplete;
}
