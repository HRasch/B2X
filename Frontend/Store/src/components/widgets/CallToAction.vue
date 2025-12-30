<template>
  <div class="py-16" :style="{ backgroundColor: settings.backgroundColor }">
    <div class="container mx-auto px-4 text-center text-white">
      <h2 class="text-4xl font-bold mb-4">{{ settings.heading }}</h2>
      <p
        v-if="settings.description"
        class="text-lg mb-8 max-w-2xl mx-auto opacity-90"
      >
        {{ settings.description }}
      </p>
      <button
        v-if="settings.buttonLink"
        class="btn btn-light"
        @click="navigateTo(settings.buttonLink)"
      >
        {{ settings.buttonText || "Learn More" }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";

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
