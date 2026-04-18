# B2X Debug System

## Overview

The B2X Debug System provides comprehensive debugging capabilities for both development and production environments. It includes user action recording, error capture, feedback collection, and real-time monitoring through SignalR streaming.

## Features

- **User Action Recording**: Automatically tracks user interactions (clicks, navigation, form submissions)
- **Error Capture**: Captures JavaScript errors, network failures, and unhandled promise rejections
- **Feedback Widget**: Allows users to submit bug reports and feedback with screenshots
- **Debug Panel**: Provides real-time debugging controls and session statistics
- **SignalR Streaming**: Real-time debug event broadcasting
- **Privacy Controls**: Automatic data sanitization and configurable privacy settings

## Quick Start

### Enabling Debug Mode

Debug mode can be enabled in several ways:

1. **URL Parameter**: Add `?debug=true` to any URL
2. **Console Command**: Run `window.enableDebug()` in browser console
3. **Keyboard Shortcut**: Press `Ctrl+Shift+D`
4. **localStorage**: Set `localStorage.setItem('debug-enabled', 'true')`

### Using Debug Features

Once enabled, a debug trigger button appears in the bottom-right corner:

- **🐛 Debug Button**: Click to open the debug panel
- **Feedback Widget**: Submit bug reports with screenshots
- **Action Recording**: Automatically tracks user interactions
- **Error Monitoring**: Captures and reports errors

## Architecture

### Frontend Components

```
src/
├── composables/
│   └── useDebugContext.ts      # Main debug context composable
├── components/
│   └── widgets/
│       ├── DebugTrigger.vue    # Debug panel trigger button
│       └── DebugFeedbackWidget.vue # Feedback submission widget
├── stores/
│   └── debug.ts                # Pinia store for debug state
├── services/
│   └── debugApi.ts             # API service for backend communication
├── plugins/
│   ├── debug-init.js           # Debug mode initialization
│   └── debug-guard.js          # Route guards for debug routes
├── config/
│   └── debug.ts                # Debug configuration
└── utils/
    └── debug.ts                # Debug utility functions
```

### Backend Integration

The system integrates with backend debug endpoints:

- `POST /api/debug/sessions` - Create debug session
- `POST /api/debug/actions` - Log user actions
- `POST /api/debug/errors` - Report errors
- `POST /api/debug/feedback` - Submit feedback
- `GET /api/debug/signalr/info` - SignalR connection info

## Configuration

Debug behavior is controlled by `src/config/debug.ts`:

```typescript
export const DEBUG_CONFIG = {
  features: {
    enableUserActionRecording: true,
    enableErrorCapture: true,
    enableFeedbackWidget: true,
    enableSignalR: true, // Real-time event streaming enabled
  },
  limits: {
    maxActionsPerSession: 1000,
    maxErrorsPerSession: 100,
  },
  privacy: {
    maskSensitiveData: true,
    excludeUrls: [/\/api\/auth/, /password/i],
  }
}
```

## Usage Examples

### Basic Debug Context Usage

```vue
<template>
  <div>
    <button @click="handleClick">Click me</button>
    <DebugTrigger v-if="isDebugEnabled" />
  </div>
</template>

<script setup>
import { useDebugContext } from '@/composables/useDebugContext'
import DebugTrigger from '@/components/widgets/DebugTrigger.vue'

const { isDebugEnabled, recordAction } = useDebugContext()

function handleClick() {
  recordAction({
    type: 'click',
    element: 'button',
    selector: '#my-button',
    metadata: { customData: 'example' }
  })
}
</script>
```

### Custom Error Reporting

```typescript
import { useDebugContext } from '@/composables/useDebugContext'

const { recordError } = useDebugContext()

try {
  // Some risky operation
  riskyOperation()
} catch (error) {
  recordError({
    type: 'custom',
    message: error.message,
    stack: error.stack,
    metadata: { operation: 'riskyOperation' }
  })
}
```

### Feedback Submission

```vue
<template>
  <button @click="openFeedback">Report Issue</button>
  <DebugFeedbackWidget v-model="showFeedback" />
</template>

<script setup>
import { ref } from 'vue'
import DebugFeedbackWidget from '@/components/widgets/DebugFeedbackWidget.vue'

const showFeedback = ref(false)

function openFeedback() {
  showFeedback.value = true
}
</script>
```

## API Reference

### useDebugContext Composable

```typescript
interface UseDebugContextReturn {
  // State
  session: Ref<DebugSession | null>
  isRecording: ComputedRef<boolean>
  actions: Ref<DebugAction[]>
  errors: Ref<DebugError[]>

  // Methods
  startSession(): Promise<void>
  stopSession(): void
  recordAction(action: DebugActionInput): void
  recordError(error: DebugErrorInput): void
  submitFeedback(feedback: DebugFeedbackInput): Promise<void>
  captureScreenshot(): Promise<string | null>
}
```

### Debug Store (Pinia)

```typescript
interface DebugStore {
  // State
  isEnabled: boolean
  session: DebugSession | null
  actions: DebugAction[]
  errors: DebugError[]
  settings: DebugSettings

  // Actions
  initialize(): void
  enable(): void
  disable(): void
  startSession(): void
  stopSession(): void
  recordAction(action: DebugAction): void
  recordError(error: DebugError): void
  submitFeedback(feedback: DebugFeedback): void
}
```

## Privacy & Security

- **Data Sanitization**: Sensitive data is automatically masked
- **Configurable Privacy**: Privacy settings can be customized
- **Domain Restrictions**: Only allowed domains can use debug features
- **Consent Requirements**: Can require user consent in production

## Development vs Production

### Development
- Full debug features enabled
- Console logging active
- All data collection enabled
- No privacy restrictions

### Production
- Limited features based on configuration
- Minimal console logging
- Privacy controls active
- Consent may be required

## Troubleshooting

### Debug Mode Not Appearing
1. Check browser console for errors
2. Verify localStorage permissions
3. Try `window.enableDebug()` in console
4. Check network requests to debug endpoints

### Actions Not Recording
1. Ensure debug session is active
2. Check browser console for errors
3. Verify event listeners are attached
4. Check privacy/exclusion settings

### SignalR Not Connecting
1. Check network connectivity to backend
2. Verify SignalR hub endpoint is accessible (`/debug-hub`)
3. Check browser console for connection errors
4. Ensure tenant context headers are included
5. Verify backend SignalR services are running

## Future Enhancements

- **Performance Monitoring**: Page load times, memory usage
- **Network Monitoring**: API call tracking and analysis
- **Heatmaps**: User interaction visualization
- **A/B Testing**: Debug-driven feature testing
- **Analytics Integration**: Debug data analytics

## Contributing

When adding new debug features:

1. Update `DEBUG_CONFIG` with new feature flags
2. Add TypeScript interfaces for new data types
3. Update privacy and security checks
4. Add comprehensive error handling
5. Update this documentation
6. Test in both development and production modes

## Support

For issues or questions:
- Check browser console for debug logs
- Review network requests to debug endpoints
- Verify configuration settings
- Test with different browsers/devices