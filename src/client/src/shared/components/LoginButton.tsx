import * as React from 'react';

import { useTranslation } from 'react-i18next';
import { Button, View } from 'react-native';

import {
  useAutoDiscovery,
  useAuthRequest,
  ResponseType,
  exchangeCodeAsync,
} from 'expo-auth-session';

import {
  backendBaseUrl,
  configAuthorizationHeader,
} from '../../integrations/backend/backend';
import {
  clientId,
  redirectUri,
  scopes,
} from '../../integrations/backend/oidcConstants';

export const LoginButton: React.FC = () => {
  const { t } = useTranslation();
  const [isTokenReady, setIsTokenReady] = React.useState(false);

  const discovery = useAutoDiscovery(backendBaseUrl);
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
    setIsTokenReady(true);
  };

  React.useEffect(() => {
    exchangeCodeForToken();
  }, [authResult]);

  if (isTokenReady) {
    return null;
  }

  return (
    <View>
      <Button
        title={t('Login')}
        disabled={!authRequest}
        onPress={() => promptAsync()}
      />
    </View>
  );
};
