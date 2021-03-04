import 'react-i18next';

import { resources } from '../../translations/init';

declare module 'react-i18next' {
  type DefaultResources = typeof resources['en'];
  interface Resources extends DefaultResources {}
}
