
Title: Pinia — state management for Vue
Source: https://pinia.vuejs.org/

Summary:
- Pinia is the official state management library for Vue 3: small, type-safe, modular, and designed to be intuitive for developers familiar with Vue’s APIs. Stores are plain objects with reactive state, getters, and actions.

Core concepts:
- Create stores with `defineStore` and use them inside components via `useStore()`.
- Stores can be split per domain (users, cart, ui) and are automatically tree-shakeable and code-splittable.

Best practices / Actionables:
- Keep stores focused: state + actions. Put presentation logic in components or composables.
- Use TypeScript types for state and getters for strong typing and autocompletion.
- Persist store slices when needed (plugins like `pinia-plugin-persistedstate`) but keep secrets out of persisted stores.
- Test stores by invoking actions and verifying state transitions; isolate side effects where possible.

Integration notes:
- Works seamlessly with Vue Devtools for state inspection and time-travel debugging.
- Use plugins to add cross-cutting concerns (persistence, subscriptions, logger).

References:
- Core concepts: https://pinia.vuejs.org/core-concepts/
- API: https://pinia.vuejs.org/api/

