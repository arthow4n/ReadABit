import * as React from 'react';

import { defaultsDeep } from 'lodash';

import { axiosIntance } from '@src/integrations/backend/backend';

import { AppSettings } from './AppSettingsContext.types';

import {
  readFromSecureStore,
  SecureStorageKey,
  writeToSecureStore,
} from '../utils/storage';

type AppSettingsContextValue = {
  appSettings: AppSettings;
  updateAppSettings: (appSettings: AppSettings) => Promise<void>;
};

const defaultAppSettings: AppSettings = {
  languageCodes: {
    studying: 'sv',
    ui: 'en',
  },
  useMobileDataForAllDataTransfer: false,
  tts: {
    // TODO: Default to true and allow editing from settings screen.
    autoSpeakWhenTapOnWord: false,
  },
};

let savedAppSettings: AppSettings = defaultAppSettings;

const applySavedAppSettings = () => {
  axiosIntance.defaults.headers.common[
    'Accept-Language'
  ] = `${savedAppSettings.languageCodes.ui},${savedAppSettings.languageCodes.studying};q=0.9`;
};

export const loadAppSettings = async () => {
  const saved = await readFromSecureStore(SecureStorageKey.AppSettings);

  if (saved) {
    savedAppSettings = defaultsDeep(savedAppSettings, defaultAppSettings);
    applySavedAppSettings();
  }
};

const AppSettingsContext = React.createContext<AppSettingsContextValue>({
  appSettings: defaultAppSettings,
  updateAppSettings: () => Promise.resolve(),
});

export const AppSettingsContextProvider: React.FC = ({ children }) => {
  const [appSettingsState, setAppSettingsState] = React.useState<AppSettings>(
    savedAppSettings,
  );

  return (
    <AppSettingsContext.Provider
      value={{
        appSettings: appSettingsState,
        updateAppSettings: async (appSettings) => {
          await writeToSecureStore(SecureStorageKey.AppSettings, appSettings);
          savedAppSettings = appSettings;
          applySavedAppSettings();
          setAppSettingsState(appSettings);
        },
      }}
    >
      {children}
    </AppSettingsContext.Provider>
  );
};

export const useAppSettingsContext = () => React.useContext(AppSettingsContext);

/**
 * Prefer using `useAppSettingsContext` when possible to get proper state update.
 */
export const getAppSettings = () => {
  return savedAppSettings;
};
