import React from 'react';

import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import produce from 'immer';
import { Body, Button, Card, CardItem, Text } from 'native-base';

import { useLinkTo, useNavigation } from '@react-navigation/native';
import { api } from '@src/integrations/backend/backend';
import { Routes, routeUrl } from '@src/navigation/routes';
import { wordFamiliarityLevelColorCodeMapping } from '@src/shared/constants/colorCode';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import {
  queryCacheKey,
  QueryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

import {
  useSelectedTokenDefinitionCardHandle,
  useWordTokenHandle,
} from './ArticleReaderRenderContext';

export const SelectedTokenDefinitionCard: React.FC = () => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const { appSettings } = useAppSettingsContext();
  const navigation = useNavigation();

  const {
    articleLanguageCode,
    getSelectedToken,
    updateWordFamiliarity,
  } = useSelectedTokenDefinitionCardHandle();
  const selectedToken = getSelectedToken();
  const { wordFamiliarityItem } = useWordTokenHandle(selectedToken?.form || '');

  const wordDefinitionsListQuery = useQuery(
    queryCacheKey(QueryCacheKey.WordDefinitionList, {
      filter_Word_Expression: selectedToken?.form ?? '',
      filter_Word_LanguageCode: articleLanguageCode,
    }),
    () => {
      if (!selectedToken) {
        throw new Error();
      }

      return api.wordDefinitions_List({
        filter_Word_Expression: selectedToken.form,
        filter_Word_LanguageCode: articleLanguageCode,
      });
    },
    {
      enabled: !!selectedToken,
    },
  );

  // Refetch word definition when navigating back from other screens,
  // like `WordDefinitionsDictionaryLookupScreen`.
  React.useEffect(() => {
    const refetchCallback = () => {
      wordDefinitionsListQuery.refetch();
      navigation.removeListener('focus', refetchCallback);
    };

    const blurCleanup = navigation.addListener('blur', () => {
      navigation.addListener('focus', refetchCallback);
    });

    return () => {
      blurCleanup();
      navigation.removeListener('focus', refetchCallback);
    };
  }, [wordDefinitionsListQuery.refetch]);

  if (!selectedToken) {
    return null;
  }

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

  const definitionListItems = wordDefinitionsListQuery.data?.items ?? [];

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
