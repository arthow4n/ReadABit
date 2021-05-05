/**
 * @example
 * escapeUri`https://example.com/?q=${"1 2"}` === "https://example.com/?q=1%202"
 */
const joinUri = (strs: TemplateStringsArray, ...components: string[]) =>
  strs.map((s, i) => s + encodeURIComponent(components[i])).join('');

/**
 * Take the first two chars of input string.
 * Mainly for cutting BCP 47 language code into shorter form
 */
const to2 = (s: string) => s.slice(0, 2);

export interface WebDictionary {
  name: string;
  supportedSourceLanguageCodes: Set<string>;
  /**
   * @param sourceLanguageCode BCP 47 like everywhere in this app,
   * the implementation of this method should transform the tag correspondingly.
   * For example even if we are calling this function with `wordPageUrl("sv-SE", "en", "även")`,
   * and the dictionary only takes `sv` in its URL,
   * then this method should take care of transforming `sv-SE` to `sv`.
   * @param targetLanguageCode This is more like "translate to this language when possible",
   * thus there's no "supportedTargetLanguageCodes".
   * If the user is looking up a Swedish word in Sweidsh-Swedish dictionary SAOL,
   * we can still call this function like `wordPageUrl("sv", "en", "även")`,
   * and ignore `targetLanguageCode` in the method implementation.
   * @param wordExpression
   */
  wordPageUrl(
    sourceLanguageCode: string,
    targetLanguageCode: string,
    wordExpression: string,
  ): string;
}

const dictionaries: WebDictionary[] = [
  {
    name: 'Glosbe',
    supportedSourceLanguageCodes: new Set(['sv', 'en']),
    wordPageUrl: (
      sourceLanguageCode: string,
      targetLanguageCode: string,
      wordExpression: string,
    ) =>
      joinUri`https://${to2(targetLanguageCode)}.glosbe.com/${to2(
        sourceLanguageCode,
      )}/${to2(targetLanguageCode)}/${wordExpression}`,
  },
  {
    name: 'SAOL/SO/SAOB – svenska.se',
    supportedSourceLanguageCodes: new Set(['sv']),
    wordPageUrl: (
      sourceLanguageCode: string,
      targetLanguageCode: string,
      wordExpression: string,
    ) => joinUri`https://svenska.se/tre/?sok=${wordExpression}`,
  },
];

export const findSupportedWebDictionary = (sourceLanguageCode: string) =>
  dictionaries.filter((x) =>
    x.supportedSourceLanguageCodes.has(sourceLanguageCode),
  );
