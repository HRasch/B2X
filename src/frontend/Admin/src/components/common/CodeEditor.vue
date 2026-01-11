<template>
  <div class="code-editor-wrapper">
    <VueMonacoEditor
      v-model:value="internalValue"
      :language="language"
      :options="editorOptions"
      :theme="theme"
      @change="handleChange"
      class="monaco-editor"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { VueMonacoEditor } from '@guolao/vue-monaco-editor';

interface Props {
  modelValue: string;
  language?: string;
  theme?: string;
  readOnly?: boolean;
  height?: string;
}

interface Emits {
  (e: 'update:modelValue', value: string): void;
}

const props = withDefaults(defineProps<Props>(), {
  language: 'html',
  theme: 'vs-light',
  readOnly: false,
  height: '400px',
});

const emit = defineEmits<Emits>();

const internalValue = ref(props.modelValue);

const editorOptions = {
  automaticLayout: true,
  fontSize: 14,
  minimap: { enabled: false },
  scrollBeyondLastLine: false,
  wordWrap: 'on',
  readOnly: props.readOnly,
  tabSize: 2,
  insertSpaces: true,
  renderWhitespace: 'selection',
  bracketMatching: 'always',
  autoClosingBrackets: 'always',
  autoClosingQuotes: 'always',
  suggestOnTriggerCharacters: true,
  acceptSuggestionOnEnter: 'on',
  quickSuggestions: {
    other: true,
    comments: false,
    strings: true,
  },
};

watch(
  () => props.modelValue,
  newValue => {
    internalValue.value = newValue;
  }
);

const handleChange = (value: string) => {
  internalValue.value = value;
  emit('update:modelValue', value);
};
</script>

<style scoped>
.code-editor-wrapper {
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  overflow: hidden;
}

.monaco-editor {
  height: v-bind(height);
}
</style>
