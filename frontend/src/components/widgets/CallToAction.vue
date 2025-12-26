<template>
  <section class="call-to-action-widget py-16" :style="{ backgroundColor: settings.backgroundColor }">
    <div class="container mx-auto text-center text-white">
      <h2 class="text-4xl font-bold mb-4">{{ settings.heading }}</h2>
      <p v-if="settings.description" class="text-lg mb-8 max-w-2xl mx-auto">
        {{ settings.description }}
      </p>
      <button
        v-if="settings.buttonLink"
        class="px-8 py-3 bg-white text-gray-900 font-semibold rounded hover:bg-gray-100 transition"
        @click="navigateTo(settings.buttonLink)"
      >
        {{ settings.buttonText || 'Learn More' }}
      </button>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

defineProps<Props>();

const router = useRouter();

const navigateTo = (link: string) => {
  router.push(link);
};
</script>

<style scoped>
.call-to-action-widget {
  width: 100%;
}
</style>
