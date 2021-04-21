import { api } from '@src/integrations/backend/backend';
import { View, Text } from 'native-base';
import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { Circle } from 'react-native-progress';
import { useQuery } from 'react-query';
import { QueryCacheKey, queryCacheKey } from '../hooks/useBackendReactQuery';
import { useNavigationRefocusEffect } from '../hooks/useNavigationRefocusEffect';

export const HomeDailyGoal: React.FC = () => {
  const { t } = useTranslation();
  const { refetch, data } = useQuery(
    queryCacheKey(QueryCacheKey.WordFamiliarityDailyGoalCheck),
    () => api().wordFamiliarities_DailyGoalCheck(),
  );

  useNavigationRefocusEffect(refetch);

  const progress = (data?.newlyCreated || 0) / (data?.newlyCreatedGoal || 1);

  return (
    <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
      <Text style={{ fontSize: 28, marginBottom: 28 }}>{t('Daily goal')}</Text>
      <Circle
        progress={progress}
        size={192}
        thickness={16}
        showsText
        formatText={() =>
          `${data?.newlyCreated || 0}/${data?.newlyCreatedGoal || 0}`
        }
        color={progress >= 1 ? 'green' : 'blue'}
      />
    </View>
  );
};
