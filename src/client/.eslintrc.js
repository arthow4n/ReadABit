module.exports = {
  extends: ['airbnb-typescript', 'plugin:jest/all'],
  plugins: ['jest', 'prettier'],
  parserOptions: {
    project: './tsconfig.json',
  },
  rules: {
    'import/prefer-default-export': 'off',
    'import/no-default-export': 'warn',
    'react/jsx-props-no-spreading': 'off',
    'react/destructuring-assignment': 'off',
    'no-console': 'off',
    'react/prop-types': 'off',
    'jest/prefer-expect-assertions': 'off',
    'operator-linebreak': 'off',
    'object-curly-newline': 'off',
    '@typescript-eslint/no-unused-vars': ['warn'],
    'implicit-arrow-linebreak': 'off',
    'function-paren-newline': 'off',
  },
};
