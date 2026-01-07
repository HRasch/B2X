# GrapesJS Email Builder Integration

**DocID**: `KB-028`  
**Last Updated**: 5. Januar 2026  
**Owner**: @Frontend  
**Status**: Active

---

## Overview

GrapesJS is a free, open-source web builder framework that provides a drag-and-drop interface for creating HTML templates. In B2X, it's integrated as the WYSIWYG email template editor in the Admin frontend, allowing users to visually design email templates without coding.

## Key Features

- **Drag-and-Drop Interface**: Visual component-based editing
- **Newsletter Preset**: Specialized for email template creation
- **Responsive Design**: Built-in device preview (Desktop, Tablet, Mobile)
- **Dark Mode Preview**: Email preview in dark mode for better UX
- **HTML/CSS Export**: Generates clean, production-ready code
- **Vue.js Integration**: Seamless v-model binding with Vue components

## Installation

```bash
npm install grapesjs grapesjs-preset-newsletter
```

## Basic Integration

```vue
<template>
  <div id="gjs-editor" ref="editorRef"></div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import grapesjs from 'grapesjs';
import 'grapesjs/dist/css/grapes.min.css';
import grapesjsPresetNewsletter from 'grapesjs-preset-newsletter';

const editorRef = ref(null);
let editor = null;

onMounted(() => {
  editor = grapesjs.init({
    container: editorRef.value,
    plugins: [grapesjsPresetNewsletter],
    storageManager: false, // Disable auto-save
    height: '600px'
  });
});
</script>
```

## Configuration Options

### Core Settings

```javascript
const editor = grapesjs.init({
  container: '#gjs-editor',
  fromElement: false, // Don't load from existing HTML
  height: '600px',
  width: 'auto',
  storageManager: false, // No local storage
  plugins: [grapesjsPresetNewsletter],
  pluginsOpts: {
    [grapesjsPresetNewsletter]: {
      // Newsletter-specific options
    }
  }
});
```

### Device Manager

```javascript
deviceManager: {
  devices: [
    { name: 'Desktop', width: '' },
    { name: 'Tablet', width: '768px', widthMedia: '992px' },
    { name: 'Mobile', width: '320px', widthMedia: '480px' }
  ]
}
```

### Asset Manager

```javascript
assetManager: {
  upload: false, // Disable upload for security
  autoAdd: true
}
```

## Vue.js Integration Patterns

### v-model Binding

```vue
<script setup>
const props = defineProps(['modelValue']);
const emit = defineEmits(['update:modelValue']);

onMounted(() => {
  editor = grapesjs.init({ /* config */ });
  
  // Load initial content
  if (props.modelValue) {
    editor.setComponents(props.modelValue);
  }
  
  // Emit changes
  editor.on('component:update', () => {
    const html = editor.getHtml();
    const css = editor.getCss();
    const fullHtml = `<!DOCTYPE html><html><head><style>${css}</style></head><body>${html}</body></html>`;
    emit('update:modelValue', fullHtml);
  });
});
</script>
```

### Reactive Updates

```vue
import { watch } from 'vue';

watch(() => props.modelValue, (newValue) => {
  if (editor && newValue && newValue !== editor.getHtml()) {
    editor.setComponents(newValue);
  }
});
```

## Custom Commands

### Device Switching

```javascript
editor.Commands.add('set-device-desktop', {
  run: editor => editor.setDevice('Desktop')
});
editor.Commands.add('set-device-tablet', {
  run: editor => editor.setDevice('Tablet')
});
editor.Commands.add('set-device-mobile', {
  run: editor => editor.setDevice('Mobile')
});
```

### Dark Mode Toggle

```javascript
editor.Commands.add('toggle-dark-preview', {
  run: editor => {
    const wrapper = editor.getWrapper();
    const isDark = wrapper.getClasses().includes('dark-mode-preview');
    wrapper[isDark ? 'removeClass' : 'addClass']('dark-mode-preview');
  }
});
```

## Styling & Theming

### Light Mode (Default)

```css
.gjs-cv-canvas { background-color: #f9fafb; }
.gjs-pn-panel { background-color: #ffffff; border-right: 1px solid #e5e7eb; }
.gjs-block { background-color: #ffffff; border: 1px solid #e5e7eb; }
```

### Dark Mode Preview

```css
.dark-mode-preview {
  background-color: #1f2937 !important;
}
.dark-mode-preview * {
  color: #f9fafb !important;
}
.dark-mode-preview a {
  color: #60a5fa !important;
}
```

## Email-Specific Features

### Newsletter Preset

The `grapesjs-preset-newsletter` plugin provides:

- Email-optimized components (text, image, button, divider)
- Table-based layouts for email compatibility
- Inline CSS generation
- Email-safe styling

### HTML Output Structure

```html
<!DOCTYPE html>
<html>
  <head>
    <meta name="color-scheme" content="light dark">
    <meta name="supported-color-schemes" content="light dark">
    <style>
      /* Generated CSS */
      .container { max-width: 600px; margin: 0 auto; }
      /* Dark mode styles */
      @media (prefers-color-scheme: dark) {
        body { background-color: #1f2937 !important; color: #f9fafb !important; }
      }
    </style>
  </head>
  <body>
    <!-- Generated HTML -->
    <div class="container">
      <h1>Welcome</h1>
      <p>This is your email content.</p>
    </div>
  </body>
</html>
```

## i18n Support

GrapesJS includes internationalization support:

```javascript
import grapesjs from 'grapesjs';

// Check if i18n is available
console.log(grapesjs.i18n ? 'i18n exists' : 'no i18n');

// Available locale methods
console.log(Object.keys(grapesjs).filter(k => k.includes('i18n') || k.includes('locale')));
```

Available i18n methods:
- `grapesjs.i18n` - Main i18n object
- Locale management functions for UI translation

## Performance Considerations

### Memory Management

```vue
<script setup>
import { onBeforeUnmount } from 'vue';

onBeforeUnmount(() => {
  if (editor) {
    editor.destroy();
  }
});
</script>
```

### Canvas Optimization

- Disable unused panels to reduce DOM complexity
- Use `storageManager: false` to prevent unnecessary saves
- Limit asset manager features for email templates

## Security Best Practices

- **No Script Execution**: Disable script tags in templates
- **Sanitized Output**: Always sanitize generated HTML before storage
- **Asset Restrictions**: Disable uploads, use pre-approved assets only
- **CSP Compliance**: Ensure generated HTML follows Content Security Policy

## Troubleshooting

### Common Issues

1. **Editor not initializing**: Check container element exists in DOM
2. **Styles not applying**: Ensure CSS is properly scoped with `:deep()`
3. **Vue reactivity issues**: Use proper watchers for prop changes
4. **Memory leaks**: Always destroy editor on component unmount

### Debug Commands

```javascript
// Export current template
editor.Commands.run('export-template');

// Get current HTML/CSS
const html = editor.getHtml();
const css = editor.getCss();
console.log({ html, css });
```

## Integration with B2X

In the Admin frontend, GrapesJS is used in:

- `EmailTemplateEditor.vue` - Main template editor component
- `EmailBuilderWYSIWYG.vue` - GrapesJS wrapper component
- Supports multi-language templates via Vue i18n
- Integrates with dark mode theming
- Exports email-compatible HTML with dark mode support

## Resources

- [GrapesJS Official Documentation](https://grapesjs.com/docs/)
- [Newsletter Preset](https://github.com/GrapesJS/presets/tree/master/presets/newsletter)
- [Vue.js Integration Examples](https://grapesjs.com/docs/frameworks/vue.html)

---

**Related Documents**:
- [KB-023] Email Template Best Practices
- [KB-027] Email Dark Mode Best Practices
- [GL-012] Frontend Quality Standards

**Last Reviewed**: 5. Januar 2026</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/knowledgebase/tools-and-tech/grapesjs-email-builder.md