/**
 * Debug Mode Initialization Plugin
 *
 * This Nuxt plugin initializes debug mode for the B2X platform.
 * Debug mode can be enabled via:
 * - URL parameter: ?debug=true
 * - localStorage: debug-enabled = true
 * - Console command: window.enableDebug()
 */

export default defineNuxtPlugin(() => {
  // Skip on server/SSR to avoid window/document access errors
  if (process.server || typeof window === 'undefined' || typeof document === 'undefined') {
    return;
  }

  // Check if debug mode should be enabled
  function shouldEnableDebug() {
    // Check URL parameter
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('debug') === 'true') {
      return true;
    }

    // Check localStorage
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem('debug-enabled') === 'true';
    }

    return false;
  }

  // Enable debug mode
  function enableDebug() {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('debug-enabled', 'true');
      console.log('🔧 Debug mode enabled. Reload the page to activate.');
      console.log('💡 Use window.disableDebug() to disable debug mode.');
    }
  }

  // Disable debug mode
  function disableDebug() {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('debug-enabled');
      console.log('🔧 Debug mode disabled. Reload the page to deactivate.');
    }
  }

  // Get debug status
  function isDebugEnabled() {
    if (typeof localStorage !== 'undefined') {
      return localStorage.getItem('debug-enabled') === 'true';
    }
    return false;
  }

  // Initialize debug mode if conditions are met
  if (shouldEnableDebug()) {
    console.log('🔧 B2X Debug Mode Active');
    console.log('📊 Debug features enabled:');
    console.log('   • User action recording');
    console.log('   • Error capture and reporting');
    console.log('   • Feedback widget');
    console.log('   • Debug panel');
    console.log('💡 Use the debug trigger button (bottom-right) to access features.');
    console.log('🔧 Use window.disableDebug() to disable debug mode.');

    // Set localStorage flag
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('debug-enabled', 'true');
    }

    // Add debug styles
    const debugStyles = document.createElement('style');
    debugStyles.textContent = `
      .debug-mode-indicator {
        position: fixed;
        top: 10px;
        right: 10px;
        background: #ef4444;
        color: white;
        padding: 4px 8px;
        border-radius: 4px;
        font-size: 11px;
        font-weight: 600;
        z-index: 9999;
        font-family: monospace;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
      }

      .debug-mode-indicator::before {
        content: '🐛 DEBUG';
      }
    `;
    document.head.appendChild(debugStyles);

    // Add debug indicator
    const indicator = document.createElement('div');
    indicator.className = 'debug-mode-indicator';
    document.body.appendChild(indicator);

    // Add console commands
    window.enableDebug = enableDebug;
    window.disableDebug = disableDebug;
    window.isDebugEnabled = isDebugEnabled;

    // Add debug info to window
    window.debugInfo = {
      version: '2.0.0',
      environment: process.env.NODE_ENV || 'development',
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight
      },
      features: [
        'user-action-recording',
        'error-capture',
        'feedback-widget',
        'debug-panel',
        'signalr-streaming'
      ]
    };

    console.log('📋 Debug info available at window.debugInfo');
  } else {
    // Debug mode disabled - add minimal enable command
    window.enableDebug = enableDebug;
    window.disableDebug = disableDebug;
    window.isDebugEnabled = isDebugEnabled;

    // Add hidden enable command hint
    console.log('🔧 Debug mode available. Use window.enableDebug() to enable.');
  }

  // Add keyboard shortcut (Ctrl+Shift+D) to toggle debug mode
  document.addEventListener('keydown', function(event) {
    if (event.ctrlKey && event.shiftKey && event.key === 'D') {
      event.preventDefault();
      if (isDebugEnabled()) {
        disableDebug();
        alert('Debug mode disabled. Reload the page.');
      } else {
        enableDebug();
        alert('Debug mode enabled. Reload the page to activate.');
      }
    }
  });
});