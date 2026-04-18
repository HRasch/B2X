import { ref, computed } from 'vue';
import { CategoryService } from '@/services/categoryService';
import type { Category, CategoryWithProducts } from '@/types/catalog';

export function useCategories() {
  const categories = ref<Category[]>([]);
  const currentCategory = ref<CategoryWithProducts | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  /**
   * Load all categories
   */
  const loadCategories = async () => {
    loading.value = true;
    error.value = null;

    try {
      categories.value = await CategoryService.getCategories();
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load categories';
      console.error('Error loading categories:', err);
    } finally {
      loading.value = false;
    }
  };

  /**
   * Load category with products by slug
   */
  const loadCategoryWithProducts = async (slug: string) => {
    loading.value = true;
    error.value = null;

    try {
      currentCategory.value = await CategoryService.getCategoryWithProducts(slug);
      if (!currentCategory.value) {
        error.value = 'Category not found';
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load category';
      console.error('Error loading category:', err);
    } finally {
      loading.value = false;
    }
  };

  /**
   * Get category by slug
   */
  const getCategoryBySlug = (slug: string): Category | undefined => {
    return categories.value.find(cat => cat.slug === slug);
  };

  /**
   * Computed properties
   */
  const hasCategories = computed(() => categories.value.length > 0);
  const categoryCount = computed(() => categories.value.length);
  const totalProducts = computed(() =>
    categories.value.reduce((sum, cat) => sum + cat.productCount, 0)
  );

  return {
    // State
    categories,
    currentCategory,
    loading,
    error,

    // Computed
    hasCategories,
    categoryCount,
    totalProducts,

    // Methods
    loadCategories,
    loadCategoryWithProducts,
    getCategoryBySlug,
  };
}
