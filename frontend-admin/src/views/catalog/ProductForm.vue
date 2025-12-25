<template>
  <div class="product-form">
    <div class="header">
      <h1>{{ isEditMode ? "Edit Product" : "Create Product" }}</h1>
      <p class="subtitle">Manage product details and inventory</p>
    </div>

    <div class="form-container">
      <!-- Form would be implemented here -->
      <div class="placeholder-message">
        <p>Product form implementation coming soon...</p>
        <router-link to="/catalog/products" class="btn btn-secondary">
          Back to Products
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from "vue-router";
import { computed } from "vue";

const route = useRoute();
const isEditMode = computed(() => route.meta.formMode === "edit");
</script>

<style scoped lang="css">
.product-form {
  padding: 2rem;
}

.header {
  margin-bottom: 2rem;
}

.header h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  color: #1f2937;
}

.subtitle {
  margin: 0;
  color: #6b7280;
  font-size: 0.9rem;
}

.form-container {
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  padding: 2rem;
}

.placeholder-message {
  text-align: center;
  color: #6b7280;
  padding: 3rem;
}

.btn {
  display: inline-block;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  cursor: pointer;
  text-decoration: none;
  transition: all 0.2s;
  margin-top: 1rem;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background-color: #4b5563;
}
</style>
