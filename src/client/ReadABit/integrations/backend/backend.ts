import axios from 'axios';
import { fetchDiscoveryAsync, TokenResponse } from 'expo-auth-session';
import Constants from 'expo-constants';
import * as SecureStore from 'expo-secure-store';

import { authTokenStorageKey, clientId, scopes } from './oidcConstants';
import { Backend } from './types';

// TODO: Make this compatible with production env.
export const backendBaseUrl = `http://${
  (Constants.manifest.debuggerHost ?? 'localhost').split(':')[0]
}:5000`;

const axiosIntance = axios.create();
// Get rid of the default transform because it overrides NSWag's transform.
axiosIntance.defaults.transformResponse = [];

let currentToken: TokenResponse | null = null;

const secureStoreAvailabePromise = SecureStore.isAvailableAsync();

export const configAuthorizationHeader = async (
  t: TokenResponse,
  shouldSave = true,
) => {
  // This is for ensuring class methods exist on the object since we are going to use them.
  currentToken = t instanceof TokenResponse ? t : new TokenResponse(t);

  axiosIntance.defaults.headers.common.Authorization = `Bearer ${t.accessToken}`;

  if (!(await secureStoreAvailabePromise)) {
    console.warn("SecureStore is unavailable. Auth tokens won't be saved.");
    return;
  }

  if (shouldSave) {
    await SecureStore.setItemAsync(
      authTokenStorageKey,
      // `getRequestConfig()` returns the params needed for `new TokenResponse()`
      JSON.stringify(currentToken.getRequestConfig()),
    );
  }
};

export const tryLoadingAuthToken = async () => {
  const storedJson = await SecureStore.getItemAsync(authTokenStorageKey).catch(
    () => '',
  );
  if (!storedJson) {
    return;
  }

  try {
    await configAuthorizationHeader(
      new TokenResponse(JSON.parse(storedJson)),
      false,
    );
  } catch (e) {
    console.warn(
      `tryLoadingAuthToken: Malformed JSON found in storage, value: ${storedJson}`,
    );
    await SecureStore.deleteItemAsync(authTokenStorageKey);
  }
};

const innerClient = new Backend.Client(backendBaseUrl, axiosIntance);

let ongoingRefreshTokenPromise: Promise<void> | null = null;

const refreshToken = async (t: TokenResponse) => {
  const discovery = await fetchDiscoveryAsync(backendBaseUrl);
  const newTokenResponse = await t.refreshAsync(
    {
      clientId,
      scopes,
    },
    discovery,
  );

  await configAuthorizationHeader(newTokenResponse);
  ongoingRefreshTokenPromise = null;
};

// FIXME: Fix the template of types.ts instead of doing it like this.
export const api = new Proxy(innerClient, {
  get(t, p, r) {
    const actual = Reflect.get(t, p, r);

    // Assuming every API function:
    // - has a `_` in it
    // - doesn't start with `process`
    // and filter non-API calls away, this is not cool, I know.
    if (!(typeof p === 'string' && /^(?!process).+_.+/.test(p))) {
      return actual;
    }

    if (typeof actual !== 'function') {
      throw new Error('Please fix the hack in backend.ts.');
    }

    return async (...args: any[]) => {
      if (currentToken === null) {
        throw new Error(
          'Calling backend API without a valid token. Did you forget to call `configAuthorizationHeader`?',
        );
      }

      if (currentToken.shouldRefresh()) {
        // Ensure there's only one ongoing token refresh request
        ongoingRefreshTokenPromise ??= refreshToken(currentToken);
        await ongoingRefreshTokenPromise;
      }

      return actual.call(innerClient, ...args);
    };
  },
});
