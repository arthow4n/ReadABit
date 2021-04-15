import * as React from 'react';

export const useRerender = () => {
  const [, rerender] = React.useReducer((s) => s + 1, 0);
  return {
    rerender: () => {
      rerender();
    },
  };
};
