import { authorize } from 'react-native-app-auth';

import {
  configAuthorizationHeader,
  useBackendApiTokenState,
} from '../../integrations/backend/backend';
import { oidcAuthConfig } from '../../integrations/backend/oidcConstants';

export const useBackendLogin = () => {
  const { hasValidToken } = useBackendApiTokenState();

  return {
    gotoLoginPage: async () => {
      const result = await authorize({
        ...oidcAuthConfig,
        dangerouslyAllowInsecureHttpRequests: true,
      });

      configAuthorizationHeader(result);
    },
    isLoggedIn: !!hasValidToken,
  };
};
