import js from '@eslint/js';
import vueTsEslintConfig from '@vue/eslint-config-typescript';
import skipFormatting from '@vue/eslint-config-prettier/skip-formatting';
import noHardcodedStrings from './eslint-rules/no-hardcoded-strings.js';

export default [
  {
    name: 'app/files-to-lint',
    files: ['**/*.{ts,mts,tsx,vue}'],
  },
  {
    name: 'app/files-to-ignore',
    ignores: [
      '**/dist/**',
      '**/dist-ssr/**',
      '**/coverage/**',
      '**/node_modules/**',
      '**/playwright-report/**',
      'reports/test-results/**',
      '**/.nuxt/**',
      '**/.output/**',
      '**/*.js', // Ignore JS files for now
      'components/**', // Ignore root components directory, use src/components/
    ],
  },
  js.configs.recommended,
  ...vueTsEslintConfig(),
  skipFormatting,
  {
    rules: {
      'vue/multi-word-component-names': 'off',
      'vue/block-lang': 'off',
      '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/no-unused-expressions': 'off',
      'no-useless-catch': 'warn',
    },
  },
  {
    files: ['**/*.vue'],
    plugins: {
      custom: {
        rules: {
          'no-hardcoded-strings': noHardcodedStrings,
        },
      },
    },
    rules: {
      'custom/no-hardcoded-strings': [
        'error',
        {
          ignoreText: ['&nbsp;', ' ', '\n', '\t', '\r'],
          ignoreAttributes: ['placeholder', 'alt', 'title', 'aria-label'],
          ignoreDirectives: ['t'],
          minLength: 3,
        },
      ],
    },
  },
];
