<template>
  <div
    class="min-h-screen bg-gradient-soft-blue flex items-center justify-center p-safe"
  >
    <soft-card class="w-full max-w-md">
      <!-- Header -->
      <div class="text-center mb-safe">
        <div
          class="w-16 h-16 rounded-soft-lg bg-gradient-soft-blue flex items-center justify-center mx-auto mb-4"
        >
          <span class="text-white font-bold text-2xl">B</span>
        </div>
        <h1 class="heading-md text-soft-900">B2Connect Admin</h1>
        <p class="text-soft-600 text-sm mt-2">Sign in to your account</p>
      </div>

      <!-- Error Message -->
      <div
        v-if="error"
        class="mb-4 p-4 bg-danger-50 border border-danger-200 rounded-soft text-danger-700 text-sm"
      >
        {{ error }}
      </div>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="space-y-4">
        <!-- Email -->
        <soft-input
          v-model="email"
          type="email"
          label="Email Address"
          placeholder="admin@example.com"
          required
        />

        <!-- Password -->
        <soft-input
          v-model="password"
          type="password"
          label="Password"
          placeholder="••••••••"
          required
        />

        <!-- Remember Me -->
        <label class="flex items-center gap-2 cursor-pointer">
          <input
            v-model="rememberMe"
            type="checkbox"
            class="w-4 h-4 rounded-soft border-soft-200 text-primary-600 focus:ring-primary-500"
          />
          <span class="text-sm text-soft-700">Remember me</span>
        </label>

        <!-- Login Button -->
        <soft-button
          variant="primary"
          size="lg"
          :loading="loading"
          class="w-full mt-6"
        >
          {{ loading ? "Signing in..." : "Sign In" }}
        </soft-button>
      </form>

      <!-- Footer -->
      <div class="text-center mt-safe pt-safe border-t border-soft-100">
        <p class="text-sm text-soft-600">
          Demo Account: <br />
          <span class="font-mono text-soft-900">admin@example.com</span><br />
          <span class="font-mono text-soft-900">password</span>
        </p>
      </div>
    </soft-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import SoftCard from "@/components/soft-ui/SoftCard.vue";
import SoftButton from "@/components/soft-ui/SoftButton.vue";
import SoftInput from "@/components/soft-ui/SoftInput.vue";

const email = ref("admin@example.com");
const password = ref("password");
const rememberMe = ref(false);
const loading = ref(false);
const error = ref("");

const authStore = useAuthStore();
const router = useRouter();

const handleLogin = async () => {
  error.value = "";
  loading.value = true;

  try {
    await authStore.login(email.value, password.value, rememberMe.value);
    router.push("/dashboard");
  } catch (err: any) {
    error.value =
      err.response?.data?.error?.message || "Login failed. Please try again.";
    console.error("Login error:", err);
  } finally {
    loading.value = false;
  }
};
</script>
