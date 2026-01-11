<template>
  <div class="not-found">
    <h1>404</h1>
    <p>Page Not Found</p>
    <router-link to="/dashboard" class="btn-home">Back to Dashboard</router-link>
  </div>
</template>

<style scoped>
.not-found {
  text-align: center;
  padding: 4rem 2rem;
}

.not-found h1 {
  font-size: 4rem;
  color: #667eea;
  margin: 0;
}

.not-found p {
  font-size: 1.5rem;
  color: #666;
  margin: 1rem 0;
}

.btn-home {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  background-color: #667eea;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.btn-home:hover {
  background-color: #5568d3;
}
</style>
