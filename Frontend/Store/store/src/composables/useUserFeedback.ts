import { toast, type ToastOptions, type Id } from 'vue3-toastify';
import { useI18n } from 'vue-i18n';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface NotificationOptions {
  title?: string;
  message: string;
  duration?: number;
  action?: {
    label: string;
    handler: () => void;
  };
}

class UserFeedbackSystem {
  private i18n: { t: (key: string) => string } | null = null;

  setI18n(i18nInstance: { t: (key: string) => string }) {
    this.i18n = i18nInstance;
  }

  success(options: NotificationOptions | string): Id {
    const opts = typeof options === 'string' ? { message: options } : options;
    return this.show('success', opts);
  }

  error(options: NotificationOptions | string): Id {
    const opts = typeof options === 'string' ? { message: options } : options;
    return this.show('error', opts);
  }

  warning(options: NotificationOptions | string): Id {
    const opts = typeof options === 'string' ? { message: options } : options;
    return this.show('warning', opts);
  }

  info(options: NotificationOptions | string): Id {
    const opts = typeof options === 'string' ? { message: options } : options;
    return this.show('info', opts);
  }

  private show(type: NotificationType, options: NotificationOptions): Id {
    const toastOptions: ToastOptions = {
      position: toast.POSITION.TOP_RIGHT,
      autoClose: options.duration || 5000,
      hideProgressBar: false,
      closeOnClick: true,
      pauseOnHover: true,
    };

    let message = options.message;
    if (options.title) {
      message = `${options.title}: ${message}`;
    }

    // Handle action button via global handler
    if (options.action) {
      const actionLabel = options.action.label;
      const actionHandler = options.action.handler;
      (window as unknown as { toastActionHandler?: (label: string) => void }).toastActionHandler = (
        label: string
      ) => {
        if (label === actionLabel) {
          actionHandler();
        }
      };
    }

    switch (type) {
      case 'success':
        return toast.success(message, toastOptions);
      case 'error':
        return toast.error(message, toastOptions);
      case 'warning':
        return toast.warning(message, toastOptions);
      case 'info':
        return toast.info(message, toastOptions);
    }
  }

  // Batch notifications
  batch(notifications: Array<{ type: NotificationType; options: NotificationOptions }>) {
    notifications.forEach(({ type, options }) => {
      this.show(type, options);
    });
  }

  // Clear all notifications
  clear() {
    toast.clearAll();
  }

  // Confirmation dialog using toast
  async confirm(options: {
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
  }): Promise<boolean> {
    return new Promise(resolve => {
      const confirmMessage = `${options.title}\n${options.message}`;

      const toastId = toast.info(confirmMessage, {
        position: toast.POSITION.TOP_CENTER,
        autoClose: false,
        closeOnClick: false,
        onClose: () => resolve(false),
      });

      (window as unknown as { confirmHandler?: (confirmed: boolean) => void }).confirmHandler = (
        confirmed: boolean
      ) => {
        toast.remove(toastId);
        resolve(confirmed);
      };
    });
  }
}

// Create singleton instance
const feedbackSystem = new UserFeedbackSystem();

// Composable for using the feedback system
export function useUserFeedback() {
  const { t } = useI18n();
  feedbackSystem.setI18n({ t });

  return {
    success: feedbackSystem.success.bind(feedbackSystem),
    error: feedbackSystem.error.bind(feedbackSystem),
    warning: feedbackSystem.warning.bind(feedbackSystem),
    info: feedbackSystem.info.bind(feedbackSystem),
    batch: feedbackSystem.batch.bind(feedbackSystem),
    clear: feedbackSystem.clear.bind(feedbackSystem),
    confirm: feedbackSystem.confirm.bind(feedbackSystem),
  };
}

// Export singleton for direct use
export { feedbackSystem as userFeedback };

// Vue plugin for global feedback system
import type { App } from 'vue';

export const UserFeedbackPlugin = {
  install(app: App) {
    app.config.globalProperties.$feedback = feedbackSystem;
    app.provide('feedback', feedbackSystem);
  },
};
