export interface PageDefinition {
  id: string;
  tenantId: string;
  pageType: string;
  pagePath: string;
  pageTitle: string;
  pageDescription: string;
  metaKeywords: string;
  templateLayout: string;
  globalSettings: Record<string, any>;
  regions: PageRegion[];
  isPublished: boolean;
  publishedAt: string;
}

export interface PageRegion {
  id: string;
  name: string;
  order: number;
  settings: Record<string, any>;
  widgets: WidgetInstance[];
}

export interface WidgetInstance {
  id: string;
  widgetTypeId: string;
  componentPath: string;
  order: number;
  settings: Record<string, any>;
}

export interface WidgetDefinition {
  id: string;
  displayName: string;
  description: string;
  componentPath: string;
  category: string;
  thumbnailUrl: string;
  defaultSettings: WidgetSetting[];
  allowedPageTypes: string[];
  isEnabled: boolean;
}

export interface WidgetSetting {
  key: string;
  displayName: string;
  description: string;
  type: WidgetSettingType;
  defaultValue: any;
  isRequired: boolean;
  displayOrder: number;
  metadata: Record<string, any>;
}

export type WidgetSettingType =
  | 'text'
  | 'number'
  | 'textarea'
  | 'richtext'
  | 'select'
  | 'multiselect'
  | 'toggle'
  | 'date'
  | 'datetime'
  | 'image'
  | 'video'
  | 'color'
  | 'json';
