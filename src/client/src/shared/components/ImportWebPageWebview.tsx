import * as React from 'react';

import WebView, { WebViewMessageEvent } from 'react-native-webview';

import * as z from 'zod';

const resultSchema = z.object({
  title: z.string().nonempty(),
  content: z.string().nonempty(),
});

export const ImportWebPageWebview: React.FC<{
  url: string;
  onParsed: (result: z.infer<typeof resultSchema>) => void;
  onFail: () => void;
}> = ({ url, onParsed, onFail }) => {
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

  const injectedJavaScript = `
  window.addEventListener('DOMContentLoaded', (event) => {    
    const title = document.title;
    const content = Array.from(document.querySelectorAll('article')).map(x => x.innerText).join('\\n\\n\\n======\\n\\n\\n');
    window.ReactNativeWebView.postMessage(JSON.stringify({ title, content }));
  });

  // WARNING: Don't remove the true at the end of script!
  // https://github.com/react-native-webview/react-native-webview/blob/master/docs/Guide.md#communicating-between-js-and-native
  true;
  `;

  return (
    <WebView
      style={{ display: 'none' }}
      source={{ uri: url }}
      injectedJavaScriptBeforeContentLoaded={injectedJavaScript}
      onMessage={extractFromMessage}
    />
  );
};
