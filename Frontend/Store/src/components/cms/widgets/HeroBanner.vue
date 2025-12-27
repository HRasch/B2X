<template>
  <div class="hero-banner" :style="heroStyles">
    <div class="hero-content">
      <h1 v-if="settings.title" class="hero-title">{{ settings.title }}</h1>
      <p v-if="settings.subtitle" class="hero-subtitle">
        {{ settings.subtitle }}
      </p>
      <button v-if="settings.buttonText" class="hero-button">
        {{ settings.buttonText }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

interface Props {
  settings?: Record<string, any>;
  widgetId?: string;
}

const props = withDefaults(defineProps<Props>(), {
  settings: () => ({}),
});

const heroStyles = computed(() => ({
  backgroundImage: props.settings.backgroundImage
    ? `url(${props.settings.backgroundImage})`
    : undefined,
  backgroundColor: props.settings.backgroundColor || "#f0f0f0",
  minHeight: props.settings.minHeight || "400px",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  color: props.settings.textColor || "#000",
  backgroundSize: "cover",
  backgroundPosition: "center",
}));
</script>

<style scoped>
.hero-banner {
  position: relative;
  overflow: hidden;
}

.hero-content {
  text-align: center;
  z-index: 1;
}

.hero-title {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 1rem;
}

.hero-subtitle {
  font-size: 1.25rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.hero-button {
  padding: 0.75rem 2rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s;
}

.hero-button:hover {
  background-color: #0056b3;
}
</style>
