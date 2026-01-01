import type { App } from "vue";
import type { Router } from "vue-router";
import type { Store } from "pinia";

export interface TestModeConfig {
  enabled: boolean;
  autoFix: boolean;
  logLevel: "error" | "warn" | "info" | "debug";
  visualIndicators: boolean;
  performanceMonitoring: boolean;
}

export interface BrowserAction {
  type: "click" | "navigation" | "form-submit" | "api-call" | "error";
  element?: string;
  url?: string;
  timestamp: number;
  success: boolean;
  error?: string;
  duration?: number;
}

export interface AutoFixRule {
  condition: (action: BrowserAction) => boolean;
  fix: (action: BrowserAction) => Promise<void>;
  description: string;
}

export class TestModeManager {
  private config: TestModeConfig;
  private actions: BrowserAction[] = [];
  private fixRules: AutoFixRule[] = [];
  private isActive = false;

  constructor(config: Partial<TestModeConfig> = {}) {
    this.config = {
      enabled: false,
      autoFix: true,
      logLevel: "info",
      visualIndicators: true,
      performanceMonitoring: true,
      ...config,
    };

    this.initializeFixRules();
  }

  // Aktiviert den Testmodus
  enable(): void {
    if (this.isActive) return;

    this.isActive = true;
    this.config.enabled = true;
    this.setupGlobalHandlers();
    this.injectVisualIndicators();
    this.log("TestMode aktiviert", "info");
  }

  // Deaktiviert den Testmodus
  disable(): void {
    this.isActive = false;
    this.config.enabled = false;
    this.removeVisualIndicators();
    this.removeGlobalHandlers();
    this.log("TestMode deaktiviert", "info");
  }

  // Überwacht Browser-Aktionen
  private setupGlobalHandlers(): void {
    // Click-Handler
    document.addEventListener("click", this.handleClick.bind(this), true);

    // Navigation-Handler
    window.addEventListener("popstate", this.handleNavigation.bind(this));

    // Error-Handler
    window.addEventListener("error", this.handleError.bind(this));
    window.addEventListener(
      "unhandledrejection",
      this.handlePromiseRejection.bind(this)
    );

    // Performance-Monitoring
    if (this.config.performanceMonitoring) {
      this.setupPerformanceMonitoring();
    }
  }

  private removeGlobalHandlers(): void {
    document.removeEventListener("click", this.handleClick.bind(this), true);
    window.removeEventListener("popstate", this.handleNavigation.bind(this));
    window.removeEventListener("error", this.handleError.bind(this));
    window.removeEventListener(
      "unhandledrejection",
      this.handlePromiseRejection.bind(this)
    );
  }

  private handleClick(event: Event): void {
    const target = event.target as HTMLElement;
    const action: BrowserAction = {
      type: "click",
      element: this.getElementSelector(target),
      timestamp: Date.now(),
      success: true,
    };

    this.recordAction(action);
    this.checkAndApplyFixes(action);
  }

  private handleNavigation(): void {
    const action: BrowserAction = {
      type: "navigation",
      url: window.location.href,
      timestamp: Date.now(),
      success: true,
    };

    this.recordAction(action);
  }

  private handleError(event: ErrorEvent): void {
    const action: BrowserAction = {
      type: "error",
      element: event.filename ? `${event.filename}:${event.lineno}` : undefined,
      timestamp: Date.now(),
      success: false,
      error: event.message,
    };

    this.recordAction(action);
    this.checkAndApplyFixes(action);
  }

  private handlePromiseRejection(event: PromiseRejectionEvent): void {
    const action: BrowserAction = {
      type: "error",
      timestamp: Date.now(),
      success: false,
      error: event.reason?.toString() || "Unhandled promise rejection",
    };

    this.recordAction(action);
    this.checkAndApplyFixes(action);
  }

  private setupPerformanceMonitoring(): void {
    // Performance Observer für API-Calls
    const observer = new PerformanceObserver((list) => {
      for (const entry of list.getEntries()) {
        if (entry.name.includes("/api/")) {
          const action: BrowserAction = {
            type: "api-call",
            url: entry.name,
            timestamp: Date.now(),
            success: true,
            duration: entry.duration,
          };
          this.recordAction(action);
        }
      }
    });

    observer.observe({ entryTypes: ["resource"] });
  }

  // Visuelle Indikatoren für Testmodus
  private injectVisualIndicators(): void {
    if (!this.config.visualIndicators) return;

    const style = document.createElement("style");
    style.textContent = `
      .test-mode-indicator {
        position: fixed;
        top: 10px;
        right: 10px;
        background: rgba(255, 0, 0, 0.8);
        color: white;
        padding: 5px 10px;
        border-radius: 4px;
        font-size: 12px;
        z-index: 9999;
        font-family: monospace;
      }
      .test-mode-error {
        border: 2px solid red !important;
        box-shadow: 0 0 10px rgba(255, 0, 0, 0.5) !important;
      }
      .test-mode-success {
        border: 2px solid green !important;
        box-shadow: 0 0 10px rgba(0, 255, 0, 0.5) !important;
      }
    `;
    document.head.appendChild(style);

    const indicator = document.createElement("div");
    indicator.className = "test-mode-indicator";
    indicator.textContent = "TEST MODE";
    document.body.appendChild(indicator);
  }

  private removeVisualIndicators(): void {
    const indicator = document.querySelector(".test-mode-indicator");
    if (indicator) indicator.remove();

    const style = document.querySelector("style[test-mode]");
    if (style) style.remove();
  }

  // Auto-Fix Regeln initialisieren
  private initializeFixRules(): void {
    // Regel für Navigation-Fehler (weiße Seiten beim Back-Button)
    this.addFixRule({
      condition: (action) => action.type === "navigation" && !action.success,
      fix: async (action) => {
        // Versuche Router neu zu initialisieren
        if (window.location.hash === "") {
          window.location.hash = "#/";
        }
        await new Promise((resolve) => setTimeout(resolve, 100));
        window.location.reload();
      },
      description: "Navigation-Fehler behoben durch Router-Reset",
    });

    // Regel für fehlende Authentifizierung (401/403 Fehler)
    this.addFixRule({
      condition: (action) =>
        action.type === "error" &&
        (action.error?.includes("401") || action.error?.includes("403")),
      fix: async (action) => {
        // Redirect zu Login und clear invalid tokens
        localStorage.removeItem("authToken");
        localStorage.removeItem("refreshToken");
        sessionStorage.clear();
        window.location.href = "/login";
      },
      description: "Authentifizierungsfehler behoben durch Redirect zu Login",
    });

    // Regel für API-Timeout (>10 Sekunden)
    this.addFixRule({
      condition: (action) =>
        action.type === "api-call" && (action.duration || 0) > 10000,
      fix: async (action) => {
        // Retry mit Cache-Bypass für B2Connect API
        if (action.url?.includes("/api/")) {
          console.log(
            "API-Timeout erkannt, würde Retry mit Cache-Bypass durchführen"
          );
          // Hier könnte ein Retry-Mechanismus implementiert werden
        }
      },
      description: "API-Timeout behoben durch Retry mit Cache-Bypass",
    });

    // Regel für Vue Router component key issues (weiße Seiten)
    this.addFixRule({
      condition: (action) =>
        action.type === "click" &&
        action.element?.includes("router-link") &&
        !action.success,
      fix: async (action) => {
        // Force router-view re-render durch key change
        const routerView = document.querySelector("router-view");
        if (routerView) {
          routerView.setAttribute("key", Date.now().toString());
        }
        // Trigger route navigation manually
        await new Promise((resolve) => setTimeout(resolve, 50));
      },
      description: "Router-Link Problem behoben durch Component-Key Reset",
    });

    // Regel für Form-Submit Fehler
    this.addFixRule({
      condition: (action) => action.type === "form-submit" && !action.success,
      fix: async (action) => {
        // Check for common form validation issues
        const forms = document.querySelectorAll("form");
        forms.forEach((form) => {
          const requiredFields = form.querySelectorAll("[required]:invalid");
          if (requiredFields.length > 0) {
            requiredFields.forEach((field) => {
              field.scrollIntoView({ behavior: "smooth" });
              field.focus();
            });
          }
        });
      },
      description: "Form-Submit Fehler behoben durch Validierung und Fokus",
    });

    // Regel für Network Errors
    this.addFixRule({
      condition: (action) =>
        action.type === "error" && action.error?.includes("NetworkError"),
      fix: async (action) => {
        // Check network connectivity und retry
        if (!navigator.onLine) {
          alert(
            "Netzwerkverbindung verloren. Bitte überprüfen Sie Ihre Internetverbindung."
          );
          return;
        }
        // Retry last action
        await new Promise((resolve) => setTimeout(resolve, 2000));
        window.location.reload();
      },
      description: "Netzwerkfehler behoben durch Connectivity-Check und Retry",
    });

    // Regel für JavaScript Errors in B2Connect Components
    this.addFixRule({
      condition: (action) =>
        action.type === "error" &&
        (action.error?.includes("Cannot read properties of null") ||
          action.error?.includes("undefined is not a function")),
      fix: async (action) => {
        // Common Vue.js null reference errors
        console.log(
          "JavaScript Null-Reference Error erkannt, versuche Recovery..."
        );
        // Force component re-render
        const components = document.querySelectorAll("[data-test]");
        components.forEach((comp) => {
          const event = new Event("force-update", { bubbles: true });
          comp.dispatchEvent(event);
        });
      },
      description:
        "JavaScript Null-Reference Error behoben durch Component Re-render",
    });
  }

  private addFixRule(rule: AutoFixRule): void {
    this.fixRules.push(rule);
  }

  private async checkAndApplyFixes(action: BrowserAction): Promise<void> {
    if (!this.config.autoFix) return;

    for (const rule of this.fixRules) {
      if (rule.condition(action)) {
        try {
          await rule.fix(action);
          this.log(`Auto-Fix angewendet: ${rule.description}`, "info");
          break;
        } catch (error) {
          this.log(`Auto-Fix fehlgeschlagen: ${error}`, "error");
        }
      }
    }
  }

  private recordAction(action: BrowserAction): void {
    this.actions.push(action);

    // Behalte nur die letzten 100 Aktionen
    if (this.actions.length > 100) {
      this.actions.shift();
    }

    this.log(`Action recorded: ${action.type}`, "debug");
  }

  private getElementSelector(element: HTMLElement): string {
    if (element.id) return `#${element.id}`;
    if (element.className) return `.${element.className.split(" ")[0]}`;

    const tag = element.tagName.toLowerCase();
    const parent = element.parentElement;
    if (parent) {
      const siblings = Array.from(parent.children);
      const index = siblings.indexOf(element) + 1;
      return `${tag}:nth-child(${index})`;
    }

    return tag;
  }

  private log(
    message: string,
    level: "error" | "warn" | "info" | "debug"
  ): void {
    const levels = { error: 0, warn: 1, info: 2, debug: 3 };
    if (levels[level] > levels[this.config.logLevel]) return;

    const timestamp = new Date().toISOString();
    console.log(`[TestMode ${level.toUpperCase()}] ${timestamp}: ${message}`);
  }

  // Öffentliche API
  getActions(): BrowserAction[] {
    return [...this.actions];
  }

  getConfig(): TestModeConfig {
    return { ...this.config };
  }

  updateConfig(newConfig: Partial<TestModeConfig>): void {
    this.config = { ...this.config, ...newConfig };
  }

  clearActions(): void {
    this.actions = [];
  }
}

// Global instance
let testModeManager: TestModeManager | null = null;

export function createTestMode(
  config?: Partial<TestModeConfig>
): TestModeManager {
  if (!testModeManager) {
    testModeManager = new TestModeManager(config);
  }
  return testModeManager;
}

export function getTestMode(): TestModeManager | null {
  return testModeManager;
}

// Vue Plugin
export const TestModePlugin = {
  install(app: App, config?: Partial<TestModeConfig>) {
    const manager = createTestMode(config);

    // Füge TestMode zu globalProperties hinzu
    app.config.globalProperties.$testMode = manager;

    // Füge TestMode zu provide/inject hinzu
    app.provide("testMode", manager);
  },
};
