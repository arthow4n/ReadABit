import * as React from 'react';

import {
  useAutoDiscovery,
  useAuthRequest,
  ResponseType,
  exchangeCodeAsync,
} from 'expo-auth-session';

import { BackendBaseUrl } from '@env';

import {
  configAuthorizationHeader,
  useBackendApiTokenState,
} from '../../integrations/backend/backend';
import {
  clientId,
  redirectUri,
  scopes,
} from '../../integrations/backend/oidcConstants';

export const useBackendLogin = () => {
  const { hasValidToken } = useBackendApiTokenState();

  // TODO: Migrate to `react-native-app-auth` because this is working weird after expo eject
  const discovery = useAutoDiscovery(BackendBaseUrl);
  const [authRequest, authResult, promptAsync] = useAuthRequest(
    {
      clientId,
      redirectUri,
      scopes,
      responseType: ResponseType.Code,
    },
    discovery,
  );

  const exchangeCodeForToken = async () => {
    if (authResult?.type !== 'success' || !discovery) {
      return;
    }

    if (!authRequest?.codeVerifier) {
      throw new Error(
        'Missing `code_verifier` from auth request. This is required for PKCE flow.',
      );
    }

    if (!authResult.params.code) {
      throw new Error(
        'Missing `code` from auth response. This is required for PKCE flow.',
      );
    }

    const tokenResponse = await exchangeCodeAsync(
      {
        clientId,
        redirectUri,
        code: authResult.params.code,
        scopes,
        extraParams: {
          code_verifier: authRequest.codeVerifier,
        },
      },
      discovery,
    );

    await configAuthorizationHeader(tokenResponse);
  };

  React.useEffect(() => {
    exchangeCodeForToken();
  }, [authResult]);

  return {
    gotoLoginPage: () => {
      promptAsync();
    },
    isLoggedIn: !!hasValidToken,
  };
};
