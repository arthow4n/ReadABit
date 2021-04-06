import { Backend } from '@src/integrations/backend/types';

export const isWord = (token: Backend.Token) =>
  !(token.upos === 'PUNCT' || !token.form.trim());
