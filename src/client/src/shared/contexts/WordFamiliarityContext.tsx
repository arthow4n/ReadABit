import * as React from 'react';

import { useQuery } from 'react-query';

import { clone } from 'lodash';

import { api } from '../../integrations/backend/backend';
import { Backend } from '../../integrations/backend/types';
import {
  QueryCacheKey,
  queryCacheKey,
  useMutateWordFamiliarityDelete,
  useMutateWordFamiliarityUpsert,
} from '../hooks/useBackendReactQuery';
import {
  readFromSecureStore,
  StorageKey,
  writeToSecureStore,
} from '../utils/storage';

type WordFamiliarityContextValue = {
  wordFamiliarity: Backend.WordFamiliarityListViewModel;
  updateWordFamiliarity: (
    wordFamiliarityItem: Backend.WordFamiliarityListItemViewModel,
  ) => Promise<void>;
  deleteWordFamiliarity: (
    wordFamiliaritySelector: Backend.WordFamiliarityListItemViewModel['word'],
  ) => Promise<void>;
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

const WordFamiliarityContext = React.createContext<WordFamiliarityContextValue>(
  {
    wordFamiliarity: defaultWordFamiliarity,
    updateWordFamiliarity: () => Promise.resolve(),
    deleteWordFamiliarity: () => Promise.resolve(),
  },
);

export const WordFamiliarityContextProvider: React.FC = ({ children }) => {
  const [
    wordFamiliarityState,
    setWordFamiliarityState,
  ] = React.useState<Backend.WordFamiliarityListViewModel>(
    savedWordFamiliarity || defaultWordFamiliarity,
  );

  const saveAndSetState = () => {
    setWordFamiliarityState(clone(savedWordFamiliarity));
    // This is not awaited because we don't rely on its state.
    writeToSecureStore(StorageKey.WordFamiliarity, savedWordFamiliarity);
  };

  useQuery(
    queryCacheKey(QueryCacheKey.WordFamiliarityList),
    () => api.wordFamiliarities_List(),
    {
      onSuccess: (data) => {
        savedWordFamiliarity = data;
        saveAndSetState();
      },
    },
  );

  const upsertHandle = useMutateWordFamiliarityUpsert();
  const deleteHandle = useMutateWordFamiliarityDelete();

  return (
    <WordFamiliarityContext.Provider
      value={{
        wordFamiliarity: wordFamiliarityState,
        updateWordFamiliarity: async (wordFamiliarityItem) => {
          const { languageCode, expression } = wordFamiliarityItem.word;
          savedWordFamiliarity.groupedWordFamiliarities[languageCode] =
            savedWordFamiliarity.groupedWordFamiliarities[languageCode] ?? {};

          savedWordFamiliarity.groupedWordFamiliarities[languageCode][
            expression
          ] = wordFamiliarityItem;

          // TODO: Consider reverting the change on error
          upsertHandle.mutateAsync({
            request: wordFamiliarityItem,
          });

          saveAndSetState();
        },
        deleteWordFamiliarity: async (wordFamiliaritySelector) => {
          const { languageCode, expression } = wordFamiliaritySelector;
          delete savedWordFamiliarity.groupedWordFamiliarities[languageCode][
            expression
          ];

          // TODO: Consider reverting the change on error
          deleteHandle.mutateAsync({
            word_Expression: wordFamiliaritySelector.expression,
            word_LanguageCode: wordFamiliaritySelector.languageCode,
          });

          saveAndSetState();
        },
      }}
    >
      {children}
    </WordFamiliarityContext.Provider>
  );
};

export const useWordFamiliarityContext = () =>
  React.useContext(WordFamiliarityContext);
