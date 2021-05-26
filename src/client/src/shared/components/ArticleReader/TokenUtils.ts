import { first } from 'lodash';

import { Backend } from '@src/integrations/backend/types';

export const isWord = (token: Backend.ConlluTokenViewModel) =>
  !(
    token.upos === 'PUNCT' ||
    (token.upos === 'NUM' && /[0-9]/.test(token.normalisedToken.form)) ||
    token.upos === 'SYM' ||
    !token.normalisedToken.form.trim()
  );

export const isProperNoun = (token: Backend.ConlluTokenViewModel) =>
  token.upos === 'PROPN';

export const getSpacesAfter = (token: Backend.ConlluTokenViewModel) =>
  (
    token.sparvPipelineMisc?.tail ??
    token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ??
    ' '
  )
    .replace(/^No$/, '')
    .replace(/\\s/g, ' ')
    .replace(/\\n/g, '\n');

export const getCompoundAndLemmaForTranslation = (
  token: Backend.ConlluTokenViewModel,
) => {
  const compound = first(token.sparvPipelineMisc?.compwf)?.join(' + ');
  const { lemma } = token.normalisedToken;

  // Lemma is rendered along with compound
  // because sometimes the quality of compound analysis isn't that good.
  return compound ? `${lemma}, ${compound}` : lemma;
};
