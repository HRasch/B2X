import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ProductService } from "@/services/productService";
import { api } from "@/services/api";

// Mock the api module
vi.mock("@/services/api", () => ({
  api: {
    get: vi.fn(),
  },
}));

describe("ProductService", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  describe("getProducts", () => {
    it("should call /catalog/products endpoint with pagination", async () => {
      const mockResponse = {
        data: {
          items: [{ id: "1", name: "Product 1", price: 99.99 }],
          page: 1,
          pageSize: 20,
          totalCount: 1,
          totalPages: 1,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      const result = await ProductService.getProducts(1, 20);

      expect(api.get).toHaveBeenCalledWith(
        "/catalog/products?page=1&pageSize=20",
      );
      expect(result.items).toHaveLength(1);
      expect(result.items[0].name).toBe("Product 1");
    });

    it("should apply category filter", async () => {
      const mockResponse = {
        data: {
          items: [],
          page: 1,
          pageSize: 20,
          totalCount: 0,
          totalPages: 0,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      await ProductService.getProducts(1, 20, { category: "electronics" });

      expect(api.get).toHaveBeenCalledWith(
        "/catalog/products?page=1&pageSize=20&category=electronics",
      );
    });

    it("should apply price range filters", async () => {
      const mockResponse = {
        data: {
          items: [],
          page: 1,
          pageSize: 20,
          totalCount: 0,
          totalPages: 0,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      await ProductService.getProducts(1, 20, { minPrice: 10, maxPrice: 100 });

      expect(api.get).toHaveBeenCalledWith(
        "/catalog/products?page=1&pageSize=20&minPrice=10&maxPrice=100",
      );
    });

    it("should handle invalid page numbers", async () => {
      const mockResponse = {
        data: {
          items: [],
          page: 1,
          pageSize: 20,
          totalCount: 0,
          totalPages: 0,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      // Page < 1 should be normalized to 1
      await ProductService.getProducts(-1, 20);

      expect(api.get).toHaveBeenCalledWith(
        "/catalog/products?page=1&pageSize=20",
      );
    });

    it("should handle invalid pageSize", async () => {
      const mockResponse = {
        data: {
          items: [],
          page: 1,
          pageSize: 20,
          totalCount: 0,
          totalPages: 0,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      // pageSize > 100 should be normalized to 20
      await ProductService.getProducts(1, 500);

      expect(api.get).toHaveBeenCalledWith(
        "/catalog/products?page=1&pageSize=20",
      );
    });

    it("should throw error on API failure", async () => {
      vi.mocked(api.get).mockRejectedValue(new Error("Network Error"));

      await expect(ProductService.getProducts(1, 20)).rejects.toThrow(
        "Network Error",
      );
    });
  });

  describe("getProductById", () => {
    it("should call /catalog/products/:id endpoint", async () => {
      const mockProduct = {
        id: "prod-123",
        name: "Test Product",
        price: 49.99,
        description: "A test product",
      };

      vi.mocked(api.get).mockResolvedValue({ data: mockProduct });

      const result = await ProductService.getProductById("prod-123");

      expect(api.get).toHaveBeenCalledWith("/catalog/products/prod-123");
      expect(result.id).toBe("prod-123");
      expect(result.name).toBe("Test Product");
    });

    it("should throw error if product not found", async () => {
      vi.mocked(api.get).mockRejectedValue(new Error("Product not found"));

      await expect(ProductService.getProductById("invalid-id")).rejects.toThrow(
        "Product not found",
      );
    });
  });

  describe("searchProducts", () => {
    it("should call /catalog/products/elasticsearch endpoint with search term", async () => {
      const mockResponse = {
        data: {
          items: [{ id: "1", name: "Blue Jacket", price: 129.99 }],
          page: 1,
          pageSize: 20,
          totalCount: 1,
          totalPages: 1,
          hasNextPage: false,
          searchMetadata: {
            queryExecutionTimeMs: 15,
            hitCount: 1,
            source: "elasticsearch",
          },
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      const result = await ProductService.searchProducts({
        searchTerm: "jacket",
        language: "en",
      });

      expect(api.get).toHaveBeenCalledWith(
        expect.stringContaining("/catalog/products/elasticsearch?"),
      );
      expect(api.get).toHaveBeenCalledWith(
        expect.stringContaining("term=jacket"),
      );
      expect(api.get).toHaveBeenCalledWith(
        expect.stringContaining("language=en"),
      );
      expect(result.items).toHaveLength(1);
      expect(result.searchMetadata?.source).toBe("elasticsearch");
    });

    it("should include all filters in search request", async () => {
      const mockResponse = {
        data: {
          items: [],
          page: 1,
          pageSize: 20,
          totalCount: 0,
          totalPages: 0,
          hasNextPage: false,
        },
      };

      vi.mocked(api.get).mockResolvedValue(mockResponse);

      await ProductService.searchProducts({
        searchTerm: "shirt",
        category: "clothing",
        minPrice: 20,
        maxPrice: 80,
        onlyAvailable: true,
        language: "de",
      });

      const callArg = vi.mocked(api.get).mock.calls[0][0];
      expect(callArg).toContain("term=shirt");
      expect(callArg).toContain("category=clothing");
      expect(callArg).toContain("minPrice=20");
      expect(callArg).toContain("maxPrice=80");
      expect(callArg).toContain("onlyAvailable=true");
      expect(callArg).toContain("language=de");
    });
  });

  describe("getCatalogStats", () => {
    it("should call /catalog/products/stats endpoint", async () => {
      const mockStats = {
        totalProducts: 1500,
        categories: 25,
        avgPrice: 75.5,
      };

      vi.mocked(api.get).mockResolvedValue({ data: mockStats });

      const result = await ProductService.getCatalogStats();

      expect(api.get).toHaveBeenCalledWith("/catalog/products/stats");
      expect(result.totalProducts).toBe(1500);
    });
  });
});
