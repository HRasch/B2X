<template>
  <div class="cart-container">
    <h1>Warenkorb</h1>

    <div v-if="cartStore.items.length === 0" class="empty-cart">
      <p>Ihr Warenkorb ist leer</p>
      <router-link to="/shop" class="continue-shopping">
        Zum Einkaufen
      </router-link>
    </div>

    <div v-else class="cart-content">
      <div class="cart-items">
        <div v-for="item in cartStore.items" :key="item.id" class="cart-item">
          <img :src="item.image" :alt="item.name" class="item-image" />

          <div class="item-details">
            <h3>{{ item.name }}</h3>
            <p>Preis: {{ item.price }}€</p>
          </div>

          <div class="item-quantity">
            <button @click="decreaseQuantity(item.id)">-</button>
            <input v-model.number="item.quantity" type="number" min="1" />
            <button @click="increaseQuantity(item.id)">+</button>
          </div>

          <div class="item-subtotal">
            {{ (item.price * item.quantity).toFixed(2) }}€
          </div>

          <button @click="removeItem(item.id)" class="remove-btn">✕</button>
        </div>
      </div>

      <div class="cart-summary">
        <h2>Zusammenfassung</h2>

        <div class="summary-row">
          <span>Zwischensumme:</span>
          <span>{{ subtotal.toFixed(2) }}€</span>
        </div>

        <div class="summary-row">
          <span>Steuern (19%):</span>
          <span>{{ tax.toFixed(2) }}€</span>
        </div>

        <div class="summary-row total">
          <span>Gesamtpreis:</span>
          <span>{{ total.toFixed(2) }}€</span>
        </div>

        <button @click="checkout" class="checkout-btn">Zur Kasse gehen</button>

        <router-link to="/shop" class="continue-shopping">
          Weiter einkaufen
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "../stores/cart";

const router = useRouter();
const cartStore = useCartStore();

const subtotal = computed(() => {
  return cartStore.items.reduce(
    (sum, item) => sum + item.price * item.quantity,
    0
  );
});

const tax = computed(() => subtotal.value * 0.19);

const total = computed(() => subtotal.value + tax.value);

const increaseQuantity = (itemId: string) => {
  const item = cartStore.items.find((i) => i.id === itemId);
  if (item) {
    item.quantity++;
  }
};

const decreaseQuantity = (itemId: string) => {
  const item = cartStore.items.find((i) => i.id === itemId);
  if (item && item.quantity > 1) {
    item.quantity--;
  }
};

const removeItem = (itemId: string) => {
  cartStore.removeItem(itemId);
};

const checkout = () => {
  router.push("/checkout");
};
</script>

<style scoped>
.cart-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.cart-container h1 {
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.empty-cart {
  text-align: center;
  padding: 3rem;
  background-color: #f9f9f9;
  border-radius: 8px;
}

.empty-cart p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 1.5rem;
}

.continue-shopping {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  background-color: #2563eb;
  color: white;
  text-decoration: none;
  border-radius: 8px;
  transition: background-color 0.3s;
}

.continue-shopping:hover {
  background-color: #1d4ed8;
}

.cart-content {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 2rem;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cart-item {
  display: grid;
  grid-template-columns: 100px 1fr auto auto auto;
  gap: 1rem;
  align-items: center;
  padding: 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: white;
}

.item-image {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 4px;
}

.item-details h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.item-details p {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
}

.item-quantity {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.item-quantity button {
  width: 30px;
  height: 30px;
  padding: 0;
  border: 1px solid #e0e0e0;
  background-color: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.item-quantity button:hover {
  background-color: #f5f5f5;
}

.item-quantity input {
  width: 50px;
  text-align: center;
  padding: 0.4rem;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
}

.item-subtotal {
  font-weight: 600;
  color: #2563eb;
  min-width: 70px;
  text-align: right;
}

.remove-btn {
  padding: 0.5rem;
  background-color: #f3f4f6;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  cursor: pointer;
  color: #ef4444;
  font-weight: bold;
  transition: background-color 0.3s;
}

.remove-btn:hover {
  background-color: #fee2e2;
}

.cart-summary {
  background-color: #f9f9f9;
  padding: 1.5rem;
  border-radius: 8px;
  height: fit-content;
  position: sticky;
  top: 20px;
}

.cart-summary h2 {
  margin: 0 0 1rem 0;
  font-size: 1.2rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.summary-row.total {
  font-size: 1.1rem;
  font-weight: 700;
  border-bottom: none;
  margin-top: 0.5rem;
  padding-top: 1rem;
  color: #2563eb;
}

.checkout-btn {
  width: 100%;
  padding: 1rem;
  background-color: #059669;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  margin-top: 1rem;
  transition: background-color 0.3s;
}

.checkout-btn:hover {
  background-color: #047857;
}

.cart-summary .continue-shopping {
  display: block;
  text-align: center;
  margin-top: 1rem;
  padding: 0.75rem;
}

@media (max-width: 768px) {
  .cart-content {
    grid-template-columns: 1fr;
  }

  .cart-item {
    grid-template-columns: 80px 1fr;
    gap: 0.75rem;
  }

  .item-image {
    width: 80px;
    height: 80px;
  }

  .item-quantity,
  .item-subtotal,
  .remove-btn {
    grid-column: 2;
  }

  .cart-summary {
    position: static;
  }
}
</style>
