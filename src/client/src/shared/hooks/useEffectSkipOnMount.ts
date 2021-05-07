import * as React from 'react';

/**
 * Mostly same as `React.useEffect`, except for that the first effect run is skipped.
 */
export const useEffectSkipOnMount = (
  effectCallback: React.EffectCallback,
  deps: React.DependencyList,
) => {
  const mounted = React.useRef(false);

  React.useEffect(() => {
    if (!mounted.current) {
      mounted.current = true;
      return;
    }

    return effectCallback();
  }, deps);
};
