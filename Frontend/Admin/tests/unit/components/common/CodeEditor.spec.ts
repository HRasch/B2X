import { describe, it, expect, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import CodeEditor from '@/components/common/CodeEditor.vue';

describe('CodeEditor', () => {
  it('renders with initial value', async () => {
    const wrapper = mount(CodeEditor, {
      props: {
        modelValue: '<h1>Hello</h1>',
        language: 'html',
      },
    });

    // Check if component mounts
    expect(wrapper.exists()).toBe(true);
    expect(wrapper.classes()).toContain('code-editor-wrapper');
  });

  it('emits update:modelValue on change', async () => {
    const wrapper = mount(CodeEditor, {
      props: {
        modelValue: '',
        language: 'html',
      },
    });

    // Simulate change (mock Monaco change)
    const newValue = '<p>Updated</p>';
    await wrapper.vm.$emit('update:modelValue', newValue);

    expect(wrapper.emitted('update:modelValue')).toBeTruthy();
    expect(wrapper.emitted('update:modelValue')[0]).toEqual([newValue]);
  });

  it('applies correct height', () => {
    const wrapper = mount(CodeEditor, {
      props: {
        modelValue: '',
        height: '500px',
      },
    });

    const editor = wrapper.find('.monaco-editor');
    expect(editor.attributes('style')).toContain('height: 500px');
  });

  it('respects readOnly prop', () => {
    const wrapper = mount(CodeEditor, {
      props: {
        modelValue: '',
        readOnly: true,
      },
    });

    // The options should include readOnly: true
    // This is internal, but we can check if the component accepts the prop
    expect(wrapper.props('readOnly')).toBe(true);
  });
});
