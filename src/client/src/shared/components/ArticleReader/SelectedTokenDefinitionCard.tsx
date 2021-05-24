import React from 'react';

import { ScrollView } from 'react-native';
import { useQuery } from 'react-query';

import produce from 'immer';
import { Button, Col, Grid, Icon, Row, Text, View } from 'native-base';

import { useLinkTo } from '@react-navigation/native';
import { api } from '@src/integrations/backend/backend';
import { Routes, routeUrl } from '@src/navigation/routes';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import {
  queryCacheKey,
  QueryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';
import { useNavigationRefocusEffect } from '@src/shared/hooks/useNavigationRefocusEffect';

import {
  useSelectedTokenDefinitionCardHandle,
  useWordTokenHandle,
} from './ArticleReaderRenderContext';
import { OnDeviceTranslation } from './OnDeviceTranslation';
import { getCompoundAndLemmaForTranslation } from './TokenUtils';

export const SelectedTokenDefinitionCard: React.FC = () => {
  const linkTo = useLinkTo();
  const { appSettings } = useAppSettingsContext();

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

      return api().wordDefinitions_List({
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
  useNavigationRefocusEffect(wordDefinitionsListQuery.refetch);

  if (!selectedToken) {
    return null;
  }

  const renderMainWordHeader = () => (
    <Text
      style={{
        fontSize: 20,
        width: '100%',
      }}
    >
      {`${selectedToken.form} (${getCompoundAndLemmaForTranslation(
        selectedToken,
      )})`}
    </Text>
  );

  const definitionListItems = wordDefinitionsListQuery.data?.items ?? [];

  const gotoDictionaryLookUp = () =>
    linkTo(
      routeUrl(Routes.WordDefinitionsDictionaryLookup, null, {
        wordsJson: JSON.stringify([selectedToken.form]),
        wordLanguageCode: wordFamiliarityItem.word.languageCode,
        // TODO: Allow choosing another dictionary language
        dictionaryLanguageCode: appSettings.languageCodes.ui,
        wordDefinitionId: definitionListItems[0]?.id,
      }),
    );

  return (
    <ScrollView
      style={{
        flex: 1,
        borderTopWidth: 4,
        borderColor: '#000',
        marginTop: 4,
        padding: 4,
      }}
    >
      <View>
        <View>{renderMainWordHeader()}</View>
        <View>
          {/* TODO: Load public suggestions */}
          <OnDeviceTranslation
            sourceLanguageCode={articleLanguageCode}
            sourceText={selectedToken.form}
            sourceText2={getCompoundAndLemmaForTranslation(selectedToken)}
            targetLanguageCode={appSettings.languageCodes.ui}
          />
          {/* TODO: Show all the available word definitions in a scrollable block */}
          <View>
            <Text style={{ fontSize: 24 }}>
              {definitionListItems[0]?.meaning}
            </Text>
          </View>
          {/* TODO: Render token PoS tags */}
        </View>
        <Grid>
          <Row>
            <Col>
              <Button style={{ height: 100 }} onPress={gotoDictionaryLookUp}>
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
    </ScrollView>
  );
};
