import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  image?: string;
  parentId?: string;
  productCount?: number;
  children?: Category[];
}

export const useCategoriesStore = defineStore('categories', () => {
  // State
  const categories = ref<Category[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // Getters
  const rootCategories = computed(() => categories.value.filter(cat => !cat.parentId));

  const getCategoryBySlug = computed(
    () => (slug: string) => categories.value.find(cat => cat.slug === slug)
  );

  const getCategoryHierarchy = computed(() => (categoryId: string) => {
    const category = categories.value.find(cat => cat.id === categoryId);
    if (!category) return [];

    const hierarchy = [category];
    let current = category;

    while (current.parentId) {
      const parent = categories.value.find(cat => cat.id === current.parentId);
      if (!parent) break;
      hierarchy.unshift(parent);
      current = parent;
    }

    return hierarchy;
  });

  // Actions
  const fetchCategories = async () => {
    loading.value = true;
    error.value = null;

    try {
      // TODO: Replace with actual API call
      // const response = await $fetch('/api/categories')
      // categories.value = response

      // Mock data for now
      categories.value = [
        {
          id: '1',
          name: 'Electronics',
          slug: 'electronics',
          description: 'Latest gadgets and electronic devices',
          image: '/images/categories/electronics.jpg',
          productCount: 245,
        },
        {
          id: '2',
          name: 'Clothing',
          slug: 'clothing',
          description: 'Fashion and apparel for all occasions',
          image: '/images/categories/clothing.jpg',
          productCount: 189,
        },
        {
          id: '3',
          name: 'Home & Garden',
          slug: 'home-garden',
          description: 'Everything for your home and garden',
          image: '/images/categories/home-garden.jpg',
          productCount: 156,
        },
        {
          id: '4',
          name: 'Sports & Outdoors',
          slug: 'sports-outdoors',
          description: 'Gear for sports and outdoor activities',
          image: '/images/categories/sports.jpg',
          productCount: 98,
        },
        {
          id: '5',
          name: 'Books',
          slug: 'books',
          description: 'Books, ebooks, and educational materials',
          image: '/images/categories/books.jpg',
          productCount: 312,
        },
        {
          id: '6',
          name: 'Health & Beauty',
          slug: 'health-beauty',
          description: 'Health, wellness, and beauty products',
          image: '/images/categories/health.jpg',
          productCount: 167,
        },
      ];
    } catch (err) {
      error.value = 'Failed to load categories';
      console.error('Error fetching categories:', err);
    } finally {
      loading.value = false;
    }
  };

  const fetchCategoryBySlug = async (slug: string) => {
    loading.value = true;
    error.value = null;

    try {
      // TODO: Replace with actual API call
      // const response = await $fetch(`/api/categories/${slug}`)
      // return response

      // Mock data for now
      return categories.value.find(cat => cat.slug === slug);
    } catch (err) {
      error.value = 'Failed to load category';
      console.error('Error fetching category:', err);
      return null;
    } finally {
      loading.value = false;
    }
  };

  return {
    // State
    categories,
    loading,
    error,

    // Getters
    rootCategories,
    getCategoryBySlug,
    getCategoryHierarchy,

    // Actions
    fetchCategories,
    fetchCategoryBySlug,
  };
});
