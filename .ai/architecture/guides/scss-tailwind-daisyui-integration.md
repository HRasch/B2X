# SCSS Integration with Tailwind CSS and DaisyUI: Best Practices

## Overview
Tailwind CSS uses PostCSS for processing, while DaisyUI is a plugin that extends Tailwind. SCSS (Sass) can be integrated alongside PostCSS for enhanced styling capabilities, particularly useful for complex theming in projects like B2Connect's tenant-themed frontend.

## Key Concepts
- **Tailwind CSS**: Utility-first framework processed via PostCSS. Uses `@theme` directive for theme variables (e.g., `--color-*`, `--font-*`).
- **DaisyUI**: Component library built on Tailwind. Configured via PostCSS plugins to enable themes and components.
- **SCSS Integration**: Adds Sass preprocessing capabilities, allowing mixins, variables, and nesting alongside Tailwind's utilities.

## Best Practices

### 1. Build Tool Configuration
- **Vite (Recommended for Vue.js)**: Vite natively supports SCSS. Install `sass` package and configure PostCSS for Tailwind/DaisyUI.
  ```bash
  npm install -D sass
  ```
  Vite config example:
  ```js
  import { defineConfig } from 'vite'
  import vue from '@vitejs/plugin-vue'

  export default defineConfig({
    plugins: [vue()],
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `@import "@/styles/variables.scss";`
        }
      }
    }
  })
  ```

- **PostCSS Setup**: Ensure Tailwind and DaisyUI are processed after SCSS.
  ```js
  // postcss.config.js
  module.exports = {
    plugins: {
      'postcss-import': {},
      'tailwindcss': {},
      'daisyui': {
        themes: ['light', 'dark'], // Configure DaisyUI themes
      },
      'autoprefixer': {},
    }
  }
  ```

### 2. Theme Architecture
- **Use CSS Custom Properties**: For runtime theming, define variables in SCSS that can be overridden dynamically.
  ```scss
  // _variables.scss
  :root {
    --primary-color: #3b82f6; // Default
    --secondary-color: #64748b;
  }

  // Component styles
  .btn-primary {
    background-color: var(--primary-color);
  }
  ```

- **Tailwind @theme Integration**: For build-time themes, use Tailwind's `@theme` in CSS files.
  ```css
  @import "tailwindcss";
  @theme {
    --color-primary: #3b82f6;
    --color-secondary: #64748b;
  }
  ```

- **DaisyUI Theme Configuration**: Configure themes in PostCSS config or CSS.
  ```css
  @plugin "daisyui" {
    themes: light --default, dark --prefersdark, custom;
  }
  ```

### 3. SCSS Organization
- **Separate Concerns**: Use SCSS for complex logic, Tailwind for utilities.
  ```scss
  // _mixins.scss
  @mixin theme-aware($property, $light-value, $dark-value) {
    #{$property}: $light-value;
    @media (prefers-color-scheme: dark) {
      #{$property}: $dark-value;
    }
  }

  // Usage
  .component {
    @include theme-aware(background-color, white, black);
  }
  ```

- **Import Strategy**: Import SCSS files before Tailwind to ensure variables are available.
  ```scss
  // main.scss
  @import "variables";
  @import "mixins";
  @import "tailwindcss";
  ```

### 4. Runtime Theming
- **Dynamic CSS Injection**: For tenant themes, compile SCSS server-side and inject CSS dynamically.
- **CSS Custom Properties Override**: Use JavaScript to update `:root` variables for instant theme changes.
  ```js
  // Update theme
  document.documentElement.style.setProperty('--primary-color', tenant.primaryColor);
  ```

- **Avoid Full Recompilation**: Use CSS variables for lightweight changes; reserve SCSS compilation for complex theme updates.

### 5. Performance Considerations
- **Build-Time vs Runtime**: Pre-compile static themes; use runtime compilation only for dynamic tenant themes.
- **Caching**: Cache compiled CSS on server-side; use browser caching for static assets.
- **Bundle Splitting**: Separate theme CSS to allow lazy loading.

### 6. Compatibility and Migration
- **PostCSS Order**: Ensure SCSS is processed before Tailwind/DaisyUI in the build pipeline.
- **Variable Conflicts**: Prefix custom SCSS variables to avoid conflicts with Tailwind/DaisyUI.
- **Testing**: Test theme switching across components; ensure DaisyUI components respect custom themes.

## Common Pitfalls
- **Processing Order**: SCSS must be processed before PostCSS plugins.
- **Variable Scope**: CSS custom properties have global scope; use carefully.
- **Build Complexity**: Adding SCSS increases build complexity; monitor performance.

## Preferred Dependencies (Based on Project Requirements)
For the B2Connect tenant theming feature, the following dependencies are recommended based on performance, support, and integration needs:

### Backend (.NET/Wolverine)
- **SCSS Compilation**: `DartSass` (NuGet) - Official Dart Sass wrapper for .NET, better support and alignment with latest Sass features compared to SharpScss. Provides high-performance SCSS compilation for runtime theming.
- **Background Job Scheduling**: `Quartz` (NuGet) - Robust job scheduler for .NET, supports clustering, persistence, and complex scheduling. Preferred over Hangfire for enterprise-scale background tasks like SCSS compilation.
- **Caching**: `StackExchange.Redis` (NuGet) - High-performance Redis client for .NET, widely used and maintained by Stack Exchange. Essential for distributed caching and invalidation in multi-tenant environments.

### Frontend (Vue.js)
- **Monaco Editor**: `@guolao/vue-monaco-editor` (npm) - Vue.js wrapper for Monaco editor, provides SCSS syntax highlighting and editing capabilities for admin theme management. Popular and actively maintained with 8k+ weekly downloads.
- **SCSS Support**: `sass` (npm, dev) - Dart Sass for Node.js, ensures consistency with backend DartSass.

### Infrastructure
- **Redis Server**: For caching layer, supports high-throughput operations required for theme caching and invalidation.

These choices prioritize:
- **Performance**: DartSass and StackExchange.Redis for fast compilation and caching.
- **Reliability**: Quartz.NET for enterprise-grade job scheduling.
- **Consistency**: Using Dart Sass across frontend/backend for SCSS handling.
- **Maintenance**: Well-maintained, community-supported libraries.

## Resources
- [Tailwind CSS Theme Variables](https://tailwindcss.com/docs/theme)
- [DaisyUI Configuration](https://daisyui.com/docs/config/)
- [Vite SCSS Preprocessing](https://vitejs.dev/guide/features.html#css-pre-processors)
- [Sass Documentation](https://sass-lang.com/documentation/)
- [Quartz.NET Documentation](https://www.quartz-scheduler.net/)
- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
- [Dart Sass for .NET](https://www.nuget.org/packages/DartSass/)

## Last Updated
2026-01-01

## Sources
- Tailwind CSS Documentation (v4 theme variables)
- DaisyUI Config Guide
- Vite CSS Preprocessors Guide
- Quartz.NET NuGet and Docs
- StackExchange.Redis NuGet and Docs