<template>
  <div class="min-h-screen">
    <!-- Hero Section -->
    <section
      class="hero min-h-[80vh] bg-gradient-to-br from-primary to-primary-focus text-primary-content"
    >
      <div class="hero-content text-center">
        <div class="max-w-4xl">
          <h1 class="text-5xl md:text-6xl font-bold mb-6 leading-tight">
            Welcome to B2Connect
          </h1>
          <p class="text-xl md:text-2xl mb-8 opacity-90 leading-relaxed">
            A modern multitenant SaaS platform for seamless business
            connectivity
          </p>

          <div
            class="flex flex-col sm:flex-row gap-4 justify-center items-center"
          >
            <BaseButton
              v-if="!isAuthenticated"
              variant="secondary"
              size="lg"
              to="/login"
              class="btn-wide"
            >
              Get Started
              <svg class="w-5 h-5 ml-2" fill="currentColor" viewBox="0 0 20 20">
                <path
                  fill-rule="evenodd"
                  d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z"
                  clip-rule="evenodd"
                />
              </svg>
            </BaseButton>

            <BaseButton
              v-else
              variant="outline"
              size="lg"
              to="/dashboard"
              class="btn-wide"
            >
              Go to Dashboard
            </BaseButton>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="py-20 px-4 bg-base-100">
      <div class="max-w-7xl mx-auto">
        <div class="text-center mb-16">
          <h2 class="text-4xl font-bold text-base-content mb-4">
            Why Choose B2Connect?
          </h2>
          <p class="text-xl text-base-content/70 max-w-2xl mx-auto">
            Experience the power of modern SaaS architecture designed for
            businesses that demand excellence
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <BaseCard variant="elevated" hover class="group">
            <div class="text-center p-6">
              <div
                class="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4 group-hover:bg-primary/20 transition-colors"
              >
                <svg
                  class="w-8 h-8 text-primary"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    fill-rule="evenodd"
                    d="M4 4a2 2 0 00-2 2v8a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2H4zm3 2a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-base-content mb-3">
                Multitenant Architecture
              </h3>
              <p class="text-base-content/70 leading-relaxed">
                Secure data isolation for multiple organizations on a single
                platform with enterprise-grade security.
              </p>
            </div>
          </BaseCard>

          <BaseCard variant="elevated" hover class="group">
            <div class="text-center p-6">
              <div
                class="w-16 h-16 bg-secondary/10 rounded-full flex items-center justify-center mx-auto mb-4 group-hover:bg-secondary/20 transition-colors"
              >
                <svg
                  class="w-8 h-8 text-secondary"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    fill-rule="evenodd"
                    d="M3 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-base-content mb-3">
                Microservices
              </h3>
              <p class="text-base-content/70 leading-relaxed">
                Scalable microservices designed for high performance,
                reliability, and easy maintenance.
              </p>
            </div>
          </BaseCard>

          <BaseCard variant="elevated" hover class="group">
            <div class="text-center p-6">
              <div
                class="w-16 h-16 bg-accent/10 rounded-full flex items-center justify-center mx-auto mb-4 group-hover:bg-accent/20 transition-colors"
              >
                <svg
                  class="w-8 h-8 text-accent"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-8.293l-3-3a1 1 0 00-1.414 0l-3 3a1 1 0 001.414 1.414L9 9.414V13a1 1 0 102 0V9.414l1.293 1.293a1 1 0 001.414-1.414z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <h3 class="text-xl font-semibold text-base-content mb-3">
                Real-time Updates
              </h3>
              <p class="text-base-content/70 leading-relaxed">
                Event-driven architecture with Wolverine for instant data
                synchronization across all services.
              </p>
            </div>
          </BaseCard>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="py-20 px-4 bg-base-200">
      <div class="max-w-4xl mx-auto text-center">
        <h2 class="text-4xl font-bold text-base-content mb-6">
          Ready to Get Started?
        </h2>
        <p class="text-xl text-base-content/70 mb-8">
          Join thousands of businesses already using B2Connect to streamline
          their operations
        </p>

        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <BaseButton variant="primary" size="lg" to="/shop" class="btn-wide">
            Browse Products
          </BaseButton>

          <BaseButton
            variant="outline"
            size="lg"
            to="/tenants"
            class="btn-wide"
          >
            View Tenants
          </BaseButton>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useAuthStore } from "../stores/auth";
import BaseButton from "@/components/ui/BaseButton.vue";
import BaseCard from "@/components/ui/BaseCard.vue";

const authStore = useAuthStore();
const isAuthenticated = computed(() => authStore.isAuthenticated);
</script>

<style scoped>
/* Hero section customizations */
.hero {
  background-attachment: fixed;
}

@media (max-width: 768px) {
  .hero {
    background-attachment: scroll;
  }
}

/* Card hover effects */
.group:hover .bg-primary\/10 {
  @apply bg-primary/20;
}

.group:hover .bg-secondary\/10 {
  @apply bg-secondary/20;
}

.group:hover .bg-accent\/10 {
  @apply bg-accent/20;
}
</style>
