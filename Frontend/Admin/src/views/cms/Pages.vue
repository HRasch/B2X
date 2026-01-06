<template>
  <div class="space-y-6">
    <PageHeader :title="$t('cms.pages.title')" :subtitle="$t('cms.pages.subtitle')">
      <template #actions>
        <router-link
          to="/cms/pages/new"
          class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          {{ $t('cms.pages.actions.new') }}
        </router-link>
      </template>
    </PageHeader>

    <CardContainer>
      <div v-if="loading" class="text-center py-8">
        <div class="text-gray-500">{{ $t('cms.pages.loading') }}</div>
      </div>

      <div v-else-if="cmsStore.pages.length === 0" class="p-8 text-center">
        <p class="text-gray-500 dark:text-soft-300">
          {{ $t('cms.pages.no_pages') }}
        </p>
      </div>

      <div v-else class="overflow-hidden">
        <table class="w-full">
          <thead class="bg-gray-100 border-b">
            <tr>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Title</th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Status</th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Updated</th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y">
            <tr v-for="page in cmsStore.pages" :key="page.id">
              <td class="px-6 py-4 text-sm text-gray-900">{{ page.title }}</td>
              <td class="px-6 py-4 text-sm">
                <span
                  :class="[
                    'px-3 py-1 rounded-full text-xs font-medium',
                    page.status === 'published'
                      ? 'bg-green-100 text-green-800'
                      : 'bg-yellow-100 text-yellow-800',
                  ]"
                >
                  {{ page.status }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm text-gray-600">
                {{ new Date(page.updatedAt).toLocaleDateString() }}
              </td>
              <td class="px-6 py-4 text-sm space-x-2">
                <router-link
                  :to="`/cms/pages/${page.id}`"
                  class="text-blue-600 hover:text-blue-800"
                >
                  Edit
                </router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useCmsStore } from '@/stores/cms';
import { PageHeader, CardContainer } from '@/components/layout';

const cmsStore = useCmsStore();
const loading = cmsStore.loading;

onMounted(() => {
  cmsStore.fetchPages();
});
</script>
