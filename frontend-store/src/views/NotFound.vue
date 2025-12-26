<template>
  <div class="not-found">
    <h1>404</h1>
    <h2>Page Not Found</h2>
    <p>The page you are looking for does not exist.</p>
    <router-link to="/" class="btn btn-primary">Go Back Home</router-link>
  </div>
</template>

<script setup lang="ts">
// NotFound component
</script>

<style scoped>
.not-found {
  text-align: center;
  padding: 4rem 1rem;
}

h1 {
  font-size: 4rem;
  margin: 0;
  color: #999;
}

h2 {
  color: #333;
  margin: 1rem 0;
}

p {
  color: #666;
  margin: 1rem 0 2rem 0;
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  background-color: #007bff;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.btn:hover {
  background-color: #0056b3;
}
</style>
