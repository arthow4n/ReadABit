export type ArticleStackParamList = {
  ArticleCreate: undefined;
  Article: { id: string };
  WordDefinitionsDictionaryLookup: {
    word: string;
    wordLanguageCode: string;
    dictionaryLanguageCode: string;
    wordDefinitionId?: string;
  };
};
