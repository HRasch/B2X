<template>
  <div
    class="hero-banner relative overflow-hidden text-center"
    :style="{
      backgroundImage: `url('${settings.backgroundImage}')`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
      height: `${settings.height || 500}px`
    }"
  >
    <!-- Overlay -->
    <div class="absolute inset-0 bg-black bg-opacity-30"></div>

    <!-- Content -->
    <div
      class="relative z-10 h-full flex flex-col justify-center items-center"
      :style="{ color: settings.textColor || '#ffffff' }"
    >
      <h1 class="text-5xl font-bold mb-4">{{ settings.title }}</h1>
      <p class="text-xl mb-8 max-w-2xl">{{ settings.subtitle }}</p>

      <button
        v-if="settings.ctaLink"
        class="px-8 py-3 bg-white text-gray-900 font-semibold rounded hover:bg-gray-100 transition"
        @click="navigateTo(settings.ctaLink)"
      >
        {{ settings.ctaText || 'Shop Now' }}
      </button>
    </div>
  </div>
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
.hero-banner {
  width: 100%;
}
</style>
