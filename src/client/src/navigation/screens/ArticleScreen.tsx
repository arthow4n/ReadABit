import React from 'react';

import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import {
  Body,
  Button,
  Card,
  CardItem,
  Content,
  Grid,
  Row,
  Text,
} from 'native-base';

import {
  useFocusEffect,
  useLinkTo,
  useNavigation,
} from '@react-navigation/native';
import { StackScreenProps } from '@react-navigation/stack';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { ContentLoading } from '../../shared/components/Loading';
import { useAppSettingsContext } from '../../shared/contexts/AppSettingsContext';
import {
  QueryCacheKey,
  queryCacheKey,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';
import { Routes, routeUrl } from '../routes';

const ArticleReader: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article }) => {
  const { t } = useTranslation();
  const [selectedToken, setSelectedToken] = React.useState<Backend.Token>();
  const linkTo = useLinkTo();
  const navigation = useNavigation();

  const { appSettings } = useAppSettingsContext();

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

                    return (
                      <React.Fragment key={token.id}>
                        <Text
                          onPress={() => setSelectedToken(token)}
                          style={{ fontSize: 28 }}
                        >
                          {token.form}
                        </Text>
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
          <Card style={{ flex: 1 }}>
            <CardItem>
              <Body>
                <Text>{`${selectedToken.form} (${selectedToken.lemma})`}</Text>
                {/* TODO: Load public suggestions */}
                {/* TODO: Load ML Kit on-device translation as one of the suggestion. Ref: https://developers.google.com/ml-kit/language/translation */}
                {/* TODO: Show all the available word definitions in a scrollable block */}
                <Text>{wordDefinitionsListQuery.data?.items[0]?.meaning}</Text>
                {/* TODO: Show buttons for changing word familiarity status */}
                {/* TODO: Render token PoS tags */}
                <Button
                  onPress={() =>
                    linkTo(
                      routeUrl(Routes.WordDefinitionsDictionaryLookup, null, {
                        word: selectedToken.form,
                        wordLanguage: article.languageCode,
                        // TODO: Allow choosing another dictionary language
                        dictionaryLanguage: appSettings.languageCodes.ui,
                        wordDefinitionId:
                          wordDefinitionsListQuery.data?.items[0]?.id,
                      }),
                    )
                  }
                >
                  <Text>{t('Dictionary')}</Text>
                </Button>
              </Body>
            </CardItem>
          </Card>
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

  return <ArticleReader article={data} />;
};
