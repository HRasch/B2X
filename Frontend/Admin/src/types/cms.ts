/* eslint-disable @typescript-eslint/no-explicit-any -- Dynamic content blocks need flexible types */

export interface Page {
  id: string;
  title: string;
  slug: string;
  content: PageBlock[];
  metaDescription: string;
  metaKeywords: string;
  ogTitle?: string;
  ogImage?: string;
  status: "draft" | "published";
  publishedAt?: Date;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  updatedBy: string;
  language: string;
  tenantId: string;
}

export interface PageBlock {
  id: string;
  type:
    | "text"
    | "image"
    | "gallery"
    | "video"
    | "html"
    | "product-grid"
    | "custom";
  order: number;
  data: Record<string, any>;
}

export interface Template {
  id: string;
  name: string;
  description: string;
  blocks: PageBlock[];
  createdAt: Date;
  updatedAt: Date;
  tenantId: string;
}

export interface MediaItem {
  id: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  url: string;
  thumbnailUrl?: string;
  altText?: string;
  uploadedAt: Date;
  uploadedBy: string;
  tenantId: string;
}

export interface CMSState {
  pages: Page[];
  currentPage: Page | null;
  templates: Template[];
  mediaItems: MediaItem[];
  loading: boolean;
  error: string | null;
}
