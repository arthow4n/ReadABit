import * as React from 'react';

import mlkitTranslate from 'react-native-mlkit-translate';

import { Text, View } from 'native-base';

import TranslatedByGoogle from '@src/assets/images/translatedByGoogle.svg';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';

export const OnDeviceTranslation: React.FC<{
  sourceLanguageCode: string;
  sourceText: string;
  sourceText2?: string;
  targetLanguageCode: string;
}> = ({ sourceLanguageCode, sourceText, sourceText2, targetLanguageCode }) => {
  const { appSettings } = useAppSettingsContext();

  // These two are undefined when loading.
  const [translationResult, setTranslationResult] = React.useState<string>();
  const [translationResult2, setTranslationResult2] = React.useState<string>();

  React.useEffect(() => {
    setTranslationResult(undefined);
    setTranslationResult2(undefined);

    (async () => {
      const result = await mlkitTranslate.translate(
        sourceText,
        sourceLanguageCode,
        targetLanguageCode,
        !appSettings.useMobileDataForAllDataTransfer,
      );
      setTranslationResult(result);
    })();

    (async () => {
      if (!sourceText2) {
        setTranslationResult2('');
        return;
      }
      const result = await mlkitTranslate.translate(
        sourceText2,
        sourceLanguageCode,
        targetLanguageCode,
        !appSettings.useMobileDataForAllDataTransfer,
      );
      setTranslationResult2(result);
    })();
  }, [
    sourceText,
    sourceText2,
    sourceLanguageCode,
    targetLanguageCode,
    appSettings.useMobileDataForAllDataTransfer,
  ]);

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
        {/* This logo is required by the usage guidelines of ML Kit on-device translation. */}
        {/* https://developers.google.com/ml-kit/language/translation/translation-terms */}
        <TranslatedByGoogle width={152} height={20} />
      </Text>
    </View>
  );
};
