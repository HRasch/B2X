<template>
  <div class="newsletter-signup" :style="containerStyles">
    <div class="newsletter-content">
      <h2 v-if="settings.title" class="newsletter-title">
        {{ settings.title }}
      </h2>
      <p v-if="settings.description" class="newsletter-description">
        {{ settings.description }}
      </p>

      <form @submit.prevent="handleSubmit" class="newsletter-form">
        <div class="form-group">
          <input
            v-model="email"
            type="email"
            placeholder="Enter your email address"
            class="email-input"
            required
          />
          <button type="submit" class="submit-button" :disabled="loading">
            {{ loading ? 'Subscribing...' : settings.buttonText || 'Subscribe' }}
          </button>
        </div>
        <div v-if="successMessage" class="success-message">
          {{ successMessage }}
        </div>
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

interface Props {
  settings?: {
    title?: string;
    description?: string;
    buttonText?: string;
    backgroundColor?: string;
    textColor?: string;
  };
  widgetId?: string;
}

const props = withDefaults(defineProps<Props>(), {
  settings: () => ({}),
});

const email = ref('');
const loading = ref(false);
const successMessage = ref('');
const errorMessage = ref('');

const containerStyles = computed(() => ({
  backgroundColor: props.settings.backgroundColor || '#f8f9fa',
  color: props.settings.textColor || '#000',
  padding: '2rem 1rem',
  borderRadius: '8px',
}));

const handleSubmit = async () => {
  loading.value = true;
  successMessage.value = '';
  errorMessage.value = '';

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000));
    successMessage.value = 'Thank you for subscribing!';
    email.value = '';
  } catch {
    errorMessage.value = 'Failed to subscribe. Please try again.';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.newsletter-signup {
  width: 100%;
  transition: background-color 0.3s;
}

.newsletter-content {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
}

.newsletter-title {
  font-size: 1.75rem;
  font-weight: bold;
  margin: 0 0 1rem 0;
}

.newsletter-description {
  font-size: 1rem;
  line-height: 1.6;
  margin: 0 0 1.5rem 0;
}

.newsletter-form {
  width: 100%;
}

.form-group {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.email-input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.email-input:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

.submit-button {
  padding: 0.75rem 2rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.submit-button:hover:not(:disabled) {
  background-color: #0056b3;
}

.submit-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.success-message {
  color: #28a745;
  font-size: 0.95rem;
  margin-top: 1rem;
}

.error-message {
  color: #dc3545;
  font-size: 0.95rem;
  margin-top: 1rem;
}
</style>
