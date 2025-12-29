import { defineStore } from "pinia";
import { ref } from "vue";

export interface CartItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
  image: string;
}

export const useCartStore = defineStore("cart", () => {
  const items = ref<CartItem[]>([]);

  const addItem = (item: CartItem) => {
    const existingItem = items.value.find((i) => i.id === item.id);

    if (existingItem) {
      existingItem.quantity += item.quantity;
    } else {
      items.value.push(item);
    }
  };

  const removeItem = (itemId: string) => {
    items.value = items.value.filter((i) => i.id !== itemId);
  };

  const updateQuantity = (itemId: string, quantity: number) => {
    const item = items.value.find((i) => i.id === itemId);
    if (item) {
      item.quantity = quantity;
    }
  };

  const clearCart = () => {
    items.value = [];
  };

  const getTotal = () => {
    return items.value.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
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
