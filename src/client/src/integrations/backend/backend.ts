import React from 'react';

import { refresh } from 'react-native-app-auth';

import axios from 'axios';

import { BackendBaseUrl } from '@env';
import {
  readFromSecureStore,
  SecureStorageKey,
  writeToSecureStore,
} from '@src/shared/utils/storage';

import { oidcAuthConfig } from './oidcConstants';
import {
  tokenManager,
  OidcTokenSetStringified,
  OidcTokenSet,
} from './tokenManager';
import { Backend } from './types';

const axiosIntance = axios.create();
// Get rid of the default transform because it overrides NSWag's transform.
axiosIntance.defaults.transformResponse = [];

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
  newToken: OidcTokenSetStringified,
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

  await configAuthorizationHeader({
    accessToken: result.accessToken,
    accessTokenExpirationDate: result.accessTokenExpirationDate,
    refreshToken:
      result.refreshToken ??
      tokenManager.currentToken?.refreshToken ??
      (() => {
        throw new Error('Missing refresh token');
      })(),
  });

  ongoingRefreshTokenPromise = null;
};

export const loadAuthToken = async () => {
  const result = await readFromSecureStore(SecureStorageKey.AuthToken);
  if (!result) {
    return;
  }

  await configAuthorizationHeader(result);
};

// This is wrapped in a function mainly because if we don't wrap this object,
// something related to React fast refresh will call into `.$$typeof`
// of every exported member in a module, which would then calls into the Proxy and
// cause weird crash that could take hours to debug.
export const api = () =>
  new Proxy(innerClient, {
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
            ongoingRefreshTokenPromise ??
            refreshToken(tokenManager.currentToken);
        }

        await ongoingRefreshTokenPromise;

        return actual.call(innerClient, ...args);
      };
    },
  });
