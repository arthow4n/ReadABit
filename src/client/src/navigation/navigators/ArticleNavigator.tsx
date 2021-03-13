import React from 'react';

import { createStackNavigator } from '@react-navigation/stack';

import { ArticleStackParamList } from './ArticleNavigator.types';

import { ArticleCreateScreen } from '../screens/ArticleCreateScreen';
import { ArticleScreen } from '../screens/ArticleScreen';

const ArticleStack = createStackNavigator<ArticleStackParamList>();

export const ArticleNavigator: React.FC = () => {
  return (
    <ArticleStack.Navigator>
      <ArticleStack.Screen
        name="ArticleCreate"
        component={ArticleCreateScreen}
      />
      <ArticleStack.Screen name="Article" component={ArticleScreen} />
    </ArticleStack.Navigator>
  );
};
