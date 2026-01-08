---
docid: KB-191
title: Vue
owner: @DocMaintainer
status: Active
created: 2026-01-08
---


Title: Vue.js 3 — Overview
Source: https://vuejs.org/

Summary:
- Vue 3 is a progressive JavaScript framework for building reactive user interfaces. It provides both the Options API and the Composition API (recommended for complex logic and reusable concerns) and compiles templates into performant render functions.
- Ecosystem: official router (`vue-router`), state management (`pinia`), devtools, and first-class TypeScript support.
- Nuxt 3: Full-stack Vue framework built on Vue 3, providing SSR, SSG, file-based routing, auto-imports, and modern development experience.
- Latest stable version: 3.5.26 (released 3 weeks ago as of Jan 2026), with Vapor Mode beta in 3.6.0-beta.2.

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
- Vapor Mode: Opt-in compilation mode for reduced bundle size and improved performance (requires `<script setup vapor>`).

Nuxt 3 Framework:
- Latest version: 3.20.2 (as of October 2025)
- Features: Server-side rendering (SSR), static site generation (SSG), file-based routing, auto-imports, TypeScript support, module system
- Migration from Nuxt 2: Major rewrite with Vue 3, Nitro engine, new module system. Breaking changes include new config format, composables API, and build system.
- Migration guide: https://nuxt.com/docs/migration/overview
- Upgrade command: `npx nuxt upgrade --dedupe`

CLI & tooling:
- Create Vue app: `npm init vue@latest` or use Vite templates.
- Create Nuxt app: `npx nuxi@latest init <project-name>`
- Dev server: `npm run dev` (via Vite for Vue, Nuxt dev server for Nuxt)
- Nuxt CLI: `nuxi` commands for building, generating, and managing Nuxt projects

References:
- Vue Guide: https://vuejs.org/guide/introduction
- Vue API: https://vuejs.org/api
- Vue Migration guide: https://v3.vuejs.org/guide/migration/introduction.html
- Nuxt Docs: https://nuxt.com/docs/getting-started/introduction
- Nuxt Migration: https://nuxt.com/docs/migration/overview
- Latest releases: https://github.com/vuejs/core/releases

