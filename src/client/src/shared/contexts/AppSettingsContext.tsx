import * as React from 'react';

import {
  readFromSecureStore,
  StorageKey,
  writeToSecureStore,
} from '../utils/storage';

type AppSettings = {
  languageCodes: {
    // TODO: Use enum for language codes.
    studying: string;
    ui: string;
  };
};

type AppSettingsContextValue = {
  appSettings: AppSettings;
  updateAppSettings: (appSettings: AppSettings) => Promise<void>;
};

const defaultAppSettings: AppSettings = {
  languageCodes: {
    studying: 'sv',
    ui: 'en',
  },
};

let savedAppSettings: AppSettings | null = null;

export const loadAppSettings = async () => {
  savedAppSettings = await readFromSecureStore<AppSettings>(
    StorageKey.AppSettings,
  );
};

const AppSettingsContext = React.createContext<AppSettingsContextValue>({
  appSettings: defaultAppSettings,
  updateAppSettings: () => Promise.resolve(),
});

export const AppSettingsContextProvider: React.FC = ({ children }) => {
  const [appSettingsState, setAppSettingsState] = React.useState<AppSettings>(
    savedAppSettings || defaultAppSettings,
  );

  return (
    <AppSettingsContext.Provider
      value={{
        appSettings: appSettingsState,
        updateAppSettings: async (appSettings) => {
          await writeToSecureStore(StorageKey.AppSettings, appSettings);
          savedAppSettings = appSettings;
          setAppSettingsState(appSettings);
        },
      }}
    >
      {children}
    </AppSettingsContext.Provider>
  );
};

export const useAppSettingsContext = () => React.useContext(AppSettingsContext);
