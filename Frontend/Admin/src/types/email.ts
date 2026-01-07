/**
 * Email Template Management Types
 */

export interface EmailTemplate {
  id: string;
  tenantId: string;
  templateKey: string;
  locale: string;
  name: string;
  description?: string;
  subject: string;
  htmlBody: string;
  plainTextBody?: string;
  layoutId?: string;
  version: number;
  status: 'draft' | 'published' | 'archived';
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  updatedBy: string;
}

export interface EmailLayout {
  id: string;
  tenantId: string;
  name: string;
  header: {
    logo?: string;
    tagline?: string;
    colors: {
      primary: string;
      secondary: string;
    };
  };
  footer: {
    contact: {
      email: string;
      phone?: string;
    };
    social: {
      facebook?: string;
      twitter?: string;
      linkedin?: string;
    };
    legal: {
      privacyUrl: string;
      termsUrl: string;
    };
  };
  styles: {
    colors: {
      primary: string;
      secondary: string;
      background: string;
      text: string;
    };
    typography: {
      fontFamily: string;
      fontSize: {
        h1: string;
        h2: string;
        h3: string;
        body: string;
        small: string;
      };
      fontWeight: {
        normal: string;
        bold: string;
      };
    };
    buttons: {
      backgroundColor: string;
      borderRadius: string;
      padding: string;
    };
    spacing: {
      padding: string;
      margin: string;
    };
  };
  customCss?: string;
  isDefault: boolean;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface EmailSendingAccount {
  id: string;
  tenantId: string;
  name: string;
  provider: 'sendgrid' | 'mailjet' | 'smtp';
  credentials: {
    apiKey?: string;
    smtp?: {
      host: string;
      port: number;
      username: string;
      password: string;
      encryption: 'none' | 'ssl' | 'tls';
    };
  };
  fromName: string;
  fromEmail: string;
  replyTo?: string;
  isDefault: boolean;
  isEnabled: boolean;
  monthlyQuota?: number;
  monthlyUsed: number;
  createdAt: string;
  updatedAt: string;
}

export interface EmailAttachment {
  id: string;
  tenantId: string;
  name: string;
  displayName: string;
  type: 'static' | 'dynamic';
  versions: Record<string, EmailAttachmentVersion>;
  usedInTemplates: string[];
  createdAt: string;
  updatedAt: string;
}

export interface EmailAttachmentVersion {
  fileName: string;
  contentType: string;
  size: number;
  url: string;
  uploadedAt: string;
}

export interface EmailLog {
  id: string;
  tenantId: string;
  templateKey: string;
  locale: string;
  toAddress: string;
  ccAddresses?: string[];
  bccAddresses?: string[];
  subject: string;
  htmlBody: string;
  plainTextBody?: string;
  context: Record<string, any>;
  accountId: string;
  externalMessageId?: string;
  status: 'sent' | 'delivered' | 'opened' | 'clicked' | 'bounced' | 'failed';
  attachmentCount: number;
  sizeBytes: number;
  sentAt: string;
  deliveredAt?: string;
  openedAt?: string;
  failedAt?: string;
  errorMessage?: string;
}

export interface EmailTemplatePreview {
  subject: string;
  htmlBody: string;
  plainTextBody?: string;
}

export interface EmailTemplateTestSend {
  toAddress: string;
  sampleData?: Record<string, unknown>;
}

export interface EmailTemplateFilters {
  status?: string;
  locale?: string;
  search?: string;
}

export interface EmailAnalytics {
  summary: {
    sent: number;
    delivered: number;
    opened: number;
    clicked: number;
    bounced: number;
    failed: number;
  };
  byTemplate: Array<{
    templateKey: string;
    sent: number;
    delivered: number;
    opened: number;
    clicked: number;
    bounced: number;
  }>;
  byLocale: Array<{
    locale: string;
    sent: number;
    delivered: number;
    opened: number;
  }>;
}

export interface LiquidVariable {
  name: string;
  description: string;
  category: string;
  example: string;
  type: 'string' | 'number' | 'boolean' | 'object' | 'array';
}

export interface EmailTemplateForm {
  templateKey: string;
  locale: string;
  name: string;
  description?: string;
  subject: string;
  htmlBody: string;
  plainTextBody?: string;
  layoutId?: string;
}

export type EmailApiError = {
  code: string;
  message: string;
  details?: Array<{
    field: string;
    message: string;
  }>;
};
