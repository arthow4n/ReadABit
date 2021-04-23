import React from 'react';

import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import {
  Button,
  Content,
  Form,
  H1,
  Input,
  Item,
  Label,
  Text,
  Picker,
  Spinner,
  H2,
} from 'native-base';
import * as z from 'zod';

import { zodResolver } from '@hookform/resolvers/zod';
import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { ContentLoading } from '@src/shared/components/Loading';
import {
  QueryCacheKey,
  queryCacheKey,
  useMutateUserPreferenceUpdate,
} from '@src/shared/hooks/useBackendReactQuery';

import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';

const formSchema = z.object({
  wordDefinitionLanguageCode: z.string().nonempty(),
  userInterfaceLanguageCode: z.string().nonempty(),
  dailyGoalResetTimeTimeZone: z.string().nonempty(),
  dailyGoalResetTimePartial: z.string().nonempty(),
  dailyGoalNewlyCreatedWordFamiliarityCount: z.number().int().positive(),
});

const SettingsScreenInner: React.FC<{
  userPreferenceData: Backend.UserPreferenceData;
  timezones: Backend.TimeZoneInfoViewModel[];
}> = ({ userPreferenceData, timezones }) => {
  const { t } = useTranslation();
  const { control, handleSubmit, errors } = useForm({
    // TODO: Pass view model instead of entity from backend to fix the nullish check.
    // These actually shouldn't be null here.
    defaultValues: {
      dailyGoalNewlyCreatedWordFamiliarityCount:
        userPreferenceData.dailyGoalNewlyCreatedWordFamiliarityCount ?? 25,
      dailyGoalResetTimePartial:
        userPreferenceData.dailyGoalResetTimePartial ?? '',
      dailyGoalResetTimeTimeZone:
        userPreferenceData.dailyGoalResetTimeTimeZone ?? '',
      userInterfaceLanguageCode:
        userPreferenceData.userInterfaceLanguageCode ?? 'en',
      wordDefinitionLanguageCode:
        userPreferenceData.wordDefinitionLanguageCode ?? 'en',
    },
    shouldUnregister: false,
    resolver: zodResolver(formSchema),
  });

  const { mutateAsync, isLoading } = useMutateUserPreferenceUpdate();

  const disabled = isLoading;

  const onSubmitPress = handleSubmit(async (values) => {
    mutateAsync({ request: { data: values } });
  });

  return (
    <Content padder>
      <H1>{t('Settings')}</H1>
      <Form>
        <H2>{t('Daily goal')}</H2>
        <Controller
          control={control}
          name="dailyGoalResetTimePartial"
          render={({ onChange, value }) => (
            <Item
              disabled={disabled}
              error={!!errors.dailyGoalResetTimePartial}
            >
              <Label>{t('Reset time')}</Label>
              <Input
                disabled={disabled}
                onChangeText={onChange}
                // TODO: Time picker
                placeholder="00:00:00"
                value={value}
              />
              <Text>{errors.dailyGoalResetTimePartial?.message}</Text>
            </Item>
          )}
        />
        <Controller
          control={control}
          name="dailyGoalResetTimeTimeZone"
          render={(handle) => (
            <Item
              disabled={disabled}
              error={!!errors.dailyGoalResetTimeTimeZone}
            >
              <Label>{t('Time zone')}</Label>
              <Picker
                enabled={!disabled}
                mode="dropdown"
                selectedValue={handle.value}
                onValueChange={handle.onChange}
              >
                {timezones.map((tz) => (
                  <Picker.Item
                    label={tz.displayName}
                    value={tz.id}
                    key={tz.id}
                  />
                ))}
              </Picker>
              <Text>{errors.dailyGoalResetTimeTimeZone?.message}</Text>
            </Item>
          )}
        />
        {/* TODO: Language settings */}
        <Button disabled={disabled} onPress={onSubmitPress}>
          <Text>{t('Save')}</Text>
          {disabled && <Spinner />}
        </Button>
      </Form>
    </Content>
  );
};

export const SettingsScreen: React.FC<
  BottomTabNavigationProp<HomeBottomTabParamList, 'Settings'>
> = () => {
  const userPreferencesGetHandle = useQuery(
    queryCacheKey(QueryCacheKey.UserPreferenceData),
    () => api().userPreferences_Get(),
  );

  const listAllSupportedTimeZonesHandle = useQuery(
    queryCacheKey(QueryCacheKey.CultureInfoListAllSupportedTimeZones),
    () => api().cultureInfo_ListAllSupportedTimeZones(),
  );

  if (!userPreferencesGetHandle.data || !listAllSupportedTimeZonesHandle.data) {
    return <ContentLoading />;
  }

  return (
    <SettingsScreenInner
      userPreferenceData={userPreferencesGetHandle.data}
      timezones={listAllSupportedTimeZonesHandle.data}
    />
  );
};
