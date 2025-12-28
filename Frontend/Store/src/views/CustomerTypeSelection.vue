<template>
  <div class="customer-type-selection">
    <div class="container">
      <div class="header">
        <h1>How are you registering?</h1>
        <p class="subtitle">Choose the account type that best fits your needs</p>
      </div>

      <div class="options-grid">
        <!-- Private Customer Option -->
        <button
          :class="['option-card', { selected: selectedType === 'private' }]"
          @click="selectType('private')"
          aria-label="Register as a private customer"
          :aria-pressed="selectedType === 'private'"
        >
          <div class="icon">👤</div>
          <h2>Private Customer</h2>
          <p class="description">Individual shopper</p>
          <p class="details">For personal purchases and shopping</p>
        </button>

        <!-- Business Customer Option -->
        <button
          :class="['option-card', { selected: selectedType === 'business' }]"
          @click="selectType('business')"
          aria-label="Register as a business customer"
          :aria-pressed="selectedType === 'business'"
        >
          <div class="icon">🏢</div>
          <h2>Business Customer</h2>
          <p class="description">Company or organization</p>
          <p class="details">For business purchases and B2B operations</p>
        </button>
      </div>

      <!-- Action Buttons -->
      <div class="actions">
        <button
          class="btn btn-primary"
          :disabled="!selectedType"
          @click="continueRegistration"
        >
          Continue
        </button>
      </div>

      <!-- Login Link -->
      <div class="login-link">
        <p>
          Already have an account?
          <router-link to="/login" class="link">Sign in here</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

type CustomerType = 'private' | 'business' | null

const router = useRouter()
const selectedType = ref<CustomerType>(null)

// Load persisted selection from localStorage on mount
onMounted(() => {
  const stored = localStorage.getItem('customerTypeSelection')
  if (stored === 'private' || stored === 'business') {
    selectedType.value = stored
  }
})

// Handle type selection
const selectType = (type: CustomerType) => {
  selectedType.value = type
  // Persist selection to localStorage
  if (type) {
    localStorage.setItem('customerTypeSelection', type)
  }
}

// Navigate to appropriate registration form
const continueRegistration = () => {
  if (selectedType.value === 'private') {
    router.push('/register/private')
  } else if (selectedType.value === 'business') {
    router.push('/register/business')
  }
}
</script>

<style scoped>
.customer-type-selection {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 1rem;
}

.container {
  max-width: 900px;
  width: 100%;
  background: white;
  border-radius: 12px;
  padding: 3rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
}

.header {
  text-align: center;
  margin-bottom: 3rem;
}

h1 {
  font-size: 2.5rem;
  color: #1a1a1a;
  margin-bottom: 0.5rem;
  font-weight: 700;
}

.subtitle {
  font-size: 1.1rem;
  color: #666;
  margin: 0;
}

.options-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.option-card {
  padding: 2rem;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  background: #f9f9f9;
  cursor: pointer;
  transition: all 0.3s ease;
  text-align: center;
  font-family: inherit;
  min-height: 280px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  /* Minimum touch target size (WCAG 2.1 AA) */
  min-width: 48px;
  min-height: 48px;
}

.option-card:hover {
  border-color: #007bff;
  background: #f0f8ff;
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 123, 255, 0.15);
}

.option-card:focus {
  outline: 2px solid #007bff;
  outline-offset: 2px;
}

.option-card.selected {
  border-color: #007bff;
  background: #f0f8ff;
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.2);
}

.icon {
  font-size: 3rem;
  line-height: 1;
}

.option-card h2 {
  font-size: 1.5rem;
  color: #1a1a1a;
  margin: 0;
  font-weight: 600;
}

.description {
  font-size: 0.95rem;
  color: #666;
  margin: 0;
  font-weight: 500;
}

.details {
  font-size: 0.85rem;
  color: #999;
  margin: 0;
}

.actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.btn {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background-color: #007bff;
  color: white;
  min-height: 48px;
  min-width: 120px;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
  box-shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.login-link {
  text-align: center;
}

.login-link p {
  color: #666;
  margin: 0;
  font-size: 0.95rem;
}

.link {
  color: #007bff;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.3s ease;
}

.link:hover {
  color: #0056b3;
  text-decoration: underline;
}

.link:focus {
  outline: 2px solid #007bff;
  outline-offset: 2px;
  border-radius: 2px;
}

/* Responsive Design */
@media (max-width: 768px) {
  .container {
    padding: 1.5rem;
  }

  h1 {
    font-size: 1.8rem;
  }

  .subtitle {
    font-size: 1rem;
  }

  .options-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .option-card {
    padding: 1.5rem;
    min-height: 240px;
  }

  .icon {
    font-size: 2.5rem;
  }

  .option-card h2 {
    font-size: 1.3rem;
  }
}

@media (max-width: 480px) {
  .container {
    padding: 1rem;
  }

  h1 {
    font-size: 1.5rem;
  }

  .header {
    margin-bottom: 2rem;
  }

  .subtitle {
    font-size: 0.95rem;
  }

  .options-grid {
    gap: 1rem;
  }

  .option-card {
    padding: 1rem;
    min-height: 200px;
  }

  .icon {
    font-size: 2rem;
  }

  .option-card h2 {
    font-size: 1.1rem;
  }

  .description {
    font-size: 0.85rem;
  }

  .details {
    font-size: 0.75rem;
  }

  .btn {
    padding: 0.6rem 1.5rem;
    font-size: 0.95rem;
  }
}
</style>
