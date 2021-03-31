import React from 'react';

import { useQuery } from 'react-query';

import { Content, Grid, Row, Text } from 'native-base';

import { useNavigation } from '@react-navigation/native';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { useWordFamiliarityContext } from '@src/shared/contexts/WordFamiliarityContext';
import {
  QueryCacheKey,
  queryCacheKey,
} from '@src/shared/hooks/useBackendReactQuery';

import { SelectedTokenDefinitionCard } from './SelectedTokenDefinitionCard';
import { WordToken } from './WordToken';

export const ArticleReader: React.FC<{
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
  // TODO: Split article into pages because it's very slow to re-render the whole article this way.

  return (
    <Grid>
      <Row size={3}>
        <Content padder>
          {article.conlluDocument.paragraphs.map((paragraph) => (
            <Text key={paragraph.id}>
              {paragraph.sentences.map((sentence) => (
                <Text key={sentence.id}>
                  {sentence.tokens.map((token) => (
                    <WordToken
                      key={token.id}
                      token={token}
                      onPress={() => setSelectedToken(token)}
                      wordFamiliarityItem={getWordFamiliarityItem(token)}
                    />
                  ))}
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
