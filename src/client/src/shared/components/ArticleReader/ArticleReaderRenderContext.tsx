import * as React from 'react';

import { compact } from 'lodash';

import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import {
  readFromSecureStore,
  StorageKey,
  writeToSecureStore,
} from '@src/shared/utils/storage';

import { ContentLoading } from '../Loading';

type ArticleReaderRenderContextValue = {
  articleLanguageCode: string;
  wordFamiliarity: Backend.WordFamiliarityListViewModel;
  updateWordFamiliarity: (
    wordFamiliarityItem: Backend.WordFamiliarityListItemViewModel,
  ) => Promise<void>;
  updateWordFamiliarityForMatchedTokens: (
    fromLevel: number,
    toLevel: number,
    tokens: Backend.Token[],
  ) => Promise<void>;
  subscribeToWordFamiliarity: (
    expression: string,
    onChange: () => void,
  ) => () => void;
  getSelectedToken: () => Backend.Token | null;
  updateSelectedToken: (token: Backend.Token) => void;
  subscribeToSelectedToken: (onChange: () => void) => () => void;
};

const defaultWordFamiliarity: Backend.WordFamiliarityListViewModel = {
  groupedWordFamiliarities: {},
};

let savedWordFamiliarity: Backend.WordFamiliarityListViewModel = {
  groupedWordFamiliarities: {},
};

export const loadWordFamiliarity = async () => {
  savedWordFamiliarity = (await readFromSecureStore<Backend.WordFamiliarityListViewModel>(
    StorageKey.WordFamiliarity,
  )) ?? { groupedWordFamiliarities: {} };
};

const saveWordFamiliarity = async () => {
  writeToSecureStore(StorageKey.WordFamiliarity, savedWordFamiliarity);
};

const ArticleReaderRenderContext = React.createContext<ArticleReaderRenderContextValue>(
  {
    articleLanguageCode: '',
    wordFamiliarity: defaultWordFamiliarity,
    updateWordFamiliarity: () => Promise.resolve(),
    updateWordFamiliarityForMatchedTokens: () => Promise.resolve(),
    subscribeToWordFamiliarity: () => () => {},
    getSelectedToken: () => null,
    updateSelectedToken: () => {},
    subscribeToSelectedToken: () => () => {},
  },
);

// Tried to write this with private field (.#) syntax
// but I then ran into some weird runtime issues seem to be related to babel config.
class SubscriptionManager {
  expressionSubscriberMapping: {
    [key: string]: Set<() => void> | undefined;
  };

  selectedTokenSubscribers: Set<() => void>;

  selectedToken?: Backend.Token;

  articleLanguageCode: string;

  constructor(articleLanguageCode: string) {
    this.articleLanguageCode = articleLanguageCode;
    this.selectedTokenSubscribers = new Set<() => void>();
    this.expressionSubscriberMapping = {};
  }

  subscribeToWordFamiliarity(
    expression: string,
    onChange: () => void,
  ): () => void {
    this.expressionSubscriberMapping[expression] =
      this.expressionSubscriberMapping[expression] || new Set();

    this.expressionSubscriberMapping[expression]?.add(onChange);

    return () => {
      this.expressionSubscriberMapping[expression]?.delete(onChange);
    };
  }

  notifyWordFamiliarityChange(expression: string) {
    this.expressionSubscriberMapping[expression]?.forEach((notify) => notify());
  }

  updateSelectedToken(token: Backend.Token) {
    this.selectedToken = token;
    this.selectedTokenSubscribers.forEach((notify) => notify());
  }

  subscribeToSelectedToken(onChange: () => void): () => void {
    this.selectedTokenSubscribers.add(onChange);
    return () => {
      this.selectedTokenSubscribers.delete(onChange);
    };
  }
}

export const ArticleReaderRenderContextProvider: React.FC<{
  articleLanguageCode: string;
}> = ({ articleLanguageCode, children }) => {
  const subscriptionManager = React.useRef(
    new SubscriptionManager(articleLanguageCode),
  );
  const [isReady, setIsReady] = React.useState(false);

  React.useEffect(() => {
    (async () => {
      // TODO: Use local cache first
      savedWordFamiliarity = await api.wordFamiliarities_List();
      saveWordFamiliarity();
      setIsReady(true);
    })();
  }, []);

  if (!isReady) {
    return <ContentLoading />;
  }

  const updateWordFamiliarity = async (
    wordFamiliarityItem: Backend.WordFamiliarityListItemViewModel,
  ) => {
    const { languageCode, expression } = wordFamiliarityItem.word;
    savedWordFamiliarity.groupedWordFamiliarities[languageCode] =
      savedWordFamiliarity.groupedWordFamiliarities[languageCode] ?? {};

    savedWordFamiliarity.groupedWordFamiliarities[languageCode][
      expression
    ] = wordFamiliarityItem;

    // TODO: Consider reverting the change on error

    api.wordFamiliarities_UpsertBatch({
      request: {
        level: wordFamiliarityItem.level,
        words: [wordFamiliarityItem.word],
      },
    });

    saveWordFamiliarity();

    subscriptionManager.current.notifyWordFamiliarityChange(expression);
  };

  const updateWordFamiliarityForMatchedTokens = async (
    fromLevel: number,
    toLevel: number,
    tokens: Backend.Token[],
  ) => {
    const words = compact(
      tokens.map((token) => {
        const expression = token.form;

        savedWordFamiliarity.groupedWordFamiliarities[articleLanguageCode] =
          savedWordFamiliarity.groupedWordFamiliarities[articleLanguageCode] ??
          {};

        if (
          (savedWordFamiliarity.groupedWordFamiliarities[articleLanguageCode][
            token.form
          ]?.level ?? 0) !== fromLevel
        ) {
          return null;
        }

        const word = {
          languageCode: articleLanguageCode,
          expression,
        };

        savedWordFamiliarity.groupedWordFamiliarities[articleLanguageCode][
          token.form
        ] = {
          level: toLevel,
          word,
        };

        subscriptionManager.current.notifyWordFamiliarityChange(expression);

        return word;
      }),
    );

    api.wordFamiliarities_UpsertBatch({
      request: {
        level: toLevel,
        words,
      },
    });

    saveWordFamiliarity();
  };

  return (
    <ArticleReaderRenderContext.Provider
      value={{
        articleLanguageCode,
        wordFamiliarity: savedWordFamiliarity,
        updateWordFamiliarity,
        updateWordFamiliarityForMatchedTokens,
        subscribeToWordFamiliarity: (expression, onChange) => {
          return subscriptionManager.current.subscribeToWordFamiliarity(
            expression,
            onChange,
          );
        },
        getSelectedToken: () => {
          return subscriptionManager.current.selectedToken || null;
        },
        updateSelectedToken: (token: Backend.Token) => {
          subscriptionManager.current.updateSelectedToken(token);
        },
        subscribeToSelectedToken: (onChange) => {
          return subscriptionManager.current.subscribeToSelectedToken(onChange);
        },
      }}
    >
      {children}
    </ArticleReaderRenderContext.Provider>
  );
};

const useRerender = () => {
  const [, rerender] = React.useReducer((s) => s + 1, 0);
  return {
    rerender: () => {
      rerender();
    },
  };
};

export const useWordTokenHandle = (expression: string) => {
  const { rerender } = useRerender();

  const {
    articleLanguageCode,
    wordFamiliarity,
    subscribeToWordFamiliarity,
    updateSelectedToken,
  } = React.useContext(ArticleReaderRenderContext);

  React.useEffect(() => {
    return subscribeToWordFamiliarity(expression, rerender);
  }, [expression]);

  return {
    wordFamiliarityItem: wordFamiliarity.groupedWordFamiliarities[
      articleLanguageCode
    ]?.[expression] ?? {
      level: 0,
      word: {
        expression,
        languageCode: articleLanguageCode,
      },
    },
    updateSelectedToken,
  };
};

export const useSelectedTokenDefinitionCardHandle = () => {
  const { rerender } = useRerender();

  const {
    articleLanguageCode,
    getSelectedToken,
    subscribeToSelectedToken,
    updateWordFamiliarity,
  } = React.useContext(ArticleReaderRenderContext);

  React.useEffect(() => {
    return subscribeToSelectedToken(rerender);
  }, []);

  return {
    articleLanguageCode,
    getSelectedToken,
    updateWordFamiliarity,
  };
};

export const useArticleReaderHandle = () => {
  const {
    updateWordFamiliarityForMatchedTokens: updateWordFamiliarityForTokens,
  } = React.useContext(ArticleReaderRenderContext);

  return {
    updateWordFamiliarityForTokens,
  };
};
