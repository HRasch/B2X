import js from '@eslint/js'
import vue from 'eslint-plugin-vue'
import vueA11y from 'eslint-plugin-vue-a11y'

export default [
  js.configs.recommended,
  ...vue.configs['flat/essential'],
  vueA11y.configs['flat/recommended'],
  {
    ignores: ['dist/**', 'node_modules/**', 'coverage/**'],
  },
  {
    rules: {
      'vue/multi-word-component-names': 'off',
      'no-unused-vars': 'off',
    },
  },
]