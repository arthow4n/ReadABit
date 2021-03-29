import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import WebView from 'react-native-webview';
import { useQuery } from 'react-query';

import {
  Content,
  Form,
  Spinner,
  Item,
  Picker,
  Textarea,
  Input,
  Label,
  Text,
  Button,
  View,
} from 'native-base';
import * as z from 'zod';

import { zodResolver } from '@hookform/resolvers/zod';
import { useLinkTo } from '@react-navigation/native';
import { StackNavigationProp } from '@react-navigation/stack';
import { ImportWebPageWebview } from '@src/shared/components/ImportWebPageWebview';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { ContentLoading } from '../../shared/components/Loading';
import { useAppSettingsContext } from '../../shared/contexts/AppSettingsContext';
import {
  queryCacheKey,
  QueryCacheKey,
  useMutateArticleCollectionsCreate,
  useMutateArticleCreate,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';
import { Routes, routeUrl } from '../routes';

const formSchema = z.object({
  articleCollectionId: z.string().nonempty(),
  importFromUrl: z.string(),
  name: z.string().nonempty(),
  text: z.string().nonempty(),
});

const ArticleCreateForm: React.FC<{
  articleCollections: Backend.ArticleCollection[];
}> = ({ articleCollections }) => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const { control, handleSubmit, errors, setValue } = useForm({
    defaultValues: {
      articleCollectionId: articleCollections[0]?.id ?? '',
      importFromUrl: '',
      name: '',
      text: '',
    },
    resolver: zodResolver(formSchema),
  });
  const [loadingFromWebPageUrl, setLoadingFromWebPageUrl] = React.useState('');

  const { mutateAsync, isLoading } = useMutateArticleCreate();
  const onSubmitPress = handleSubmit(async (values) => {
    const result = await mutateAsync({ request: values });
    linkTo(routeUrl(Routes.Article, { id: result.id }));
  });

  const disabled = isLoading || !!loadingFromWebPageUrl;

  return (
    <Form>
      <Item picker disabled={disabled} error={!!errors.articleCollectionId}>
        <Controller
          control={control}
          name="articleCollectionId"
          render={({ onChange, value }) => (
            <Picker
              enabled={!disabled}
              mode="dropdown"
              selectedValue={value}
              onValueChange={onChange}
            >
              {articleCollections.map((ac) => (
                <Picker.Item label={ac.name} value={ac.id} key={ac.id} />
              ))}
            </Picker>
          )}
        />
        <Text>{errors.articleCollectionId?.message}</Text>
      </Item>
      <Item stackedLabel disabled={disabled} error={!!errors.name}>
        <Label>{t('Title')}</Label>
        <Controller
          control={control}
          name="name"
          render={({ onChange, value }) => (
            <Input disabled={disabled} onChangeText={onChange} value={value} />
          )}
        />
        <Text>{errors.name?.message}</Text>
      </Item>
      <Item stackedLabel disabled={disabled} error={!!errors.importFromUrl}>
        <Controller
          control={control}
          name="name"
          render={({ onChange, value }) => (
            <View>
              <Label>{t('Import from web page')}</Label>
              <Input
                disabled={disabled}
                onChangeText={onChange}
                placeholder={t('URL')}
                value={value}
              />
              <Button
                disabled={disabled}
                onPress={() => {
                  setLoadingFromWebPageUrl(value);
                }}
              >
                <Text>{t('Load content')}</Text>
                {loadingFromWebPageUrl && <Spinner />}
              </Button>
            </View>
          )}
        />
        {loadingFromWebPageUrl && (
          <ImportWebPageWebview
            url={loadingFromWebPageUrl}
            onParsed={({ title, content }) => {
              setValue('name', title);
              setValue('text', content);
              setLoadingFromWebPageUrl('');
            }}
            onFail={() => {
              setLoadingFromWebPageUrl('');
            }}
          />
        )}
        <Text>{errors.importFromUrl?.message}</Text>
      </Item>
      <Controller
        control={control}
        name="text"
        render={({ onChange, value }) => (
          <Textarea
            disabled={disabled}
            rowSpan={5}
            bordered
            placeholder={t('Content')}
            onChangeText={onChange}
            value={value}
          />
        )}
      />
      <Text>{errors.text?.message}</Text>
      <Button disabled={disabled} onPress={onSubmitPress}>
        <Text>{t('Import')}</Text>
        {disabled && <Spinner />}
      </Button>
    </Form>
  );
};

export const ArticleCreateScreen: React.FC<
  StackNavigationProp<ArticleStackParamList, 'Article'>
> = () => {
  const { t } = useTranslation();
  const { appSettings } = useAppSettingsContext();

  const { mutateAsync } = useMutateArticleCollectionsCreate();

  const { refetch, data } = useQuery(
    queryCacheKey(QueryCacheKey.ArticleCollectionList, {
      filter_LanguageCode: appSettings.languageCodes.studying,
    }),
    // TODO: Handle large list
    () =>
      api.articleCollections_List({
        filter_LanguageCode: appSettings.languageCodes.studying,
      }),
    {
      onSuccess: async ({ items }) => {
        if (!items.length) {
          const r = await mutateAsync({
            request: {
              languageCode: appSettings.languageCodes.studying,
              name: t('Quick imports'),
              public: false,
            },
          });
          console.log(r);
          refetch();
        }
      },
    },
  );

  if (!data?.items.length) {
    return <ContentLoading />;
  }

  return (
    <Content>
      <ArticleCreateForm articleCollections={data.items} />
    </Content>
  );
};
