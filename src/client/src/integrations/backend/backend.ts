import axios from 'axios';
import { fetchDiscoveryAsync, TokenResponse, TokenResponseConfig } from 'expo-auth-session';
import Constants from 'expo-constants';
import React from 'react';
import { readFromSecureStore, StorageKey, writeToSecureStore } from '../../shared/utils/storage';

import { clientId, scopes } from './oidcConstants';
import { Backend } from './types';

// TODO: Make this compatible with production env.
export const backendBaseUrl = `http://${
  (Constants.manifest.debuggerHost ?? 'localhost').split(':')[0]
}:5000`;

const axiosIntance = axios.create();
// Get rid of the default transform because it overrides NSWag's transform.
axiosIntance.defaults.transformResponse = [];

class TokenManager {
  #currentToken: TokenResponse | null = null;

  #tokenChangeListeners: Set<(() => void)> = new Set();

  get currentToken() {
    return this.#currentToken;
  }

  set currentToken(value) {
    this.#currentToken = value;
    this.#tokenChangeListeners.forEach((listener) => listener());
  }

  subscribeToTokenChange = (listener: () => void) => {
    this.#tokenChangeListeners.add(listener);
  };

  unsubscribeToTokenChange = (listener: () => void) => {
    this.#tokenChangeListeners.delete(listener);
  };
}

const tokenManager = new TokenManager();

export const useBackendApiTokenState = () => {
  const [, setBumper] = React.useState({});

  const bump = React.useCallback(() => setBumper({}), []);

  React.useEffect(() => {
    tokenManager.subscribeToTokenChange(bump);
    return () => tokenManager.unsubscribeToTokenChange(bump);
  }, []);

  return {
    hasValidToken: !!tokenManager.currentToken,
  };
};

export const configAuthorizationHeader = async (
  t: TokenResponse,
  shouldSave = true,
) => {
  // This is for ensuring class methods exist on the object since we are going to use them.
  tokenManager.currentToken = t instanceof TokenResponse ? t : new TokenResponse(t);

  axiosIntance.defaults.headers.common.Authorization = `Bearer ${t.accessToken}`;

  if (shouldSave) {
    await writeToSecureStore(StorageKey.AuthToken, tokenManager.currentToken.getRequestConfig());
  }
};

const innerClient = new Backend.Client(backendBaseUrl, axiosIntance);

let ongoingRefreshTokenPromise: Promise<void> | null = null;

const refreshToken = async (t: TokenResponse) => {
  const discovery = await fetchDiscoveryAsync(backendBaseUrl);

  // FIXME: Bounce user back to login screen if got exception here.
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

export const loadAuthToken = async () => {
  const saved = await readFromSecureStore<TokenResponseConfig>(StorageKey.AuthToken);
  if (saved == null) {
    return;
  }

  await refreshToken(new TokenResponse(saved));
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
      if (tokenManager.currentToken === null) {
        throw new Error(
          'Calling backend API without a valid token. Did you forget to call `configAuthorizationHeader`?',
        );
      }

      if (tokenManager.currentToken.shouldRefresh()) {
        // Ensure there's only one ongoing token refresh request
        ongoingRefreshTokenPromise =
          ongoingRefreshTokenPromise ?? refreshToken(tokenManager.currentToken);
      }

      await ongoingRefreshTokenPromise;

      return actual.call(innerClient, ...args);
    };
  },
});
