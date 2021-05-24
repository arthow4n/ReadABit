import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import WebView from 'react-native-webview';

import { first } from 'lodash';
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
import { findSupportedWebDictionary } from '@src/shared/constants/dictionaries';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';

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
    wordsJson,
    wordLanguageCode,
    dictionaryLanguageCode,
    wordDefinitionId,
  } = route.params;

  // TODO: Allow cycling through words (e.g. lemma)
  const words: string[] = JSON.parse(wordsJson);
  const word = first(words) ?? '';

  const { appSettings } = useAppSettingsContext();
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

  // TODO: Support saving custom dictionary
  // TODO: Support choosing another dictionary (findSupportedWebDictionary)
  const webDictionary = first(findSupportedWebDictionary(wordLanguageCode));

  React.useEffect(() => {
    navigation.setOptions({
      headerTitle: webDictionary?.name ?? t('Dictionary'),
    });
  }, [webDictionary]);

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
        {/* TODO: State for no dictionaries available. */}
        {webDictionary && (
          <WebView
            scalesPageToFit={false}
            source={{
              uri: webDictionary.wordPageUrl(
                wordLanguageCode,
                appSettings.languageCodes.ui,
                word,
              ),
            }}
            injectedJavaScriptBeforeContentLoaded={
              webDictionary.injectedJavaScriptBeforeContentLoaded
            }
          />
        )}
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
