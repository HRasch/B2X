import js from '@eslint/js';
import pluginVue from 'eslint-plugin-vue';
import vueTsEslintConfig from '@vue/eslint-config-typescript';
import skipFormatting from '@vue/eslint-config-prettier/skip-formatting';

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
      '**/test-results/**',
      '**/*.js', // Ignore JS files for now
    ],
  },
  js.configs.recommended,
  ...pluginVue.configs['flat/essential'],
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
];
