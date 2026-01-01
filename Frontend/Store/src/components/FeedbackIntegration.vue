// Beispiel-Integration in Store Frontend
// frontend/Store/src/components/App.vue oder ähnliche Hauptkomponente

<template>
  <div id="app">
    <!-- Bestehende App-Struktur -->

    <!-- Feedback Button (z.B. in Footer oder Header) -->
    <button
      @click="showFeedback = true"
      class="feedback-trigger"
      aria-label="Feedback geben"
    >
      💬 Feedback
    </button>

    <!-- Feedback Modal -->
    <FeedbackModal
      :is-open="showFeedback"
      @close="showFeedback = false"
      @submitted="onFeedbackSubmitted"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import FeedbackModal from '../../../shared/components/FeedbackModal.vue'

const showFeedback = ref(false)

const onFeedbackSubmitted = (result: any) => {
  console.log('Feedback submitted:', result)
  // Optional: Show success toast, track analytics, etc.

  // Auto-close after 3 seconds
  setTimeout(() => {
    showFeedback.value = false
  }, 3000)
}
</script>

<style scoped>
.feedback-trigger {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  background: #3b82f6;
  color: white;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: all 0.2s;
  z-index: 100;
}

.feedback-trigger:hover {
  transform: scale(1.05);
  background: #2563eb;
}
</style></content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/frontend/Store/src/components/FeedbackIntegration.vue