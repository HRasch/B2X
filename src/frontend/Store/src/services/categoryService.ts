import { ProductService, type Product } from './productService';
import type { Category, CategoryWithProducts } from '@/types/catalog';

export class CategoryService {
  /**
   * Extract unique categories from products
   * Phase 1 implementation - uses product category strings
   */
  static async getCategories(): Promise<Category[]> {
    try {
      // Get all products to extract categories
      const response = await ProductService.getProducts(1, 1000); // Large page to get all
      const products = response.items;

      // Extract unique categories and count products per category
      const categoryMap = new Map<string, { count: number; products: Product[] }>();

      products.forEach(product => {
        product.categories.forEach(categoryName => {
          if (!categoryMap.has(categoryName)) {
            categoryMap.set(categoryName, { count: 0, products: [] });
          }
          const category = categoryMap.get(categoryName)!;
          category.count++;
          category.products.push(product);
        });
      });

      // Convert to Category array
      const categories: Category[] = Array.from(categoryMap.entries()).map(([name, data]) => ({
        id: this.generateCategoryId(name),
        name,
        slug: this.generateSlug(name),
        productCount: data.count,
        children: [], // Phase 1: no hierarchy
      }));

      return categories.sort((a, b) => b.productCount - a.productCount); // Sort by product count desc
    } catch (error) {
      console.error('Error fetching categories:', error);
      throw error;
    }
  }

  /**
   * Get category with its products
   */
  static async getCategoryWithProducts(slug: string): Promise<CategoryWithProducts | null> {
    try {
      const categories = await this.getCategories();
      const category = categories.find(c => c.slug === slug);

      if (!category) return null;

      // Get products for this category
      const response = await ProductService.getProducts(1, 100);
      const products = response.items.filter(p => p.categories.includes(category.name));

      return {
        ...category,
        products,
      };
    } catch (error) {
      console.error('Error fetching category with products:', error);
      throw error;
    }
  }

  /**
   * Generate a simple category ID from name
   * Phase 1: simple hash, Phase 2: use real category IDs
   */
  private static generateCategoryId(name: string): string {
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
      const char = name.charCodeAt(i);
      hash = (hash << 5) - hash + char;
      hash = hash & hash; // Convert to 32-bit integer
    }
    return Math.abs(hash).toString();
  }

  /**
   * Generate URL-friendly slug from category name
   */
  private static generateSlug(name: string): string {
    return name
      .toLowerCase()
      .replace(/[^a-z0-9\s-]/g, '') // Remove special chars
      .replace(/\s+/g, '-') // Replace spaces with hyphens
      .replace(/-+/g, '-') // Replace multiple hyphens with single
      .trim();
  }
}
