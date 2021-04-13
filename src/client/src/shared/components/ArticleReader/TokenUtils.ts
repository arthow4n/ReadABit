import { Backend } from '@src/integrations/backend/types';

export const isWord = (token: Backend.Token) =>
  !(
    token.upos === 'PUNCT' ||
    token.upos === 'NUM' ||
    token.upos === 'SYM' ||
    !token.form.trim()
  );

export const getSpacesAfter = (token: Backend.Token) =>
  (token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ?? ' ')
    .replace(/^No$/, '')
    .replace(/\\s/g, ' ')
    .replace(/\\n/g, '\n');
