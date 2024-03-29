import React from 'react';

import { useTranslation } from 'react-i18next';

import { Container } from 'native-base';

import { createStackNavigator } from '@react-navigation/stack';

import { ArticleStackParamList } from './ArticleNavigator.types';

import { SafeArea } from '../../shared/components/SafeArea';
import { StackNavigatorHeader } from '../../shared/components/StackNavigatorHeader';
import { ArticleCreateScreen } from '../screens/ArticleCreateScreen';
import { ArticleScreen } from '../screens/ArticleScreen';
import { WordDefinitionsDictionaryLookupScreen } from '../screens/WordDefinitionsDictionaryLookupScreen';

const ArticleStack = createStackNavigator<ArticleStackParamList>();

export const ArticleNavigator: React.FC = () => {
  const { t } = useTranslation();

  return (
    <SafeArea>
      <Container>
        <ArticleStack.Navigator
          headerMode="screen"
          screenOptions={{
            header: (props) => {
              return <StackNavigatorHeader {...props} />;
            },
          }}
        >
          <ArticleStack.Screen
            name="ArticleCreate"
            component={ArticleCreateScreen}
            options={{ headerTitle: t('Import article') }}
          />
          <ArticleStack.Screen
            name="Article"
            component={ArticleScreen}
            options={{ headerTitle: t('Read') }}
          />
          <ArticleStack.Screen
            name="WordDefinitionsDictionaryLookup"
            component={WordDefinitionsDictionaryLookupScreen}
            options={{ headerTitle: t('Dictionary') }}
          />
        </ArticleStack.Navigator>
      </Container>
    </SafeArea>
  );
};
