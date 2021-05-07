import React from 'react';

import { Controller, useForm, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useQuery } from 'react-query';

import {
  Content,
  Form,
  H1,
  Input,
  Item,
  Label,
  Text,
  Picker,
  H2,
  View,
  Switch,
} from 'native-base';
import { useDebouncedCallback } from 'use-debounce/lib';
import * as z from 'zod';

import { zodResolver } from '@hookform/resolvers/zod';
import { BottomTabNavigationProp } from '@react-navigation/bottom-tabs';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { ContentLoading } from '@src/shared/components/Loading';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import { appSettingsSchema } from '@src/shared/contexts/AppSettingsContext.types';
import {
  QueryCacheKey,
  queryCacheKey,
  useMutateUserPreferenceUpdate,
} from '@src/shared/hooks/useBackendReactQuery';
import { useEffectSkipOnMount } from '@src/shared/hooks/useEffectSkipOnMount';

import { HomeBottomTabParamList } from '../navigators/HomeNavigator.types';

const remoteUserPreferenceFormSchema = z.object({
  dailyGoalResetTimeTimeZone: z.string().nonempty(),
  dailyGoalResetTimePartial: z.string().nonempty(),
  dailyGoalNewlyCreatedWordFamiliarityCount: z.number().int().positive(),
});

const SettingsScreenInner: React.FC<{
  userPreferenceData: Backend.UserPreferenceData;
  timezones: Backend.TimeZoneInfoViewModel[];
}> = ({ userPreferenceData, timezones }) => {
  const { appSettings, updateAppSettings } = useAppSettingsContext();
  const { t } = useTranslation();
  const remoteUserPreferenceFormHandle = useForm({
    // TODO: Pass view model instead of entity from backend to fix the nullish check.
    // These actually shouldn't be null here.
    defaultValues: {
      dailyGoalNewlyCreatedWordFamiliarityCount:
        userPreferenceData.dailyGoalNewlyCreatedWordFamiliarityCount ?? 25,
      dailyGoalResetTimePartial:
        userPreferenceData.dailyGoalResetTimePartial ?? '',
      dailyGoalResetTimeTimeZone:
        userPreferenceData.dailyGoalResetTimeTimeZone ?? '',
    },
    shouldUnregister: false,
    resolver: zodResolver(remoteUserPreferenceFormSchema),
    mode: 'onChange',
  });

  const remoteUserPreferenceFormWatch = useWatch({
    control: remoteUserPreferenceFormHandle.control,
  });

  const localAppSettingsFormHandle = useForm({
    defaultValues: appSettings,
    shouldUnregister: false,
    resolver: zodResolver(appSettingsSchema),
    mode: 'onChange',
  });

  const localAppSettingsFormWatch = useWatch({
    control: localAppSettingsFormHandle.control,
  });

  const { mutateAsync } = useMutateUserPreferenceUpdate();

  const submitRemoteUserPreference = useDebouncedCallback(
    (data: Backend.UserPreferenceData) => {
      mutateAsync({
        request: { data },
      });
    },
    300,
  );

  useEffectSkipOnMount(() => {
    if (
      !remoteUserPreferenceFormHandle.formState.isValidating &&
      remoteUserPreferenceFormHandle.formState.isValid
    ) {
      submitRemoteUserPreference(remoteUserPreferenceFormHandle.getValues());
    }
  }, [
    remoteUserPreferenceFormHandle.formState.isValidating,
    remoteUserPreferenceFormHandle.formState.isValid,
    JSON.stringify(remoteUserPreferenceFormWatch),
  ]);

  useEffectSkipOnMount(() => {
    if (
      !localAppSettingsFormHandle.formState.isValidating &&
      localAppSettingsFormHandle.formState.isValid
    ) {
      updateAppSettings(localAppSettingsFormHandle.getValues());
    }
  }, [
    localAppSettingsFormHandle.formState.isValidating,
    localAppSettingsFormHandle.formState.isValid,
    JSON.stringify(localAppSettingsFormWatch),
  ]);

  const renderDailyGoalFormPartial = () => (
    <View>
      <H2>{t('Daily goal')}</H2>
      <Controller
        control={remoteUserPreferenceFormHandle.control}
        name="dailyGoalResetTimePartial"
        render={({ onChange, value }) => (
          <Item
            error={
              !!remoteUserPreferenceFormHandle.errors.dailyGoalResetTimePartial
            }
          >
            <Label>{t('Reset time')}</Label>
            <Input
              onChangeText={onChange}
              // TODO: Time picker
              placeholder="00:00:00"
              value={value}
            />
            <Text>
              {
                remoteUserPreferenceFormHandle.errors.dailyGoalResetTimePartial
                  ?.message
              }
            </Text>
          </Item>
        )}
      />
      <Controller
        control={remoteUserPreferenceFormHandle.control}
        name="dailyGoalResetTimeTimeZone"
        render={(handle) => (
          <Item>
            <Label>{t('Time zone')}</Label>
            <Picker
              mode="dropdown"
              selectedValue={handle.value}
              onValueChange={handle.onChange}
            >
              {timezones.map((tz) => (
                <Picker.Item label={tz.displayName} value={tz.id} key={tz.id} />
              ))}
            </Picker>
          </Item>
        )}
      />
    </View>
  );

  // TODO: i18n for language codes
  const renderLanguageCodesFormPartial = () => (
    <View>
      <H2>{t('Language', { count: 100 })}</H2>
      <Controller
        control={localAppSettingsFormHandle.control}
        name="languageCodes.studying"
        render={(handle) => (
          <Item>
            <Label>{t('Studying')}</Label>
            <Picker
              mode="dropdown"
              selectedValue={handle.value}
              onValueChange={handle.onChange}
            >
              {['sv'].map((x) => (
                <Picker.Item label={x} value={x} key={x} />
              ))}
            </Picker>
          </Item>
        )}
      />
      <Controller
        control={localAppSettingsFormHandle.control}
        name="languageCodes.ui"
        render={(handle) => (
          <Item>
            <Label>{t('User interface')}</Label>
            <Picker
              mode="dropdown"
              selectedValue={handle.value}
              onValueChange={handle.onChange}
            >
              {['en', 'zh-TW'].map((x) => (
                <Picker.Item label={x} value={x} key={x} />
              ))}
            </Picker>
          </Item>
        )}
      />
    </View>
  );

  const renderTtsFormPartial = () => (
    <View>
      <H2>{t('Text-to-speech')}</H2>
      <Controller
        control={localAppSettingsFormHandle.control}
        name="tts.autoSpeakWhenTapOnWord"
        render={({ onChange, value }) => (
          <Item>
            <Label>{t('When tapping on word')}</Label>
            <Switch value={value} onValueChange={onChange} />
          </Item>
        )}
      />
    </View>
  );

  const renderNetworkFormPartial = () => (
    <View>
      <H2>{t('Network')}</H2>
      <Controller
        control={localAppSettingsFormHandle.control}
        name="saveDataUsage"
        render={({ onChange, value }) => (
          <Item>
            <Label>{t('Save data usage')}</Label>
            <Switch value={value} onValueChange={onChange} />
          </Item>
        )}
      />
    </View>
  );

  return (
    <Content padder>
      <H1>{t('Settings')}</H1>
      <Form>
        {renderDailyGoalFormPartial()}
        {renderLanguageCodesFormPartial()}
        {renderTtsFormPartial()}
        {renderNetworkFormPartial()}
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
    {
      enabled: false,
    },
  );

  const listAllSupportedTimeZonesHandle = useQuery(
    queryCacheKey(QueryCacheKey.CultureInfoListAllSupportedTimeZones),
    () => api().cultureInfo_ListAllSupportedTimeZones(),
  );

  React.useEffect(() => {
    userPreferencesGetHandle.refetch();
  }, []);

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
