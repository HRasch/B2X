<template>
  <div class="space-y-6">
    <PageHeader :title="$t('shop.products.title')" :subtitle="$t('shop.products.subtitle')">
      <template #actions>
        <router-link
          to="/shop/products/new"
          class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          {{ $t('shop.products.actions.new') }}
        </router-link>
      </template>
    </PageHeader>

    <CardContainer>
      <div v-if="loading" class="text-center py-8">
        <div class="text-gray-500">{{ $t('shop.products.loading') }}</div>
      </div>

      <div
        v-else-if="shopStore.products.length === 0"
        class="bg-white dark:bg-soft-800 rounded-lg shadow p-8 text-center"
      >
        <p class="text-gray-500 dark:text-soft-300">{{ $t('shop.products.empty') }}</p>
      </div>

      <div v-else class="overflow-hidden">
        <table class="w-full">
          <thead class="bg-gray-100 dark:bg-soft-700 border-b dark:border-soft-600">
            <tr>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100">
                {{ $t('shop.products.headers.name') }}
              </th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100">
                {{ $t('shop.products.headers.sku') }}
              </th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100">
                {{ $t('shop.products.headers.price') }}
              </th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100">
                {{ $t('shop.products.headers.stock') }}
              </th>
              <th class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100">
                {{ $t('shop.products.headers.actions') }}
              </th>
            </tr>
          </thead>
          <tbody class="divide-y dark:divide-soft-600">
            <tr
              v-for="product in shopStore.products"
              :key="product.id"
              class="hover:bg-gray-50 dark:hover:bg-soft-700"
            >
              <td class="px-6 py-4 text-sm text-gray-900 dark:text-soft-100">{{ product.name }}</td>
              <td class="px-6 py-4 text-sm text-gray-600 dark:text-soft-300">{{ product.sku }}</td>
              <td class="px-6 py-4 text-sm text-gray-900 dark:text-soft-100">
                {{ product.basePrice }} {{ product.currency }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-600 dark:text-soft-300">
                {{ product.stock }}
              </td>
              <td class="px-6 py-4 text-sm space-x-2">
                <router-link
                  :to="`/shop/products/${product.id}`"
                  class="text-blue-600 hover:text-blue-800"
                >
                  {{ $t('shop.products.actions.edit') }}
                </router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useShopStore } from '@/stores/shop';

const shopStore = useShopStore();
const loading = shopStore.loading;

onMounted(() => {
  shopStore.fetchProducts();
});
</script>
