/* eslint-disable @typescript-eslint/no-explicit-any -- Test mocks use any */
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useCmsStore } from '@/stores/cms';
import { cmsApi } from '@/services/api/cms';
import type { Page, PaginatedResponse } from '@/types';

vi.mock('@/services/api/cms', () => ({
  cmsApi: {
    getPages: vi.fn(),
    getPage: vi.fn(),
    createPage: vi.fn(),
    updatePage: vi.fn(),
    deletePage: vi.fn(),
    publishPage: vi.fn(),
    getTemplates: vi.fn(),
    uploadMedia: vi.fn(),
    getMedia: vi.fn(),
  },
}));

describe('CMS Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  describe('Initial State', () => {
    it('should initialize with empty arrays', () => {
      const store = useCmsStore();
      expect(store.pages).toEqual([]);
      expect(store.templates).toEqual([]);
      expect(store.mediaItems).toEqual([]);
      expect(store.currentPage).toBeNull();
    });
  });

  describe('Fetch Pages', () => {
    it('should fetch pages successfully', async () => {
      const mockPages: Page[] = [
        {
          id: '1',
          title: 'Home',
          slug: 'home',
          content: [],
          metaDescription: 'Home page',
          metaKeywords: 'home',
          status: 'published',
          publishedAt: new Date(),
          createdAt: new Date(),
          updatedAt: new Date(),
          createdBy: 'admin',
          updatedBy: 'admin',
          language: 'en',
          tenantId: 'tenant-1',
        },
      ];

      const mockResponse: PaginatedResponse<Page> = {
        items: mockPages,
        total: 1,
        page: 1,
        pageSize: 10,
        totalPages: 1,
      };

      vi.mocked(cmsApi.getPages).mockResolvedValue(mockResponse);

      const store = useCmsStore();
      await store.fetchPages();

      expect(store.pages).toEqual(mockPages);
      expect(store.loading).toBe(false);
      expect(store.error).toBeNull();
    });

    it('should handle fetch error', async () => {
      const error = new Error('Network error');
      vi.mocked(cmsApi.getPages).mockRejectedValue(error);

      const store = useCmsStore();
      await store.fetchPages();

      expect(store.pages).toEqual([]);
      expect(store.error).toBe('Network error');
    });

    it('should set loading state', async () => {
      vi.mocked(cmsApi.getPages).mockImplementation(
        () =>
          new Promise(resolve => {
            setTimeout(
              () =>
                resolve({
                  items: [],
                  total: 0,
                  page: 1,
                  pageSize: 10,
                  totalPages: 0,
                }),
              100
            );
          })
      );

      const store = useCmsStore();
      const promise = store.fetchPages();
      expect(store.loading).toBe(true);
      await promise;
      expect(store.loading).toBe(false);
    });
  });

  describe('Fetch Page', () => {
    it('should fetch single page', async () => {
      const mockPage: Page = {
        id: '1',
        title: 'About',
        slug: 'about',
        content: [],
        metaDescription: 'About page',
        metaKeywords: 'about',
        status: 'published',
        publishedAt: new Date(),
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };

      vi.mocked(cmsApi.getPage).mockResolvedValue(mockPage);

      const store = useCmsStore();
      await store.fetchPage('1');

      expect(store.currentPage).toEqual(mockPage);
    });
  });

  describe('Save Page', () => {
    it('should create new page', async () => {
      const newPage: Page = {
        id: '1',
        title: 'New Page',
        slug: 'new-page',
        content: [],
        metaDescription: '',
        metaKeywords: '',
        status: 'draft',
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };

      vi.mocked(cmsApi.createPage).mockResolvedValue(newPage);

      const store = useCmsStore();
      const result = await store.savePage({
        title: 'New Page',
        slug: 'new-page',
        content: [],
        metaDescription: '',
        metaKeywords: '',
        status: 'draft',
        language: 'en',
        tenantId: 'tenant-1',
      } as any);

      expect(store.pages.length).toBeGreaterThan(0);
      expect(store.pages[store.pages.length - 1].id).toBe(newPage.id);
      expect(store.currentPage).toEqual(newPage);
      expect(result).toEqual(newPage);
    });

    it('should update existing page', async () => {
      const existingPage: Page = {
        id: '1',
        title: 'Updated Page',
        slug: 'updated-page',
        content: [],
        metaDescription: 'Updated',
        metaKeywords: 'updated',
        status: 'draft',
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };

      vi.mocked(cmsApi.updatePage).mockResolvedValue(existingPage);

      const store = useCmsStore();
      const result = await store.savePage(existingPage);

      expect(store.currentPage).toEqual(existingPage);
      expect(result).toEqual(existingPage);
    });
  });

  describe('Publish Page', () => {
    it('should publish page', async () => {
      const publishedPage: Page = {
        id: '1',
        title: 'Published Page',
        slug: 'published-page',
        content: [],
        metaDescription: '',
        metaKeywords: '',
        status: 'published',
        publishedAt: new Date(),
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };

      vi.mocked(cmsApi.publishPage).mockResolvedValue(publishedPage);

      const store = useCmsStore();
      store.pages = [
        {
          id: '1',
          title: 'Draft Page',
          slug: 'draft-page',
          content: [],
          metaDescription: '',
          metaKeywords: '',
          status: 'draft',
          createdAt: new Date(),
          updatedAt: new Date(),
          createdBy: 'admin',
          updatedBy: 'admin',
          language: 'en',
          tenantId: 'tenant-1',
        },
      ];

      const result = await store.publishPage('1');

      expect(result.status).toBe('published');
      expect(store.pages[0].status).toBe('published');
    });
  });

  describe('Delete Page', () => {
    it('should delete page', async () => {
      vi.mocked(cmsApi.deletePage).mockResolvedValue(undefined as any);

      const store = useCmsStore();
      const page1: Page = {
        id: '1',
        title: 'Page 1',
        slug: 'page-1',
        content: [],
        metaDescription: '',
        metaKeywords: '',
        status: 'draft',
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };
      const page2: Page = {
        id: '2',
        title: 'Page 2',
        slug: 'page-2',
        content: [],
        metaDescription: '',
        metaKeywords: '',
        status: 'draft',
        createdAt: new Date(),
        updatedAt: new Date(),
        createdBy: 'admin',
        updatedBy: 'admin',
        language: 'en',
        tenantId: 'tenant-1',
      };

      store.pages = [page1, page2];
      store.currentPage = page1;

      await store.deletePage('1');

      expect(store.pages).toHaveLength(1);
      expect(store.pages[0].id).toBe('2');
      expect(store.currentPage).toBeNull();
    });
  });

  describe('Fetch Templates', () => {
    it('should fetch templates', async () => {
      const mockTemplates = [
        {
          id: '1',
          name: 'Hero Template',
          description: 'Hero section',
          blocks: [],
          createdAt: new Date(),
          updatedAt: new Date(),
          tenantId: 'tenant-1',
        },
      ];

      vi.mocked(cmsApi.getTemplates).mockResolvedValue(mockTemplates);

      const store = useCmsStore();
      await store.fetchTemplates();

      expect(store.templates).toEqual(mockTemplates);
    });
  });

  describe('Upload Media', () => {
    it('should upload media file', async () => {
      const mockMedia = {
        id: '1',
        fileName: 'image.jpg',
        fileType: 'image/jpeg',
        fileSize: 1024,
        url: 'http://example.com/image.jpg',
        altText: 'Test image',
        uploadedAt: new Date(),
        uploadedBy: 'admin',
        tenantId: 'tenant-1',
      };

      vi.mocked(cmsApi.uploadMedia).mockResolvedValue(mockMedia);

      const store = useCmsStore();
      const file = new File(['test'], 'test.jpg', { type: 'image/jpeg' });
      const result = await store.uploadMedia(file, 'Test image');

      expect(store.mediaItems.length).toBeGreaterThan(0);
      expect(result).toEqual(mockMedia);
    });
  });
});
