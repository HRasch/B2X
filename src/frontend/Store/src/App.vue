<template>
  <div id="app" class="min-h-screen flex flex-col">
    <!-- Simple Navbar for testing -->
    <nav class="navbar bg-base-200 shadow-lg sticky top-0 z-50">
      <div class="flex-1">
        <NuxtLink to="/" class="btn btn-ghost normal-case text-xl">{{
          $t('common.brand')
        }}</NuxtLink>
      </div>
      <div class="flex-none gap-2">
        <ul class="menu menu-horizontal px-1 gap-2">
          <li>
            <NuxtLink to="/" class="btn btn-ghost">{{ $t('navigation.home') }}</NuxtLink>
          </li>
          <li>
            <NuxtLink to="/shop" class="btn btn-ghost">{{ $t('navigation.shop') }}</NuxtLink>
          </li>
          <li>
            <ClientOnly>
              <LanguageSwitcher />
              <template #fallback>
                <span class="btn btn-ghost loading">{{ $t('ui.loading') }}</span>
              </template>
            </ClientOnly>
          </li>
        </ul>
      </div>
    </nav>

    <!-- Main content -->
    <main id="main-content" class="grow container mx-auto p-4">
      <NuxtPage />
    </main>

    <!-- Footer -->
    <footer class="footer footer-center p-4 bg-base-300 text-base-content">
      <p>© 2026 B2X Store - Test Mode</p>
    </footer>

    <!-- Debug System -->
    <ClientOnly>
      <DebugTrigger />
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import LanguageSwitcher from '~/components/common/LanguageSwitcher.vue';
import DebugTrigger from '~/components/DebugTrigger.vue';
</script>

<style scoped>
.navbar {
  background-color: #ffffff;
  border-bottom: 1px solid #e0e0e0;
  padding: 0 1rem;
}
</style>
