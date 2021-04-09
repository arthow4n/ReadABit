import { BackendBaseUrl } from '@env';

const clientId = 'ReadABit';
const redirectUrl = 'com.readabit.client.auth://';
const scopes = ['openid', 'profile', 'email', 'offline_access'];

export const oidcAuthConfig = {
  issuer: BackendBaseUrl,
  clientId,
  scopes,
  redirectUrl,
};
