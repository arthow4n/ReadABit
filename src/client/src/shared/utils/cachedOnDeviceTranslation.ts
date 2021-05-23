import mlkitTranslate from 'react-native-mlkit-translate';

import { getAppSettings } from '../contexts/AppSettingsContext';

type SourceLanguageCode = string;
type TargetLanguageCode = string;
type WordExpression = string;
type TranslationResult = string;

const translationCache = new Map<
  SourceLanguageCode,
  Map<TargetLanguageCode, Map<WordExpression, TranslationResult>>
>();

export const getFromTranslationCache = (
  sourceLanguageCode: string,
  targetLanguageCode: string,
  wordExpression: string,
) =>
  translationCache
    .get(sourceLanguageCode)
    ?.get(targetLanguageCode)
    ?.get(wordExpression);

export const writeToTranslationCache = (
  sourceLanguageCode: string,
  targetLanguageCode: string,
  wordExpression: string,
  translationResult: string,
) => {
  let s = translationCache.get(sourceLanguageCode);
  if (!s) {
    s = new Map<TargetLanguageCode, Map<WordExpression, TranslationResult>>();
    translationCache.set(sourceLanguageCode, s);
  }

  let t = s.get(targetLanguageCode);
  if (!t) {
    t = new Map<WordExpression, TranslationResult>();
    s.set(sourceLanguageCode, t);
  }

  t.set(wordExpression, translationResult);
};

export const getTranslation = async (
  sourceLanguageCode: string,
  targetLanguageCode: string,
  wordExpression: string,
) => {
  const cached = getFromTranslationCache(
    sourceLanguageCode,
    targetLanguageCode,
    wordExpression,
  );
  if (cached) {
    return cached;
  }

  const result = await mlkitTranslate
    .translate(
      wordExpression,
      sourceLanguageCode,
      targetLanguageCode,
      getAppSettings().saveDataUsage,
    )
    .catch(() => wordExpression);

  writeToTranslationCache(
    sourceLanguageCode,
    targetLanguageCode,
    wordExpression,
    result,
  );

  return result;
};

let currentWarmUpWorkSet = new Set<string>();

export const enqueueTranslationWarmUp = (
  sourceLanguageCode: string,
  workSet: Set<string>,
) => {
  currentWarmUpWorkSet = workSet;

  (async () => {
    // eslint-disable-next-line no-restricted-syntax
    for (const wordExpression of workSet) {
      if (currentWarmUpWorkSet !== workSet) {
        return;
      }

      await getTranslation(
        sourceLanguageCode,
        getAppSettings().languageCodes.ui,
        wordExpression,
      );

      // TODO: Try and see if idle callback yields better result.
      await new Promise((r) => setTimeout(r, 17));
    }
  })();
};
