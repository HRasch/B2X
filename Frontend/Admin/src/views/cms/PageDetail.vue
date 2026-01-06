<template>
  <div class="space-y-6">
    <PageHeader
      :title="$t('cms.page_detail.title')"
      :subtitle="$t('cms.page_detail.subtitle')"
    >
      <template #actions>
        <router-link
          to="/cms/pages"
          class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
        >
          ← {{ $t('cms.page_detail.back_to_pages') }}
        </router-link>
      </template>
    </PageHeader>

    <CardContainer
      :title="$t('cms.page_detail.editor_title')"
      :subtitle="$t('cms.page_detail.coming_soon')"
    >
      <div class="text-center py-12">
        <p class="text-gray-500 dark:text-gray-400">{{ $t('cms.page_detail.coming_soon') }}</p>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useCmsStore } from '@/stores/cms';

const route = useRoute();
const cmsStore = useCmsStore();
const pageId = route.params.id as string;

onMounted(() => {
  if (pageId && pageId !== 'new') {
    cmsStore.fetchPage(pageId);
  }
});
</script>
