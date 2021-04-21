import { useNavigation } from '@react-navigation/core';
import * as React from 'react';

export const useNavigationRefocusEffect = (onRefocus: () => void) => {
  const navigation = useNavigation();

  React.useEffect(() => {
    const refetchCallback = () => {
      onRefocus();
      navigation.removeListener('focus', refetchCallback);
    };

    const blurCleanup = navigation.addListener('blur', () => {
      navigation.addListener('focus', refetchCallback);
    });

    return () => {
      blurCleanup();
      navigation.removeListener('focus', refetchCallback);
    };
  }, [onRefocus]);
};
