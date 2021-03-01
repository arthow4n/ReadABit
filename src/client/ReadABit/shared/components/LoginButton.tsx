import * as React from "react";
import {
  useAutoDiscovery,
  useAuthRequest,
  makeRedirectUri,
} from "expo-auth-session";

import { View, Button, Text } from "react-native";
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
