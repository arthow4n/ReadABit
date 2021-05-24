export const jsForWebView = ({
  beforeDOMContentLoaded = '',
  onDOMContentLoaded = '',
}: {
  beforeDOMContentLoaded?: string;
  onDOMContentLoaded?: string;
}) =>
  `${beforeDOMContentLoaded}

window.addEventListener('DOMContentLoaded', (event) => {
${onDOMContentLoaded}
});

// WARNING: Don't remove the true at the end of script!
// https://github.com/react-native-webview/react-native-webview/blob/master/docs/Guide.md#communicating-between-js-and-native
true;
`;
