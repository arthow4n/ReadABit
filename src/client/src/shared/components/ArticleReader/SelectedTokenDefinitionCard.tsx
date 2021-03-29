import React from 'react';

import { useTranslation } from 'react-i18next';

import produce from 'immer';
import { Body, Button, Card, CardItem, Text } from 'native-base';

import { useLinkTo } from '@react-navigation/native';
import { Backend } from '@src/integrations/backend/types';
import { Routes, routeUrl } from '@src/navigation/routes';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import { useWordFamiliarityContext } from '@src/shared/contexts/WordFamiliarityContext';

export const SelectedTokenDefinitionCard: React.FC<{
  definitionListItems: Backend.WordDefinition[];
  selectedToken: Backend.Token;
  wordFamiliarityItem: Backend.WordFamiliarityListItemViewModel;
}> = ({ definitionListItems, selectedToken, wordFamiliarityItem }) => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const { updateWordFamiliarity } = useWordFamiliarityContext();
  const { appSettings } = useAppSettingsContext();

  const renderMainWordHeader = () => (
    <Text
      style={{
        width: '100%',
        backgroundColor:
          wordFamiliarityLevelColorCodeMapping[
            wordFamiliarityItem.level.toString()
          ],
      }}
      onPress={() => {
        updateWordFamiliarity(
          produce(wordFamiliarityItem, (draft) => {
            draft.level += 1;
            if (draft.level > 3) {
              draft.level = -1;
            }
          }),
        );
      }}
    >
      {/* TODO: An icon indicating this is clickable for changing familiarity status. */}
      {/* TODO: Mark as known button. */}
      {`${selectedToken.form} (${selectedToken.lemma})`}
    </Text>
  );

  const gotoDictionaryLookUp = () =>
    linkTo(
      routeUrl(Routes.WordDefinitionsDictionaryLookup, null, {
        word: selectedToken.form,
        wordLanguageCode: wordFamiliarityItem.word.languageCode,
        // TODO: Allow choosing another dictionary language
        dictionaryLanguageCode: appSettings.languageCodes.ui,
        wordDefinitionId: definitionListItems[0]?.id,
      }),
    );

  return (
    <Card style={{ flex: 1 }}>
      <CardItem>
        <Body>
          {renderMainWordHeader()}
          {/* TODO: Load public suggestions */}
          {/* TODO: Load ML Kit on-device translation as one of the suggestion. Ref: https://developers.google.com/ml-kit/language/translation */}
          {/* TODO: Show all the available word definitions in a scrollable block */}
          <Text>{definitionListItems[0]?.meaning}</Text>
          {/* TODO: Render token PoS tags */}
          <Button onPress={gotoDictionaryLookUp}>
            <Text>{t('Dictionary')}</Text>
          </Button>
        </Body>
      </CardItem>
    </Card>
  );
};
