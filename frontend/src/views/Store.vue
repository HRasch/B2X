<template>
  <div class="store-container">
    <!-- Header with Cart -->
    <div class="store-header">
      <h1>B2Connect Shop</h1>
      <div class="header-actions">
        <div class="search-bar">
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="Produkte suchen..."
            @input="filterProducts"
          >
        </div>
        <router-link to="/cart" class="cart-button">
          🛒 Warenkorb ({{ cartStore.items.length }})
        </router-link>
      </div>
    </div>

    <!-- Category Filter -->
    <div class="category-filter">
      <button 
        v-for="cat in categories" 
        :key="cat"
        @click="selectedCategory = cat"
        :class="['category-btn', { active: selectedCategory === cat }]"
      >
        {{ cat }}
      </button>
    </div>

    <!-- Products Grid -->
    <div class="products-grid">
      <div v-if="filteredProducts.length === 0" class="no-products">
        <p>Keine Produkte gefunden.</p>
      </div>
      
      <ProductCard 
        v-for="product in filteredProducts" 
        :key="product.id"
        :product="product"
        @add-to-cart="addToCart"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useCartStore } from '../stores/cart'
import ProductCard from '../components/shop/ProductCard.vue'

interface Product {
  id: string
  name: string
  price: number
  b2bPrice: number
  image: string
  category: string
  description: string
  inStock: boolean
  rating: number
}

const cartStore = useCartStore()

// Mock Products
const products = ref<Product[]>([
  {
    id: '1',
    name: 'Laptop Pro 15"',
    price: 1299,
    b2bPrice: 1099,
    image: 'https://via.placeholder.com/300x200?text=Laptop+Pro',
    category: 'Elektronik',
    description: 'Professioneller Laptop mit 16GB RAM',
    inStock: true,
    rating: 4.8
  },
  {
    id: '2',
    name: 'Wireless Mouse',
    price: 49,
    b2bPrice: 39,
    image: 'https://via.placeholder.com/300x200?text=Wireless+Mouse',
    category: 'Zubehör',
    description: 'Kabellose Maus mit USB Nano-Receiver',
    inStock: true,
    rating: 4.5
  },
  {
    id: '3',
    name: 'USB-C Hub',
    price: 79,
    b2bPrice: 59,
    image: 'https://via.placeholder.com/300x200?text=USB-C+Hub',
    category: 'Zubehör',
    description: '7-in-1 USB-C Hub mit HDMI und SD-Kartenleser',
    inStock: true,
    rating: 4.6
  },
  {
    id: '4',
    name: 'Mechanical Keyboard',
    price: 159,
    b2bPrice: 129,
    image: 'https://via.placeholder.com/300x200?text=Mechanical+Keyboard',
    category: 'Zubehör',
    description: 'RGB Mechanische Tastatur',
    inStock: true,
    rating: 4.7
  },
  {
    id: '5',
    name: 'Monitor 4K 27"',
    price: 599,
    b2bPrice: 499,
    image: 'https://via.placeholder.com/300x200?text=4K+Monitor',
    category: 'Elektronik',
    description: '4K UHD Monitor mit USB-C',
    inStock: false,
    rating: 4.9
  },
  {
    id: '6',
    name: 'Webcam HD',
    price: 89,
    b2bPrice: 69,
    image: 'https://via.placeholder.com/300x200?text=Webcam+HD',
    category: 'Zubehör',
    description: '1080p HD Webcam mit Autofokus',
    inStock: true,
    rating: 4.4
  }
])

const searchQuery = ref('')
const selectedCategory = ref('Alle')
const categories = ref(['Alle', 'Elektronik', 'Zubehör'])

const filteredProducts = computed(() => {
  return products.value.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesCategory = selectedCategory.value === 'Alle' || product.category === selectedCategory.value
    return matchesSearch && matchesCategory
  })
})

const filterProducts = () => {
  // Filter is handled by computed property
}

const addToCart = (product: Product) => {
  cartStore.addItem({
    id: product.id,
    name: product.name,
    price: product.price,
    quantity: 1,
    image: product.image
  })
}
</script>

<style scoped>
.store-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.store-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  gap: 2rem;
}

.store-header h1 {
  font-size: 2rem;
  color: #333;
  margin: 0;
  flex-shrink: 0;
}

.header-actions {
  display: flex;
  gap: 1rem;
  flex: 1;
  align-items: center;
}

.search-bar {
  flex: 1;
  min-width: 250px;
}

.search-bar input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.search-bar input:focus {
  outline: none;
  border-color: #2563eb;
}

.cart-button {
  padding: 0.75rem 1.5rem;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  text-decoration: none;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.cart-button:hover {
  background-color: #1d4ed8;
}

.category-filter {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.category-btn {
  padding: 0.5rem 1rem;
  border: 2px solid #e0e0e0;
  background-color: white;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
  font-weight: 500;
}

.category-btn:hover {
  border-color: #2563eb;
  color: #2563eb;
}

.category-btn.active {
  background-color: #2563eb;
  color: white;
  border-color: #2563eb;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2rem;
}

.no-products {
  grid-column: 1 / -1;
  text-align: center;
  padding: 3rem;
  color: #999;
  font-size: 1.1rem;
}

@media (max-width: 768px) {
  .store-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    flex-direction: column;
  }

  .search-bar input {
    min-width: unset;
  }

  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 1rem;
  }
}
</style>
