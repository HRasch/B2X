<script setup lang="ts">
/**
 * PageBuilderView - Route view for /pages/:id/edit
 */
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { PageBuilder } from '@/components/page-builder'
import { usePageBuilderStore } from '@/stores/pageBuilder'

const route = useRoute()
const router = useRouter()
const store = usePageBuilderStore()

const pageId = computed(() => route.params.id as string)
const isSaving = ref(false)
const isPublishing = ref(false)
const saveError = ref<string | null>(null)

// Watch for route changes
watch(pageId, (newId) => {
  if (newId) {
    store.loadPage(newId)
  }
}, { immediate: true })

async function handleSave() {
  if (isSaving.value) return
  
  isSaving.value = true
  saveError.value = null
  
  try {
    // TODO: Implement actual API call
    await new Promise(resolve => setTimeout(resolve, 500))
    console.log('Page saved:', store.page)
    // Show success notification
  } catch (error) {
    saveError.value = 'Fehler beim Speichern'
    console.error('Save error:', error)
  } finally {
    isSaving.value = false
  }
}

async function handlePublish() {
  if (isPublishing.value) return
  
  isPublishing.value = true
  saveError.value = null
  
  try {
    // TODO: Implement actual API call
    await new Promise(resolve => setTimeout(resolve, 500))
    console.log('Page published:', store.page)
    // Show success notification and redirect
    router.push({ name: 'pages-list' })
  } catch (error) {
    saveError.value = 'Fehler beim Veröffentlichen'
    console.error('Publish error:', error)
  } finally {
    isPublishing.value = false
  }
}
</script>

<template>
  <div class="page-builder-view">
    <!-- Error Banner -->
    <div v-if="saveError" class="page-builder-view__error">
      <span>{{ saveError }}</span>
      <button @click="saveError = null">&times;</button>
    </div>

    <!-- Loading State -->
    <div v-if="!pageId" class="page-builder-view__loading">
      <span>Laden...</span>
    </div>

    <!-- Page Builder -->
    <PageBuilder
      v-else
      :page-id="pageId"
      @save="handleSave"
      @publish="handlePublish"
    />
  </div>
</template>

<style scoped>
.page-builder-view {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.page-builder-view__error {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background-color: #fef2f2;
  border-bottom: 1px solid #fee2e2;
  color: #dc2626;
  font-size: 0.875rem;
}

.page-builder-view__error button {
  background: none;
  border: none;
  color: #dc2626;
  font-size: 1.25rem;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.page-builder-view__loading {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #6b7280;
  font-size: 0.875rem;
}
</style>
