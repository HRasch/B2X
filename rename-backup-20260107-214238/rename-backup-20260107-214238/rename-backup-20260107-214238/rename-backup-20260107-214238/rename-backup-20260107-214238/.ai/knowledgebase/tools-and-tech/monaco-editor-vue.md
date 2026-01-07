# Monaco Editor - Vue.js Integration

**DocID**: `KB-026`  
**Status**: Active  
**Created**: 2026-01-05  
**Owner**: GitHub Copilot  
**Last Updated**: 2026-01-05

---

## Overview

Monaco Editor is the code editor that powers VS Code. For Vue.js integration, use `@guolao/vue-monaco-editor` - a Vue 3 wrapper that provides reactive Monaco integration.

**Official Resources:**
- Monaco Editor: https://microsoft.github.io/monaco-editor/
- Vue Monaco Editor: https://github.com/imguolao/monaco-vue

---

## Installation

```bash
npm install @guolao/vue-monaco-editor
```

**Current Stable Version** (as of Jan 2026): `1.6.0`

---

## Global Setup (Recommended)

Register the plugin globally in `main.ts`:

```typescript
import { createApp } from 'vue'
import { install as VueMonacoEditorPlugin } from '@guolao/vue-monaco-editor'
import App from './App.vue'

const app = createApp(App)
app.use(VueMonacoEditorPlugin)
app.mount('#app')
```

**Why Global Registration?**
- Monaco requires proper loader initialization
- Prevents multiple Monaco instances
- Ensures proper cleanup and resource management
- Required for the plugin to work correctly

---

## Component Usage

### Basic Component Wrapper

```vue
<template>
  <div class="code-editor-wrapper">
    <VueMonacoEditor
      v-model:value="code"
      :language="language"
      :options="editorOptions"
      :theme="theme"
      @change="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { VueMonacoEditor } from '@guolao/vue-monaco-editor'

const code = ref('<h1>Hello World</h1>')
const language = ref('html')
const theme = ref('vs-light') // 'vs-dark' for dark mode

const editorOptions = {
  automaticLayout: true,
  fontSize: 14,
  minimap: { enabled: false },
  scrollBeyondLastLine: false,
  wordWrap: 'on',
  readOnly: false,
  tabSize: 2,
  insertSpaces: true
}

const handleChange = (value: string) => {
  console.log('Editor changed:', value)
}
</script>
```

### Reusable Component Pattern

```vue
<!-- CodeEditor.vue -->
<template>
  <div class="code-editor-wrapper">
    <VueMonacoEditor
      v-model:value="internalValue"
      :language="language"
      :options="editorOptions"
      :theme="theme"
      @change="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { VueMonacoEditor } from '@guolao/vue-monaco-editor'

interface Props {
  modelValue: string
  language?: string
  theme?: string
  readOnly?: boolean
  height?: string
}

const props = withDefaults(defineProps<Props>(), {
  language: 'html',
  theme: 'vs-light',
  readOnly: false,
  height: '400px'
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const internalValue = ref(props.modelValue)

const editorOptions = {
  automaticLayout: true,
  fontSize: 14,
  minimap: { enabled: false },
  scrollBeyondLastLine: false,
  wordWrap: 'on',
  readOnly: props.readOnly,
  tabSize: 2,
  insertSpaces: true
}

watch(() => props.modelValue, (newValue) => {
  internalValue.value = newValue
})

const handleChange = (value: string) => {
  internalValue.value = value
  emit('update:modelValue', value)
}
</script>

<style scoped>
.code-editor-wrapper {
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  overflow: hidden;
}
</style>
```

### Usage in Parent Component

```vue
<template>
  <CodeEditor
    v-model="htmlContent"
    language="html"
    :height="'500px'"
    :read-only="false"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import CodeEditor from '@/components/common/CodeEditor.vue'

const htmlContent = ref('<p>Edit me</p>')
</script>
```

---

## Supported Languages

Monaco supports many languages out of the box:

- `javascript`, `typescript`
- `html`, `css`, `scss`, `less`
- `json`, `xml`, `yaml`
- `python`, `java`, `csharp`
- `sql`, `markdown`
- `dockerfile`, `shell`

For custom languages (e.g., Liquid templates), you need to register them separately.

---

## Localization (UI Language)

Monaco Editor supports localization for its UI elements (menus, dialogs, tooltips). Configure this during app initialization.

### Supported Locales

Monaco supports: `cs`, `de`, `es`, `fr`, `it`, `ja`, `ko`, `pl`, `pt-br`, `ru`, `tr`, `zh-cn`, `zh-tw`

### Configuration in main.ts

```typescript
import { createApp } from 'vue'
import { install as VueMonacoEditorPlugin } from '@guolao/vue-monaco-editor'

// Configure Monaco locale based on current i18n locale
const configureMonacoLocale = async () => {
  const currentLocale = localStorage.getItem('locale') || 
                       navigator.language.split('-')[0] || 'en';
  
  // Monaco supported locales mapping
  const monacoLocales: Record<string, string> = {
    'de': 'de',
    'fr': 'fr', 
    'es': 'es',
    'it': 'it',
    'pt': 'pt-br', // Portuguese Brazil
    'pl': 'pl'
    // nl not supported by Monaco, falls back to English
  };
  
  const monacoLocale = monacoLocales[currentLocale];
  if (monacoLocale) {
    try {
      // Dynamically import the locale file
      await import(`monaco-editor/esm/vs/nls.${monacoLocale}.js`);
      console.log(`Monaco locale loaded: ${monacoLocale}`);
    } catch (error) {
      console.warn(`Failed to load Monaco locale: ${monacoLocale}`, error);
    }
  }
};

// Initialize app with localized Monaco
const initApp = async () => {
  const app = createApp(App);
  
  // ... other app setup ...
  
  // Configure Monaco locale before installing plugin
  await configureMonacoLocale();
  app.use(VueMonacoEditorPlugin);
  
  app.mount('#app');
};

initApp();
```

### B2Connect Implementation

**Location**: `frontend/Admin/src/main.ts`

**Supported Languages**: German (de), French (fr), Spanish (es), Italian (it), Portuguese (pt → pt-br), Polish (pl)

**Fallback**: English for unsupported locales (nl, en)

---

## Editor Options

### Common Options

```typescript
{
  // Layout
  automaticLayout: true,        // Auto-resize with container
  fontSize: 14,                 // Font size in pixels
  lineHeight: 20,               // Line height
  
  // Features
  minimap: { enabled: false },  // Show/hide minimap
  scrollBeyondLastLine: false,  // Allow scrolling past end
  wordWrap: 'on',              // 'on' | 'off' | 'wordWrapColumn' | 'bounded'
  
  // Editing
  readOnly: false,              // Read-only mode
  tabSize: 2,                   // Tab size
  insertSpaces: true,           // Use spaces instead of tabs
  
  // Suggestions
  quickSuggestions: {
    other: true,
    comments: false,
    strings: true
  },
  suggestOnTriggerCharacters: true,
  acceptSuggestionOnEnter: 'on', // 'on' | 'off' | 'smart'
  
  // Rendering
  renderWhitespace: 'selection', // 'none' | 'boundary' | 'selection' | 'all'
  bracketMatching: 'always',     // 'always' | 'near' | 'never'
  autoClosingBrackets: 'always', // 'always' | 'languageDefined' | 'beforeWhitespace' | 'never'
  autoClosingQuotes: 'always'
}
```

### Full Options Reference

https://microsoft.github.io/monaco-editor/api/interfaces/monaco.editor.IStandaloneEditorConstructionOptions.html

---

## Themes

### Built-in Themes

- `vs` - Visual Studio Light (default)
- `vs-dark` - Visual Studio Dark
- `hc-black` - High Contrast Black
- `hc-light` - High Contrast Light

### Dynamic Theme Switching

```vue
<script setup lang="ts">
import { ref, watch } from 'vue'

const isDark = ref(false)
const theme = ref('vs-light')

watch(isDark, (dark) => {
  theme.value = dark ? 'vs-dark' : 'vs-light'
})
</script>
```

---

## Performance Considerations

### Bundle Size

Monaco Editor is ~7MB minified. Optimize with:

1. **Lazy Loading**: Load editor only when needed
2. **CDN Option**: Use CDN instead of bundling
3. **Code Splitting**: Split into separate chunk

### Lazy Loading Example

```vue
<script setup lang="ts">
import { defineAsyncComponent } from 'vue'

const CodeEditor = defineAsyncComponent(() =>
  import('@/components/common/CodeEditor.vue')
)
</script>
```

### Using CDN (Alternative)

```typescript
// vite.config.ts
export default {
  build: {
    rollupOptions: {
      external: ['monaco-editor']
    }
  }
}
```

Then load from CDN in `index.html`.

---

## Testing Considerations

### Unit Testing Challenges

Monaco requires browser environment with DOM APIs. Standard Node-based tests (Vitest) will fail.

**Solutions:**

1. **Mock the Component**: Mock Monaco in tests
2. **E2E Tests**: Use Playwright/Cypress for integration tests
3. **Happy DOM**: Use `happy-dom` environment (limited support)

### Mock Example

```typescript
// __mocks__/@guolao/vue-monaco-editor.ts
export const VueMonacoEditor = {
  name: 'VueMonacoEditor',
  template: '<div class="monaco-mock"></div>',
  props: ['value', 'language', 'options', 'theme'],
  emits: ['change']
}
```

### Test with Mock

```typescript
import { mount } from '@vue/test-utils'
import CodeEditor from '@/components/common/CodeEditor.vue'

vi.mock('@guolao/vue-monaco-editor', () => ({
  VueMonacoEditor: {
    name: 'VueMonacoEditor',
    template: '<div class="monaco-mock"></div>',
    props: ['value', 'language', 'options', 'theme']
  }
}))

test('renders editor wrapper', () => {
  const wrapper = mount(CodeEditor, {
    props: { modelValue: '<h1>Test</h1>' }
  })
  expect(wrapper.find('.code-editor-wrapper').exists()).toBe(true)
})
```

---

## Common Issues & Solutions

### Issue 1: "loader.__getMonacoInstance is not a function"

**Cause**: Plugin not registered globally

**Solution**: Add to `main.ts`:
```typescript
import { install as VueMonacoEditorPlugin } from '@guolao/vue-monaco-editor'
app.use(VueMonacoEditorPlugin)
```

### Issue 2: Editor Not Rendering

**Cause**: Container has no height

**Solution**: Set explicit height:
```vue
<style>
.monaco-editor {
  height: 400px; /* or use v-bind */
}
</style>
```

### Issue 3: TypeScript Errors

**Cause**: Missing type definitions

**Solution**: Types are included in `@guolao/vue-monaco-editor@1.6.0+`

### Issue 4: v-model Not Updating

**Cause**: Using `v-model` instead of `v-model:value`

**Solution**: Use `v-model:value` or implement custom v-model wrapper:
```vue
<VueMonacoEditor v-model:value="code" />
```

---

## Custom Language Registration

For custom syntax (e.g., Liquid templates):

```typescript
import * as monaco from 'monaco-editor'

monaco.languages.register({ id: 'liquid' })

monaco.languages.setMonarchTokensProvider('liquid', {
  tokenizer: {
    root: [
      [/\{\{.*?\}\}/, 'liquid-variable'],
      [/\{%.*?%\}/, 'liquid-tag']
    ]
  }
})

monaco.editor.defineTheme('liquid-theme', {
  base: 'vs',
  inherit: true,
  rules: [
    { token: 'liquid-variable', foreground: '0000FF' },
    { token: 'liquid-tag', foreground: 'FF0000' }
  ],
  colors: {}
})
```

**Note**: Custom language registration requires additional setup and is beyond basic usage.

---

## Accessibility

Monaco Editor has built-in accessibility features:

- Keyboard navigation
- Screen reader support
- High contrast themes
- ARIA labels

Ensure:
- Provide proper labels for assistive tech
- Test with keyboard-only navigation
- Use high-contrast themes for visually impaired users

---

## Migration from Other Editors

### From Textarea

Before:
```vue
<textarea v-model="code" rows="20"></textarea>
```

After:
```vue
<CodeEditor v-model="code" :height="'400px'" />
```

### From CodeMirror

Monaco provides similar features with better TypeScript/IntelliSense support. Main differences:
- Monaco: VS Code-like experience
- CodeMirror: Lighter, more customizable

---

## B2Connect Implementation

**Location**: `frontend/Admin/src/components/common/CodeEditor.vue`

**Used In**:
- Email template editing (HTML/Liquid)
- CMS template overrides (future)
- JSON configuration editing (future)

**Integration**: Registered globally in `main.ts`

---

## Resources

### Official Documentation
- Monaco Editor API: https://microsoft.github.io/monaco-editor/api/index.html
- Monaco Playground: https://microsoft.github.io/monaco-editor/playground.html
- Vue Monaco Editor GitHub: https://github.com/imguolao/monaco-vue

### Package Registry
- NPM: https://www.npmjs.com/package/@guolao/vue-monaco-editor

### Related
- VS Code API (for advanced features): https://code.visualstudio.com/api
- Language Server Protocol: https://microsoft.github.io/language-server-protocol/

---

**Maintained by**: GitHub Copilot  
**Next Review**: 2026-04-05 (Quarterly)  
**Version Tracking**: Monitor `@guolao/vue-monaco-editor` for updates
