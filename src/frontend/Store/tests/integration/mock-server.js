/**
 * Mock API Server for Integration Tests
 *
 * This server provides mock responses for integration tests
 * when the full backend is not available.
 */

import express from 'express';
import cors from 'cors';

const app = express();
const PORT = 8000;

// Middleware
app.use(cors());
app.use(express.json());

// Health check endpoint
app.get('/health', (req, res) => {
  res.json({ status: 'healthy', timestamp: new Date().toISOString() });
});

// Mock catalog products endpoint
app.get('/api/catalog/products', (req, res) => {
  const { page = 1, pageSize = 10, search, category } = req.query;

  // Mock products data
  const mockProducts = [
    {
      id: '550e8400-e29b-41d4-a716-446655440000',
      name: 'Test Product 1',
      description: 'A test product for integration testing',
      price: 99.99,
      category: 'Electronics',
      imageUrl: '/images/test-product-1.jpg',
      inStock: true,
      createdAt: '2024-01-01T00:00:00Z',
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440001',
      name: 'Test Product 2',
      description: 'Another test product',
      price: 149.99,
      category: 'Electronics',
      imageUrl: '/images/test-product-2.jpg',
      inStock: true,
      createdAt: '2024-01-02T00:00:00Z',
    },
  ];

  // Filter by search term
  let filteredProducts = mockProducts;
  if (search) {
    filteredProducts = mockProducts.filter(
      product =>
        product.name.toLowerCase().includes(search.toString().toLowerCase()) ||
        product.description.toLowerCase().includes(search.toString().toLowerCase())
    );
  }

  // Filter by category
  if (category) {
    filteredProducts = filteredProducts.filter(
      product => product.category.toLowerCase() === category.toString().toLowerCase()
    );
  }

  // Pagination
  const startIndex = (parseInt(page.toString()) - 1) * parseInt(pageSize.toString());
  const endIndex = startIndex + parseInt(pageSize.toString());
  const paginatedProducts = filteredProducts.slice(startIndex, endIndex);

  res.json({
    items: paginatedProducts,
    totalCount: filteredProducts.length,
    page: parseInt(page.toString()),
    pageSize: parseInt(pageSize.toString()),
    totalPages: Math.ceil(filteredProducts.length / parseInt(pageSize.toString())),
  });
});

// Mock individual product endpoint
app.get('/api/catalog/products/:id', (req, res) => {
  const { id } = req.params;

  // Mock product data
  const mockProduct = {
    id: '550e8400-e29b-41d4-a716-446655440000',
    name: 'Test Product 1',
    description: 'A test product for integration testing',
    price: 99.99,
    category: 'Electronics',
    imageUrl: '/images/test-product-1.jpg',
    inStock: true,
    specifications: {
      brand: 'Test Brand',
      model: 'TP-001',
      warranty: '2 years',
    },
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  };

  if (id === '550e8400-e29b-41d4-a716-446655440000') {
    res.json(mockProduct);
  } else {
    res.status(404).json({ error: 'Product not found' });
  }
});

// Mock v1 products endpoint (legacy)
app.get('/api/v1/products', (req, res) => {
  res.json({
    products: [
      {
        id: '550e8400-e29b-41d4-a716-446655440000',
        name: 'Test Product 1 (v1)',
        price: 99.99,
      },
    ],
    total: 1,
  });
});

// Mock v2 products endpoint (development)
app.get('/api/v2/products', (req, res) => {
  res.json({
    data: [
      {
        id: '550e8400-e29b-41d4-a716-446655440000',
        name: 'Test Product 1 (v2)',
        price: 99.99,
        metadata: {
          version: '2.0',
        },
      },
    ],
    meta: {
      total: 1,
      page: 1,
      pageSize: 10,
    },
  });
});

// Mock search endpoint
app.post('/api/catalog/products/search', (req, res) => {
  const { query, filters } = req.body;

  const mockResults = [
    {
      id: '550e8400-e29b-41d4-a716-446655440000',
      name: 'Test Product 1',
      score: 0.95,
      highlights: ['Test Product'],
    },
  ];

  res.json({
    items: mockResults,
    total: 1,
    query: query,
    took: 15, // milliseconds
  });
});

// Error handling for invalid requests
app.get('/api/catalog/products', (req, res) => {
  const { page, pageSize } = req.query;

  if (page && parseInt(page.toString()) < 1) {
    return res.status(400).json({ error: 'Page must be greater than 0' });
  }

  if (pageSize && parseInt(pageSize.toString()) > 100) {
    return res.status(400).json({ error: 'Page size cannot exceed 100' });
  }

  // If no validation errors, return normal response
  res.json({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
  });
});

// Start server
app.listen(PORT, () => {
  console.log(`Mock API server running on http://localhost:${PORT}`);
  console.log('Available endpoints:');
  console.log('  GET  /health');
  console.log('  GET  /api/catalog/products');
  console.log('  GET  /api/catalog/products/:id');
  console.log('  GET  /api/v1/products');
  console.log('  GET  /api/v2/products');
  console.log('  POST /api/catalog/products/search');
});
