module.exports = {
  trailingComma: 'all',
  singleQuote: true,
  importOrder: [
    '^react$',
    '^react',
    '^[^@.].*',
    '^@',
    '^[.]/',
    '^[.][.]/',
    '.*',
  ],
  importOrderSeparation: true,
  overrides: [
    {
      files: '*.{ts,tsx}',
      options: {
        parser: 'babel-ts',
      },
    },
  ],
};
