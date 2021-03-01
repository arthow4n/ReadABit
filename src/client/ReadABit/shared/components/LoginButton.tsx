import * as React from "react";
import { View, Button, Text } from "react-native";
import {
  useAutoDiscovery,
  useAuthRequest,
  makeRedirectUri,
} from "expo-auth-session";
import * as SecureStore from "expo-secure-store";

import { backendBaseUrl } from "../../integrations/backend/backend";

export const LoginButton: React.FC = () => {
  const discovery = useAutoDiscovery(backendBaseUrl);
  const [request, result, promptAsync] = useAuthRequest(
    {
      clientId: "ReadABit",
      redirectUri: makeRedirectUri(),
      scopes: ["openid", "profile", "email", "offline_access"],
    },
    discovery,
  );

  React.useEffect(() => {
    if (result?.type !== "success" || !SecureStore.isAvailableAsync()) {
      return;
    }

    // TODO: Look into what's being returned in params and choose what to save.
    // At least the refresh token is needed?
    // SecureStore.setItemAsync(
    //   "READABIT_OIDC_TOKENS",
    //   JSON.stringify(result.params),
    // );

    // TODO: Apply the auth token to API client, consider using `axios`.
    // TODO: Use refresh token
  }, [!!result]);

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
