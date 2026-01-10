/**
 * Debug Utilities
 *
 * Helper functions for debug operations including:
 * - Screenshot capture
 * - Element selectors
 * - Data sanitization
 * - Performance monitoring
 * - Data compression
 */

import { DEBUG_CONFIG } from '@/config/debug';

// Screenshot capture using html2canvas
export async function captureScreenshot(): Promise<string | null> {
  if (!DEBUG_CONFIG.features.enableScreenshots) {
    return null;
  }

  try {
    // Dynamic import to avoid bundling html2canvas unless needed
    const html2canvas = (await import('html2canvas')).default;

    const canvas = await html2canvas(document.body, {
      allowTaint: false,
      useCORS: true,
      scale: 0.5, // Reduce quality for performance
      width: Math.min(window.innerWidth, 1920),
      height: Math.min(window.innerHeight, 1080),
      ignoreElements: element => {
        // Ignore debug elements and sensitive content
        return (
          element.classList.contains('debug-trigger') ||
          element.classList.contains('debug-panel') ||
          element.classList.contains('debug-mode-indicator') ||
          element.hasAttribute('data-sensitive')
        );
      },
    });

    return canvas.toDataURL('image/jpeg', 0.8);
  } catch (error) {
    console.warn('Screenshot capture failed:', error);
    return null;
  }
}

// Generate unique CSS selector for element
export function getElementSelector(element: HTMLElement): string {
  if (!element) return '';

  // Try ID first
  if (element.id) {
    return `#${element.id}`;
  }

  // Try data attributes
  const dataAttrs = Array.from(element.attributes)
    .filter(attr => attr.name.startsWith('data-') && !attr.name.includes('sensitive'))
    .map(attr => `[${attr.name}="${attr.value}"]`);

  if (dataAttrs.length > 0) {
    return `${element.tagName.toLowerCase()}${dataAttrs.join('')}`;
  }

  // Try classes (excluding debug classes)
  const classes = Array.from(element.classList).filter(
    cls => !cls.startsWith('debug-') && cls !== 'debug-mode-indicator'
  );

  if (classes.length > 0) {
    return `${element.tagName.toLowerCase()}.${classes.join('.')}`;
  }

  // Generate path-based selector
  const path: string[] = [];
  let current: HTMLElement | null = element;

  while (current && current !== document.body) {
    let selector = current.tagName.toLowerCase();

    // Add nth-child if needed
    if (current.parentElement) {
      const siblings = Array.from(current.parentElement.children);
      const index = siblings.indexOf(current);
      if (siblings.filter(sibling => sibling.tagName === current!.tagName).length > 1) {
        selector += `:nth-child(${index + 1})`;
      }
    }

    path.unshift(selector);
    current = current.parentElement;

    // Prevent overly long selectors
    if (path.length > 5) break;
  }

  return path.join(' > ');
}

// Sanitize data for privacy
export function sanitizeData(data: any): any {
  if (!DEBUG_CONFIG.privacy.maskSensitiveData) {
    return data;
  }

  if (typeof data === 'string') {
    // Mask potential sensitive data
    return data
      .replace(/\b\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}\b/g, '****-****-****-****') // Credit cards
      .replace(/\b\d{3}[-.]?\d{3}[-.]?\d{4}\b/g, '***-***-****') // Phone numbers
      .replace(/\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b/g, '***@***.***') // Emails
      .replace(/\b[A-Z]{2}\d{6}\b/g, '**000000') // Passport numbers
      .replace(/\b\d{9}\b/g, '*********'); // SSN-like numbers
  }

  if (Array.isArray(data)) {
    return data.map(item => sanitizeData(item));
  }

  if (typeof data === 'object' && data !== null) {
    const sanitized: any = {};
    for (const [key, value] of Object.entries(data)) {
      // Skip sensitive keys
      if (
        DEBUG_CONFIG.privacy.excludeUrls.some(
          pattern => pattern.test(key) || (typeof value === 'string' && pattern.test(value))
        )
      ) {
        sanitized[key] = '[REDACTED]';
      } else {
        sanitized[key] = sanitizeData(value);
      }
    }
    return sanitized;
  }

  return data;
}

// Generate correlation ID
export function generateCorrelationId(): string {
  return Date.now().toString(36) + Math.random().toString(36).substr(2, 9);
}

// Get user agent info
export function getUserAgentInfo(): {
  browser: string;
  version: string;
  os: string;
  device: string;
} {
  const ua = navigator.userAgent;
  const browser = ua.match(/(Chrome|Firefox|Safari|Edge|Opera|IE)\/?\s*(\d+)/i);
  const os = ua.match(/(Windows|Mac|Linux|iOS|Android)/i);
  const device = /Mobile|Tablet|iPad|iPhone|Android/.test(ua) ? 'mobile' : 'desktop';

  return {
    browser: browser ? browser[1] : 'Unknown',
    version: browser ? browser[2] : 'Unknown',
    os: os ? os[1] : 'Unknown',
    device,
  };
}

// Get viewport info
export function getViewportInfo(): {
  width: number;
  height: number;
  pixelRatio: number;
  orientation?: string;
} {
  return {
    width: window.innerWidth,
    height: window.innerHeight,
    pixelRatio: window.devicePixelRatio || 1,
    orientation: (screen as any)?.orientation?.type,
  };
}

// Performance monitoring
export function measurePerformance<T>(name: string, fn: () => T): T {
  const start = performance.now();
  try {
    const result = fn();
    const duration = performance.now() - start;
    console.log(`⚡ ${name}: ${duration.toFixed(2)}ms`);
    return result;
  } catch (error) {
    const duration = performance.now() - start;
    console.error(`❌ ${name} failed after ${duration.toFixed(2)}ms:`, error);
    throw error;
  }
}

// Compress data for storage/transmission
export function compressData(data: any): string {
  try {
    const jsonString = JSON.stringify(data);
    // Simple compression - in production, use proper compression
    return btoa(encodeURIComponent(jsonString));
  } catch (error) {
    console.warn('Data compression failed:', error);
    return JSON.stringify(data);
  }
}

// Decompress data
export function decompressData(compressed: string): any {
  try {
    const jsonString = decodeURIComponent(atob(compressed));
    return JSON.parse(jsonString);
  } catch (error) {
    console.warn('Data decompression failed:', error);
    return null;
  }
}

// Check if URL is allowed for debugging
export function isUrlAllowed(url: string): boolean {
  try {
    const urlObj = new URL(url);
    return DEBUG_CONFIG.privacy.allowedDomains.includes(urlObj.hostname);
  } catch {
    return false;
  }
}

// Get current page info
export function getPageInfo(): {
  url: string;
  title: string;
  referrer: string;
  timestamp: Date;
} {
  return {
    url: window.location.href,
    title: document.title,
    referrer: document.referrer,
    timestamp: new Date(),
  };
}

// Debounce function for performance
export function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout | null = null;

  return (...args: Parameters<T>) => {
    if (timeout) clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}

// Throttle function for performance
export function throttle<T extends (...args: any[]) => any>(
  func: T,
  limit: number
): (...args: Parameters<T>) => void {
  let inThrottle = false;

  return (...args: Parameters<T>) => {
    if (!inThrottle) {
      func(...args);
      inThrottle = true;
      setTimeout(() => (inThrottle = false), limit);
    }
  };
}

// Check if element is visible
export function isElementVisible(element: HTMLElement): boolean {
  const rect = element.getBoundingClientRect();
  return (
    rect.width > 0 &&
    rect.height > 0 &&
    rect.top >= 0 &&
    rect.left >= 0 &&
    rect.bottom <= window.innerHeight &&
    rect.right <= window.innerWidth
  );
}

// Get element context (surrounding elements)
export function getElementContext(element: HTMLElement, levels = 2): any {
  const context: any = {
    element: getElementSelector(element),
    text: element.textContent?.substring(0, 100),
    attributes: {},
  };

  // Add relevant attributes
  Array.from(element.attributes).forEach(attr => {
    if (!attr.name.includes('sensitive') && attr.value.length < 100) {
      context.attributes[attr.name] = attr.value;
    }
  });

  // Add parent context
  if (levels > 0 && element.parentElement) {
    context.parent = getElementContext(element.parentElement, levels - 1);
  }

  return context;
}

// Format error for logging
export function formatError(error: Error | ErrorEvent | any): {
  message: string;
  stack?: string;
  type: string;
  metadata?: any;
} {
  if (error instanceof ErrorEvent) {
    return {
      message: error.message,
      stack: error.error?.stack,
      type: 'javascript',
      metadata: {
        filename: error.filename,
        lineno: error.lineno,
        colno: error.colno,
      },
    };
  }

  if (error instanceof Error) {
    return {
      message: error.message,
      stack: error.stack,
      type: 'javascript',
    };
  }

  return {
    message: String(error),
    type: 'unknown',
  };
}

// Export utilities
export const debugUtils = {
  captureScreenshot,
  getElementSelector,
  sanitizeData,
  generateCorrelationId,
  getUserAgentInfo,
  getViewportInfo,
  measurePerformance,
  compressData,
  decompressData,
  isUrlAllowed,
  getPageInfo,
  debounce,
  throttle,
  isElementVisible,
  getElementContext,
  formatError,
};

export default debugUtils;
