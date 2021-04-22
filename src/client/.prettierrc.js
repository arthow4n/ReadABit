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
  // Commented out because this often doesn't work for normal tsx files
  // but sometimes it works great for files with weird syntax.
  // overrides: [
  //   {
  //     files: '*.{ts,tsx}',
  //     options: {
  //       parser: 'babel-ts',
  //     },
  //   },
  // ],
};
