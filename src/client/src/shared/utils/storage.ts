import * as SecureStore from 'expo-secure-store';

import AsyncStorage from '@react-native-async-storage/async-storage';
import { OidcTokenSet } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';

import { AppSettings } from '../contexts/AppSettingsContext.types';

export enum SecureStorageKey {
  AuthToken = 'ReadABit.AuthToken',
  'AppSettings' = 'ReadABit.AppSettings',
}

export enum AsyncStorageKey {
  WordFamiliarity = 'ReadABit.WordFamiliarity',
}

const secureStoreAvailabePromise = SecureStore.isAvailableAsync();

export async function readFromSecureStore(
  key: SecureStorageKey.AppSettings,
): Promise<AppSettings | null>;
export async function readFromSecureStore(
  key: SecureStorageKey.AuthToken,
): Promise<OidcTokenSet | null>;
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
  if (!(await secureStoreAvailabePromise)) {
    console.warn(
      `writeToSecureStore: SecureStore is unavailable. ${key} won't be saved.`,
    );
    return;
  }

  await SecureStore.setItemAsync(key, JSON.stringify(data));
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
