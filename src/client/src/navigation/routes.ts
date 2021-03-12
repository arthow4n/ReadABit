import { LinkingOptions } from '@react-navigation/native';

import { backendBaseUrl } from '../integrations/backend/backend';
import { redirectUri } from '../integrations/backend/oidcConstants';

/**
 * Route table of the whole app.
 *
 * Pass this to `react-navigation`'s `NavigationContainer`'s `linking` prop.
 */
export const linking: LinkingOptions = {
  prefixes: [backendBaseUrl, redirectUri],
  config: {
    screens: {
      RootNavigator: {
        screens: {
          HomeNavigator: {
            initialRouteName: 'Home',
            screens: {
              Home: '',
              ArticleCollectionNavigator: {
                screens: {
                  ArticleCollectionList: 'ArticleCollections/',
                  ArticleCollection: 'ArticleCollections/:id',
                },
              },
              Settings: 'Settings',
            },
          },
          ArticleCreate: 'Articles/Create',
          Article: 'Articles/:id',
        },
      },
      NoMatch: '*',
    },
  },
};
