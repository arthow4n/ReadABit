import React from 'react';

import { Content, Spinner } from 'native-base';

export const ContentLoading: React.FC = () => (
  <Content centerContent>
    <Spinner />
  </Content>
);
