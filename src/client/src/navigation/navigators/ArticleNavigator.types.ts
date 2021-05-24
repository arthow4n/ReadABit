export type ArticleStackParamList = {
  ArticleCreate: {
    preselectedArticleCollectionId?: string;
  };
  Article: { id: string };
  WordDefinitionsDictionaryLookup: {
    /**
     * string[] stringified.
     */
    wordsJson: string;
    wordLanguageCode: string;
    dictionaryLanguageCode: string;
    wordDefinitionId?: string;
  };
};
