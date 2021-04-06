import React from 'react';

import { useQuery } from 'react-query';

import produce from 'immer';
import { Button, Col, Grid, Icon, Row, Text, View } from 'native-base';

import { useLinkTo, useNavigation } from '@react-navigation/native';
import { api } from '@src/integrations/backend/backend';
import { Routes, routeUrl } from '@src/navigation/routes';
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
  const linkTo = useLinkTo();
  const { appSettings } = useAppSettingsContext();
  const navigation = useNavigation();

  const {
    articleLanguageCode,
    getSelectedToken,
    updateWordFamiliarity,
  } = useSelectedTokenDefinitionCardHandle();
  const selectedToken = getSelectedToken();
  const { wordFamiliarityItem } = useWordTokenHandle(selectedToken);

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
        fontSize: 28,
        width: '100%',
      }}
    >
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
    <View
      style={{ flex: 1, borderTopWidth: 4, borderColor: '#000', marginTop: 4 }}
    >
      <Grid>
        <Row>{renderMainWordHeader()}</Row>
        <Row>
          {/* TODO: Load public suggestions */}
          {/* TODO: Load ML Kit on-device translation as one of the suggestion. Ref: https://developers.google.com/ml-kit/language/translation */}
          {/* TODO: Show all the available word definitions in a scrollable block */}
          <Text style={{ fontSize: 28 }}>
            {definitionListItems[0]?.meaning}
          </Text>
          {/* TODO: Render token PoS tags */}
        </Row>
        <Row>
          <Col>
            <Button onPress={gotoDictionaryLookUp}>
              <Icon name="search-circle-outline" />
            </Button>
          </Col>
          {[0, 1, 2, 3, -1].map((level) => (
            <Col
              key={level}
              // @ts-ignore
              onPress={() => {
                updateWordFamiliarity(
                  produce(wordFamiliarityItem, (draft) => {
                    draft.level = level;
                  }),
                );
              }}
            >
              <View
                style={{
                  flex: 1,
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
              >
                <Text
                  style={{
                    color:
                      wordFamiliarityItem.level === level
                        ? '#FF0000'
                        : '#000000',
                  }}
                >
                  {level}
                </Text>
              </View>
            </Col>
          ))}
        </Row>
      </Grid>
    </View>
  );
};
