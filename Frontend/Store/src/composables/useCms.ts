import { ref, onMounted } from 'vue';
import { cmsApi } from '@/services/api/cms';
import type { PageDefinition } from '@/types/cms';

interface UseNotification {
  error: (message: string) => void;
}

export function useCms(notificationService?: UseNotification) {
  const pageDefinition = ref<PageDefinition | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const getRegion = (regionName: string) => {
    if (!pageDefinition.value) return null;
    return pageDefinition.value.regions.find(r => r.name === regionName);
  };

  const fetchPageDefinition = async (pagePath: string) => {
    loading.value = true;
    error.value = null;

    try {
      pageDefinition.value = await cmsApi.getPageDefinition(pagePath);

      // Update document title and meta tags
      updatePageMeta();
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load page';
      if (notificationService?.error) {
        notificationService.error(error.value);
      }
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const updatePageMeta = () => {
    if (!pageDefinition.value) return;

    // Set page title
    document.title = pageDefinition.value.pageTitle || 'Page';

    // Set meta description
    const metaDescription = document.querySelector('meta[name="description"]');
    if (metaDescription && pageDefinition.value.pageDescription) {
      metaDescription.setAttribute('content', pageDefinition.value.pageDescription);
    }

    // Set meta keywords
    const metaKeywords = document.querySelector('meta[name="keywords"]');
    if (metaKeywords && pageDefinition.value.metaKeywords) {
      metaKeywords.setAttribute('content', pageDefinition.value.metaKeywords);
    }

    // Update Open Graph tags
    updateOpenGraphTags();
  };

  const updateOpenGraphTags = () => {
    if (!pageDefinition.value) return;

    const ogTitle = document.querySelector('meta[property="og:title"]');
    if (ogTitle) {
      ogTitle.setAttribute('content', pageDefinition.value.pageTitle);
    }

    const ogDescription = document.querySelector('meta[property="og:description"]');
    if (ogDescription && pageDefinition.value.pageDescription) {
      ogDescription.setAttribute('content', pageDefinition.value.pageDescription);
    }
  };

  return {
    pageDefinition,
    loading,
    error,
    getRegion,
    fetchPageDefinition,
  };
}
