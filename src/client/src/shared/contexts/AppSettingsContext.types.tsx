export type AppSettings = {
  languageCodes: {
    // TODO: Use enum for language codes.
    studying: string;
    ui: string;
  };
  /**
   * If true, mobile data will be used as well for downloading larger content.
   */
  useMobileDataForAllDataTransfer: boolean;
  tts: {
    autoSpeakWhenTapOnWord: boolean;
  };
};
