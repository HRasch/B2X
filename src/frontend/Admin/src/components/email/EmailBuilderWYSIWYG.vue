<template>
  <div class="email-wysiwyg-editor" :class="{ 'preview-dark': previewMode === 'dark' }">
    <div id="gjs-editor" ref="editorRef"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import grapesjs from 'grapesjs';
import 'grapesjs/dist/css/grapes.min.css';
import grapesjsPresetNewsletter from 'grapesjs-preset-newsletter';
import DOMPurify from 'dompurify';
import type { Editor } from 'grapesjs';

// Constants for security and configuration
const EMAIL_SAFE_TAGS = [
  'html',
  'head',
  'body',
  'meta',
  'title',
  'style',
  'link',
  'div',
  'span',
  'p',
  'br',
  'strong',
  'b',
  'em',
  'i',
  'u',
  'h1',
  'h2',
  'h3',
  'h4',
  'h5',
  'h6',
  'table',
  'thead',
  'tbody',
  'tr',
  'td',
  'th',
  'a',
  'img',
  'ul',
  'ol',
  'li',
  'blockquote',
  'pre',
  'code',
];

// Debounce utility function
function debounce<T extends (...args: unknown[]) => unknown>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout;
  return (...args: Parameters<T>) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}

// HTML sanitization function
function sanitizeHtml(html: string): string {
  return DOMPurify.sanitize(html, {
    ALLOWED_TAGS: EMAIL_SAFE_TAGS,
    ALLOWED_ATTR: ['href', 'src', 'alt', 'title', 'class', 'id', 'style'],
    ALLOW_DATA_ATTR: false,
    FORBID_TAGS: ['script', 'object', 'embed', 'form', 'input', 'button'],
    FORBID_ATTR: ['onclick', 'onload', 'onerror', 'onmouseover'],
  });
}

// Extract body content from full HTML document
function extractBodyContent(html: string): string {
  const parser = new DOMParser();
  const doc = parser.parseFromString(html, 'text/html');
  const body = doc.querySelector('body');
  return body ? body.innerHTML : html;
}

interface Props {
  modelValue: string;
  height?: string;
  previewMode?: 'light' | 'dark';
}

interface Emits {
  (e: 'update:modelValue', value: string): void;
}

const props = withDefaults(defineProps<Props>(), {
  height: '600px',
  previewMode: 'light',
});

const emit = defineEmits<Emits>();

const { locale } = useI18n();

const editorRef = ref<HTMLElement | null>(null);
let editor: Editor | null = null;

// Debounced emit function to reduce update frequency
const debouncedEmit = debounce((value: string) => {
  emit('update:modelValue', value);
}, 300);

onMounted(() => {
  try {
    if (!editorRef.value) return;

    // Initialize GrapesJS with i18n support
    editor = grapesjs.init({
      container: editorRef.value,
      fromElement: false,
      height: props.height,
      width: 'auto',
      storageManager: false,
      plugins: [grapesjsPresetNewsletter],
      pluginsOpts: {
        [grapesjsPresetNewsletter as string]: {
          // Newsletter preset options
        },
      },
      canvas: {
        styles: [],
        scripts: [],
      },
      assetManager: {
        upload: false,
        autoAdd: true,
      },
      i18n: {
        locale: locale.value,
        detectLocale: false,
        localeFallback: 'en',
        messages: {},
      },
      panels: {
        defaults: [
          {
            id: 'basic-actions',
            el: '.panel__basic-actions',
            buttons: [
              {
                id: 'visibility',
                active: true,
                className: 'btn-toggle-borders',
                label: '<i class="fa fa-clone"></i>',
                command: 'sw-visibility',
              },
              {
                id: 'export',
                className: 'btn-open-export',
                label: '<i class="fa fa-code"></i>',
                command: 'export-template',
              },
              {
                id: 'dark-preview',
                className: 'btn-dark-preview',
                label: '🌙',
                command: 'toggle-dark-preview',
                togglable: true,
                attributes: { title: 'Toggle dark mode preview' },
              },
            ],
          },
          {
            id: 'panel-devices',
            el: '.panel__devices',
            buttons: [
              {
                id: 'device-desktop',
                label: '<i class="fa fa-desktop"></i>',
                command: 'set-device-desktop',
                active: true,
                togglable: false,
              },
              {
                id: 'device-tablet',
                label: '<i class="fa fa-tablet"></i>',
                command: 'set-device-tablet',
                togglable: false,
              },
              {
                id: 'device-mobile',
                label: '<i class="fa fa-mobile"></i>',
                command: 'set-device-mobile',
                togglable: false,
              },
            ],
          },
        ],
      },
      deviceManager: {
        devices: [
          {
            name: 'Desktop',
            width: '',
          },
          {
            name: 'Tablet',
            width: '768px',
            widthMedia: '992px',
          },
          {
            name: 'Mobile',
            width: '320px',
            widthMedia: '480px',
          },
        ],
      },
    });

    // Load initial content if provided - handle both full HTML and body content
    if (props.modelValue) {
      const bodyContent = extractBodyContent(props.modelValue);
      editor.setComponents(bodyContent);
    }

    // Emit changes when content updates with sanitization and debouncing
    editor.on('component:update', () => {
      const html = editor.getHtml();

      // Sanitize HTML for email safety
      const sanitizedHtml = sanitizeHtml(html);

      // Emit just the body content - parent handles full document formatting
      debouncedEmit(sanitizedHtml);
    });

    // Add dark mode preview toggle command
    editor.Commands.add('toggle-dark-preview', {
      run: editor => {
        const wrapper = editor.getWrapper();
        const isDark = wrapper.getClasses().includes('dark-mode-preview');
        if (isDark) {
          wrapper.removeClass('dark-mode-preview');
        } else {
          wrapper.addClass('dark-mode-preview');
        }
      },
    });

    // Custom commands
    editor.Commands.add('set-device-desktop', {
      run: editor => editor.setDevice('Desktop'),
    });
    editor.Commands.add('set-device-tablet', {
      run: editor => editor.setDevice('Tablet'),
    });
    editor.Commands.add('set-device-mobile', {
      run: editor => editor.setDevice('Mobile'),
    });
    editor.Commands.add('export-template', {
      run: editor => {
        const html = editor.getHtml();
        const css = editor.getCss();
        console.log('HTML:', html);
        console.log('CSS:', css);
      },
    });
  } catch (error) {
    console.error('Failed to initialize GrapesJS editor:', error);
    // Graceful fallback - could emit an error event or show a message
  }
});

onBeforeUnmount(() => {
  if (editor) {
    editor.destroy();
  }
});

// Watch for modelValue changes and update editor content
// Extract body content to compare with editor's current HTML
watch(
  () => props.modelValue,
  newValue => {
    if (editor && newValue) {
      const bodyContent = extractBodyContent(newValue);
      const currentHtml = editor.getHtml();
      if (bodyContent !== currentHtml) {
        editor.setComponents(bodyContent);
      }
    }
  }
);

// Watch for locale changes and update GrapesJS i18n
watch(
  () => locale.value,
  newLocale => {
    if (editor && editor.I18n) {
      editor.I18n.setLocale(newLocale);
    }
  }
);
</script>

<style scoped>
.email-wysiwyg-editor {
  width: 100%;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  overflow: hidden;
}

/* GrapesJS custom styles */
:deep(.gjs-cv-canvas) {
  background-color: #f9fafb;
}

:deep(.gjs-pn-panel) {
  background-color: #ffffff;
  border-right: 1px solid #e5e7eb;
}

:deep(.gjs-block) {
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 0.25rem;
  margin: 0.5rem;
  padding: 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
  color: #1f2937;
}

:deep(.gjs-block__media) {
  opacity: 0.6;
}

:deep(.gjs-block-label) {
  color: #374151;
  font-weight: 500;
}

:deep(.gjs-block:hover) {
  border-color: #3b82f6;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
}

:deep(.gjs-toolbar) {
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 0.25rem;
}

:deep(.gjs-pn-btn) {
  color: #374151;
}

:deep(.gjs-category-title) {
  color: #1f2937;
  font-weight: 600;
  background-color: #f3f4f6;
}

:deep(.gjs-block-category) {
  border-bottom: 1px solid #e5e7eb;
}

:deep(.gjs-field),
:deep(.gjs-sm-property__field),
:deep(.gjs-select) {
  background-color: #ffffff;
  border-color: #d1d5db;
  color: #1f2937;
}

:deep(.gjs-sm-sector-title) {
  color: #1f2937;
  background-color: #f9fafb;
}

:deep(.gjs-sm-label) {
  color: #4b5563;
}

/* Email Preview Dark Mode (independent from app theme) */
.preview-dark :deep(.gjs-cv-canvas) {
  background-color: #1f2937;
}

.preview-dark :deep(.gjs-pn-panel) {
  background-color: #111827;
  border-right: 1px solid #374151;
  color: #f9fafb;
}

.preview-dark :deep(.gjs-block) {
  background-color: #1f2937;
  border: 1px solid #4b5563;
  color: #f9fafb;
}

.preview-dark :deep(.gjs-block__media) {
  opacity: 0.8;
  filter: brightness(1.2);
}

.preview-dark :deep(.gjs-block-label) {
  color: #e5e7eb;
  font-weight: 500;
}

.preview-dark :deep(.gjs-category-title) {
  color: #f9fafb;
  background-color: #374151;
}

.preview-dark :deep(.gjs-block-category) {
  border-bottom: 1px solid #4b5563;
}

.preview-dark :deep(.gjs-block:hover) {
  border-color: #3b82f6;
  background-color: #374151;
}

.preview-dark :deep(.gjs-toolbar) {
  background-color: #1f2937;
  border: 1px solid #374151;
}

.preview-dark :deep(.gjs-pn-btn) {
  color: #f9fafb;
}

.preview-dark :deep(.gjs-field),
.preview-dark :deep(.gjs-sm-property__field),
.preview-dark :deep(.gjs-select) {
  background-color: #374151;
  border-color: #4b5563;
  color: #f9fafb;
}

.preview-dark :deep(.gjs-field:focus),
.preview-dark :deep(.gjs-sm-property__field:focus) {
  border-color: #3b82f6;
  background-color: #1f2937;
}

.preview-dark :deep(.gjs-sm-label) {
  color: #9ca3af;
}

.preview-dark :deep(.gjs-sm-sector-title) {
  color: #f9fafb;
  background-color: #1f2937;
}

.preview-dark :deep(.gjs-sm-sector) {
  background-color: #111827;
  border-color: #374151;
}

.preview-dark :deep(.gjs-sm-property) {
  background-color: #1f2937;
  color: #f9fafb;
}

.preview-dark :deep(.gjs-clm-tags) {
  background-color: #1f2937;
  color: #f9fafb;
}

.preview-dark :deep(.gjs-layer) {
  background-color: #1f2937;
  border-color: #374151;
  color: #f9fafb;
}

/* Dark mode email preview in canvas */
:deep(.dark-mode-preview) {
  background-color: #1f2937 !important;
}

:deep(.dark-mode-preview *) {
  color: #f9fafb !important;
}

:deep(.dark-mode-preview a) {
  color: #60a5fa !important;
}

:deep(.dark-mode-preview h1),
:deep(.dark-mode-preview h2),
:deep(.dark-mode-preview h3),
:deep(.dark-mode-preview h4),
:deep(.dark-mode-preview h5),
:deep(.dark-mode-preview h6) {
  color: #f9fafb !important;
}

:deep(.dark-mode-preview [data-gjs-type='wrapper']) {
  background-color: #1f2937 !important;
}

:deep(.dark-mode-preview table),
:deep(.dark-mode-preview td),
:deep(.dark-mode-preview th) {
  border-color: #374151 !important;
}
</style>
