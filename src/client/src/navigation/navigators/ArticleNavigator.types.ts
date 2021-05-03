export type ArticleStackParamList = {
  ArticleCreate: {
    // Don't know why but without making this required
    // the `route.params` object in react-navigationcould be undefined
    preselectedArticleCollectionId: string | null;
  };
  Article: { id: string };
  WordDefinitionsDictionaryLookup: {
    word: string;
    wordLanguageCode: string;
    dictionaryLanguageCode: string;
    wordDefinitionId?: string;
  };
};
