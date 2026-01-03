<template>
  <section class="video-widget py-8">
    <div
      :style="{ paddingBottom: paddingBottom }"
      class="relative w-full h-0 overflow-hidden rounded"
    >
      <iframe
        :src="settings.videoUrl"
        :allow="settings.autoplay ? 'autoplay' : ''"
        allowfullscreen
        frameborder="0"
        class="absolute top-0 left-0 w-full h-full"
      ></iframe>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

const props = defineProps<Props>();

const paddingBottom = computed(() => {
  const ratio = props.settings.aspectRatio || '16:9';
  const [width, height] = ratio.split(':').map(Number);
  return `${(height / width) * 100}%`;
});
</script>

<style scoped>
.video-widget {
  width: 100%;
}
</style>
