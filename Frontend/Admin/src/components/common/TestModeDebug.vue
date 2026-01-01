<template>
  <div v-if="isVisible" class="test-mode-debug-panel">
    <div class="debug-header">
      <h3>🧪 TestMode Debug Panel</h3>
      <div class="debug-controls">
        <button @click="toggleAutoFix" :class="{ active: config.autoFix }">
          Auto-Fix: {{ config.autoFix ? "ON" : "OFF" }}
        </button>
        <button @click="clearActions">Clear Actions</button>
        <button @click="exportActions">Export Log</button>
        <button @click="closePanel">×</button>
      </div>
    </div>

    <div class="debug-content">
      <div class="actions-list">
        <h4>Recent Actions ({{ actions.length }})</h4>
        <div
          class="action-item"
          v-for="(action, index) in recentActions"
          :key="index"
          :class="{ error: !action.success, success: action.success }"
        >
          <span class="action-type">{{ action.type }}</span>
          <span class="action-details">
            {{ action.element || action.url || action.error }}
            <span v-if="action.duration" class="duration"
              >({{ action.duration.toFixed(0) }}ms)</span
            >
          </span>
          <span class="action-time">{{ formatTime(action.timestamp) }}</span>
        </div>
      </div>

      <div class="stats-panel">
        <h4>Statistics</h4>
        <div class="stat-item">
          <span>Total Actions:</span>
          <span>{{ actions.length }}</span>
        </div>
        <div class="stat-item">
          <span>Errors:</span>
          <span class="error-count">{{ errorCount }}</span>
        </div>
        <div class="stat-item">
          <span>Success Rate:</span>
          <span
            :class="{
              'success-rate': true,
              good: successRate > 90,
              poor: successRate < 70,
            }"
          >
            {{ successRate.toFixed(1) }}%
          </span>
        </div>
        <div class="stat-item">
          <span>Avg API Response:</span>
          <span>{{ avgApiResponse.toFixed(0) }}ms</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { getTestMode } from "@/utils/testMode";
import type { BrowserAction } from "@/utils/testMode";

const testMode = getTestMode();
const isVisible = ref(false);
const actions = ref<BrowserAction[]>([]);

const config = computed(
  () =>
    testMode?.getConfig() || {
      enabled: false,
      autoFix: false,
      logLevel: "info",
      visualIndicators: false,
      performanceMonitoring: false,
    }
);

const recentActions = computed(() => actions.value.slice(-20).reverse());

const errorCount = computed(
  () => actions.value.filter((a) => !a.success).length
);

const successRate = computed(() => {
  if (actions.value.length === 0) return 100;
  return (
    ((actions.value.length - errorCount.value) / actions.value.length) * 100
  );
});

const avgApiResponse = computed(() => {
  const apiActions = actions.value.filter(
    (a) => a.type === "api-call" && a.duration
  );
  if (apiActions.length === 0) return 0;
  return (
    apiActions.reduce((sum, a) => sum + (a.duration || 0), 0) /
    apiActions.length
  );
});

// Keyboard shortcut to toggle debug panel
const handleKeyPress = (event: KeyboardEvent) => {
  if (event.ctrlKey && event.shiftKey && event.key === "T") {
    event.preventDefault();
    isVisible.value = !isVisible.value;
  }
};

const updateActions = () => {
  if (testMode) {
    actions.value = testMode.getActions();
  }
};

const toggleAutoFix = () => {
  if (testMode) {
    testMode.updateConfig({ autoFix: !config.value.autoFix });
  }
};

const clearActions = () => {
  if (testMode) {
    testMode.clearActions();
    actions.value = [];
  }
};

const exportActions = () => {
  const data = {
    timestamp: new Date().toISOString(),
    config: config.value,
    actions: actions.value,
  };

  const blob = new Blob([JSON.stringify(data, null, 2)], {
    type: "application/json",
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `testmode-log-${new Date().toISOString().slice(0, 19)}.json`;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
};

const closePanel = () => {
  isVisible.value = false;
};

const formatTime = (timestamp: number) => {
  return new Date(timestamp).toLocaleTimeString();
};

onMounted(() => {
  document.addEventListener("keydown", handleKeyPress);

  // Update actions periodically
  const interval = setInterval(updateActions, 1000);

  onUnmounted(() => {
    document.removeEventListener("keydown", handleKeyPress);
    clearInterval(interval);
  });
});
</script>

<style scoped>
.test-mode-debug-panel {
  position: fixed;
  bottom: 10px;
  right: 10px;
  width: 400px;
  max-height: 600px;
  background: rgba(0, 0, 0, 0.9);
  color: white;
  border-radius: 8px;
  font-family: monospace;
  font-size: 12px;
  z-index: 10000;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
  overflow: hidden;
}

.debug-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background: rgba(255, 255, 255, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.debug-header h3 {
  margin: 0;
  font-size: 14px;
}

.debug-controls {
  display: flex;
  gap: 5px;
}

.debug-controls button {
  padding: 4px 8px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 10px;
}

.debug-controls button.active {
  background: rgba(0, 255, 0, 0.3);
  border-color: rgba(0, 255, 0, 0.5);
}

.debug-controls button:hover {
  background: rgba(255, 255, 255, 0.2);
}

.debug-content {
  padding: 10px;
  max-height: 500px;
  overflow-y: auto;
}

.actions-list h4,
.stats-panel h4 {
  margin: 0 0 10px 0;
  font-size: 13px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  padding-bottom: 5px;
}

.action-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 4px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.action-item.error {
  color: #ff6b6b;
}

.action-item.success {
  color: #51cf66;
}

.action-type {
  font-weight: bold;
  min-width: 80px;
}

.action-details {
  flex: 1;
  margin: 0 10px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.duration {
  color: #ffd43b;
  font-size: 10px;
}

.action-time {
  font-size: 10px;
  color: #868e96;
}

.stats-panel {
  margin-top: 15px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  padding: 3px 0;
}

.error-count {
  color: #ff6b6b;
}

.success-rate.good {
  color: #51cf66;
}

.success-rate.poor {
  color: #ff6b6b;
}
</style>
