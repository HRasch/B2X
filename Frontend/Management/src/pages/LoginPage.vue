<template>
  <div class="login-page">
    <div class="login-container">
      <h2>{{ $t('auth.login.title') }}</h2>
      <p>{{ $t('auth.login.subtitle') }}</p>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">{{ $t('auth.login.email') }}</label>
          <input id="email" v-model="email" type="email" required :placeholder="$t('auth.login.emailPlaceholder')" />
        </div>

        <div class="form-group">
          <label for="password">{{ $t('auth.login.password') }}</label>
          <input id="password" v-model="password" type="password" required :placeholder="$t('auth.login.passwordPlaceholder')" />
        </div>

        <button type="submit" :disabled="loading" class="btn-submit">
          {{ loading ? $t('auth.login.signingIn') : $t('auth.login.submit') }}
        </button>

        <div v-if="error" class="error-message">
          {{ error }}
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import apiClient from '@/services/api';

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const loading = ref(false);
const error = ref('');

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  try {
    const response = await apiClient.post('/auth/login', {
      email: email.value,
      password: password.value,
    });

    const { token, userId, email: userEmail } = response.data.data;
    authStore.setAuth(token, userId, userEmail);
    router.push('/dashboard');
  } catch (err: unknown) {
    const maybe = err as { response?: { data?: { message?: string } } } | undefined;
    error.value = maybe?.response?.data?.message ?? 'Login failed. Please try again.';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-container {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
  color: #333; /* Explicit text color for container */
}

.login-container h2 {
  color: #333 !important; /* Force dark text */
  margin-bottom: 0.5rem;
}

.login-container p {
  color: #666 !important; /* Force dark gray text */
  margin-bottom: 2rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  color: #333 !important; /* Force dark text for labels */
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s;
  color: #333; /* Ensure input text is dark */
  background-color: white; /* Ensure input background is white */
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.btn-submit {
  width: 100%;
  padding: 0.75rem;
  background-color: #667eea;
  color: white !important; /* Force white text on button */
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-submit:hover:not(:disabled) {
  background-color: #5568d3;
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  margin-top: 1rem;
  padding: 0.75rem;
  background-color: #fee;
  color: #c33 !important; /* Force red text for errors */
  border-radius: 4px;
  font-size: 0.9rem;
}
</style>
