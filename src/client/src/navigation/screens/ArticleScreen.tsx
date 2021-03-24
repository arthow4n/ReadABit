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

import { useLinkTo } from '@react-navigation/native';
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
  const { appSettings } = useAppSettingsContext();

  return (
    <Grid>
      <Row size={1}>
        {selectedToken && (
          <Card>
            <CardItem header>
              <Text>{`${selectedToken.form} (${selectedToken.lemma})`}</Text>
            </CardItem>
            <CardItem>
              <Body>
                {/* TODO: Show saved word definition */}
                {/* TODO: Render token PoS tags */}
                <Button
                  onPress={() =>
                    linkTo(
                      routeUrl(Routes.WordDefinitionsDictionaryLookup, null, {
                        word: selectedToken.form,
                        wordLanguage: article.languageCode,
                        // TODO: Allow choosing another dictionary language
                        dictionaryLanguage: appSettings.languageCodes.ui,
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
                        <Text>{spacesAfter}</Text>
                      </React.Fragment>
                    );
                  })}
                </Text>
              ))}
            </Text>
          ))}
        </Content>
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
