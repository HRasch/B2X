<template>
  <section class="py-12">
    <div class="container mx-auto px-4">
      <!-- Title -->
      <h2 v-if="settings.title" class="text-3xl font-bold mb-12 text-center">
        {{ settings.title }}
      </h2>

      <!-- Testimonials Carousel -->
      <div v-if="testimonials.length > 0" class="card bg-base-200 max-w-3xl mx-auto">
        <div class="card-body text-center">
          <blockquote class="text-lg italic mb-4">"{{ currentTestimonial.text }}"</blockquote>
          <p class="font-semibold">{{ currentTestimonial.author }}</p>
          <p class="text-sm opacity-70">{{ currentTestimonial.title }}</p>
        </div>

        <!-- Navigation -->
        <div v-if="testimonials.length > 1" class="card-body flex flex-row justify-center gap-4">
          <button class="btn btn-sm btn-outline" @click="previousTestimonial">← Previous</button>
          <span class="flex items-center px-4">
            {{ currentIndex + 1 }} / {{ testimonials.length }}
          </span>
          <button class="btn btn-sm btn-outline" @click="nextTestimonial">Next →</button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';

interface Testimonial {
  text: string;
  author: string;
  title: string;
}

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

const props = defineProps<Props>();

const currentIndex = ref(0);
let autoplayInterval: ReturnType<typeof setInterval> | null = null;

const testimonials = computed(() => {
  try {
    if (typeof props.settings.testimonials === 'string') {
      return JSON.parse(props.settings.testimonials);
    }
    return props.settings.testimonials || [];
  } catch {
    return [];
  }
});

const currentTestimonial = computed(() => {
  return testimonials.value[currentIndex.value] || {};
});

const nextTestimonial = () => {
  currentIndex.value = (currentIndex.value + 1) % testimonials.value.length;
};

const previousTestimonial = () => {
  currentIndex.value =
    (currentIndex.value - 1 + testimonials.value.length) % testimonials.value.length;
};

onMounted(() => {
  if (props.settings.autoplay && testimonials.value.length > 1) {
    autoplayInterval = setInterval(nextTestimonial, props.settings.autoplayInterval || 5000);
  }
});

onUnmounted(() => {
  if (autoplayInterval) {
    clearInterval(autoplayInterval);
  }
});
</script>
