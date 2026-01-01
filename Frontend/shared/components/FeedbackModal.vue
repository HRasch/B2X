<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen" class="feedback-modal-overlay" @click="close">
        <div class="feedback-modal" @click.stop>
          <div class="feedback-modal-header">
            <h2>Fragen & Hinweise</h2>
            <button class="close-button" @click="close" aria-label="Schließen">
              ×
            </button>
          </div>

          <form @submit.prevent="submitFeedback" class="feedback-form">
            <div class="form-group">
              <label for="category">Kategorie *</label>
              <select
                id="category"
                v-model="formData.category"
                required
                class="form-select"
              >
                <option value="" disabled>Bitte wählen...</option>
                <option value="question">Frage</option>
                <option value="bug">Problem</option>
                <option value="enhancement">Verbesserungsvorschlag</option>
                <option value="other">Sonstiges</option>
              </select>
            </div>

            <div class="form-group">
              <label for="description">Beschreibung *</label>
              <textarea
                id="description"
                v-model="formData.description"
                required
                maxlength="1000"
                placeholder="Beschreiben Sie Ihr Anliegen so detailliert wie möglich..."
                class="form-textarea"
                :class="{ 'error': descriptionError }"
              />
              <div class="char-count">
                {{ formData.description.length }}/1000 Zeichen
              </div>
              <div v-if="descriptionError" class="error-message">
                {{ descriptionError }}
              </div>
            </div>

            <div class="form-group">
              <label>Dateien anhängen (optional)</label>
              <div class="file-upload">
                <input
                  ref="fileInput"
                  type="file"
                  multiple
                  accept="image/*,.log,.txt"
                  @change="handleFileSelect"
                  class="file-input"
                />
                <button
                  type="button"
                  @click="$refs.fileInput.click()"
                  class="file-button"
                >
                  Dateien auswählen
                </button>
                <span class="file-info">
                  Max. 3 Dateien, 5MB pro Datei
                </span>
              </div>

              <div v-if="selectedFiles.length > 0" class="file-list">
                <div
                  v-for="(file, index) in selectedFiles"
                  :key="index"
                  class="file-item"
                >
                  <span class="file-name">{{ file.name }}</span>
                  <span class="file-size">({{ formatFileSize(file.size) }})</span>
                  <button
                    type="button"
                    @click="removeFile(index)"
                    class="remove-file"
                    aria-label="Datei entfernen"
                  >
                    ×
                  </button>
                </div>
              </div>
            </div>

            <div class="privacy-notice">
              <h4>Datenschutzhinweis</h4>
              <p>
                Ihre Daten werden automatisch anonymisiert verarbeitet, um Ihre Privatsphäre zu schützen.
                Es werden keine personenbezogenen Informationen gespeichert.
              </p>
            </div>

            <div class="form-actions">
              <button
                type="button"
                @click="close"
                class="cancel-button"
                :disabled="isSubmitting"
              >
                Abbrechen
              </button>
              <button
                type="submit"
                class="submit-button"
                :disabled="isSubmitting || !isFormValid"
              >
                {{ isSubmitting ? 'Wird gesendet...' : 'Senden' }}
              </button>
            </div>
          </form>

          <div v-if="submitResult" class="submit-result">
            <div class="success-message">
              <h3>✓ Vielen Dank!</h3>
              <p>{{ submitResult.message }}</p>
              <p v-if="submitResult.correlationId" class="correlation-id">
                Korrelations-ID: <code>{{ submitResult.correlationId }}</code>
              </p>
              <p class="note">
                Sie können diese ID verwenden, um den Status Ihrer Anfrage zu verfolgen.
              </p>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useContextCollector, type CollectedContext } from '../composables/useContextCollector'

interface Props {
  isOpen: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'submitted', result: SubmitResult): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { collect } = useContextCollector()

const formData = ref({
  category: '',
  description: ''
})

const selectedFiles = ref<File[]>([])
const fileInput = ref<HTMLInputElement>()
const isSubmitting = ref(false)
const submitResult = ref<SubmitResult | null>(null)

interface SubmitResult {
  correlationId: string
  issueUrl: string
  status: string
  message: string
}

const descriptionError = computed(() => {
  if (formData.value.description.length < 10) {
    return 'Beschreibung muss mindestens 10 Zeichen lang sein'
  }
  if (formData.value.description.length > 1000) {
    return 'Beschreibung darf maximal 1000 Zeichen lang sein'
  }
  return ''
})

const isFormValid = computed(() => {
  return formData.value.category &&
         formData.value.description.length >= 10 &&
         formData.value.description.length <= 1000 &&
         !isSubmitting.value
})

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const files = Array.from(target.files || [])

  // Validate files
  const validFiles = files.filter(file => {
    if (file.size > 5 * 1024 * 1024) { // 5MB
      alert(`Datei ${file.name} ist zu groß (max. 5MB)`)
      return false
    }
    if (!['image/jpeg', 'image/png', 'image/gif', 'text/plain', 'application/json'].includes(file.type) &&
        !file.name.endsWith('.log') && !file.name.endsWith('.txt')) {
      alert(`Dateityp von ${file.name} wird nicht unterstützt`)
      return false
    }
    return true
  })

  // Limit to 3 files total
  const totalFiles = selectedFiles.value.length + validFiles.length
  if (totalFiles > 3) {
    alert('Maximal 3 Dateien erlaubt')
    return
  }

  selectedFiles.value.push(...validFiles)
}

const removeFile = (index: number) => {
  selectedFiles.value.splice(index, 1)
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const submitFeedback = async () => {
  if (!isFormValid.value) return

  isSubmitting.value = true
  submitResult.value = null

  try {
    // Collect context data
    const context: CollectedContext = await collect()

    // Prepare attachments
    const attachments = await Promise.all(
      selectedFiles.value.map(async (file) => ({
        fileName: file.name,
        contentType: file.type,
        content: Array.from(new Uint8Array(await file.arrayBuffer()))
      }))
    )

    // Prepare request
    const request = {
      category: formData.value.category,
      description: formData.value.description,
      attachments,
      context
    }

    // Submit feedback
    const response = await fetch('/api/support/feedback', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`)
    }

    const result: SubmitResult = await response.json()
    submitResult.value = result

    // Emit success event
    emit('submitted', result)

    // Reset form after short delay
    setTimeout(() => {
      resetForm()
    }, 3000)

  } catch (error) {
    console.error('Failed to submit feedback:', error)
    submitResult.value = {
      correlationId: '',
      issueUrl: '',
      status: 'error',
      message: 'Fehler beim Senden. Bitte versuchen Sie es später erneut.'
    }
  } finally {
    isSubmitting.value = false
  }
}

const resetForm = () => {
  formData.value = { category: '', description: '' }
  selectedFiles.value = []
  submitResult.value = null
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

const close = () => {
  if (!isSubmitting.value) {
    resetForm()
    emit('close')
  }
}

// Reset form when modal opens
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    resetForm()
  }
})
</script>

<style scoped>
.feedback-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.feedback-modal {
  background: white;
  border-radius: 8px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  max-width: 600px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
}

.feedback-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.feedback-modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #111827;
}

.close-button {
  background: none;
  border: none;
  font-size: 2rem;
  cursor: pointer;
  color: #6b7280;
  padding: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.close-button:hover {
  background-color: #f3f4f6;
  color: #374151;
}

.feedback-form {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.form-select,
.form-textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-textarea {
  min-height: 120px;
  resize: vertical;
}

.form-textarea.error {
  border-color: #ef4444;
}

.char-count {
  text-align: right;
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.error-message {
  color: #ef4444;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.file-upload {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.file-input {
  display: none;
}

.file-button {
  padding: 0.5rem 1rem;
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.875rem;
  transition: background-color 0.2s;
}

.file-button:hover {
  background: #e5e7eb;
}

.file-info {
  font-size: 0.875rem;
  color: #6b7280;
}

.file-list {
  margin-top: 1rem;
}

.file-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  background: #f9fafb;
  border-radius: 4px;
  margin-bottom: 0.5rem;
}

.file-name {
  flex: 1;
  font-size: 0.875rem;
}

.file-size {
  font-size: 0.875rem;
  color: #6b7280;
}

.remove-file {
  background: none;
  border: none;
  color: #ef4444;
  cursor: pointer;
  font-size: 1.25rem;
  padding: 0;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 2px;
  transition: background-color 0.2s;
}

.remove-file:hover {
  background: #fee2e2;
}

.privacy-notice {
  background: #f0f9ff;
  border: 1px solid #bae6fd;
  border-radius: 6px;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

.privacy-notice h4 {
  margin: 0 0 0.5rem 0;
  color: #0c4a6e;
  font-size: 1rem;
}

.privacy-notice p {
  margin: 0;
  font-size: 0.875rem;
  color: #0369a1;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.cancel-button,
.submit-button {
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.cancel-button {
  background: #f3f4f6;
  color: #374151;
}

.cancel-button:hover:not(:disabled) {
  background: #e5e7eb;
}

.submit-button {
  background: #3b82f6;
  color: white;
}

.submit-button:hover:not(:disabled) {
  background: #2563eb;
}

.submit-button:disabled,
.cancel-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.submit-result {
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.success-message h3 {
  margin: 0 0 1rem 0;
  color: #059669;
  font-size: 1.25rem;
}

.success-message p {
  margin: 0 0 0.5rem 0;
  color: #374151;
}

.correlation-id {
  background: #f3f4f6;
  padding: 0.5rem;
  border-radius: 4px;
  font-family: monospace;
  margin: 0.5rem 0;
}

.note {
  font-size: 0.875rem;
  color: #6b7280;
  font-style: italic;
}

/* Modal transitions */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .feedback-modal,
.modal-leave-active .feedback-modal {
  transition: transform 0.3s ease;
}

.modal-enter-from .feedback-modal,
.modal-leave-to .feedback-modal {
  transform: scale(0.95);
}
</style></content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/frontend/shared/components/FeedbackModal.vue