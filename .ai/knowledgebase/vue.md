
Title: Vue.js 3 — Overview
Source: https://vuejs.org/

Summary:
- Vue 3 is a progressive JavaScript framework for building reactive user interfaces. It provides both the Options API and the Composition API (recommended for complex logic and reusable concerns) and compiles templates into performant render functions.
- Ecosystem: official router (`vue-router`), state management (`pinia`), devtools, and first-class TypeScript support.

Core concepts:
- Reactivity: `ref`, `reactive`, `computed`, and `watch` primitives power change detection.
- Composition API: organize logic by feature, not by lifecycle hook; use `setup()` for component initialization.
- Single File Components (SFCs): `.vue` files with `<template>`, `<script>`, and `<style>` sections; support `<script setup>` for ergonomic composition.

Best practices / Actionables:
- Prefer the Composition API for complex components and shared logic via composables.
- Keep templates declarative; avoid manipulating the DOM directly.
- Security: never insert untrusted HTML via `v-html` without sanitization; validate and escape user inputs on the server.
- Testing: use `@vue/test-utils` and Vitest/Jest for unit tests; use Playwright or Cypress for E2E.
- Performance: use lazy-loaded routes, component-level code-splitting, and memoize expensive computations with `computed`.

CLI & tooling:
- Create app: `npm init vue@latest` or use Vite templates.
- Dev server: `npm run dev` (via Vite)

References:
- Guide: https://vuejs.org/guide/introduction
- API: https://vuejs.org/api
- Migration guide: https://v3.vuejs.org/guide/migration/introduction.html

