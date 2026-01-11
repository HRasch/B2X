import { computed } from 'vue';
import { sanitizeHtml } from '@/utils/sanitize';

export const useSafeHtml = (html: string) => {
  return computed(() => sanitizeHtml(html));
};
