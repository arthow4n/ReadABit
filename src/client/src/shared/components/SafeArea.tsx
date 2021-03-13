import React from 'react';

import { SafeAreaView } from 'react-native-safe-area-context';

export const SafeArea: React.FC = ({ children }) => {
  return (
    <SafeAreaView style={{ flex: 1, backgroundColor: '#3F51B5' }}>
      {children}
    </SafeAreaView>
  );
};
