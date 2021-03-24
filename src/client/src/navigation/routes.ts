import { LinkingOptions } from '@react-navigation/native';

import { backendBaseUrl } from '../integrations/backend/backend';
import { redirectUri } from '../integrations/backend/oidcConstants';

// Read the GUIDE: comments for how to add a new route to the app.

// GUIDE:
// Step 1: Add a new value to `Routes`.
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

// GUIDE:
// Step 2: Add corresponding URL path to `exactPathMapping`
// The URL should not start with a slash `/`.
// Use `:` in the URL for parameters that needs to be injected.
// Query params don't need to be marked here
// as they are automatically injected by `react-navigation`.
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

// GUIDE:
// Step 3: Add the new route to the route table below (`linking`).
// Don't forget to create/edit the corresponding navigator and screen files.

/**
 * This is the route table of the whole app.
 * Format: https://reactnavigation.org/docs/configuring-links/
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

// GUIDE:
// Step 4: Add an overload to `routeUrl`
// so we have a standardised and type-safe way for generating URL.
/**
 * @example
 * const linkTo = useLinkTo();
 * // Navigating away to a specific route.
 * <Button onPress={() => linkTo(routeUrl(...))} />
 */
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
