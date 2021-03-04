import { initReactI18next } from 'react-i18next';

import i18n from 'i18next';

import enCore from './en/core.json';
import zhTWCore from './zh-TW/core.json';

export const resources = {
  en: {
    core: enCore,
  },
  'zh-TW': {
    core: zhTWCore,
  },
};

i18n.use(initReactI18next).init({
  resources,
  // TODO: Support multiple language
  lng: 'en',
  fallbackLng: 'en',
  defaultNS: 'core',
  fallbackNS: 'core',
  // These should match `i18next-scanner.config.js`
  keySeparator: false,
  nsSeparator: false,
  interpolation: {
    escapeValue: false, // Unneeded because of React.
  },
});
