import { LinkingOptions } from '@react-navigation/native';

import { backendBaseUrl } from '../integrations/backend/backend';
import { redirectUri } from '../integrations/backend/oidcConstants';

export enum Routes {
  Login = 'Login',
  Home = 'Home',
  Settings = 'Settings',
  ArticleCollectionList = 'ArticleCollectionList',
  ArticleCollection = 'ArticleCollection',
  Article = 'Article',
  ArticleCreate = 'ArticleCreate',
  WordDefinitionsDictionaryLookup = 'WordDefinitionsDictionaryLookup',
}

const exactPathMapping: Record<Routes, string> = {
  [Routes.Home]: '',
  [Routes.Login]: 'Login/',
  [Routes.Settings]: 'Settings/',
  [Routes.ArticleCollectionList]: 'ArticleCollections/',
  [Routes.ArticleCollection]: 'ArticleCollections/:id',
  [Routes.Article]: 'Articles/:id',
  [Routes.ArticleCreate]: 'Articles/Create',
  [Routes.WordDefinitionsDictionaryLookup]: 'WordDefinitions/DictionaryLookUp',
};

export function routeUrl(
  route: Routes.Article,
  routeParams: { id: string },
): string;
export function routeUrl(route: Routes.ArticleCreate): string;
export function routeUrl(
  route: Routes.WordDefinitionsDictionaryLookup,
  routeParams: null,
  queryParams: {
    word: string;
    wordLanguage: string;
    dictionaryLanguage: string;
  },
): string;
export function routeUrl(
  route: Routes,
  routeParams?: Record<string, string> | null,
  queryParams?: Record<string, string> | null,
) {
  let path = exactPathMapping[route];

  Object.entries(routeParams ?? {}).forEach(([key, value]) => {
    path = path.replace(`:${key}`, encodeURIComponent(value));
  });

  const query = Object.entries(queryParams ?? {})
    .map(
      ([key, value]) =>
        `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
    )
    .join('&');

  return `/${path}${query ? `?${query}` : ''}`;
}

/**
 * Route table of the whole app.
 *
 * Pass this to `react-navigation`'s `NavigationContainer`'s `linking` prop.
 */
export const linking: LinkingOptions = {
  prefixes: [backendBaseUrl, redirectUri],
  config: {
    screens: {
      LoginNavigator: {
        initialRouteName: Routes.Login,
        screens: {
          [Routes.Login]: {
            path: exactPathMapping[Routes.Login],
            exact: true,
          },
        },
      },
      HomeNavigator: {
        screens: {
          [Routes.Home]: {
            path: exactPathMapping[Routes.Home],
            exact: true,
          },
          ArticleCollectionNavigator: {
            screens: {
              [Routes.ArticleCollectionList]: {
                path: exactPathMapping[Routes.ArticleCollectionList],
                exact: true,
              },
              [Routes.ArticleCollection]: {
                path: exactPathMapping[Routes.ArticleCollection],
                exact: true,
              },
            },
          },
          [Routes.Settings]: {
            path: exactPathMapping[Routes.Settings],
            exact: true,
          },
        },
      },
      ArticleNavigator: {
        screens: {
          [Routes.ArticleCreate]: {
            path: exactPathMapping[Routes.ArticleCreate],
            exact: true,
          },
          [Routes.Article]: {
            path: exactPathMapping[Routes.Article],
            exact: true,
          },
          [Routes.WordDefinitionsDictionaryLookup]: {
            path: exactPathMapping[Routes.WordDefinitionsDictionaryLookup],
            exact: true,
          },
        },
      },
      NoMatch: '*',
    },
  },
};
