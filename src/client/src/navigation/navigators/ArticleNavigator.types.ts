export type ArticleStackParamList = {
  ArticleCreate: undefined;
  Article: { id: string };
  WordDefinitionsDictionaryLookup: {
    word: string;
    wordLanguage: string;
    dictionaryLanguage: string;
    wordDefinitionId?: string;
  };
};
