import * as React from 'react';

import WebView, { WebViewMessageEvent } from 'react-native-webview';

import { Asset } from 'expo-asset';
import * as z from 'zod';

import readabilityRawJs from '@src/../node_modules/@mozilla/readability/Readability.js.txt';

import { useRerender } from '../hooks/useRerender';
import { jsForWebView } from '../utils/webview';

let readabilityRawJsContent: string | null = null;
const readabilityRawJsLoadPromise = (async () => {
  const asset = Asset.fromModule(readabilityRawJs);
  await asset.downloadAsync();

  readabilityRawJsContent = await (await fetch(asset.uri)).text();
})();

export enum Scraper {
  SimplifiedBodyInnerTextScraper = `
  const title = document.title;
  [...document.querySelectorAll('select,input')].forEach(el => el.remove());
  const content = document.body.innerText;
  `,
  ReadabilityScraper = `
  const article = new Readability(document, { charThreshold: 64 }).parse();
  const title = article.title;
  const content = article.textContent;
  `,
}

const resultSchema = z.object({
  title: z.string().nonempty(),
  content: z.string().nonempty(),
});

export const ImportWebPageWebview: React.FC<{
  url: string;
  onParsed: (result: z.infer<typeof resultSchema>) => void;
  onFail: () => void;
  scraper: Scraper;
}> = ({ url, onParsed, onFail, scraper }) => {
  const { rerender } = useRerender();

  if (!readabilityRawJsContent) {
    readabilityRawJsLoadPromise.then(rerender);
    return null;
  }

  // TODO: Allow choosing more parser options e.g. event listener timing.

  const extractFromMessage = async (event: WebViewMessageEvent) => {
    const result = await resultSchema.safeParseAsync(
      JSON.parse(event.nativeEvent.data),
    );

    if (!result.success) {
      onFail();
      return;
    }

    onParsed(result.data);
  };

  return (
    <WebView
      style={{ display: 'none' }}
      source={{ uri: url }}
      injectedJavaScriptBeforeContentLoaded={jsForWebView({
        beforeDOMContentLoaded: `
${readabilityRawJsContent}
`,
        onDOMContentLoaded: `
${scraper}
window.ReactNativeWebView.postMessage(JSON.stringify({ title, content }));
`,
      })}
      onMessage={extractFromMessage}
    />
  );
};
