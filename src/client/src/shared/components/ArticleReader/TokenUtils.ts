import { first } from 'lodash';

import { Backend } from '@src/integrations/backend/types';

export const isWord = (token: Backend.ConlluTokenViewModel) =>
  !(
    token.upos === 'PUNCT' ||
    token.upos === 'NUM' ||
    token.upos === 'SYM' ||
    !token.normalisedToken.form.trim()
  );

export const getSpacesAfter = (token: Backend.ConlluTokenViewModel) =>
  (
    token.sparvPipelineMisc?.tail ??
    token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ??
    ' '
  )
    .replace(/^No$/, '')
    .replace(/\\s/g, ' ')
    .replace(/\\n/g, '\n');

export const getCompoundOrLemmaForTranslation = (
  token: Backend.ConlluTokenViewModel,
) =>
  first(token.sparvPipelineMisc?.compwf)?.join(' + ') ??
  token.normalisedToken.lemma;
