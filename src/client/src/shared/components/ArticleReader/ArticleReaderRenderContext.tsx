import * as React from 'react';

import Tts from 'react-native-tts';

import produce from 'immer';
import { compact, has, round } from 'lodash';

import { useNavigation } from '@react-navigation/core';
import { api } from '@src/integrations/backend/backend';
import { Backend } from '@src/integrations/backend/types';
import { useAppSettingsContext } from '@src/shared/contexts/AppSettingsContext';
import { useRerender } from '@src/shared/hooks/useRerender';
import { enqueueTranslationWarmUp } from '@src/shared/utils/cachedOnDeviceTranslation';
import {
  AsyncStorageKey,
  readFromAsyncStore,
  writeToAsyncStore,
} from '@src/shared/utils/storage';

import { getCompoundOrLemmaForTranslation, isWord } from './TokenUtils';

import { ContentLoading } from '../Loading';

type ReadingProgressPointingTo = {
  pageIndex: number;
};

type ShallowConlluPointer = {
  conlluPointer: { paragraphIndex: number; sentenceIndex: number };
};

export type TokenWithPointer = Backend.ConlluTokenViewModel &
  ShallowConlluPointer & {
    readRatio: number;
  };

export type SentenceForPageRendering = {
  tokens: TokenWithPointer[];
} & ShallowConlluPointer;

type ArticleReaderRenderContextValue = {
  article: Backend.ArticleViewModel;
  articlePages: SentenceForPageRendering[][];
  readingProgressPointingTo: ReadingProgressPointingTo;
  articleLanguageCode: string;
  wordFamiliarity: Backend.WordFamiliarityListViewModel;
  updateWordFamiliarity: (
    wordFamiliarityItem: Backend.WordFamiliarityListItemViewModel,
  ) => Promise<void>;
  updateWordFamiliarityForMatchedTokens: (
    fromLevel: number,
    toLevel: number,
    tokens: Backend.ConlluTokenViewModel[],
  ) => Promise<void>;
  subscribeToWordFamiliarity: (
    expression: string,
    onChange: () => void,
  ) => () => void;
  getSelectedToken: () => Backend.ConlluTokenViewModel | null;
  updateSelectedToken: (token: Backend.ConlluTokenViewModel) => void;
  updateReadingProgress: (
    request: Backend.ArticleReadingProgressUpsert,
  ) => void;
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

  selectedToken?: Backend.ConlluTokenViewModel;

  articleLanguageCode: string;

  constructor(articleLanguageCode: string) {
    this.articleLanguageCode = articleLanguageCode;
    this.selectedTokenSubscribers = new Set<() => void>();
    this.expressionSubscriberMapping = Object.create(null);
  }

  subscribeToWordFamiliarity(
    expression: string,
    onChange: () => void,
  ): () => void {
    if (!has(this.expressionSubscriberMapping, expression)) {
      this.expressionSubscriberMapping[expression] = new Set();
    }

    this.expressionSubscriberMapping[expression]?.add(onChange);

    return () => {
      this.expressionSubscriberMapping[expression]?.delete(onChange);
    };
  }

  notifyWordFamiliarityChange(expression: string) {
    if (!has(this.expressionSubscriberMapping, expression)) {
      return;
    }

    this.expressionSubscriberMapping[expression]?.forEach((notify) => notify());
  }

  updateSelectedToken(token: Backend.ConlluTokenViewModel) {
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

const createPaginatedParagraphs = (
  article: Backend.ArticleViewModel,
): {
  articlePages: SentenceForPageRendering[][];
  readingProgressPointingTo: ReadingProgressPointingTo;
  tokenWordExpressionSet: Set<string>;
} => {
  const tokenWordExpressionSet = new Set<string>();
  const articlePages: SentenceForPageRendering[][] = [];

  const readingProgressPointingTo: ReadingProgressPointingTo = {
    pageIndex: 0,
  };

  const flattenedTokens = article.conlluDocument.paragraphs
    .flatMap((p) => p.sentences)
    .flatMap((s) => s.tokens);

  const allTokensCount = flattenedTokens.length;
  let tokenCounter = 0;

  let currentPageTokensCount = 0;
  let page: SentenceForPageRendering[] = [];
  const allSentences = article.conlluDocument.paragraphs.flatMap(
    (paragraph, paragraphIndex) =>
      paragraph.sentences.map((sentence, sentenceIndex) => ({
        sentence,
        conlluPointer: {
          sentenceIndex,
          paragraphIndex,
        },
      })),
  );
  allSentences.forEach(
    ({ sentence, conlluPointer }, sentenceIndexInAllSentences) => {
      currentPageTokensCount += sentence.tokens.length;

      if (
        conlluPointer.paragraphIndex ===
          article.readingProgress.conlluTokenPointer.paragraphIndex &&
        conlluPointer.sentenceIndex ===
          article.readingProgress.conlluTokenPointer.sentenceIndex
      ) {
        readingProgressPointingTo.pageIndex = articlePages.length;
      }

      const sentenceForPageRendering: SentenceForPageRendering = {
        conlluPointer,
        tokens: sentence.tokens.map((t) => {
          tokenCounter += 1;
          if (isWord(t)) {
            tokenWordExpressionSet.add(t.normalisedToken.form);
            tokenWordExpressionSet.add(getCompoundOrLemmaForTranslation(t));
          }

          return {
            ...t,
            conlluPointer,
            readRatio: round(tokenCounter / allTokensCount, 2),
          };
        }),
      };

      page.push(sentenceForPageRendering);

      if (
        currentPageTokensCount >= 300 ||
        sentenceIndexInAllSentences === allSentences.length - 1
      ) {
        articlePages.push(page);
        page = [];
        currentPageTokensCount = 0;
      }
    },
  );

  return {
    articlePages,
    readingProgressPointingTo,
    tokenWordExpressionSet,
  };
};

export const ArticleReaderRenderContextProvider: React.FC<{
  article: Backend.ArticleViewModel;
}> = ({ article, children }) => {
  const navigation = useNavigation();
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
  const { articlePages, readingProgressPointingTo } = React.useMemo(() => {
    const paginated = createPaginatedParagraphs(article);
    enqueueTranslationWarmUp(
      article.languageCode,
      paginated.tokenWordExpressionSet,
    );
    return paginated;
  }, [article]);

  const updateNavigationTitle = (readRatio: number) => {
    navigation.setOptions({
      headerTitle: `(${round(readRatio * 100, 2)}%) ${article.name}`,
    });
  };

  React.useEffect(() => {
    (async () => {
      // TODO: Use local cache first
      savedWordFamiliarity = await api().wordFamiliarities_List();
      saveWordFamiliarity();
      updateNavigationTitle(article.readingProgress.readRatio);
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

    if (!has(savedWordFamiliarity.groupedWordFamiliarities, languageCode)) {
      savedWordFamiliarity.groupedWordFamiliarities[
        languageCode
      ] = Object.create(null);
    }

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
    tokens: Backend.ConlluTokenViewModel[],
  ) => {
    const words = compact(
      tokens.map((token) => {
        const expression = token.normalisedToken.form;

        if (
          !has(
            savedWordFamiliarity.groupedWordFamiliarities,
            article.languageCode,
          )
        ) {
          savedWordFamiliarity.groupedWordFamiliarities[
            article.languageCode
          ] = Object.create(null);
        }

        if (
          has(
            savedWordFamiliarity.groupedWordFamiliarities[article.languageCode],
            expression,
          ) &&
          (savedWordFamiliarity.groupedWordFamiliarities[article.languageCode][
            expression
          ]?.level ?? 0) !== fromLevel
        ) {
          return null;
        }

        const word = {
          languageCode: article.languageCode,
          expression,
        };

        savedWordFamiliarity.groupedWordFamiliarities[article.languageCode][
          expression
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

  const ttsSpeak = async (text: string) => {
    const voice = await ttsVoice;

    if (!voice) {
      return;
    }

    if (
      (voice.networkConnectionRequired || voice.notInstalled) &&
      appSettings.saveDataUsage
    ) {
      return;
    }

    Tts.stop();
    Tts.speak(text);
  };

  const updateReadingProgress = (
    request: Backend.ArticleReadingProgressUpsert,
  ) => {
    api().articles_UpsertReadingProgress({
      id: article.id,
      request,
    });
    updateNavigationTitle(request.readRatio);
  };

  return (
    <ArticleReaderRenderContext.Provider
      value={{
        article,
        articlePages,
        readingProgressPointingTo,
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
        updateSelectedToken: (token: Backend.ConlluTokenViewModel) => {
          subscriptionManager.current.updateSelectedToken(token);
        },
        updateReadingProgress,
        subscribeToSelectedToken: (onChange) => {
          return subscriptionManager.current.subscribeToSelectedToken(onChange);
        },
        ttsSpeak,
      }}
    >
      {children}
    </ArticleReaderRenderContext.Provider>
  );
};

const retriveWordFamiliarityItem = (
  wordFamiliarity: Backend.WordFamiliarityListViewModel,
  articleLanguageCode: string,
  token?: Backend.ConlluTokenViewModel | null,
): Backend.WordFamiliarityListItemViewModel => {
  const wordFamiliarityItem = wordFamiliarity.groupedWordFamiliarities[
    articleLanguageCode
  ]?.[token?.normalisedToken.form ?? ''] ?? {
    level: 0,
    word: {
      expression: token?.normalisedToken.form ?? '',
      languageCode: articleLanguageCode,
    },
  };

  return wordFamiliarityItem;
};

export const useWordTokenHandle = (
  token?: Backend.ConlluTokenViewModel | null,
) => {
  const { rerender } = useRerender();

  const expression = token?.normalisedToken.form ?? '';

  const {
    articleLanguageCode,
    wordFamiliarity,
    subscribeToWordFamiliarity,
    updateSelectedToken,
    ttsSpeak,
    updateReadingProgress,
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
    updateReadingProgress,
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
    articlePages,
    readingProgressPointingTo,
  } = React.useContext(ArticleReaderRenderContext);

  return {
    updateWordFamiliarityForTokens,
    ttsSpeak,
    article,
    articlePages,
    readingProgressPointingTo,
  };
};
