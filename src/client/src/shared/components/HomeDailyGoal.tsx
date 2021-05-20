import * as React from 'react';

import { useTranslation } from 'react-i18next';
import { Circle } from 'react-native-progress';
import { useQuery } from 'react-query';

import { View, Text, Grid, Col } from 'native-base';

import { api } from '@src/integrations/backend/backend';

import { QueryCacheKey, queryCacheKey } from '../hooks/useBackendReactQuery';
import { useNavigationRefocusEffect } from '../hooks/useNavigationRefocusEffect';

export const HomeDailyGoal: React.FC = () => {
  const { t } = useTranslation();
  const { refetch, data } = useQuery(
    queryCacheKey(QueryCacheKey.UserAchievementGetDailyGoalStreakState),
    () => api().userAchievements_GetDailyGoalStreakState(),
  );

  useNavigationRefocusEffect(refetch);

  const progress =
    (data?.dailyGoalCheckResult.newlyCreated || 0) /
    (data?.dailyGoalCheckResult.newlyCreatedGoal || 1);

  return (
    <Grid style={{ padding: 28 }}>
      <Col style={{ marginRight: 28 }}>
        <View
          style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}
        >
          <Text style={{ fontSize: 20, marginBottom: 28 }}>
            {t('Daily goal')}
          </Text>
          <Circle
            progress={progress}
            size={144}
            thickness={16}
            textStyle={{ fontSize: 32 }}
            showsText
            formatText={() =>
              `${data?.dailyGoalCheckResult.newlyCreated || 0}/${
                data?.dailyGoalCheckResult.newlyCreatedGoal || 0
              }`
            }
            color={progress >= 1 ? 'green' : 'blue'}
          />
        </View>
      </Col>
      <Col>
        <View
          style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}
        >
          <Text style={{ fontSize: 20, marginBottom: 28 }}>
            {t('Streak days')}
          </Text>
          {data?.currentStreakDays === undefined ? null : (
            <Text style={{ fontSize: 32, marginBottom: 28 }}>
              {data.currentStreakDays}
            </Text>
          )}
        </View>
      </Col>
    </Grid>
  );
};
