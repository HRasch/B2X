/**
 * CMS Store (Pinia)
 * @todo Refactor error handling from catch(err: any) to typed unknown pattern
 * @see https://typescript-eslint.io/rules/no-explicit-any/
 */
/* eslint-disable @typescript-eslint/no-explicit-any -- Legacy error handling, refactor in KB-STORE-TYPING sprint */

import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Page, Template, MediaItem } from '@/types/cms';
import { cmsApi } from '@/services/api/cms';

export const useCmsStore = defineStore('cms', () => {
  const pages = ref<Page[]>([]);
  const currentPage = ref<Page | null>(null);
  const templates = ref<Template[]>([]);
  const mediaItems = ref<MediaItem[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchPages(filters?: any) {
    loading.value = true;
    error.value = null;
    try {
      const response = await cmsApi.getPages(filters);
      pages.value = response.items;
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchPage(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentPage.value = await cmsApi.getPage(id);
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function savePage(page: Page | Omit<Page, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true;
    error.value = null;
    try {
      if ('id' in page) {
        const updated = await cmsApi.updatePage(page.id, page);
        currentPage.value = updated;
        const index = pages.value.findIndex((p: any) => p.id === page.id);
        if (index !== -1) pages.value[index] = updated;
        return updated;
      } else {
        const created = await cmsApi.createPage(page as any);
        pages.value.push(created);
        currentPage.value = created;
        return created;
      }
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function publishPage(pageId: string) {
    loading.value = true;
    try {
      const updated = await cmsApi.publishPage(pageId);
      if (currentPage.value?.id === pageId) currentPage.value = updated;
      const index = pages.value.findIndex((p: any) => p.id === pageId);
      if (index !== -1) pages.value[index] = updated;
      return updated;
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deletePage(pageId: string) {
    loading.value = true;
    try {
      await cmsApi.deletePage(pageId);
      pages.value = pages.value.filter((p: any) => p.id !== pageId);
      if (currentPage.value?.id === pageId) currentPage.value = null;
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchTemplates() {
    loading.value = true;
    error.value = null;
    try {
      templates.value = await cmsApi.getTemplates();
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function fetchMedia() {
    loading.value = true;
    error.value = null;
    try {
      const response = await cmsApi.getMedia();
      mediaItems.value = response.items;
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function uploadMedia(file: File, altText?: string) {
    loading.value = true;
    error.value = null;
    try {
      const mediaItem = await cmsApi.uploadMedia(file, altText);
      mediaItems.value.push(mediaItem);
      return mediaItem;
    } catch (err: any) {
      error.value = err.message;
      throw err;
    } finally {
      loading.value = false;
    }
  }

  return {
    pages,
    currentPage,
    templates,
    mediaItems,
    loading,
    error,
    fetchPages,
    fetchPage,
    savePage,
    publishPage,
    deletePage,
    fetchTemplates,
    fetchMedia,
    uploadMedia,
  };
});
