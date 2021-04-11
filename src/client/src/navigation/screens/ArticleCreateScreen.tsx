import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
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
  const onSubmitPress = handleSubmit(
    async ({ articleCollectionId, name, text }) => {
      const result = await mutateAsync({
        request: {
          articleCollectionId,
          name,
          text: `${name}\n\n${text}`,
        },
      });
      linkTo(routeUrl(Routes.Article, { id: result.id }));
    },
  );

  const isLoadingFromWebPageUrl = !!loadingFromWebPageUrl;
  const disabled = isLoading || isLoadingFromWebPageUrl;

  const renderImportFromUrlInput = () => (
    <Item stackedLabel disabled={disabled} error={!!errors.importFromUrl}>
      <Controller
        control={control}
        name="importFromUrl"
        render={({ onChange, value }) => (
          <>
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
              {isLoadingFromWebPageUrl && <Spinner />}
            </Button>
            {isLoadingFromWebPageUrl && (
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
          </>
        )}
      />
      <Text>{errors.importFromUrl?.message}</Text>
    </Item>
  );

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
      {renderImportFromUrlInput()}
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
      api().articleCollections_List({
        filter_LanguageCode: appSettings.languageCodes.studying,
      }),
    {
      onSuccess: async ({ items }) => {
        if (!items.length) {
          await mutateAsync({
            request: {
              languageCode: appSettings.languageCodes.studying,
              name: t('Quick imports'),
              public: false,
            },
          });
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
