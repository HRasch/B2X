import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import { CartItem } from '~/types';

const CART_STORAGE_KEY = 'b2connect-cart';

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([]);
  const isLoading = ref(false);

  // Load cart from localStorage on init
  const loadCart = () => {
    if (process.client) {
      const stored = localStorage.getItem(CART_STORAGE_KEY);
      if (stored) {
        try {
          items.value = JSON.parse(stored);
        } catch (error) {
          console.error('Failed to parse cart from localStorage:', error);
          items.value = [];
        }
      }
    }
  };

  // Save cart to localStorage whenever items change
  watch(
    items,
    newItems => {
      if (process.client) {
        localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(newItems));
      }
    },
    { deep: true }
  );

  // Initialize cart on store creation
  loadCart();

  const addItem = (item: Omit<CartItem, 'quantity'> & { quantity?: number }): void => {
    const existingItem = items.value.find(i => i.id === item.id);

    if (existingItem) {
      existingItem.quantity += item.quantity || 1;
    } else {
      items.value.push({ ...item, quantity: item.quantity || 1 });
    }
  };

  const removeItem = (itemId: string): void => {
    items.value = items.value.filter(i => i.id !== itemId);
  };

  const updateQuantity = (itemId: string, quantity: number): void => {
    if (quantity <= 0) {
      removeItem(itemId);
      return;
    }

    const item = items.value.find(i => i.id === itemId);
    if (item) {
      item.quantity = quantity;
    }
  };

  const clearCart = (): void => {
    items.value = [];
  };

  const getTotal = computed((): number => {
    return items.value.reduce((sum, item) => sum + item.price * item.quantity, 0);
  });

  const getItemCount = computed((): number => {
    return items.value.reduce((sum, item) => sum + item.quantity, 0);
  });

  const isInCart = computed(() => {
    return (itemId: string) => items.value.some(i => i.id === itemId);
  });

  const getCartShareUrl = (): string => {
    if (process.client) {
      const cartData = btoa(
        JSON.stringify(
          items.value.map(item => ({
            id: item.id,
            quantity: item.quantity,
          }))
        )
      );
      return `${window.location.origin}/cart?shared=${cartData}`;
    }
    return '';
  };

  const loadSharedCart = (sharedData: string): void => {
    try {
      const decoded = JSON.parse(atob(sharedData));
      // Note: In a real app, you'd fetch product details from API
      // For now, we'll assume the shared data includes full product info
      if (Array.isArray(decoded)) {
        items.value = decoded;
      }
    } catch (error) {
      console.error('Failed to load shared cart:', error);
    }
  };

  return {
    items,
    isLoading,
    addItem,
    removeItem,
    updateQuantity,
    clearCart,
    getTotal,
    getItemCount,
    isInCart,
    getCartShareUrl,
    loadSharedCart,
  };
});
