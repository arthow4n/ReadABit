import React from 'react';

import { appendErrors, Controller, useForm } from 'react-hook-form';
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

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import { useAppSettingsContext } from '../../shared/contexts/AppSettingsContext';
import {
  QueryCacheKey,
  useMutateArticleCollectionsCreate,
  useMutateArticleCreate,
} from '../../shared/hooks/useBackendReactQuery';
import { ArticleStackParamList } from '../navigators/ArticleNavigator.types';
import { Routes, routeUrl } from '../routes';

const formSchema = z.object({
  name: z.string().nonempty(),
  articleCollectionId: z.string().nonempty(),
  text: z.string().nonempty(),
});

const ArticleCreateForm: React.FC<{
  articleCollections: Backend.ArticleCollection[];
}> = ({ articleCollections }) => {
  const { t } = useTranslation();
  const linkTo = useLinkTo();
  const { control, handleSubmit, errors } = useForm({
    defaultValues: {
      name: '',
      articleCollectionId: articleCollections[0]?.id ?? '',
      text: '',
    },
    resolver: zodResolver(formSchema),
  });

  const { mutateAsync, isLoading } = useMutateArticleCreate();
  const onSubmitPress = handleSubmit(async (values) => {
    const result = await mutateAsync({ request: values });
    linkTo(routeUrl(Routes.Article, { id: result.id }));
  });

  const disabled = isLoading;

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
    [QueryCacheKey.ArticleCollectionList],
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
    return (
      <Content centerContent>
        <Spinner />
      </Content>
    );
  }

  return (
    <Content>
      <ArticleCreateForm articleCollections={data.items} />
    </Content>
  );
};
