<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Products</h1>
      <router-link
        to="/shop/products/new"
        class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
      >
        + New Product
      </router-link>
    </div>

    <div v-if="loading" class="text-center py-8">
      <div class="text-gray-500">Loading products...</div>
    </div>

    <div
      v-else-if="shopStore.products.length === 0"
      class="bg-white dark:bg-soft-800 rounded-lg shadow p-8 text-center"
    >
      <p class="text-gray-500 dark:text-soft-300">
        No products yet. Create your first product to get started.
      </p>
    </div>

    <div
      v-else
      class="bg-white dark:bg-soft-800 rounded-lg shadow overflow-hidden"
    >
      <table class="w-full">
        <thead
          class="bg-gray-100 dark:bg-soft-700 border-b dark:border-soft-600"
        >
          <tr>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Product Name
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              SKU
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Price
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Stock
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Actions
            </th>
          </tr>
        </thead>
        <tbody class="divide-y dark:divide-soft-600">
          <tr
            v-for="product in shopStore.products"
            :key="product.id"
            class="hover:bg-gray-50 dark:hover:bg-soft-700"
          >
            <td class="px-6 py-4 text-sm text-gray-900 dark:text-soft-100">
              {{ product.name }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-600 dark:text-soft-300">
              {{ product.sku }}
            </td>
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
                Edit
              </router-link>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useShopStore } from "@/stores/shop";

const shopStore = useShopStore();
const loading = shopStore.loading;

onMounted(() => {
  shopStore.fetchProducts();
});
</script>
