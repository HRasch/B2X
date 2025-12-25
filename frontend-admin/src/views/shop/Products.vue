<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-3xl font-bold text-gray-900">Products</h1>
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

    <div v-else-if="shopStore.products.length === 0" class="bg-white rounded-lg shadow p-8 text-center">
      <p class="text-gray-500">No products yet. Create your first product to get started.</p>
    </div>

    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-100 border-b">
          <tr>
            <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Product Name</th>
            <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">SKU</th>
            <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Price</th>
            <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Stock</th>
            <th class="px-6 py-3 text-left text-sm font-medium text-gray-900">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y">
          <tr v-for="product in shopStore.products" :key="product.id">
            <td class="px-6 py-4 text-sm text-gray-900">{{ product.name }}</td>
            <td class="px-6 py-4 text-sm text-gray-600">{{ product.sku }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ product.basePrice }} {{ product.currency }}</td>
            <td class="px-6 py-4 text-sm text-gray-600">{{ product.stock }}</td>
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
import { onMounted } from 'vue'
import { useShopStore } from '@/stores/shop'

const shopStore = useShopStore()
const loading = shopStore.loading

onMounted(() => {
  shopStore.fetchProducts()
})
</script>
