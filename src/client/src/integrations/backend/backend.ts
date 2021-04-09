import React from 'react';

import { refresh } from 'react-native-app-auth';

import axios from 'axios';

import { BackendBaseUrl } from '@env';

import { oidcAuthConfig } from './oidcConstants';
import { Backend } from './types';

import {
  readFromSecureStore,
  SecureStorageKey,
  writeToSecureStore,
} from '../../shared/utils/storage';

const axiosIntance = axios.create();
// Get rid of the default transform because it overrides NSWag's transform.
axiosIntance.defaults.transformResponse = [];

export type OidcTokenSet = {
  accessToken: string;
  accessTokenExpirationDate: Date;
  refreshToken: string | null;
};

class TokenManager {
  #currentToken: OidcTokenSet | null = null;

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
  newToken: {
    accessToken: string;
    refreshToken: string;
    accessTokenExpirationDate: string;
  },
  shouldSave = true,
) => {
  tokenManager.currentToken = {
    accessToken: newToken.accessToken,
    refreshToken: newToken.refreshToken,
    accessTokenExpirationDate: new Date(newToken.accessTokenExpirationDate),
  };

  axiosIntance.defaults.headers.common.Authorization = `Bearer ${newToken.accessToken}`;

  if (shouldSave) {
    await writeToSecureStore(
      SecureStorageKey.AuthToken,
      tokenManager.currentToken,
    );
  }
};

const innerClient = new Backend.Client(BackendBaseUrl, axiosIntance);

let ongoingRefreshTokenPromise: Promise<void> | null = null;

const refreshToken = async (token: OidcTokenSet) => {
  if (!token.refreshToken) {
    throw new Error('Missing refresh token');
  }

  const result = await refresh(oidcAuthConfig, {
    refreshToken: token.refreshToken,
  });

  if (!result.refreshToken) {
    throw new Error('Missing refresh token');
  }

  await configAuthorizationHeader({
    accessToken: result.accessToken,
    accessTokenExpirationDate: result.accessTokenExpirationDate,
    refreshToken: result.refreshToken,
  });

  ongoingRefreshTokenPromise = null;
};

export const loadAuthToken = async () => {
  const saved = await readFromSecureStore(SecureStorageKey.AuthToken);
  if (saved == null) {
    return;
  }

  await refreshToken(saved);
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

      if (
        +tokenManager.currentToken.accessTokenExpirationDate <
        Date.now() + 1000 * 60 * 60
      ) {
        // Ensure there's only one ongoing token refresh request
        ongoingRefreshTokenPromise =
          ongoingRefreshTokenPromise ?? refreshToken(tokenManager.currentToken);
      }

      await ongoingRefreshTokenPromise;

      return actual.call(innerClient, ...args);
    };
  },
});
