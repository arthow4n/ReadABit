import * as React from 'react';

import { Button, StyleSheet } from 'react-native';
import { useMutation } from 'react-query';

import { api } from '../../integrations/backend/backend';
import { LoginButton } from '../../shared/components/LoginButton';
import { Text, View } from '../../shared/components/Themed';

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  title: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  separator: {
    marginVertical: 30,
    height: 1,
    width: '80%',
  },
});

export function TabOneScreen() {
  const { mutate } = useMutation(
    api.articleCollections_CreateArticleCollection,
  );

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Tab One</Text>
      <View
        style={styles.separator}
        lightColor="#eee"
        darkColor="rgba(255,255,255,0.1)"
      />
      <Button
        title="Test create article collection"
        onPress={() => mutate({ languageCode: '', name: 'Test' })}
      />
      <LoginButton />
    </View>
  );
}
