<template>
  <div class="space-y-6">
    <router-link to="/cms/pages" class="text-blue-600 hover:text-blue-800">
      ← Back to Pages
    </router-link>

    <div>
      <h1 class="text-3xl font-bold text-gray-900">Page Editor</h1>
      <p class="text-gray-600 mt-2">Coming soon...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useCmsStore } from '@/stores/cms'

const route = useRoute()
const cmsStore = useCmsStore()
const pageId = route.params.id as string

onMounted(() => {
  if (pageId && pageId !== 'new') {
    cmsStore.fetchPage(pageId)
  }
})
</script>
