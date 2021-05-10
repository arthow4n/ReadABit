export type ArticleStackParamList = {
  ArticleCreate: {
    preselectedArticleCollectionId?: string;
  };
  Article: { id: string };
  WordDefinitionsDictionaryLookup: {
    word: string;
    wordLanguageCode: string;
    dictionaryLanguageCode: string;
    wordDefinitionId?: string;
  };
};
