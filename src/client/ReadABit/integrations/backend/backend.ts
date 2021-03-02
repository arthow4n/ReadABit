import axios from "axios";
import {
  DiscoveryDocument,
  fetchDiscoveryAsync,
  TokenResponse,
} from "expo-auth-session";
import Constants from "expo-constants";
import * as SecureStore from "expo-secure-store";
import { authTokenStorageKey, clientId, scopes } from "./oidcConstants";
import { Backend } from "./types";

// TODO: Make this compatible with production env.
export const backendBaseUrl = `http://${
  (Constants.manifest.debuggerHost ?? "localhost").split(":")[0]
}:5000`;

const axiosIntance = axios.create();
let tokenResponse: TokenResponse | null = null;

export const configAuthorizationHeader = async (
  t: TokenResponse,
  shouldSave = true,
) => {
  tokenResponse = t;
  axiosIntance.defaults.headers.common[
    "Authorization"
  ] = `Bearer ${t.accessToken}`;

  if (!SecureStore.isAvailableAsync()) {
    console.warn("SecureStore is unavailable. Auth tokens won't be saved.");
    return;
  }

  if (shouldSave) {
    await SecureStore.setItemAsync(
      authTokenStorageKey,
      JSON.stringify(tokenResponse),
    );
  }
};

export const tryLoadingAuthToken = async () => {
  const storedJson = await SecureStore.getItemAsync(authTokenStorageKey);
  if (!storedJson) {
    return;
  }
  configAuthorizationHeader(JSON.parse(storedJson), false);
};

const innerClient = new Backend.Client(backendBaseUrl, axiosIntance);

// FIXME: Fix the template of types.ts instead of doing it like this.
export const api = new Proxy(innerClient, {
  get(t, p, r) {
    const actual = Reflect.get(t, p, r);

    // Assuming every proprty that has a `_` in it and doesn't start with `process` is an API call and filter them away, this is not cool, I know.
    if (!(typeof p === "string" && /^(?!process).+_.+/.test(p))) {
      return actual;
    }

    if (typeof actual !== "function") {
      throw new Error("Please fix the hack in backend.ts.");
    }

    // Refresh the tokens before sending actual the API call.
    return async (...args: any[]) => {
      if (tokenResponse === null) {
        throw new Error(
          "Calling backend API without valid token, did you forget to call `configAuthorizationHeader`?",
        );
      }

      // TODO: Confirm the refresh flow actually works
      if (tokenResponse.shouldRefresh()) {
        const discovery = await fetchDiscoveryAsync(backendBaseUrl);
        const newTokenResponse = await tokenResponse.refreshAsync(
          {
            clientId,
            scopes,
          },
          discovery,
        );

        await configAuthorizationHeader(newTokenResponse);
      }

      return actual.call(innerClient, ...args);
    };
  },
});
