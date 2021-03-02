import { makeRedirectUri } from "expo-auth-session";

export const clientId = "ReadABit";
export const authTokenStorageKey = "READABIT.OIDC_TOKEN_RESPONSE";
export const redirectUri = makeRedirectUri();
export const scopes = ["openid", "profile", "email", "offline_access"];
