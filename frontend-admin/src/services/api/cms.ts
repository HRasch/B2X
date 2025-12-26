import { apiClient } from "../client";
import type { Page, Template, MediaItem } from "@/types/cms";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

export const cmsApi = {
  // Pages
  getPages(filters?: any, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Page>>("/pages", {
      params: { ...filters, ...pagination },
    });
  },

  getPage(id: string) {
    return apiClient.get<Page>(`/pages/${id}`);
  },

  createPage(data: Omit<Page, "id" | "createdAt" | "updatedAt">) {
    return apiClient.post<Page>("/pages", data);
  },

  updatePage(id: string, data: Partial<Page>) {
    return apiClient.put<Page>(`/pages/${id}`, data);
  },

  deletePage(id: string) {
    return apiClient.delete<void>(`/pages/${id}`);
  },

  publishPage(id: string) {
    return apiClient.post<Page>(`/pages/${id}/publish`, {});
  },

  unpublishPage(id: string) {
    return apiClient.post<Page>(`/pages/${id}/unpublish`, {});
  },

  // Versions
  getPageVersions(id: string) {
    return apiClient.get<any[]>(`/pages/${id}/versions`);
  },

  restorePageVersion(id: string, version: number) {
    return apiClient.post<Page>(`/pages/${id}/versions/${version}/restore`, {});
  },

  // Templates
  getTemplates() {
    return apiClient.get<Template[]>("/templates");
  },

  getTemplate(id: string) {
    return apiClient.get<Template>(`/templates/${id}`);
  },

  createTemplate(data: Omit<Template, "id" | "createdAt" | "updatedAt">) {
    return apiClient.post<Template>("/templates", data);
  },

  updateTemplate(id: string, data: Partial<Template>) {
    return apiClient.put<Template>(`/templates/${id}`, data);
  },

  deleteTemplate(id: string) {
    return apiClient.delete<void>(`/templates/${id}`);
  },

  // Media
  getMedia(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<MediaItem>>("/media", {
      params: pagination,
    });
  },

  uploadMedia(file: File, altText?: string) {
    const formData = new FormData();
    formData.append("file", file);
    if (altText) {
      formData.append("altText", altText);
    }
    return apiClient.post<MediaItem>("/media/upload", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  deleteMedia(id: string) {
    return apiClient.delete<void>(`/media/${id}`);
  },
};
