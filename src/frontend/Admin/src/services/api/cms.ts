/**
 * CMS API Service
 */

import { apiClient } from '../client';
import type { Page, Template, MediaItem, PageFilters, PageVersion } from '@/types/cms';
import type { PaginatedResponse, PaginationParams } from '@/types/api';

export const cmsApi = {
  // Pages
  getPages(filters?: PageFilters, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Page>>('/api/layout/pages', {
      params: { ...filters, ...pagination },
    });
  },

  getPage(id: string) {
    return apiClient.get<Page>(`/api/layout/pages/${id}`);
  },

  createPage(data: Omit<Page, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<Page>('/api/layout/pages', data);
  },

  updatePage(id: string, data: Partial<Page>) {
    return apiClient.put<Page>(`/api/layout/pages/${id}`, data);
  },

  deletePage(id: string) {
    return apiClient.delete<void>(`/api/layout/pages/${id}`);
  },

  publishPage(id: string) {
    return apiClient.post<Page>(`/api/layout/pages/${id}/publish`, {});
  },

  unpublishPage(id: string) {
    return apiClient.post<Page>(`/api/layout/pages/${id}/unpublish`, {});
  },

  // Versions
  getPageVersions(id: string) {
    return apiClient.get<PageVersion[]>(`/api/layout/pages/${id}/versions`);
  },

  restorePageVersion(id: string, version: number) {
    return apiClient.post<Page>(`/api/layout/pages/${id}/versions/${version}/restore`, {});
  },

  // Templates
  getTemplates() {
    return apiClient.get<Template[]>('/api/layout/templates');
  },

  getTemplate(id: string) {
    return apiClient.get<Template>(`/api/layout/templates/${id}`);
  },

  createTemplate(data: Omit<Template, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<Template>('/api/layout/templates', data);
  },

  updateTemplate(id: string, data: Partial<Template>) {
    return apiClient.put<Template>(`/api/layout/templates/${id}`, data);
  },

  deleteTemplate(id: string) {
    return apiClient.delete<void>(`/api/layout/templates/${id}`);
  },

  // Media
  getMedia(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<MediaItem>>('/api/layout/media', {
      params: pagination,
    });
  },

  uploadMedia(file: File, altText?: string) {
    const formData = new FormData();
    formData.append('file', file);
    if (altText) {
      formData.append('altText', altText);
    }
    return apiClient.post<MediaItem>('/api/layout/media/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  deleteMedia(id: string) {
    return apiClient.delete<void>(`/api/layout/media/${id}`);
  },
};
