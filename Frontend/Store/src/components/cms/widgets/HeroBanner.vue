<template>
  <div
    class="hero min-h-[400px]"
    :style="{
      backgroundImage: settings.backgroundImage
        ? `url(${settings.backgroundImage})`
        : undefined,
    }"
  >
    <div
      class="hero-overlay"
      :style="{
        backgroundColor: settings.backgroundColor || 'rgba(0, 0, 0, 0.5)',
      }"
    ></div>
    <div class="hero-content text-center text-white">
      <div class="max-w-md">
        <h1 v-if="settings.title" class="text-5xl font-bold mb-4">
          {{ settings.title }}
        </h1>
        <p v-if="settings.subtitle" class="text-xl mb-6">
          {{ settings.subtitle }}
        </p>
        <button
          v-if="settings.buttonText"
          class="btn btn-primary"
          @click="$emit('banner-click')"
        >
          {{ settings.buttonText }}
        </button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
interface Props {
  settings?: Record<string, any>;
  widgetId?: string;
}

withDefaults(defineProps<Props>(), {
  settings: () => ({}),
});

defineEmits<{
  "banner-click": [];
}>();
</script>

<style scoped>
.hero {
  background-size: cover;
  background-position: center;
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
