import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Dialog from 'react-native-dialog';
import DocumentPicker from 'react-native-document-picker';
import { readFile } from 'react-native-fs';
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
  Icon,
} from 'native-base';
import * as z from 'zod';

import { zodResolver } from '@hookform/resolvers/zod';
import { useLinkTo } from '@react-navigation/native';
import { StackScreenProps } from '@react-navigation/stack';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { ArticleStackParamList } from '@src/navigation/navigators/ArticleNavigator.types';
import { Routes, routeUrl } from '@src/navigation/routes';
import {
  ImportWebPageWebview,
  Scraper,
} from '@src/shared/components/ImportWebPageWebview';
import { ContentLoading } from '@src/shared/components/Loading';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import {
  queryCacheKey,
  QueryCacheKey,
  useMutateArticleCollectionsCreate,
  useMutateArticleCreate,
} from '@src/shared/hooks/useBackendReactQuery';

const articleCreateFormSchema = z.object({
  articleCollectionId: z.string().nonempty(),
  importFromUrl: z.string(),
  scraper: z.nativeEnum(Scraper),
  name: z.string().nonempty(),
  text: z.string().nonempty(),
});

const articleCollectionCreateFormSchema = z.object({
  dialogOpen: z.boolean(),
  name: z.string().nonempty(),
  public: z.boolean(),
});

// TODO: Consider separating article collection creation dialog into another component
const ArticleCreateForm: React.FC<{
  articleCollections: Backend.ArticleCollection[];
  preselectedArticleCollectionId: string | null;
}> = ({ articleCollections, preselectedArticleCollectionId }) => {
  const { t } = useTranslation();
  const { appSettings } = useAppSettingsContext();
  const linkTo = useLinkTo();
  const articleCollectionCreateFormHandle = useForm({
    resolver: zodResolver(articleCollectionCreateFormSchema),
    defaultValues: {
      dialogOpen: false,
      name: '',
      public: false,
    },
  });
  const { control, handleSubmit, errors, setValue, getValues } = useForm({
    defaultValues: {
      articleCollectionId:
        preselectedArticleCollectionId ?? articleCollections[0]?.id ?? '',
      importFromUrl: '',
      scraper: Scraper.ReadabilityScraper,
      name: '',
      text: '',
    },
    resolver: zodResolver(articleCreateFormSchema),
  });
  const [loadingFromWebPageUrl, setLoadingFromWebPageUrl] = React.useState('');

  const articleCollectionsCreateHandle = useMutateArticleCollectionsCreate();
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
  const disabled =
    isLoading ||
    isLoadingFromWebPageUrl ||
    articleCollectionsCreateHandle.isLoading;

  const renderArticleCollectionPicker = () => (
    <Item picker disabled={disabled} error={!!errors.articleCollectionId}>
      <Controller
        control={control}
        name="articleCollectionId"
        render={({ onChange, value }) => (
          <View style={{ flex: 1, flexDirection: 'row' }}>
            <View style={{ flex: 1 }}>
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
            </View>
            <Controller
              control={articleCollectionCreateFormHandle.control}
              name="dialogOpen"
              render={(dialogOpen) => (
                <View>
                  <Button
                    disabled={disabled}
                    onPress={() => dialogOpen.onChange(true)}
                  >
                    {articleCollectionsCreateHandle.isLoading ? (
                      <Spinner />
                    ) : (
                      <Icon name="add-outline" />
                    )}
                  </Button>
                  <Dialog.Container
                    // TODO: Fix light/dark mode styling issues
                    // (currently text is white if system is dark mode)
                    contentStyle={{ backgroundColor: '#000000' }}
                    visible={
                      articleCollectionCreateFormHandle.getValues().dialogOpen
                    }
                  >
                    <Dialog.Title>
                      {t('Create new article collection')}
                    </Dialog.Title>
                    <Controller
                      control={articleCollectionCreateFormHandle.control}
                      name="name"
                      render={(name) => (
                        <Dialog.Input
                          label={t('Article collection name')}
                          value={name.value}
                          onChangeText={name.onChange}
                        />
                      )}
                    />
                    <Controller
                      control={articleCollectionCreateFormHandle.control}
                      name="public"
                      render={(isPublic) => (
                        <Dialog.Switch
                          label={t('Public')}
                          value={isPublic.value}
                          onValueChange={isPublic.onChange}
                        />
                      )}
                    />
                    <Dialog.Button
                      label={t('Cancel')}
                      onPress={() => dialogOpen.onChange(false)}
                    />
                    <Dialog.Button
                      label={t('Create')}
                      onPress={articleCollectionCreateFormHandle.handleSubmit(
                        async (values) => {
                          await articleCollectionsCreateHandle.mutateAsync({
                            request: {
                              languageCode: appSettings.languageCodes.studying,
                              name: values.name,
                              public: values.public,
                            },
                          });

                          articleCollectionCreateFormHandle.reset();
                        },
                      )}
                    />
                  </Dialog.Container>
                </View>
              )}
            />
          </View>
        )}
      />
      <Text>{errors.articleCollectionId?.message}</Text>
    </Item>
  );

  const renderScraperPicker = () => (
    <Item>
      <Controller
        control={control}
        name="scraper"
        render={(scraperHandle) => (
          <Picker
            enabled={!disabled}
            mode="dropdown"
            selectedValue={scraperHandle.value}
            onValueChange={scraperHandle.onChange}
          >
            <Picker.Item
              label="ReadabilityScraper"
              value={Scraper.ReadabilityScraper}
              key="ReadabilityScraper"
            />
            <Picker.Item
              label="SimplifiedBodyInnerTextScraper"
              value={Scraper.SimplifiedBodyInnerTextScraper}
              key="SimplifiedBodyInnerTextScraper"
            />
          </Picker>
        )}
      />
    </Item>
  );

  const renderImportFromUrlArea = () => (
    <Controller
      control={control}
      name="importFromUrl"
      render={({ onChange, value }) => (
        <>
          <Item stackedLabel disabled={disabled} error={!!errors.importFromUrl}>
            <Label>{t('Import from web page')}</Label>
            <Input
              disabled={disabled}
              onChangeText={onChange}
              placeholder={t('URL')}
              value={value}
            />
            <Text>{errors.importFromUrl?.message}</Text>
          </Item>
          {renderScraperPicker()}
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
              scraper={getValues().scraper}
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
  );

  const renderImportFromFileInput = () => (
    <Button
      disabled={disabled}
      onPress={async () => {
        try {
          const { uri, name } = await DocumentPicker.pick({
            type: ['text/plain'],
          });

          const content = await readFile(uri);

          setValue('name', name.replace(/\.txt$/, ''));
          setValue('text', content);
        } catch (err) {
          if (DocumentPicker.isCancel(err)) {
            return;
          }
          throw err;
        }
      }}
    >
      <Text>{t('Import from file')}</Text>
    </Button>
  );

  const renderTitleContentEditorArea = () => (
    <>
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
    </>
  );

  return (
    <Form>
      {renderArticleCollectionPicker()}
      {renderImportFromFileInput()}
      {renderImportFromUrlArea()}
      {renderTitleContentEditorArea()}
      <Button disabled={disabled} onPress={onSubmitPress}>
        <Text>{t('Import')}</Text>
        {disabled && <Spinner />}
      </Button>
    </Form>
  );
};

export const ArticleCreateScreen: React.FC<
  StackScreenProps<ArticleStackParamList, 'ArticleCreate'>
> = ({ route }) => {
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
      <ArticleCreateForm
        articleCollections={data.items}
        preselectedArticleCollectionId={
          route.params.preselectedArticleCollectionId
        }
      />
    </Content>
  );
};
