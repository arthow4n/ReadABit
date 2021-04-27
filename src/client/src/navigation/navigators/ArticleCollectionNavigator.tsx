import * as React from 'react';

import { useTranslation } from 'react-i18next';

import { Container } from 'native-base';

import { createStackNavigator } from '@react-navigation/stack';
import { SafeArea } from '@src/shared/components/SafeArea';
import { StackNavigatorHeader } from '@src/shared/components/StackNavigatorHeader';

import { ArticleCollectionStackParamList } from './ArticleCollectionNavigator.types';

import { ArticleCollectionListScreen } from '../screens/ArticleCollectionListScreen';

const ArticleCollectionStack = createStackNavigator<ArticleCollectionStackParamList>();

export const ArticleCollectionNavigator: React.FC = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <Container>
        <ArticleCollectionStack.Navigator
          headerMode="screen"
          screenOptions={{
            header: (props) => {
              return <StackNavigatorHeader {...props} />;
            },
          }}
        >
          <ArticleCollectionStack.Screen
            name="ArticleCollectionList"
            // TODO: Actual component
            component={ArticleCollectionListScreen}
            options={{ headerTitle: t('Article collection', { count: 100 }) }}
          />
          <ArticleCollectionStack.Screen
            name="ArticleCollection"
            // TODO: Actual component
            component={() => <></>}
            options={{ headerTitle: t('Article', { count: 100 }) }}
          />
        </ArticleCollectionStack.Navigator>
      </Container>
    </SafeArea>
  );
};
