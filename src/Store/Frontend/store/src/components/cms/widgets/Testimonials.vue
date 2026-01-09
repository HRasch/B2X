<template>
  <div class="testimonials">
    <div v-if="settings.title" class="testimonials-header">
      <h2 class="testimonials-title">{{ settings.title }}</h2>
      <p v-if="settings.subtitle" class="testimonials-subtitle">
        {{ settings.subtitle }}
      </p>
    </div>

    <div class="testimonials-grid">
      <div v-for="testimonial in testimonials" :key="testimonial.id" class="testimonial-card">
        <div class="stars">
          <span v-for="i in testimonial.rating || 5" :key="i" class="star">★</span>
        </div>
        <p class="testimonial-text">{{ testimonial.text }}</p>
        <div class="testimonial-author">
          <img
            v-if="testimonial.avatar"
            :src="testimonial.avatar"
            :alt="testimonial.author"
            class="author-avatar"
          />
          <div class="author-info">
            <p class="author-name">{{ testimonial.author }}</p>
            <p class="author-title">{{ testimonial.title }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Testimonial {
  id: string;
  text: string;
  author: string;
  title?: string;
  avatar?: string;
  rating?: number;
}

interface Props {
  settings?: {
    title?: string;
    subtitle?: string;
    testimonials?: Testimonial[];
  };
  widgetId?: string;
}

const props = withDefaults(defineProps<Props>(), {
  settings: () => ({
    testimonials: [] as Testimonial[],
  }),
});

const testimonials = computed(() => props.settings.testimonials || []);
</script>

<style scoped>
.testimonials {
  width: 100%;
  padding: 2rem 1rem;
}

.testimonials-header {
  text-align: center;
  margin-bottom: 3rem;
}

.testimonials-title {
  font-size: 2rem;
  font-weight: bold;
  margin: 0 0 0.5rem 0;
}

.testimonials-subtitle {
  color: #666;
  font-size: 1rem;
  margin: 0;
}

.testimonials-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.testimonial-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1.5rem;
  transition: box-shadow 0.3s;
}

.testimonial-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stars {
  color: #ffc107;
  font-size: 1.25rem;
  margin-bottom: 1rem;
}

.star {
  margin-right: 0.25rem;
}

.testimonial-text {
  color: #333;
  line-height: 1.6;
  margin: 1rem 0;
  font-style: italic;
}

.testimonial-author {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #f0f0f0;
}

.author-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  object-fit: cover;
}

.author-info {
  flex: 1;
}

.author-name {
  font-weight: bold;
  margin: 0;
  font-size: 0.95rem;
}

.author-title {
  color: #666;
  margin: 0;
  font-size: 0.85rem;
}
</style>
