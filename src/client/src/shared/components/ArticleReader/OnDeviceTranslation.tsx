import * as React from 'react';

import { Text, View } from 'native-base';

import TranslatedByGoogle from '@src/assets/images/translatedByGoogle.svg';
import {
  getFromTranslationCache,
  getTranslation,
} from '@src/shared/utils/cachedOnDeviceTranslation';

export const OnDeviceTranslation: React.FC<{
  sourceLanguageCode: string;
  sourceText: string;
  sourceText2?: string;
  targetLanguageCode: string;
}> = ({ sourceLanguageCode, sourceText, sourceText2, targetLanguageCode }) => {
  const cachedTranslationResult = getFromTranslationCache(
    sourceLanguageCode,
    targetLanguageCode,
    sourceText,
  );
  const cachedTranslationResult2 =
    sourceText2 &&
    getFromTranslationCache(
      sourceLanguageCode,
      targetLanguageCode,
      sourceText2,
    );

  const [
    delayedTranslationResult,
    setDelayedTranslationResult,
  ] = React.useState<string>();
  const [
    delayedTranslationResult2,
    setDelayedTranslationResult2,
  ] = React.useState<string>();

  React.useEffect(() => {
    (async () => {
      if (cachedTranslationResult) {
        return;
      }
      setDelayedTranslationResult(
        await getTranslation(
          sourceLanguageCode,
          targetLanguageCode,
          sourceText,
        ),
      );
    })();

    (async () => {
      if (!sourceText2) {
        setDelayedTranslationResult2('');
        return;
      }
      if (cachedTranslationResult2) {
        return;
      }
      setDelayedTranslationResult2(
        await getTranslation(
          sourceLanguageCode,
          targetLanguageCode,
          sourceText2,
        ),
      );
    })();
  }, [
    sourceLanguageCode,
    targetLanguageCode,
    sourceText,
    sourceText2,
    cachedTranslationResult,
    cachedTranslationResult2,
  ]);

  const translationResult = cachedTranslationResult ?? delayedTranslationResult;
  const translationResult2 =
    cachedTranslationResult2 ?? delayedTranslationResult2;

  // Wait for loading because otherwise the translation logo might be rendered in a weird place.
  if (translationResult === undefined || translationResult2 === undefined) {
    return <Text style={{ fontSize: 20 }}> </Text>;
  }

  return (
    <View>
      <Text>
        <Text style={{ marginRight: 16, fontSize: 20 }}>
          {`${translationResult} `}
        </Text>
        {!!translationResult2 && (
          <Text style={{ marginRight: 16, fontSize: 20 }}>
            {`(${translationResult2}) `}
          </Text>
        )}
      </Text>
      <View style={{ marginTop: 4 }}>
        {/* This logo is required by the usage guidelines of ML Kit on-device translation. */}
        {/* https://developers.google.com/ml-kit/language/translation/translation-terms */}
        <TranslatedByGoogle width={152} height={20} />
      </View>
    </View>
  );
};
