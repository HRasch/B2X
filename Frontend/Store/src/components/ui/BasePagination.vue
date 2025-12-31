<template>
  <nav aria-label="Pagination Navigation" class="flex justify-center">
    <ul class="flex items-center gap-1">
      <!-- Previous button -->
      <li>
        <button
          :disabled="currentPage === 1"
          class="btn btn-sm btn-outline"
          :class="{ 'btn-disabled': currentPage === 1 }"
          @click="goToPage(currentPage - 1)"
          aria-label="Go to previous page"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
          </svg>
          <span class="sr-only">Previous</span>
        </button>
      </li>

      <!-- Page numbers -->
      <li v-for="page in visiblePages" :key="page">
        <button
          v-if="page !== '...'"
          :class="[
            'btn btn-sm',
            page === currentPage ? 'btn-primary' : 'btn-outline'
          ]"
          @click="goToPage(page)"
          :aria-label="`Go to page ${page}`"
          :aria-current="page === currentPage ? 'page' : undefined"
        >
          {{ page }}
        </button>
        <span v-else class="px-2 py-1 text-base-content/60">...</span>
      </li>

      <!-- Next button -->
      <li>
        <button
          :disabled="currentPage === totalPages"
          class="btn btn-sm btn-outline"
          :class="{ 'btn-disabled': currentPage === totalPages }"
          @click="goToPage(currentPage + 1)"
          aria-label="Go to next page"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
          </svg>
          <span class="sr-only">Next</span>
        </button>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
interface Props {
  currentPage: number;
  totalPages: number;
  maxVisiblePages?: number;
}

const props = withDefaults(defineProps<Props>(), {
  maxVisiblePages: 5,
});

const emit = defineEmits<{
  pageChange: [page: number];
}>();

const goToPage = (page: number) => {
  if (page >= 1 && page <= props.totalPages && page !== props.currentPage) {
    emit('pageChange', page);
  }
};

const visiblePages = computed(() => {
  const { currentPage, totalPages, maxVisiblePages } = props;
  const pages: (number | string)[] = [];

  if (totalPages <= maxVisiblePages) {
    // Show all pages if total is less than max visible
    for (let i = 1; i <= totalPages; i++) {
      pages.push(i);
    }
  } else {
    // Always show first page
    pages.push(1);

    // Calculate range around current page
    const start = Math.max(2, currentPage - Math.floor(maxVisiblePages / 2));
    const end = Math.min(totalPages - 1, start + maxVisiblePages - 3);

    // Add ellipsis if there's a gap after first page
    if (start > 2) {
      pages.push('...');
    }

    // Add pages in range
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    // Add ellipsis if there's a gap before last page
    if (end < totalPages - 1) {
      pages.push('...');
    }

    // Always show last page
    if (totalPages > 1) {
      pages.push(totalPages);
    }
  }

  return pages;
});
</script>

<style scoped>
/* Screen reader only text */
.sr-only {
  @apply absolute w-px h-px p-0 -m-px overflow-hidden whitespace-nowrap border-0;
  clip: rect(0, 0, 0, 0);
}
</style>