import { defineStore } from 'pinia';
import { ref } from 'vue';
import { UserProfile, Order, Address } from '~/types';

export const useUserStore = defineStore('user', () => {
  const profile = ref<UserProfile | null>(null);
  const orders = ref<Order[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const setProfile = (userProfile: UserProfile) => {
    profile.value = userProfile;
  };

  const updateProfile = (updates: Partial<UserProfile>) => {
    if (profile.value) {
      profile.value = { ...profile.value, ...updates };
    }
  };

  const addAddress = (address: Address) => {
    if (profile.value) {
      profile.value.addresses.push(address);
    }
  };

  const updateAddress = (index: number, address: Address) => {
    if (profile.value && profile.value.addresses[index]) {
      profile.value.addresses[index] = address;
    }
  };

  const removeAddress = (index: number) => {
    if (profile.value) {
      profile.value.addresses.splice(index, 1);
    }
  };

  const setOrders = (userOrders: Order[]) => {
    orders.value = userOrders;
  };

  const addOrder = (order: Order) => {
    orders.value.unshift(order);
  };

  const getOrderById = (orderId: string) => {
    return orders.value.find(order => order.id === orderId);
  };

  const clearProfile = () => {
    profile.value = null;
    orders.value = [];
  };

  return {
    profile,
    orders,
    loading,
    error,
    setProfile,
    updateProfile,
    addAddress,
    updateAddress,
    removeAddress,
    setOrders,
    addOrder,
    getOrderById,
    clearProfile,
  };
});
