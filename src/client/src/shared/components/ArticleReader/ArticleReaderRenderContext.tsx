import * as React from 'react';

import Tts from 'react-native-tts';

import produce from 'immer';
import { compact } from 'lodash';

import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import {
  AsyncStorageKey,
  readFromAsyncStore,
  writeToAsyncStore,
} from '@src/shared/utils/storage';
import { useRerender } from '@src/shared/hooks/useRerender';

import { ContentLoading } from '../Loading';

type ArticleReaderRenderContextValue = {
  article: Backend.ArticleViewModel;
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
  ttsSpeak: (text: string) => void;
};

let savedWordFamiliarity: Backend.WordFamiliarityListViewModel = {
  groupedWordFamiliarities: {},
};

export const loadWordFamiliarity = async () => {
  savedWordFamiliarity = (await readFromAsyncStore(
    AsyncStorageKey.WordFamiliarity,
  )) ?? { groupedWordFamiliarities: {} };
};

const saveWordFamiliarity = async () => {
  writeToAsyncStore(AsyncStorageKey.WordFamiliarity, savedWordFamiliarity);
};

const ArticleReaderRenderContext = React.createContext<ArticleReaderRenderContextValue>(
  (null as unknown) as ArticleReaderRenderContextValue,
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
  article: Backend.ArticleViewModel;
}> = ({ article, children }) => {
  const { appSettings } = useAppSettingsContext();
  const subscriptionManager = React.useRef(
    new SubscriptionManager(article.languageCode),
  );
  const [isReady, setIsReady] = React.useState(false);
  const ttsVoice = React.useMemo(async () => {
    await Tts.getInitStatus();
    const voices = await Tts.voices();

    const voice = voices.find((v) =>
      v.language.startsWith(article.languageCode),
    );
    if (voice) {
      await Tts.setDefaultLanguage(voice.language);
    }

    // TODO: Maybe allow choosing engine/voice/...etc.

    return voice;
  }, [article]);

  React.useEffect(() => {
    (async () => {
      // TODO: Use local cache first
      savedWordFamiliarity = await api().wordFamiliarities_List();
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
    if (!expression) {
      return;
    }

    savedWordFamiliarity.groupedWordFamiliarities[languageCode] =
      savedWordFamiliarity.groupedWordFamiliarities[languageCode] ?? {};

    savedWordFamiliarity.groupedWordFamiliarities[languageCode][
      expression
    ] = wordFamiliarityItem;

    // TODO: Consider reverting the change on error

    api().wordFamiliarities_UpsertBatch({
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

        savedWordFamiliarity.groupedWordFamiliarities[article.languageCode] =
          savedWordFamiliarity.groupedWordFamiliarities[article.languageCode] ??
          {};

        if (
          (savedWordFamiliarity.groupedWordFamiliarities[article.languageCode][
            token.form
          ]?.level ?? 0) !== fromLevel
        ) {
          return null;
        }

        const word = {
          languageCode: article.languageCode,
          expression,
        };

        savedWordFamiliarity.groupedWordFamiliarities[article.languageCode][
          token.form
        ] = {
          level: toLevel,
          word,
        };

        subscriptionManager.current.notifyWordFamiliarityChange(expression);

        return word;
      }),
    );

    api().wordFamiliarities_UpsertBatch({
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
        article,
        articleLanguageCode: article.languageCode,
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
        ttsSpeak: async (text: string) => {
          const voice = await ttsVoice;

          if (!voice) {
            return;
          }

          if (
            (voice.networkConnectionRequired || voice.notInstalled) &&
            !appSettings.useMobileDataForAllDataTransfer
          ) {
            return;
          }

          Tts.stop();
          Tts.speak(text);
        },
      }}
    >
      {children}
    </ArticleReaderRenderContext.Provider>
  );
};

const retriveWordFamiliarityItem = (
  wordFamiliarity: Backend.WordFamiliarityListViewModel,
  articleLanguageCode: string,
  token?: Backend.Token | null,
): Backend.WordFamiliarityListItemViewModel => {
  const wordFamiliarityItem = wordFamiliarity.groupedWordFamiliarities[
    articleLanguageCode
  ]?.[token?.form ?? ''] ?? {
    level: 0,
    word: {
      expression: token?.form ?? '',
      languageCode: articleLanguageCode,
    },
  };

  return wordFamiliarityItem;
};

export const useWordTokenHandle = (token?: Backend.Token | null) => {
  const { rerender } = useRerender();

  const expression = token?.form ?? '';

  const {
    articleLanguageCode,
    wordFamiliarity,
    subscribeToWordFamiliarity,
    updateSelectedToken,
    ttsSpeak,
  } = React.useContext(ArticleReaderRenderContext);

  React.useEffect(() => {
    return subscribeToWordFamiliarity(expression, rerender);
  }, [expression]);

  return {
    wordFamiliarityItem: retriveWordFamiliarityItem(
      wordFamiliarity,
      articleLanguageCode,
      token,
    ),
    updateSelectedToken,
    ttsSpeak,
  };
};

export const useSelectedTokenDefinitionCardHandle = () => {
  const { rerender } = useRerender();

  const {
    articleLanguageCode,
    getSelectedToken,
    subscribeToSelectedToken,
    wordFamiliarity,
    updateWordFamiliarity,
  } = React.useContext(ArticleReaderRenderContext);

  React.useEffect(() => {
    return subscribeToSelectedToken(rerender);
  }, []);

  React.useEffect(() => {
    const wordFamiliarityItem = retriveWordFamiliarityItem(
      wordFamiliarity,
      articleLanguageCode,
      getSelectedToken(),
    );

    if (wordFamiliarityItem.level === 0) {
      updateWordFamiliarity(
        produce(wordFamiliarityItem, (draft) => {
          draft.level = 1;
        }),
      );
    }
  }, [getSelectedToken()]);

  return {
    articleLanguageCode,
    getSelectedToken,
    updateWordFamiliarity,
  };
};

export const useArticleReaderHandle = () => {
  const {
    updateWordFamiliarityForMatchedTokens: updateWordFamiliarityForTokens,
    ttsSpeak,
    article,
  } = React.useContext(ArticleReaderRenderContext);

  return {
    updateWordFamiliarityForTokens,
    ttsSpeak,
    article,
  };
};
