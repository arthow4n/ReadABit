import * as React from "react";
import { View, Button, Text } from "react-native";
import {
  useAutoDiscovery,
  useAuthRequest,
  ResponseType,
  exchangeCodeAsync,
} from "expo-auth-session";

import {
  backendBaseUrl,
  configAuthorizationHeader,
} from "../../integrations/backend/backend";
import {
  clientId,
  redirectUri,
  scopes,
} from "../../integrations/backend/oidcConstants";

export const LoginButton: React.FC = () => {
  const discovery = useAutoDiscovery(backendBaseUrl);
  const [request, result, promptAsync] = useAuthRequest(
    {
      clientId,
      redirectUri,
      scopes,
      responseType: ResponseType.Code,
    },
    discovery,
  );

  const exchangeCodeForToken = async () => {
    if (result?.type !== "success" || !discovery) {
      return;
    }

    if (!request?.codeVerifier) {
      throw new Error(
        "Missing `code_verifier` from auth request. This is required for PKCE flow.",
      );
    }

    if (!result.params.code) {
      throw new Error(
        "Missing `code` from auth response. This is required for PKCE flow.",
      );
    }

    const tokenResponse = await exchangeCodeAsync(
      {
        clientId,
        redirectUri,
        code: result.params.code,
        scopes: scopes,
        extraParams: {
          code_verifier: request.codeVerifier,
        },
      },
      discovery,
    );

    await configAuthorizationHeader(tokenResponse);
  };

  React.useEffect(() => {
    exchangeCodeForToken();
  }, [result]);

  return (
    <View>
      <Button
        title="Login!"
        disabled={!request}
        onPress={() => promptAsync()}
      />
      {result && <Text>{JSON.stringify(result, null, 2)}</Text>}
    </View>
  );
};
