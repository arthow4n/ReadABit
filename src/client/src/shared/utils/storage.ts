import * as SecureStore from 'expo-secure-store';

export enum StorageKey {
  AuthToken = 'ReadABit.AuthToken',
  AppSettings = 'ReadABit.AppSettings',
  WordFamiliarity = 'ReadABit.WordFamiliarity',
}

const secureStoreAvailabePromise = SecureStore.isAvailableAsync();

export const readFromSecureStore = async <T>(
  key: StorageKey,
): Promise<T | null> => {
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
};

export const writeToSecureStore = async <T>(key: StorageKey, data: T) => {
  if (!(await secureStoreAvailabePromise)) {
    console.warn(
      `writeToSecureStore: SecureStore is unavailable. ${key} won't be saved.`,
    );
    return;
  }

  await SecureStore.setItemAsync(key, JSON.stringify(data));
};
