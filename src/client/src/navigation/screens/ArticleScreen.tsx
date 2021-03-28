import React from 'react';

import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import produce from 'immer';
import {
  Body,
  Button,
  Card,
  CardItem,
  Content,
  Grid,
  Row,
  Text,
  View,
} from 'native-base';

import { useLinkTo, useNavigation } from '@react-navigation/native';
import { StackScreenProps } from '@react-navigation/stack';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { ContentLoading } from '../../shared/components/Loading';
import { useAppSettingsContext } from '../../shared/contexts/AppSettingsContext';
import {
  useWordFamiliarityContext,
  WordFamiliarityContextProvider,
} from '../../shared/contexts/WordFamiliarityContext';
import {
  QueryCacheKey,
  queryCacheKey,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';
import { Routes, routeUrl } from '../routes';

const wordFamiliarityLevelColorCodeMapping: Record<string, string> = {
  0: '#C2D1FF',
  1: '#FFD4B0',
  2: '#EBE9A2',
  3: '#A7D9C0',
  // #E8AEE1 for red
};

const SelectedTokenDefinitionCard: React.FC<{
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

const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  const [selectedToken, setSelectedToken] = React.useState<Backend.Token>();
  const navigation = useNavigation();
  const { wordFamiliarity } = useWordFamiliarityContext();

  const wordDefinitionsListQuery = useQuery(
    queryCacheKey(QueryCacheKey.WordDefinitionList, {
      filter_Word_Expression: selectedToken?.form ?? '',
      filter_Word_LanguageCode: article.languageCode,
    }),
    () => {
      if (!selectedToken) {
        throw new Error();
      }

      return api.wordDefinitions_List({
        filter_Word_Expression: selectedToken.form,
        filter_Word_LanguageCode: article.languageCode,
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

  const getWordFamiliarityItem = (
    token: Backend.Token,
  ): Backend.WordFamiliarityListItemViewModel => {
    return (
      wordFamiliarity.groupedWordFamiliarities[article.languageCode]?.[
        token.form
      ] ?? {
        level: 0,
        word: {
          expression: token.form,
          languageCode: article.languageCode,
        },
      }
    );
  };

  // TODO: Save article reading progress.

  return (
    <Grid>
      <Row size={3}>
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => {
                    const spacesAfter = (
                      token.misc.match(/\|?Spaces?After=(.+)\|?/)?.[1] ?? ' '
                    )
                      .replace(/^No$/, '')
                      .replace(/\\s/g, ' ')
                      .replace(/\\n/g, '\n');

                    const wordFamiliarityItem = getWordFamiliarityItem(token);

                    return (
                      <React.Fragment key={token.id}>
                        {/* TODO: Render and highlight multiple words token,
                            for example, "på grund av" */}
                        {/* TODO: Support selected multiple words */}
                        <View
                          style={
                            wordFamiliarityLevelColorCodeMapping[
                              wordFamiliarityItem.level
                            ]
                              ? {
                                  borderBottomWidth: 4,
                                  borderColor:
                                    wordFamiliarityLevelColorCodeMapping[
                                      wordFamiliarityItem.level
                                    ],
                                }
                              : {}
                          }
                        >
                          <Text
                            onPress={() => setSelectedToken(token)}
                            style={{
                              fontSize: 28,
                            }}
                          >
                            {token.form}
                          </Text>
                        </View>
                        <Text style={{ fontSize: 28 }}>{spacesAfter}</Text>
                      </React.Fragment>
                    );
                  })}
                </Text>
              ))}
            </Text>
          ))}
        </Content>
      </Row>
      <Row size={1}>
        {selectedToken && (
          <SelectedTokenDefinitionCard
            definitionListItems={wordDefinitionsListQuery.data?.items ?? []}
            selectedToken={selectedToken}
            wordFamiliarityItem={getWordFamiliarityItem(selectedToken)}
          />
        )}
      </Row>
    </Grid>
  );
};

export const ArticleScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'Article'>
> = ({ route }) => {
  const { id } = route.params;

  const { data } = useQuery(queryCacheKey(QueryCacheKey.Article, id), () =>
    api.articles_Get({ id }),
  );

  if (!data) {
    return <ContentLoading />;
  }

  return (
    <WordFamiliarityContextProvider>
      <ArticleReader article={data} />
    </WordFamiliarityContextProvider>
  );
};
