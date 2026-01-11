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
        // Root categories
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
        // Electronics subcategories
        {
          id: '1-1',
          name: 'Smartphones',
          slug: 'electronics/smartphones',
          description: 'Latest smartphones and mobile devices',
          image: '/images/categories/smartphones.jpg',
          parentId: '1',
          productCount: 89,
        },
        {
          id: '1-2',
          name: 'Laptops',
          slug: 'electronics/laptops',
          description: 'Laptops, notebooks, and computing devices',
          image: '/images/categories/laptops.jpg',
          parentId: '1',
          productCount: 67,
        },
        {
          id: '1-3',
          name: 'Audio',
          slug: 'electronics/audio',
          description: 'Headphones, speakers, and audio equipment',
          image: '/images/categories/audio.jpg',
          parentId: '1',
          productCount: 45,
        },
        {
          id: '1-4',
          name: 'Gaming',
          slug: 'electronics/gaming',
          description: 'Gaming consoles, accessories, and gear',
          image: '/images/categories/gaming.jpg',
          parentId: '1',
          productCount: 44,
        },
        // Clothing subcategories
        {
          id: '2-1',
          name: 'Men\'s Clothing',
          slug: 'clothing/mens',
          description: 'Fashion for men',
          image: '/images/categories/mens-clothing.jpg',
          parentId: '2',
          productCount: 95,
        },
        {
          id: '2-2',
          name: 'Women\'s Clothing',
          slug: 'clothing/womens',
          description: 'Fashion for women',
          image: '/images/categories/womens-clothing.jpg',
          parentId: '2',
          productCount: 94,
        },
        // Home & Garden subcategories
        {
          id: '3-1',
          name: 'Furniture',
          slug: 'home-garden/furniture',
          description: 'Home furniture and decor',
          image: '/images/categories/furniture.jpg',
          parentId: '3',
          productCount: 78,
        },
        {
          id: '3-2',
          name: 'Garden Tools',
          slug: 'home-garden/garden-tools',
          description: 'Tools and equipment for gardening',
          image: '/images/categories/garden-tools.jpg',
          parentId: '3',
          productCount: 42,
        },
        {
          id: '3-3',
          name: 'Home Decor',
          slug: 'home-garden/home-decor',
          description: 'Decorative items for your home',
          image: '/images/categories/home-decor.jpg',
          parentId: '3',
          productCount: 36,
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
