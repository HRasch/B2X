/* eslint-disable @typescript-eslint/no-explicit-any -- Test mocks use any */
/* eslint-disable @typescript-eslint/no-unused-vars -- Test setup variables */
import { describe, it, expect, beforeEach, vi } from "vitest";
import { setActivePinia, createPinia } from "pinia";
import { useShopStore } from "@/stores/shop";
import { shopApi } from "@/services/api/shop";
import type { Product, PaginatedResponse } from "@/types";

vi.mock("@/services/api/shop", () => ({
  shopApi: {
    getProducts: vi.fn(),
    getProduct: vi.fn(),
    createProduct: vi.fn(),
    updateProduct: vi.fn(),
    deleteProduct: vi.fn(),
    getCategories: vi.fn(),
    getPricingRules: vi.fn(),
    savePricingRule: vi.fn(),
    getDiscounts: vi.fn(),
  },
}));

describe("Shop Store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  describe("Initial State", () => {
    it("should initialize with empty arrays", () => {
      const store = useShopStore();
      expect(store.products).toEqual([]);
      expect(store.categories).toEqual([]);
      expect(store.pricingRules).toEqual([]);
      expect(store.discounts).toEqual([]);
      expect(store.currentProduct).toBeNull();
    });
  });

  describe("Fetch Products", () => {
    it("should fetch products successfully", async () => {
      const mockProducts: Product[] = [
        {
          id: "1",
          name: "Product 1",
          sku: "PROD-001",
          description: "Description 1",
          basePrice: 99.99,
          currency: "EUR",
          images: [],
          categoryId: "cat-1",
          attributes: {},
          stock: 10,
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date(),
          tenantId: "tenant-1",
        },
      ];

      const mockResponse: PaginatedResponse<Product> = {
        items: mockProducts,
        total: 1,
        page: 1,
        pageSize: 10,
        totalPages: 1,
      };

      vi.mocked(shopApi.getProducts).mockResolvedValue(mockResponse);

      const store = useShopStore();
      await store.fetchProducts();

      expect(store.products).toEqual(mockProducts);
      expect(store.loading).toBe(false);
    });

    it("should filter products", async () => {
      const filters = { isActive: true };
      vi.mocked(shopApi.getProducts).mockResolvedValue({
        items: [],
        total: 0,
        page: 1,
        pageSize: 10,
        totalPages: 0,
      });

      const store = useShopStore();
      await store.fetchProducts(filters);

      expect(shopApi.getProducts).toHaveBeenCalledWith(filters);
    });
  });

  describe("Fetch Single Product", () => {
    it("should fetch product by id", async () => {
      const mockProduct: Product = {
        id: "1",
        name: "Product",
        sku: "PROD-001",
        description: "Description",
        basePrice: 99.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 10,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date(),
        tenantId: "tenant-1",
      };

      vi.mocked(shopApi.getProduct).mockResolvedValue(mockProduct);

      const store = useShopStore();
      await store.fetchProduct("1");

      expect(store.currentProduct).toEqual(mockProduct);
    });
  });

  describe("Save Product", () => {
    it("should create new product", async () => {
      const newProduct: Product = {
        id: "1",
        name: "New Product",
        sku: "NEW-001",
        description: "New product description",
        basePrice: 49.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 50,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date(),
        tenantId: "tenant-1",
      };

      vi.mocked(shopApi.createProduct).mockResolvedValue(newProduct);

      const store = useShopStore();
      store.products = [];
      const result = await store.saveProduct({
        name: "New Product",
        sku: "NEW-001",
        description: "New product description",
        basePrice: 49.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 50,
        isActive: true,
        tenantId: "tenant-1",
      } as any);

      expect(store.products.length).toBe(1);
      expect(store.currentProduct).toEqual(newProduct);
    });

    it("should update existing product", async () => {
      const updatedProduct: Product = {
        id: "1",
        name: "Updated Product",
        sku: "PROD-001",
        description: "Updated description",
        basePrice: 79.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 20,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date(),
        tenantId: "tenant-1",
      };

      vi.mocked(shopApi.updateProduct).mockResolvedValue(updatedProduct);

      const store = useShopStore();
      const result = await store.saveProduct(updatedProduct);

      expect(store.currentProduct).toEqual(updatedProduct);
    });
  });

  describe("Delete Product", () => {
    it("should delete product", async () => {
      vi.mocked(shopApi.deleteProduct).mockResolvedValue(undefined as any);

      const store = useShopStore();
      const product1: Product = {
        id: "1",
        name: "Product 1",
        sku: "PROD-001",
        description: "Description",
        basePrice: 99.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 10,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date(),
        tenantId: "tenant-1",
      };
      const product2: Product = {
        id: "2",
        name: "Product 2",
        sku: "PROD-002",
        description: "Description",
        basePrice: 49.99,
        currency: "EUR",
        images: [],
        categoryId: "cat-1",
        attributes: {},
        stock: 5,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date(),
        tenantId: "tenant-1",
      };

      store.products = [product1, product2];
      store.currentProduct = product1;

      await store.deleteProduct("1");

      expect(store.products).toHaveLength(1);
      expect(store.products[0].id).toBe("2");
      expect(store.currentProduct).toBeNull();
    });
  });

  describe("Fetch Categories", () => {
    it("should fetch all categories", async () => {
      const mockCategories = [
        {
          id: "1",
          name: "Category 1",
          description: "Desc",
          parentId: undefined,
          slug: "cat-1",
          tenantId: "tenant-1",
        },
      ];

      vi.mocked(shopApi.getCategories).mockResolvedValue(mockCategories);

      const store = useShopStore();
      await store.fetchCategories();

      expect(store.categories).toEqual(mockCategories);
    });
  });

  describe("Fetch Pricing Rules", () => {
    it("should fetch pricing rules", async () => {
      const mockResponse: PaginatedResponse<any> = {
        items: [
          {
            id: "1",
            name: "Volume Discount",
            type: "tiered",
            conditions: [],
            effect: { type: "discount", value: 10 },
            isActive: true,
            tenantId: "tenant-1",
          },
        ],
        total: 1,
        page: 1,
        pageSize: 10,
        totalPages: 1,
      };

      vi.mocked(shopApi.getPricingRules).mockResolvedValue(mockResponse);

      const store = useShopStore();
      await store.fetchPricingRules();

      expect(store.pricingRules).toHaveLength(1);
      expect(store.pricingRules[0].name).toBe("Volume Discount");
    });
  });

  describe("Fetch Discounts", () => {
    it("should fetch discounts", async () => {
      const mockResponse: PaginatedResponse<any> = {
        items: [
          {
            id: "1",
            code: "SAVE10",
            type: "percentage",
            value: 10,
            maxUses: 100,
            usedCount: 50,
            startDate: new Date(),
            endDate: new Date(),
            isActive: true,
            tenantId: "tenant-1",
          },
        ],
        total: 1,
        page: 1,
        pageSize: 10,
        totalPages: 1,
      };

      vi.mocked(shopApi.getDiscounts).mockResolvedValue(mockResponse);

      const store = useShopStore();
      await store.fetchDiscounts();

      expect(store.discounts).toHaveLength(1);
      expect(store.discounts[0].code).toBe("SAVE10");
    });
  });
});
