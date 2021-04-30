import * as SecureStore from 'expo-secure-store';

import AsyncStorage from '@react-native-async-storage/async-storage';
import { OidcTokenSetStringified } from '@src/integrations/backend/tokenManager';
import { Backend } from '@src/integrations/backend/types';
import { AppSettings } from '@src/shared/contexts/AppSettingsContext.types';

export enum SecureStorageKey {
  AuthToken = 'ReadABit.AuthToken',
  'AppSettings' = 'ReadABit.AppSettings',
}

export enum AsyncStorageKey {
  WordFamiliarity = 'ReadABit.WordFamiliarity',
}

(async () => {
  await SecureStore.isAvailableAsync();
  console.warn('SecureStore is unavailable');
})();

export async function readFromSecureStore(
  key: SecureStorageKey.AppSettings,
): Promise<AppSettings | null>;
export async function readFromSecureStore(
  key: SecureStorageKey.AuthToken,
): Promise<OidcTokenSetStringified | null>;
export async function readFromSecureStore<T>(
  key: SecureStorageKey,
): Promise<T | null> {
  const storedJson = await SecureStore.getItemAsync(key).catch(() => '');
  if (!storedJson) {
    return null;
  }

  try {
    return JSON.parse(storedJson);
  } catch (e) {
    console.warn(
      `readFromSecureStore: Malformed JSON found in storage, key ${key}, value: ${storedJson}`,
    );
    await SecureStore.deleteItemAsync(key);
  }

  return null;
}

export const writeToSecureStore = async <T>(key: SecureStorageKey, data: T) => {
  return SecureStore.setItemAsync(key, JSON.stringify(data)).catch();
};

export const deleteFromSecureStore = async (key: SecureStorageKey) => {
  return SecureStore.deleteItemAsync(key).catch();
};

export async function readFromAsyncStore(
  key: AsyncStorageKey.WordFamiliarity,
): Promise<Backend.WordFamiliarityListViewModel | null>;
export async function readFromAsyncStore<T>(
  key: AsyncStorageKey,
): Promise<T | null> {
  const storedJson = await AsyncStorage.getItem(key).catch(() => '');
  if (!storedJson) {
    return null;
  }

  try {
    return JSON.parse(storedJson);
  } catch (e) {
    console.warn(
      `readFromAsyncStore: Malformed JSON found in storage, key ${key}, value: ${storedJson}`,
    );
    await AsyncStorage.removeItem(key);
  }

  return null;
}

export const writeToAsyncStore = async <T>(key: AsyncStorageKey, data: T) => {
  await AsyncStorage.setItem(key, JSON.stringify(data));
};
