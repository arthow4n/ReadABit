const fs = require('fs');
const path = require('path');
const typescript = require('typescript');

module.exports = {
  input: ['src/**/*.{js,jsx,ts,tsx}', '!**/node_modules/**'],
  // https://github.com/i18next/i18next-scanner/issues/140#issuecomment-571154030
  output: './',
  options: {
    debug: true,
    func: {
      list: ['i18next.t', 'i18n.t'],
      // https://github.com/i18next/i18next-scanner/issues/88#issuecomment-562446459
      extensions: ['.js', '.jsx'],
    },
    trans: {
      component: 'Trans',
      i18nKey: 'i18nKey',
      defaultsKey: 'defaults',
      extensions: ['.js', '.jsx'],
      fallbackKey: function (ns, value) {
        return value;
      },
    },
    lngs: ['en', 'zh-TW'],
    defaultLng: 'en',
    ns: ['core'],
    defaultNs: 'core',
    defaultValue: (lng, ns, key) => (lng === 'en' ? key : ''),
    resource: {
      loadPath: 'src/translations/{{lng}}/{{ns}}.json',
      savePath: 'src/translations/{{lng}}/{{ns}}.json',
      jsonIndent: 2,
      lineEnding: '\n',
    },
    // Separators set to false to disable and gettext style
    // https://github.com/i18next/i18next-scanner#nsseparator
    // https://github.com/i18next/i18next-scanner#keyseparator
    nsSeparator: false,
    keySeparator: false,
    interpolation: {
      prefix: '{{',
      suffix: '}}',
    },
  },
  // For TypeScript support
  // https://github.com/i18next/i18next-scanner/issues/88
  transform: function tsTransform(file, enc, done) {
    const { base, ext } = path.parse(file.path);

    if (['.ts', '.tsx'].includes(ext) && !base.includes('.d.ts')) {
      const content = fs.readFileSync(file.path, enc);

      const { outputText } = typescript.transpileModule(content, {
        compilerOptions: require('./tsconfig.json')['compilerOptions'],
        fileName: path.basename(file.path),
      });

      this.parser.parseTransFromString(outputText);
      this.parser.parseFuncFromString(outputText, {
        list: ['t'],
      });
    }

    done();
  },
};
