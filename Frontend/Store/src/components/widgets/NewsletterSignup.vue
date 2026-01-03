<template>
  <section class="newsletter-widget py-8">
    <div class="bg-gray-100 rounded p-8 max-w-md mx-auto">
      <h3 class="text-2xl font-bold mb-4">{{ settings.heading }}</h3>
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <input
          v-model="email"
          type="email"
          :placeholder="settings.placeholder"
          required
          class="w-full px-4 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <button
          type="submit"
          class="w-full px-4 py-2 bg-blue-600 text-white font-semibold rounded hover:bg-blue-700 transition"
        >
          {{ settings.buttonText || 'Subscribe' }}
        </button>
      </form>
      <p
        v-if="message"
        :class="['mt-4 text-sm', messageType === 'success' ? 'text-green-600' : 'text-red-600']"
      >
        {{ message }}
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';

interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

defineProps<Props>();

const email = ref('');
const message = ref('');
const messageType = ref<'success' | 'error'>('success');

const handleSubmit = async () => {
  try {
    // TODO: Implement newsletter subscription API call
    message.value = 'Thank you for subscribing!';
    messageType.value = 'success';
    email.value = '';
  } catch (error) {
    message.value = 'Something went wrong. Please try again.';
    messageType.value = 'error';
  }
};
</script>

<style scoped>
.newsletter-widget {
  width: 100%;
}
</style>
