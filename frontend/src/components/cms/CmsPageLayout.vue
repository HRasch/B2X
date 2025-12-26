<template>
  <div v-if="loading" class="text-center py-12">
    <LoadingSpinner />
  </div>

  <div v-else-if="error" class="bg-red-50 border border-red-200 rounded p-4">
    <p class="text-red-700">{{ error }}</p>
  </div>

  <div v-else-if="page" :class="`layout-${page.templateLayout}`">
    <!-- Layout: Full-Width -->
    <template v-if="page.templateLayout === 'full-width'">
      <div class="region-header">
        <RegionRenderer
          v-if="getRegion('header')"
          :region="getRegion('header')!"
        />
      </div>

      <main class="region-main">
        <RegionRenderer
          v-if="getRegion('main')"
          :region="getRegion('main')!"
        />
      </main>

      <div class="region-footer">
        <RegionRenderer
          v-if="getRegion('footer')"
          :region="getRegion('footer')!"
        />
      </div>
    </template>

    <!-- Layout: Sidebar -->
    <template v-else-if="page.templateLayout === 'sidebar'">
      <div class="region-header">
        <RegionRenderer
          v-if="getRegion('header')"
          :region="getRegion('header')!"
        />
      </div>

      <div class="container mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8 my-8">
        <main class="lg:col-span-2 region-main">
          <RegionRenderer
            v-if="getRegion('main')"
            :region="getRegion('main')!"
          />
        </main>

        <aside class="region-sidebar">
          <RegionRenderer
            v-if="getRegion('sidebar')"
            :region="getRegion('sidebar')!"
          />
        </aside>
      </div>

      <div class="region-footer">
        <RegionRenderer
          v-if="getRegion('footer')"
          :region="getRegion('footer')!"
        />
      </div>
    </template>

    <!-- Layout: Three-Column -->
    <template v-else-if="page.templateLayout === 'three-column'">
      <div class="region-header">
        <RegionRenderer
          v-if="getRegion('header')"
          :region="getRegion('header')!"
        />
      </div>

      <div class="container mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8 my-8">
        <aside class="region-sidebar-left">
          <RegionRenderer
            v-if="getRegion('sidebar-left')"
            :region="getRegion('sidebar-left')!"
          />
        </aside>

        <main class="region-main">
          <RegionRenderer
            v-if="getRegion('main')"
            :region="getRegion('main')!"
          />
        </main>

        <aside class="region-sidebar-right">
          <RegionRenderer
            v-if="getRegion('sidebar-right')"
            :region="getRegion('sidebar-right')!"
          />
        </aside>
      </div>

      <div class="region-footer">
        <RegionRenderer
          v-if="getRegion('footer')"
          :region="getRegion('footer')!"
        />
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';
import { useCms } from '@/composables/useCms';
import RegionRenderer from '@/components/cms/RegionRenderer.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

const route = useRoute();
const { pageDefinition: page, loading, error, fetchPageDefinition, getRegion } = useCms();

const loadPage = async () => {
  try {
    const pagePath = (route.params.pathMatch as string) || '/';
    await fetchPageDefinition('/' + pagePath);
  } catch (err) {
    console.error('Failed to load CMS page', err);
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
