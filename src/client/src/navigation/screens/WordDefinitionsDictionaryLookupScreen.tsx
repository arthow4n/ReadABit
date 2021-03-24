import React from 'react';

import WebView from 'react-native-webview';

import { Grid, Row } from 'native-base';

import { StackScreenProps } from '@react-navigation/stack';

import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

export const WordDefinitionsDictionaryLookupScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'WordDefinitionsDictionaryLookup'>
> = ({ route }) => {
  const { word, wordLanguage, dictionaryLanguage } = route.params;

  // TODO: Support choosing another dictionary
  // TODO: Support saving custom dictionary
  const dictionaryWebViewUrl = `https://${encodeURIComponent(
    dictionaryLanguage,
  )}.glosbe.com/${encodeURIComponent(dictionaryLanguage)}/${encodeURIComponent(
    wordLanguage,
  )}/${encodeURIComponent(word)}`;

  return (
    <Grid style={{ flex: 1 }}>
      <Row size={3}>
        <WebView source={{ uri: dictionaryWebViewUrl }} />
      </Row>
      <Row size={1}>{/* TODO: Word definition input */}</Row>
    </Grid>
  );
};
