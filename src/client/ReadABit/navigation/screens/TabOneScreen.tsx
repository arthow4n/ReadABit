import * as React from 'react';
import { StyleSheet } from 'react-native';
import { useQuery } from 'react-query';

import EditScreenInfo from '../../shared/components/EditScreenInfo';
import { Text, View } from '../../shared/components/Themed';
import { api } from '../../integrations/backend/backend';

export default function TabOneScreen() {
  const { isLoading, data, isError, error } = useQuery(["articles", "00000000-0000-0000-0000-000000000000"], () => api.articles_GetArticle("00000000-0000-0000-0000-000000000000"));

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Tab One</Text>
      <View style={styles.separator} lightColor="#eee" darkColor="rgba(255,255,255,0.1)" />
      {isLoading && <Text>useQuery is isLoading...</Text>}
      {data && <Text>{data.title}</Text>}
      {isError && <Text>{JSON.stringify(error)}</Text>}
      <EditScreenInfo path="/screens/TabOneScreen.tsx" />
    </View>
  );
}

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
