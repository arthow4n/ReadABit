import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import WebView from 'react-native-webview';

import {
  Button,
  Card,
  Form,
  Grid,
  Input,
  Item,
  Row,
  Spinner,
  Text,
} from 'native-base';
import * as z from 'zod';

import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigation } from '@react-navigation/core';
import { StackScreenProps } from '@react-navigation/stack';

import {
  useMutateWordDefninitionCreate,
  useMutateWordDefninitionUpdate,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';

const formSchema = z.object({
  meaning: z.string().nonempty(),
});

export const WordDefinitionsDictionaryLookupScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'WordDefinitionsDictionaryLookup'>
> = ({ route }) => {
  const {
    word,
    wordLanguageCode,
    dictionaryLanguageCode,
    wordDefinitionId,
  } = route.params;

  const { t } = useTranslation();
  const navigation = useNavigation();

  const { control, handleSubmit, errors } = useForm({
    defaultValues: {
      meaning: '',
    },
    resolver: zodResolver(formSchema),
  });

  const wordDefinitionCreateHandle = useMutateWordDefninitionCreate();
  const wordDefinitionUpdateHandle = useMutateWordDefninitionUpdate({
    filter_Word_Expression: word,
    filter_Word_LanguageCode: wordLanguageCode,
  });

  // TODO: Support switching to seach with lemma instead of word form

  // TODO: Support choosing another dictionary (findSupportedWebDictionary)
  // TODO: Support saving custom dictionary
  const dictionaryWebViewUrl = `https://${encodeURIComponent(
    dictionaryLanguageCode,
  )}.glosbe.com/${encodeURIComponent(wordLanguageCode)}/${encodeURIComponent(
    dictionaryLanguageCode,
  )}/${encodeURIComponent(word)}`;

  const onSubmitPress = handleSubmit(async (values) => {
    // TODO: Optimistic saving & local caching
    // So the user can jump back to article reader screen immediately and see the new definition
    // without have to wait for network.

    if (wordDefinitionId) {
      await wordDefinitionUpdateHandle.mutateAsync({
        id: wordDefinitionId,
        request: {
          languageCode: dictionaryLanguageCode,
          meaning: values.meaning,
          public: true,
        },
      });
    } else {
      await wordDefinitionCreateHandle.mutateAsync({
        request: {
          languageCode: dictionaryLanguageCode,
          meaning: values.meaning,
          public: true,
          word: {
            expression: word,
            languageCode: wordLanguageCode,
          },
        },
      });
    }

    navigation.goBack();
  });

  const disabled =
    wordDefinitionCreateHandle.isLoading ||
    wordDefinitionUpdateHandle.isLoading;

  return (
    <Grid style={{ flex: 1 }}>
      <Row size={3}>
        <WebView source={{ uri: dictionaryWebViewUrl }} />
      </Row>
      <Row size={1}>
        <Card style={{ flex: 1 }}>
          <Form>
            <Item disabled={disabled}>
              <Controller
                control={control}
                name="meaning"
                render={({ onChange, value }) => (
                  <Input
                    disabled={disabled}
                    onChangeText={onChange}
                    value={value}
                    placeholder={t('Meaning of the word')}
                  />
                )}
              />
              <Button disabled={disabled} onPress={onSubmitPress}>
                <Text>{t('Save')}</Text>
                {disabled && <Spinner />}
              </Button>
              <Text>{errors.meaning?.message}</Text>
            </Item>
          </Form>
        </Card>
      </Row>
    </Grid>
  );
};
