import { Backend } from '@src/integrations/backend/types';

export const isWord = (token: Backend.Token) =>
  !(
    token.upos === 'PUNCT' ||
    token.upos === 'NUM' ||
    token.upos === 'SYM' ||
    !token.form.trim()
  );
