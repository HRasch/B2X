<template>
  <section class="product-grid-widget py-12">
    <div class="container mx-auto">
      <!-- Title -->
      <h2 v-if="settings.title" class="text-3xl font-bold mb-8">
        {{ settings.title }}
      </h2>

      <!-- Loading -->
      <div v-if="loading" class="text-center py-12">
        <LoadingSpinner />
      </div>

      <!-- Products Grid -->
      <div
        v-else
        :class="`grid grid-cols-1 md:grid-cols-2 lg:grid-cols-${settings.columns || 3} gap-6`"
      >
        <div v-for="i in 6" :key="i" class="bg-gray-100 rounded p-4 animate-pulse">
          <div class="bg-gray-300 h-48 rounded mb-4"></div>
          <div class="bg-gray-300 h-4 rounded mb-2"></div>
          <div class="bg-gray-300 h-4 rounded w-1/2"></div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="mt-12 text-center">
        <button
          v-for="page in totalPages"
          :key="page"
          :class="[
            'mx-1 px-3 py-1 rounded',
            page === currentPage
              ? 'bg-blue-600 text-white'
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300',
          ]"
          @click="currentPage = page"
        >
          {{ page }}
        </button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

defineProps<Props>();

const loading = ref(false);
const currentPage = ref(1);
const totalItems = ref(24);

const itemsPerPage = computed(() => 6);
const totalPages = computed(() => Math.ceil(totalItems.value / itemsPerPage.value));
</script>

<style scoped>
.product-grid-widget {
  width: 100%;
}
</style>
