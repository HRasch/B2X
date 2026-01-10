<template>
  <div
    class="min-h-screen bg-gradient-to-br from-primary/10 to-secondary/10 flex items-center justify-center p-4"
  >
    <div class="card bg-base-100 shadow-xl w-full max-w-md">
      <div class="card-body">
        <!-- Header -->
        <div class="text-center mb-6">
          <div
            class="w-16 h-16 rounded-lg bg-primary flex items-center justify-center mx-auto mb-4"
          >
            <span class="text-primary-content font-bold text-2xl">B</span>
          </div>
          <h1 class="text-2xl font-bold text-base-content">{{ $t('auth.login.title') }}</h1>
          <p class="text-base-content/60 text-sm mt-2">{{ $t('auth.login.subtitle') }}</p>
        </div>

        <!-- Error Message -->
        <div v-if="error" class="alert alert-error mb-4">
          <svg class="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <span>{{ error }}</span>
        </div>

        <!-- Login Form -->
        <form @submit.prevent="handleLogin" class="space-y-4">
          <div class="form-control">
            <label class="label">
              <span class="label-text">{{ $t('auth.login.email') }}</span>
            </label>
            <input
              v-model="email"
              type="email"
              :placeholder="$t('auth.login.emailPlaceholder')"
              class="input input-bordered w-full"
              required
            />
          </div>

          <!-- Password -->
          <div class="form-control">
            <label class="label">
              <span class="label-text">{{ $t('auth.login.password') }}</span>
            </label>
            <input
              v-model="password"
              type="password"
              :placeholder="$t('auth.login.passwordPlaceholder')"
              class="input input-bordered w-full"
              required
            />
          </div>

          <!-- Remember Me -->
          <div class="form-control">
            <label class="label cursor-pointer justify-start gap-2">
              <input
                v-model="rememberMe"
                type="checkbox"
                class="checkbox checkbox-primary checkbox-sm"
              />
              <span class="label-text">{{ $t('auth.login.rememberMe') }}</span>
            </label>
          </div>

          <!-- Login Button -->
          <button type="submit" class="btn btn-primary w-full mt-6" :disabled="loading">
            <span v-if="loading" class="loading loading-spinner loading-sm"></span>
            {{ loading ? $t('auth.login.signingIn') : $t('auth.login.submit') }}
          </button>
        </form>

        <!-- Footer -->
        <div class="divider mt-6"></div>
        <div class="text-center">
          <p class="text-sm text-base-content/60">{{ $t('auth.login.demoAccount') }}</p>
          <p class="font-mono text-sm mt-1">{{ $t('auth.login.demoCredentials') }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

// Default tenant GUID
const DEFAULT_TENANT_ID =
  import.meta.env.VITE_DEFAULT_TENANT_ID || '00000000-0000-0000-0000-000000000001';

const email = ref('admin@example.com');
const password = ref('password');
const rememberMe = ref(false);
const loading = ref(false);
const error = ref('');

const authStore = useAuthStore();
const router = useRouter();

// Ensure tenant ID is set when login page loads
onMounted(() => {
  if (!localStorage.getItem('tenantId')) {
    localStorage.setItem('tenantId', DEFAULT_TENANT_ID);
  }
});

const handleLogin = async () => {
  error.value = '';
  loading.value = true;

  try {
    await authStore.login(email.value, password.value, rememberMe.value);
    router.push('/dashboard');
  } catch (err: unknown) {
    const errorMessage = err instanceof Error ? err.message : 'Login failed. Please try again.';
    // Check for API error response structure
    const apiError = err as {
      response?: { data?: { error?: { message?: string } } };
    };
    error.value = apiError.response?.data?.error?.message || errorMessage;
    console.error('Login error:', err);
  } finally {
    loading.value = false;
  }
};
</script>
