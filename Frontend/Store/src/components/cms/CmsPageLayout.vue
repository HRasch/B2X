<template>
  <!-- Loading State -->
  <div v-if="loading" class="flex items-center justify-center min-h-screen">
    <div class="loading loading-spinner loading-lg"></div>
  </div>

  <!-- Error State -->
  <div v-else-if="error" class="alert alert-error shadow-lg m-4">
    <svg
      xmlns="http://www.w3.org/2000/svg"
      class="stroke-current shrink-0 h-6 w-6"
      fill="none"
      viewBox="0 0 24 24"
    >
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2m2-2l2 2m1-2a9 9 0 11-18 0 9 9 0 0118 0z"
      />
    </svg>
    <span>{{ error }}</span>
  </div>

  <!-- Layout Rendering -->
  <div v-else-if="page" :class="`layout-${page.templateLayout}`">
    <!-- Layout: Full-Width -->
    <template v-if="page.templateLayout === 'full-width'">
      <header v-if="getRegion('header')" class="bg-base-200 shadow-lg">
        <RegionRenderer :region="getRegion('header')!" />
      </header>

      <main class="container mx-auto py-8 px-4">
        <RegionRenderer v-if="getRegion('main')" :region="getRegion('main')!" />
      </main>

      <footer v-if="getRegion('footer')" class="bg-base-200 shadow-lg mt-8">
        <RegionRenderer :region="getRegion('footer')!" />
      </footer>
    </template>

    <!-- Layout: Sidebar -->
    <template v-else-if="page.templateLayout === 'sidebar'">
      <header v-if="getRegion('header')" class="bg-base-200 shadow-lg">
        <RegionRenderer :region="getRegion('header')!" />
      </header>

      <div
        class="container mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8 py-8 px-4"
      >
        <main class="lg:col-span-2">
          <RegionRenderer
            v-if="getRegion('main')"
            :region="getRegion('main')!"
          />
        </main>

        <aside class="lg:col-span-1">
          <div class="card bg-base-200 shadow-lg">
            <div class="card-body">
              <RegionRenderer
                v-if="getRegion('sidebar')"
                :region="getRegion('sidebar')!"
              />
            </div>
          </div>
        </aside>
      </div>

      <footer v-if="getRegion('footer')" class="bg-base-200 shadow-lg mt-8">
        <RegionRenderer :region="getRegion('footer')!" />
      </footer>
    </template>

    <!-- Layout: Three-Column -->
    <template v-else-if="page.templateLayout === 'three-column'">
      <header v-if="getRegion('header')" class="bg-base-200 shadow-lg">
        <RegionRenderer :region="getRegion('header')!" />
      </header>

      <div
        class="container mx-auto grid grid-cols-1 lg:grid-cols-4 gap-4 py-8 px-4"
      >
        <aside class="lg:col-span-1">
          <div class="card bg-base-200 shadow-lg">
            <div class="card-body">
              <RegionRenderer
                v-if="getRegion('sidebar-left')"
                :region="getRegion('sidebar-left')!"
              />
            </div>
          </div>
        </aside>

        <main class="lg:col-span-2">
          <RegionRenderer
            v-if="getRegion('main')"
            :region="getRegion('main')!"
          />
        </main>

        <aside class="lg:col-span-1">
          <div class="card bg-base-200 shadow-lg">
            <div class="card-body">
              <RegionRenderer
                v-if="getRegion('sidebar-right')"
                :region="getRegion('sidebar-right')!"
              />
            </div>
          </div>
        </aside>
      </div>

      <footer v-if="getRegion('footer')" class="bg-base-200 shadow-lg mt-8">
        <RegionRenderer :region="getRegion('footer')!" />
      </footer>
    </template>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from "vue-router";
import { useCms } from "@/composables/useCms";
import RegionRenderer from "@/components/cms/RegionRenderer.vue";

const route = useRoute();
const {
  pageDefinition: page,
  loading,
  error,
  fetchPageDefinition,
  getRegion,
} = useCms();

const loadPage = async () => {
  try {
    const pagePath = (route.params.pathMatch as string) || "/";
    await fetchPageDefinition("/" + pagePath);
  } catch (err) {
    console.error("Failed to load CMS page", err);
  }
};

// Load page on mount and when route changes
await loadPage();
</script>

<style scoped>
.layout-full-width {
  width: 100%;
}

.layout-sidebar,
.layout-three-column {
  width: 100%;
}

.region-header {
  width: 100%;
}

.region-footer {
  width: 100%;
  margin-top: 4rem;
  border-top: 1px solid #e5e7eb;
  padding-top: 2rem;
}

.region-main {
  min-height: 400px;
}

.region-sidebar,
.region-sidebar-left,
.region-sidebar-right {
  display: flex;
  flex-direction: column;
}
</style>
