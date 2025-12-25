import { apiClient } from '../client'
import type { Page, Template, MediaItem } from '@/types/cms'
import type { PaginatedResponse, PaginationParams } from '@/types/api'

export const cmsApi = {
  // Pages
  getPages(filters?: any, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Page>>('/cms/pages', { params: { ...filters, ...pagination } })
  },

  getPage(id: string) {
    return apiClient.get<Page>(`/cms/pages/${id}`)
  },

  createPage(data: Omit<Page, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<Page>('/cms/pages', data)
  },

  updatePage(id: string, data: Partial<Page>) {
    return apiClient.put<Page>(`/cms/pages/${id}`, data)
  },

  deletePage(id: string) {
    return apiClient.delete<void>(`/cms/pages/${id}`)
  },

  publishPage(id: string) {
    return apiClient.post<Page>(`/cms/pages/${id}/publish`, {})
  },

  unpublishPage(id: string) {
    return apiClient.post<Page>(`/cms/pages/${id}/unpublish`, {})
  },

  // Versions
  getPageVersions(id: string) {
    return apiClient.get<any[]>(`/cms/pages/${id}/versions`)
  },

  restorePageVersion(id: string, version: number) {
    return apiClient.post<Page>(`/cms/pages/${id}/versions/${version}/restore`, {})
  },

  // Templates
  getTemplates() {
    return apiClient.get<Template[]>('/cms/templates')
  },

  getTemplate(id: string) {
    return apiClient.get<Template>(`/cms/templates/${id}`)
  },

  createTemplate(data: Omit<Template, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<Template>('/cms/templates', data)
  },

  updateTemplate(id: string, data: Partial<Template>) {
    return apiClient.put<Template>(`/cms/templates/${id}`, data)
  },

  deleteTemplate(id: string) {
    return apiClient.delete<void>(`/cms/templates/${id}`)
  },

  // Media
  getMedia(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<MediaItem>>('/cms/media', { params: pagination })
  },

  uploadMedia(file: File, altText?: string) {
    const formData = new FormData()
    formData.append('file', file)
    if (altText) {
      formData.append('altText', altText)
    }
    return apiClient.post<MediaItem>('/cms/media/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },

  deleteMedia(id: string) {
    return apiClient.delete<void>(`/cms/media/${id}`)
  },
}
