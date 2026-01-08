---
docid: KB-190
title: Vite
owner: @DocMaintainer
status: Active
created: 2026-01-08
---


Title: Vite — tooling for modern frontend builds
Source: https://vite.dev/

Summary:
- Vite is a modern frontend build tool that uses native ES modules during development and Rollup for optimized production bundles. It provides an instant dev server, fast HMR, and a pluggable plugin system tailored for frameworks (Vue, React, Svelte, etc.).
- Features: dev server with on-demand ESM, fast HMR that scales with app size, first-class SSR support, optimized production via Rollup, and a well-typed programmatic API.

Quick commands:
- Dev server: `npm run dev` (or `vite`)
- Build: `vite build`
- Preview production bundle: `vite preview`

Best practices / Actionables:
- Use `optimizeDeps` for tricky dependency pre-bundling issues.
- Keep secret/config values in environment variables and `.env` files (do not commit secrets).
- Use official plugins and the `vite.config` to keep consistent build outputs for CI.

References:
- Guide: https://vite.dev/guide/
- Config reference: https://vite.dev/config/

