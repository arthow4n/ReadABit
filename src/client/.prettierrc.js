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
};
