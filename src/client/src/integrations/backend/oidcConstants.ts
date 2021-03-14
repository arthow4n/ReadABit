import { makeRedirectUri } from 'expo-auth-session';

export const clientId = 'ReadABit';
export const redirectUri = makeRedirectUri();
export const scopes = ['openid', 'profile', 'email', 'offline_access'];
