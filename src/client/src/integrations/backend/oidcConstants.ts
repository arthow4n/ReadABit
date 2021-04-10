import { AuthConfiguration } from 'react-native-app-auth';

import { BackendBaseUrl } from '@env';
import { isDevEnv } from '@src/shared/utils/environment';

const clientId = 'ReadABit';
const redirectUrl = 'com.readabit.client.auth://';
const scopes = ['openid', 'profile', 'email', 'offline_access'];

export const oidcAuthConfig: AuthConfiguration = {
  issuer: BackendBaseUrl,
  clientId,
  scopes,
  redirectUrl,
  dangerouslyAllowInsecureHttpRequests: isDevEnv,
};

if (
  oidcAuthConfig.issuer.startsWith('http:') &&
  !oidcAuthConfig.dangerouslyAllowInsecureHttpRequests
) {
  // Early throwing custom error because this took hours to debug.
  throw new Error(
    'react-native-app-auth will crash the app directly if the issuer is HTTP while `dangerouslyAllowInsecureHttpRequests` is not true',
  );
}
