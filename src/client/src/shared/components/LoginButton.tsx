import * as React from 'react';

import { useTranslation } from 'react-i18next';
import { Button } from 'react-native';
import { useQuery } from 'react-query';

import {
  useAutoDiscovery,
  useAuthRequest,
  ResponseType,
  exchangeCodeAsync,
} from 'expo-auth-session';

import { View, Text } from './Themed';

import {
  api,
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
  const getUserInfoQueryHandle = useQuery(
    ['articles_GetUserInfo'],
    () => api.articles_GetUserInfo(),
    {
      enabled: isTokenReady,
      onSuccess: (data) => {
        console.log(data);
      },
    },
  );

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
    return (
      <View>
        <Text>getUserInfoQueryHandle</Text>
        <Text>{JSON.stringify(getUserInfoQueryHandle)}</Text>
      </View>
    );
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
