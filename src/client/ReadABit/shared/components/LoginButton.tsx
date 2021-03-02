import * as React from "react";
import { View, Button, Text } from "react-native";
import {
  useAutoDiscovery,
  useAuthRequest,
  makeRedirectUri,
  ResponseType,
  exchangeCodeAsync,
} from "expo-auth-session";
import * as SecureStore from "expo-secure-store";

import { backendBaseUrl } from "../../integrations/backend/backend";

const clientId = "ReadABit";
const authTokenStorageKey = "READABIT.OIDC_TOKEN_RESPONSE";
const redirectUri = makeRedirectUri();
const scopes = ["openid", "profile", "email", "offline_access"];

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

    if (!SecureStore.isAvailableAsync()) {
      console.warn("SecureStore is unavailable. Auth tokens won't be saved.");
      return;
    }

    await SecureStore.setItemAsync(
      authTokenStorageKey,
      JSON.stringify(tokenResponse),
    );

    // TODO: Apply the auth token to API client, consider using `axios`.
    // TODO: Use refresh token.
    // TODO: Load auth tokens on app load.
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
