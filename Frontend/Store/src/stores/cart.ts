import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { CartItem } from '~/types';

export type { CartItem };

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([]);

  const addItem = (item: CartItem): void => {
    const existingItem = items.value.find(i => i.id === item.id);

    if (existingItem) {
      existingItem.quantity += item.quantity;
    } else {
      items.value.push(item);
    }
  };

  const removeItem = (itemId: string): void => {
    items.value = items.value.filter(i => i.id !== itemId);
  };

  const updateQuantity = (itemId: string, quantity: number): void => {
    const item = items.value.find(i => i.id === itemId);
    if (item) {
      item.quantity = quantity;
    }
  };

  const clearCart = (): void => {
    items.value = [];
  };

  const getTotal = (): number => {
    return items.value.reduce((sum, item) => sum + item.price * item.quantity, 0);
  };

  return {
    items,
    addItem,
    removeItem,
    updateQuantity,
    clearCart,
    getTotal,
  };
});
