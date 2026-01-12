<template>
  <div class="login-container">
    <div class="login-box">
      <h2>{{ t('auth.login.title') }}</h2>

      <!-- E2E Test Mode Notice (only visible during E2E tests) -->
      <div v-if="isE2EMode" class="dev-notice">
        <p>
          🧪 <strong>{{ t('auth.login.e2eMode.title') }}</strong>
        </p>
        <p class="text-sm">{{ t('auth.login.e2eMode.description') }}</p>
      </div>

      <!-- Development Help (seeded credentials) -->
      <div v-if="isDev && !isE2EMode" class="dev-help">
        <p class="text-sm text-muted">
          {{
            t('auth.login.devHelp.hint', {
              email: t('auth.login.devHelp.email'),
              password: t('auth.login.devHelp.password'),
            })
          }}
        </p>
      </div>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">{{ t('auth.login.form.email.label') }}</label>
          <input
            id="email"
            v-model="email"
            type="email"
            :placeholder="t('auth.login.form.email.placeholder')"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">{{ t('auth.login.form.password.label') }}</label>
          <input
            id="password"
            v-model="password"
            type="password"
            :placeholder="t('auth.login.form.password.placeholder')"
            required
          />
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>

        <button type="submit" class="btn btn-primary" :disabled="loading">
          {{ loading ? t('auth.login.actions.loggingIn') : t('auth.login.actions.login') }}
        </button>
      </form>

      <p class="signup-link">
        {{ t('auth.login.signup.prompt') }}
        <router-link to="/register/private">{{ t('auth.login.signup.link') }}</router-link>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '../stores/auth';

const { t } = useI18n();
const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const error = ref('');
const loading = ref(false);

// Environment detection
const isDev = import.meta.env.DEV;
// E2E mode is ONLY enabled via environment variable (set during E2E test runs)
const isE2EMode = import.meta.env.VITE_E2E_TEST === 'true';

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  // E2E Test mode bypass (ONLY during E2E tests, NOT general development)
  if (isE2EMode) {
    // Mock successful login for E2E tests
    authStore.accessToken = `e2e-token-${Date.now()}`;
    authStore.tenantId = '00000000-0000-0000-0000-000000000001';
    authStore.setUser({
      id: 'e2e-user-id',
      tenantId: '00000000-0000-0000-0000-000000000001',
      email: email.value || 'e2e@example.com',
      firstName: 'E2E',
      lastName: 'Test',
      status: 'active',
      lastLoginAt: new Date(),
      emailConfirmed: true,
    });

    localStorage.setItem('access_token', authStore.accessToken);
    localStorage.setItem('tenant_id', authStore.tenantId);

    router.push('/dashboard');
    loading.value = false;
    return;
  }

  // Normal login flow (uses backend with seeded InMemory database)
  const success = await authStore.login(email.value, password.value);

  if (success) {
    router.push('/dashboard');
  } else {
    error.value = 'Login failed. Please check your credentials.';
  }

  loading.value = false;
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}

.login-box {
  width: 100%;
  max-width: 400px;
  padding: 2rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

h2 {
  text-align: center;
  margin-bottom: 2rem;
  color: #333;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

input:focus {
  outline: none;
  border-color: #007bff;
}

.error-message {
  color: #dc3545;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background-color: #f8d7da;
  border-radius: 4px;
}

.btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.signup-link {
  text-align: center;
  margin-top: 1.5rem;
  color: #666;
}

.signup-link a {
  color: #007bff;
  text-decoration: none;
}

.signup-link a:hover {
  text-decoration: underline;
}

.dev-notice {
  background-color: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 4px;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

.dev-notice p {
  margin: 0;
  color: #856404;
}

.dev-notice .text-sm {
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.dev-help {
  background-color: #e8f4f8;
  border: 1px solid #bee5eb;
  border-radius: 4px;
  padding: 0.75rem;
  margin-bottom: 1rem;
}

.dev-help p {
  margin: 0;
  color: #0c5460;
}

.dev-help code {
  background-color: #fff;
  padding: 0.125rem 0.25rem;
  border-radius: 3px;
  font-family: monospace;
  font-size: 0.85em;
}

.text-sm {
  font-size: 0.875rem;
}

.text-muted {
  color: #6c757d;
}

.dev-toggle {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e0e0e0;
  text-align: center;
}

.dev-toggle label {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: #666;
  cursor: pointer;
}

.dev-toggle input[type='checkbox'] {
  width: auto;
  cursor: pointer;
}
</style>
