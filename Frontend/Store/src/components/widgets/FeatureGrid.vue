<template>
  <section class="py-12">
    <div class="container mx-auto px-4">
      <!-- Title -->
      <h2 v-if="settings.title" class="text-3xl font-bold mb-12 text-center">
        {{ settings.title }}
      </h2>

      <!-- Features Grid -->
      <div :class="`grid grid-cols-1 md:grid-cols-2 lg:grid-cols-${settings.columns || 3} gap-8`">
        <div
          v-for="(feature, index) in features"
          :key="index"
          class="card bg-base-100 shadow-lg hover:shadow-xl transition-shadow"
        >
          <div class="card-body text-center">
            <div class="text-5xl mb-4">{{ feature.icon }}</div>
            <h3 class="card-title justify-center">{{ feature.title }}</h3>
            <p class="text-base-content/70">{{ feature.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Feature {
  icon: string;
  title: string;
  description: string;
}

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

const props = defineProps<Props>();

const features = computed(() => {
  try {
    if (typeof props.settings.features === 'string') {
      return JSON.parse(props.settings.features);
    }
    return props.settings.features || [];
  } catch {
    return [];
  }
});
</script>
