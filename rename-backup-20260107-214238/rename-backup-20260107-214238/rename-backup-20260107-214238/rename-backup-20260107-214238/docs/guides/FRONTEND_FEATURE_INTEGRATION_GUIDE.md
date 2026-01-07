# Frontend Feature Integration Guide

**Last Updated**: December 26, 2025  
**Status**: Production-Ready  
**Version**: 1.0.0

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [High Availability with CQRS & ElasticSearch](#high-availability-with-cqrs--elasticsearch)
4. [Provider-Based Pricing & Availability Model](#provider-based-pricing--availability-model)
5. [Feature Development Workflow](#feature-development-workflow)
6. [Core Modules](#core-modules)
7. [Integration Patterns](#integration-patterns)
8. [API Integration](#api-integration)
9. [State Management](#state-management)
10. [Component Development](#component-development)
11. [Routing & Navigation](#routing--navigation)
12. [Performance Optimization](#performance-optimization)
13. [SEO & Meta Tags](#seo--meta-tags)
14. [Testing Strategy](#testing-strategy)
15. [Common Use Cases](#common-use-cases)
16. [Troubleshooting](#troubleshooting)

---

## Overview

The Frontend is the customer-facing e-commerce interface for B2X. It provides:

- **Product Discovery**: Browse, search, filter products
- **Shopping Cart**: Add/remove items, manage quantities
- **Checkout**: Order placement with payment integration
- **User Account**: Registration, profile, order history
- **Order Management**: Track orders, manage returns
- **Multi-tenant Support**: Dynamic tenant branding
- **Internationalization (i18n)**: Multi-language support
- **Responsive Design**: Mobile-first approach
- **Theme Customization**: Dynamic theme switching

### Technology Stack

| Technology | Purpose |
|-----------|---------|
| **Vue 3** | Progressive framework |
| **TypeScript** | Type-safe development |
| **Pinia** | State management |
| **Vue Router** | Routing & navigation |
| **Axios** | HTTP client |
| **Tailwind CSS** | Styling |
| **Vite** | Build tooling |
| **Vitest** | Unit testing |
| **Playwright** | E2E testing |
| **vue-i18n** | Internationalization |

---

## Architecture

### Project Structure

```
frontend/
├── src/
│   ├── components/                 # Reusable components
│   │   ├── layout/                # Layout components
│   │   │   ├── Header.vue         # Navigation & header
│   │   │   ├── Footer.vue         # Footer
│   │   │   ├── Sidebar.vue        # Sidebar (mobile)
│   │   │   └── MainLayout.vue     # Main app layout
│   │   ├── common/                # Shared components
│   │   │   ├── Button.vue         # Button component
│   │   │   ├── Card.vue           # Card wrapper
│   │   │   ├── Modal.vue          # Modal dialog
│   │   │   ├── Pagination.vue     # Pagination
│   │   │   ├── LoadingSpinner.vue # Loading indicator
│   │   │   ├── Badge.vue          # Badge component
│   │   │   └── Toast.vue          # Toast notifications
│   │   ├── products/              # Product-specific
│   │   │   ├── ProductCard.vue    # Product preview
│   │   │   ├── ProductGrid.vue    # Product grid
│   │   │   ├── ProductFilter.vue  # Filter sidebar
│   │   │   ├── ProductSearch.vue  # Search bar
│   │   │   ├── RatingStars.vue    # Rating display
│   │   │   └── ReviewList.vue     # Customer reviews
│   │   ├── cart/                  # Shopping cart
│   │   │   ├── CartItem.vue       # Cart line item
│   │   │   ├── CartSummary.vue    # Cart totals
│   │   │   └── CartIcon.vue       # Cart badge
│   │   ├── checkout/              # Checkout flow
│   │   │   ├── ShippingForm.vue   # Shipping info
│   │   │   ├── BillingForm.vue    # Billing info
│   │   │   ├── PaymentForm.vue    # Payment method
│   │   │   └── OrderSummary.vue   # Order review
│   │   ├── auth/                  # Authentication
│   │   │   ├── LoginForm.vue      # Login form
│   │   │   ├── RegisterForm.vue   # Registration form
│   │   │   └── PasswordReset.vue  # Password reset
│   │   └── user/                  # User account
│   │       ├── ProfileForm.vue    # Profile editor
│   │       ├── AddressBook.vue    # Saved addresses
│   │       └── OrderHistory.vue   # Past orders
│   │
│   ├── views/                     # Page-level components (routes)
│   │   ├── Home.vue               # Homepage
│   │   ├── Products.vue           # Product listing
│   │   ├── ProductDetail.vue      # Single product
│   │   ├── Cart.vue               # Shopping cart
│   │   ├── Checkout.vue           # Checkout flow
│   │   ├── OrderConfirmation.vue  # Order success
│   │   ├── auth/
│   │   │   ├── Login.vue          # Login page
│   │   │   ├── Register.vue       # Registration page
│   │   │   └── PasswordReset.vue  # Reset password
│   │   ├── user/
│   │   │   ├── Profile.vue        # User profile
│   │   │   ├── Orders.vue         # Order history
│   │   │   ├── Addresses.vue      # Address management
│   │   │   └── Settings.vue       # User settings
│   │   ├── NotFound.vue           # 404 page
│   │   └── Error.vue              # Error page
│   │
│   ├── stores/                    # Pinia state management
│   │   ├── auth.ts                # Authentication state
│   │   ├── cart.ts                # Shopping cart state
│   │   ├── products.ts            # Products state
│   │   ├── orders.ts              # Orders state
│   │   ├── tenant.ts              # Tenant/theme state
│   │   ├── notifications.ts       # Toast notifications
│   │   └── filters.ts             # Product filters
│   │
│   ├── services/                  # API services
│   │   ├── client.ts              # Axios configuration
│   │   └── api/
│   │       ├── auth.ts            # Auth API
│   │       ├── products.ts        # Products API
│   │       ├── cart.ts            # Cart API
│   │       ├── orders.ts          # Orders API
│   │       ├── reviews.ts         # Reviews API
│   │       ├── user.ts            # User API
│   │       └── search.ts          # Search API
│   │
│   ├── types/                     # TypeScript interfaces
│   │   ├── auth.ts                # Auth types
│   │   ├── product.ts             # Product types
│   │   ├── cart.ts                # Cart types
│   │   ├── order.ts               # Order types
│   │   ├── review.ts              # Review types
│   │   ├── user.ts                # User types
│   │   ├── payment.ts             # Payment types
│   │   └── api.ts                 # Common API types
│   │
│   ├── router/                    # Routing configuration
│   │   ├── index.ts               # Router setup
│   │   ├── routes.ts              # Route definitions
│   │   └── guards.ts              # Route guards
│   │
│   ├── middleware/                # Custom middleware
│   │   ├── auth.ts                # Authentication guard
│   │   ├── tenant.ts              # Tenant context
│   │   └── checkout.ts            # Checkout validation
│   │
│   ├── composables/               # Reusable logic hooks
│   │   ├── useAuth.ts             # Auth utilities
│   │   ├── useCart.ts             # Cart utilities
│   │   ├── useSearch.ts           # Search utilities
│   │   ├── usePagination.ts       # Pagination logic
│   │   ├── useForm.ts             # Form handling
│   │   ├── useNotification.ts     # Toast notifications
│   │   ├── useFetch.ts            # Data fetching
│   │   ├── useLocalStorage.ts     # LocalStorage manager
│   │   └── useInfiniteScroll.ts   # Infinite scroll
│   │
│   ├── utils/                     # Utility functions
│   │   ├── constants.ts           # App constants
│   │   ├── format.ts              # Format utilities
│   │   ├── validation.ts          # Form validation
│   │   ├── currency.ts            # Currency formatting
│   │   ├── image.ts               # Image optimization
│   │   └── helpers.ts             # Helper functions
│   │
│   ├── directives/                # Custom Vue directives
│   │   ├── v-lazy.ts              # Lazy loading
│   │   ├── v-click-outside.ts     # Click outside
│   │   └── v-intersection.ts      # Intersection observer
│   │
│   ├── filters/                   # Vue filters
│   │   ├── currency.ts            # Currency filter
│   │   ├── date.ts                # Date filter
│   │   └── truncate.ts            # Text truncate
│   │
│   ├── locales/                   # i18n translations
│   │   ├── en.json                # English
│   │   ├── de.json                # German
│   │   └── fr.json                # French
│   │
│   ├── styles/                    # Global styles
│   │   ├── main.css               # Global CSS
│   │   ├── variables.css           # CSS variables
│   │   ├── animations.css         # Animations
│   │   └── responsive.css         # Responsive utilities
│   │
│   ├── App.vue                    # Root component
│   └── main.ts                    # Entry point
│
├── tests/
│   ├── setup.ts                   # Test configuration
│   ├── unit/                      # Unit tests
│   │   ├── stores/
│   │   ├── services/
│   │   ├── composables/
│   │   ├── utils/
│   │   └── filters/
│   ├── components/                # Component tests
│   └── e2e/                       # End-to-end tests
│       ├── shopping.spec.ts
│       ├── checkout.spec.ts
│       ├── auth.spec.ts
│       └── search.spec.ts
│
├── public/                        # Static assets
│   ├── images/
│   ├── icons/
│   └── favicon.ico
│
├── package.json
├── vite.config.ts
├── vitest.config.ts
├── playwright.config.ts
├── tsconfig.json
└── README.md
```

### Dependency Flow

```
Views/Pages
    ↓
Composables + Components
    ↓
Stores (Pinia)
    ↓
Services (API)
    ↓
HTTP Client (Axios)
    ↓
Backend API Gateway
```

---

## High Availability with CQRS & ElasticSearch

### Architecture Overview

Das Store-Frontend sichert Hochverfügbarkeit durch eine **Command Query Responsibility Segregation (CQRS)** Architektur mit **ElasticSearch** als zentraler Datenbasis für Lesezugriffe. Dies ermöglicht:

- **Skalierbare Lesezugriffe**: ElasticSearch optimiert für schnelle Suchabfragen
- **Entkopplung von Lese- und Schreibvorgängen**: Getrennte Datenflüsse
- **Echtzeit-Indexierungen**: Domain Events triggern Index-Updates
- **Fehlertoleranz**: Read-Replicas und automatische Failover
- **Performance**: Schnelle Abfragen ohne Auswirkung auf transaktionale Systeme

### System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    STORE FRONTEND (Vue 3)                       │
│  - Product Discovery                                             │
│  - Search & Filtering                                            │
│  - Category Browsing                                             │
└────────────┬────────────────────────────────────────────────────┘
             │ ReadCommand Queries
             ↓
┌─────────────────────────────────────────────────────────────────┐
│              API GATEWAY / SEARCH SERVICE                        │
│  - Query Router (CQRS ReadCommands)                              │
│  - Search Handler                                                │
└────────────┬────────────────────────────────────────────────────┘
             │ ElasticSearch Queries
             ↓
┌─────────────────────────────────────────────────────────────────┐
│              ELASTICSEARCH CLUSTER (Read Database)               │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐  │
│  │ Products Index   │  │ Categories Index │  │ Filters Index│  │
│  │ (Denormalized)   │  │ (Flat structure) │  │ (Fast access)│  │
│  └──────────────────┘  └──────────────────┘  └──────────────┘  │
└────────────┬────────────────────────────────────────────────────┘
             ↑
             │ Index Updates
             │
┌────────────┴─────────────────────────────────────────────────────┐
│                    EVENT BUS (Message Queue)                      │
│  ProductCreatedEvent                                              │
│  ProductUpdatedEvent                                              │
│  ProductDeletedEvent                                              │
│  CategoryUpdatedEvent                                             │
│  InventoryChangedEvent                                            │
└────────────┬────────────────────────────────────────────────────┘
             ↑
             │ Domain Events Published
             │
┌────────────┴─────────────────────────────────────────────────────┐
│              ADMIN FRONTEND & BACKEND SERVICES                    │
│  ┌───────────────────────────┐  ┌───────────────────────────┐   │
│  │   Admin-Frontend          │  │   Catalog Service (Core)  │   │
│  │  - Product Management     │  │  - Write Operations       │   │
│  │  - Category Management    │  │  - Command Handlers       │   │
│  │  - Inventory Updates      │  │  - Domain Logic           │   │
│  └───────────────────────────┘  └───────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

### Data Flow: Read Path (Store Frontend)

```
┌─────────────────────────┐
│  User Action            │
│  - Search Product       │
│  - Filter by Category   │
│  - Sort by Price        │
└────────┬────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Frontend Component                     │
│  <SearchBar @search="onSearch" />       │
│  <ProductFilter @filter="onFilter" />   │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Pinia Store (useProductsStore)         │
│  - Holds search query                   │
│  - Manages filter state                 │
│  - Tracks loading state                 │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  API Service (searchApi.ts)             │
│  ReadCommand Request                    │
│  {                                      │
│    query: "laptop",                     │
│    filters: {                           │
│      category: "electronics",           │
│      minPrice: 500,                     │
│      maxPrice: 2000                     │
│    },                                   │
│    page: 1,                             │
│    size: 20                             │
│  }                                      │
└────────┬────────────────────────────────┘
         │ HTTP POST /api/search
         ↓
┌─────────────────────────────────────────┐
│  Gateway / Search Service               │
│  - Validate ReadCommand                 │
│  - Build ElasticSearch Query             │
│  - Handle Tenant Context                │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  ElasticSearch Cluster                  │
│  GET /products/_search                   │
│  {                                      │
│    "query": {                           │
│      "bool": {                          │
│        "must": [                        │
│          {                              │
│            "multi_match": {             │
│              "query": "laptop",         │
│              "fields": ["name", "desc"] │
│            }                            │
│          }                              │
│        ],                               │
│        "filter": [                      │
│          { "term": { "category": ... }},│
│          { "range": { "price": {...} }} │
│        ]                                │
│      }                                  │
│    },                                   │
│    "from": 0,                           │
│    "size": 20                           │
│  }                                      │
└────────┬────────────────────────────────┘
         │ Search Results (< 100ms)
         ↓
┌─────────────────────────────────────────┐
│  Response                               │
│  [                                      │
│    {                                    │
│      id: "prod-123",                    │
│      name: "Laptop Pro",                │
│      price: 1299,                       │
│      category: "electronics",           │
│      images: [...],                     │
│      rating: 4.8                        │
│    },                                   │
│    ...                                  │
│  ]                                      │
└────────┬────────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────────┐
│  Frontend Store & Components            │
│  - Update products array                │
│  - Re-render search results             │
│  - Show loading indicator               │
└─────────────────────────────────────────┘
```

### Data Flow: Write Path (Domain Events)

```
┌──────────────────────────────────────┐
│  ADMIN FRONTEND EVENT                │
│  User edits product in admin panel    │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  Catalog Service Command             │
│  UpdateProductCommand                │
│  {                                   │
│    productId: "prod-123",            │
│    name: "Laptop Pro Max",           │
│    price: 1499,                      │
│    category: "electronics",          │
│    tenantId: "tenant-1"              │
│  }                                   │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  WRITE OPERATION (Database)          │
│  - Update Product in PostgreSQL       │
│  - Update inventory if needed         │
│  - Increment version number           │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  DOMAIN EVENT PUBLISHED              │
│  ProductUpdatedEvent                 │
│  {                                   │
│    eventId: "evt-456",               │
│    aggregateId: "prod-123",          │
│    aggregateType: "Product",         │
│    eventType: "ProductUpdated",      │
│    timestamp: "2025-12-26T10:30:00", │
│    tenantId: "tenant-1",             │
│    data: {                           │
│      productId: "prod-123",          │
│      changes: {                      │
│        name: "Laptop Pro Max",       │
│        price: 1499                   │
│      }                               │
│    }                                 │
│  }                                   │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  MESSAGE BUS / EVENT QUEUE           │
│  - RabbitMQ / Azure Service Bus      │
│  - Topics: domain.product.*          │
│  - Retention: 7 days                 │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  BACKGROUND SERVICE                  │
│  Search Index Updater                │
│  - Consumes ProductUpdatedEvent      │
│  - Fetches full product details      │
│  - Enriches data (prices, images)    │
│  - Updates ElasticSearch index       │
└────────┬─────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────┐
│  ELASTICSEARCH OPERATION             │
│  Index Update                        │
│  PUT /products/_doc/prod-123        │
│  {                                   │
│    "id": "prod-123",                 │
│    "name": "Laptop Pro Max",         │
│    "description": "...",             │
│    "price": 1499,                    │
│    "category": "electronics",        │
│    "images": [...],                  │
│    "rating": 4.8,                    │
│    "inStock": true,                  │
│    "updatedAt": "2025-12-26T10:30",  │
│    "tenantId": "tenant-1"            │
│  }                                   │
└────────┬─────────────────────────────┘
         │ Updated Index
         ↓
┌──────────────────────────────────────┐
│  STORE FRONTEND SEES NEW DATA        │
│  Next search returns updated result  │
│  - Automatic refresh on page focus   │
│  - Real-time via WebSocket if needed │
└──────────────────────────────────────┘
```

### Implementation: ReadCommands

**File**: `backend/services/Catalog/Application/ReadCommands/SearchProductsReadCommand.cs`

```csharp
namespace B2X.Catalog.Application.ReadCommands
{
    /// <summary>
    /// Read command for searching products via ElasticSearch
    /// Part of CQRS pattern - READ side optimization
    /// </summary>
    public class SearchProductsReadCommand : IRequest<SearchResultsResponse>
    {
        public string TenantId { get; init; }
        public string Query { get; init; }
        public ProductFilters Filters { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string SortBy { get; init; } = "relevance";
    }

    public class SearchProductsReadCommandHandler 
        : IRequestHandler<SearchProductsReadCommand, SearchResultsResponse>
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<SearchProductsReadCommandHandler> _logger;

        public SearchProductsReadCommandHandler(
            IElasticClient elasticClient,
            ILogger<SearchProductsReadCommandHandler> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<SearchResultsResponse> Handle(
            SearchProductsReadCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Searching products: Query={Query}, TenantId={TenantId}",
                request.Query,
                request.TenantId);

            // Build ElasticSearch query
            var searchDescriptor = new SearchDescriptor<ProductSearchDocument>()
                .Index($"products-{request.TenantId.ToLower()}")
                .Query(q => q
                    .Bool(b => b
                        .Must(m =>
                        {
                            if (!string.IsNullOrEmpty(request.Query))
                            {
                                m = m.MultiMatch(mm => mm
                                    .Query(request.Query)
                                    .Fields(f => f
                                        .Field(p => p.Name, 2.0)
                                        .Field(p => p.Description, 1.0)
                                        .Field(p => p.Category, 1.5)));
                            }
                            return m;
                        })
                        .Filter(ApplyFilters(request.Filters))))
                .Sort(s => ApplySorting(s, request.SortBy))
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .Highlight(h => h
                    .Fields(f => f.Field(p => p.Name), f => f.Field(p => p.Description)));

            var response = await _elasticClient.SearchAsync<ProductSearchDocument>(
                searchDescriptor,
                cancellationToken);

            if (!response.IsValid)
            {
                _logger.LogError("ElasticSearch error: {Error}", response.ServerError?.Error?.Reason);
                throw new SearchException("Search operation failed");
            }

            return new SearchResultsResponse
            {
                Items = response.Documents.Select(d => new ProductSearchResult
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    Category = d.Category,
                    Images = d.Images,
                    Rating = d.Rating,
                    ReviewCount = d.ReviewCount,
                    InStock = d.InStock,
                    Highlight = GetHighlight(response, d.Id)
                }).ToList(),
                Total = (int)response.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(response.Total / (double)request.PageSize)
            };
        }

        private QueryContainer ApplyFilters(ProductFilters filters)
        {
            var queries = new List<QueryContainer>();

            if (filters.CategoryIds?.Any() == true)
            {
                queries.Add(Query<ProductSearchDocument>.Terms(t =>
                    t.Field(p => p.CategoryId).Terms(filters.CategoryIds)));
            }

            if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
            {
                queries.Add(Query<ProductSearchDocument>.Range(r =>
                    r.Field(p => p.Price)
                        .GreaterThanOrEquals(filters.MinPrice)
                        .LessThanOrEquals(filters.MaxPrice)));
            }

            if (filters.MinRating.HasValue)
            {
                queries.Add(Query<ProductSearchDocument>.Range(r =>
                    r.Field(p => p.Rating).GreaterThanOrEquals(filters.MinRating)));
            }

            if (filters.InStockOnly)
            {
                queries.Add(Query<ProductSearchDocument>.Term(t =>
                    t.Field(p => p.InStock).Value(true)));
            }

            return queries.Any() ? Query<ProductSearchDocument>.Bool(b => b.Filter(queries)) : null;
        }

        private SortDescriptor<ProductSearchDocument> ApplySorting(
            SortDescriptor<ProductSearchDocument> sortDescriptor,
            string sortBy)
        {
            return sortBy switch
            {
                "price-asc" => sortDescriptor.Field(f => f.Price, SortOrder.Ascending),
                "price-desc" => sortDescriptor.Field(f => f.Price, SortOrder.Descending),
                "newest" => sortDescriptor.Field(f => f.CreatedAt, SortOrder.Descending),
                "rating" => sortDescriptor.Field(f => f.Rating, SortOrder.Descending),
                _ => sortDescriptor.Score(ScoreOrder.Descending) // Default: relevance
            };
        }
    }
}
```

### Implementation: Domain Event Handler

**File**: `backend/services/Catalog/Application/EventHandlers/ProductUpdatedEventHandler.cs`

```csharp
namespace B2X.Catalog.Application.EventHandlers
{
    /// <summary>
    /// Background service that listens to domain events
    /// and updates the ElasticSearch index in real-time
    /// </summary>
    public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductUpdatedEventHandler> _logger;

        public ProductUpdatedEventHandler(
            IProductRepository productRepository,
            IElasticClient elasticClient,
            ILogger<ProductUpdatedEventHandler> logger)
        {
            _productRepository = productRepository;
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Handling ProductUpdatedEvent for ProductId={ProductId}, TenantId={TenantId}",
                    notification.AggregateId,
                    notification.TenantId);

                // Fetch the updated product with all details
                var product = await _productRepository.GetByIdAsync(
                    notification.AggregateId,
                    cancellationToken);

                if (product == null)
                {
                    _logger.LogWarning("Product not found: {ProductId}", notification.AggregateId);
                    return;
                }

                // Build search document from the product aggregate
                var searchDocument = new ProductSearchDocument
                {
                    Id = product.Id,
                    TenantId = product.TenantId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Category = product.Category.Name,
                    CategoryId = product.Category.Id,
                    Sku = product.Sku,
                    InStock = product.Stock > 0,
                    Stock = product.Stock,
                    Images = product.Images.OrderBy(i => i.Order)
                        .Select(i => new ImageSearchDocument 
                        { 
                            Url = i.Url, 
                            Alt = i.Alt,
                            IsPrimary = i.IsPrimary
                        }).ToList(),
                    Rating = product.AverageRating,
                    ReviewCount = product.ReviewCount,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    Tags = product.Tags?.Split(',') ?? Array.Empty<string>()
                };

                // Update index in ElasticSearch
                var indexName = $"products-{product.TenantId.ToLower()}";
                var response = await _elasticClient.IndexAsync(
                    searchDocument,
                    i => i.Index(indexName).Id(product.Id),
                    cancellationToken);

                if (response.IsValid)
                {
                    _logger.LogInformation(
                        "Successfully indexed product {ProductId} in {Index}",
                        product.Id,
                        indexName);
                }
                else
                {
                    _logger.LogError(
                        "Failed to index product {ProductId}: {Error}",
                        product.Id,
                        response.ServerError?.Error?.Reason);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ProductUpdatedEvent");
                throw;
            }
        }
    }
}
```

### Implementation: Frontend Search Service

**File**: `src/services/api/search.ts`

```typescript
// src/services/api/search.ts

import { client } from '@/services/client';
import type { Product, ProductFilter } from '@/types/product';

export interface SearchRequest {
  query?: string;
  filters?: ProductFilter;
  page?: number;
  pageSize?: number;
  sortBy?: 'relevance' | 'price-asc' | 'price-desc' | 'newest' | 'rating';
}

export interface SearchResponse {
  items: Product[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  highlights?: Map<string, string[]>; // Highlighted search terms
}

export const searchApi = {
  /**
   * Search products using CQRS ReadCommand
   * Queries are handled by ElasticSearch for high performance
   */
  async search(request: SearchRequest): Promise<SearchResponse> {
    return client.post<SearchResponse>('/api/products/search', {
      query: request.query,
      filters: {
        categoryIds: request.filters?.category ? [request.filters.category] : [],
        minPrice: request.filters?.minPrice,
        maxPrice: request.filters?.maxPrice,
        minRating: request.filters?.rating,
        inStockOnly: request.filters?.inStock || false
      },
      page: request.page || 1,
      pageSize: request.pageSize || 20,
      sortBy: request.sortBy || 'relevance'
    });
  },

  /**
   * Get autocomplete suggestions while typing
   * Also uses ElasticSearch for instant responses
   */
  async autocomplete(
    query: string,
    limit: number = 5
  ): Promise<string[]> {
    return client.get('/api/products/autocomplete', {
      params: { q: query, limit }
    });
  },

  /**
   * Get available filter options
   * Cached from ElasticSearch aggregations
   */
  async getFilters(): Promise<{
    categories: Category[];
    priceRange: { min: number; max: number };
    ratings: number[];
  }> {
    return client.get('/api/products/filters');
  }
};
```

### Frontend Integration: Product Search Component

```vue
<!-- src/components/products/ProductSearch.vue -->

<template>
  <div class="search-section space-y-4">
    <!-- Search Input with Autocomplete -->
    <div class="relative">
      <input
        v-model="searchQuery"
        @input="handleSearchInput"
        type="text"
        placeholder="Search products..."
        class="w-full px-4 py-2 border rounded-lg"
        autocomplete="off"
      />
      
      <!-- Autocomplete Dropdown -->
      <div v-if="showAutocomplete && suggestions.length" class="absolute top-full mt-1 w-full bg-white border rounded shadow-lg">
        <div
          v-for="suggestion in suggestions"
          :key="suggestion"
          @click="selectSuggestion(suggestion)"
          class="px-4 py-2 hover:bg-gray-100 cursor-pointer"
        >
          {{ suggestion }}
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div v-if="showFilters" class="space-y-4">
      <!-- Category Filter -->
      <div>
        <h3 class="font-semibold mb-2">Category</h3>
        <div class="space-y-2">
          <label v-for="category in categories" :key="category.id" class="flex items-center">
            <input
              type="checkbox"
              :checked="selectedCategory === category.id"
              @change="selectCategory(category.id)"
            />
            <span class="ml-2">{{ category.name }}</span>
          </label>
        </div>
      </div>

      <!-- Price Range Filter -->
      <div>
        <h3 class="font-semibold mb-2">Price Range</h3>
        <div class="space-y-2">
          <label>
            Min: <input
              v-model.number="minPrice"
              type="number"
              @change="performSearch"
              class="w-20 px-2 py-1 border rounded"
            />
          </label>
          <label>
            Max: <input
              v-model.number="maxPrice"
              type="number"
              @change="performSearch"
              class="w-20 px-2 py-1 border rounded"
            />
          </label>
        </div>
      </div>

      <!-- Rating Filter -->
      <div>
        <h3 class="font-semibold mb-2">Rating</h3>
        <select
          v-model.number="minRating"
          @change="performSearch"
          class="w-full px-2 py-1 border rounded"
        >
          <option :value="null">All ratings</option>
          <option :value="4">4+ stars</option>
          <option :value="3">3+ stars</option>
          <option :value="2">2+ stars</option>
          <option :value="1">1+ stars</option>
        </select>
      </div>

      <!-- In Stock Only -->
      <label class="flex items-center">
        <input
          v-model="inStockOnly"
          type="checkbox"
          @change="performSearch"
        />
        <span class="ml-2">In Stock Only</span>
      </label>
    </div>

    <!-- Sort Options -->
    <select
      v-model="sortBy"
      @change="performSearch"
      class="w-full px-4 py-2 border rounded"
    >
      <option value="relevance">Most Relevant</option>
      <option value="newest">Newest</option>
      <option value="price-asc">Price: Low to High</option>
      <option value="price-desc">Price: High to Low</option>
      <option value="rating">Highest Rated</option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { searchApi } from '@/services/api/search';
import { useSearch } from '@/composables/useSearch';

const emit = defineEmits<{
  'search-results': [results: any];
}>();

const { search: debouncedSearch } = useSearch();

const searchQuery = ref('');
const suggestions = ref<string[]>([]);
const showAutocomplete = ref(false);
const showFilters = ref(false);
const categories = ref<any[]>([]);
const selectedCategory = ref<string | null>(null);
const minPrice = ref<number | null>(null);
const maxPrice = ref<number | null>(null);
const minRating = ref<number | null>(null);
const inStockOnly = ref(false);
const sortBy = ref<'relevance' | 'price-asc' | 'price-desc' | 'newest' | 'rating'>('relevance');
const loading = ref(false);

const handleSearchInput = async (event: Event) => {
  const query = (event.target as HTMLInputElement).value;
  
  if (query.length < 2) {
    showAutocomplete.value = false;
    return;
  }

  // Get autocomplete suggestions
  suggestions.value = await searchApi.autocomplete(query, 5);
  showAutocomplete.value = suggestions.value.length > 0;

  // Perform debounced search
  await performSearch();
};

const selectSuggestion = (suggestion: string) => {
  searchQuery.value = suggestion;
  showAutocomplete.value = false;
  performSearch();
};

const selectCategory = (categoryId: string) => {
  selectedCategory.value = selectedCategory.value === categoryId ? null : categoryId;
  performSearch();
};

const performSearch = async () => {
  loading.value = true;
  try {
    const results = await searchApi.search({
      query: searchQuery.value || undefined,
      filters: {
        category: selectedCategory.value || undefined,
        minPrice: minPrice.value || undefined,
        maxPrice: maxPrice.value || undefined,
        rating: minRating.value || undefined,
        inStock: inStockOnly.value
      },
      pageSize: 20,
      sortBy: sortBy.value
    });

    emit('search-results', results);
  } finally {
    loading.value = false;
  }
};

// Initialize available filters
const initializeFilters = async () => {
  const filters = await searchApi.getFilters();
  categories.value = filters.categories;
};

onMounted(initializeFilters);
</script>
```

### Event Publishing Configuration

**File**: `backend/services/Catalog/Infrastructure/EventPublishing/DomainEventPublisher.cs`

```csharp
namespace B2X.Catalog.Infrastructure.EventPublishing
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint; // MassTransit
        private readonly ILogger<DomainEventPublisher> _logger;

        public async Task PublishAsync<TDomainEvent>(
            TDomainEvent domainEvent,
            CancellationToken cancellationToken = default)
            where TDomainEvent : IDomainEvent
        {
            try
            {
                _logger.LogInformation(
                    "Publishing domain event: {EventType}, AggregateId: {AggregateId}",
                    typeof(TDomainEvent).Name,
                    domainEvent.AggregateId);

                // Publish to message bus (RabbitMQ, Azure Service Bus, etc.)
                await _publishEndpoint.Publish(domainEvent, cancellationToken);

                _logger.LogInformation(
                    "Successfully published: {EventType}",
                    typeof(TDomainEvent).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish domain event");
                throw;
            }
        }
    }
}
```

### Key Benefits

| Benefit | Description |
|---------|-------------|
| **Hochleistung** | ElasticSearch ermöglicht < 100ms Suchabfragen auf Millionen von Produkten |
| **Skalierbarkeit** | Lesevorgänge sind unabhängig von Schreibvorgängen skalierbar |
| **Fehlertoleranz** | ElasticSearch Replicas ermöglichen automatisches Failover |
| **Echtzeit-Updates** | Domain Events triggern sofortige Index-Updates |
| **Konsistenz** | Eventual Consistency Model mit konfigurierbarem TTL |
| **Tenancy** | Separate Indizes pro Tenant für Datenisolation |
| **Monitoring** | ElasticSearch bietet detailliertes Query-Monitoring |

### Monitoring & Debugging

```typescript
// Überwachen Sie ElasticSearch-Abfragen
POST /elasticsearch-monitoring/search-queries
{
  "timestamp": "2025-12-26T10:30:00Z",
  "query": "laptop",
  "execution_time_ms": 45,
  "hits": 1250,
  "tenant_id": "tenant-1"
}
```

---

## Provider-Based Pricing & Availability Model

### Konzept-Übersicht

Preise und Verfügbarkeit werden **nicht direkt auf den Produkten gespeichert**. Stattdessen wird ein **Provider-Konzept** implementiert, das die flexible Anbindung an verschiedene **ERP-Systeme** ermöglicht. Dies erlaubt:

- **Multi-Supplier Support**: Verschiedene Lieferanten/ERP-Systeme
- **Flexible Preisgestaltung**: Dynamische Preise pro Provider
- **Echtzeit-Verfügbarkeit**: Live-Bestands-Daten
- **Tenant-spezifische Provider**: Unterschiedliche ERP pro Mandant
- **Fallback-Mechanismen**: Automatischer Wechsel bei Ausfällen
- **Cache-Strategien**: Optimiert für Performance

### System-Architektur

```
┌──────────────────────────────────────────────────────────────┐
│                    STORE FRONTEND                            │
│  Zeigt Produkte mit Preisen & Verfügbarkeit                  │
└────────┬─────────────────────────────────────────────────────┘
         │ Produkt-ID + Provider-Präferenz
         ↓
┌──────────────────────────────────────────────────────────────┐
│         PROVIDER RESOLVER SERVICE (Query Handler)            │
│  - Bestimmt beste Provider basierend auf Tenant-Config       │
│  - Verwaltet Provider-Prioritäten                            │
│  - Handhabt Fallback-Logik                                   │
└────────┬─────────────────────────────────────────────────────┘
         │
         ├─────────────┬──────────────────┬─────────────┐
         ↓             ↓                  ↓             ↓
    ┌────────┐   ┌────────┐        ┌──────────┐  ┌──────────┐
    │Provider│   │Provider│        │ Provider │  │ Cache    │
    │1: SAP  │   │2: Oracle│       │ 3: NAV   │  │ (Redis)  │
    │ERP     │   │ERP      │       │ ERP      │  │ Layer    │
    └────┬───┘   └────┬────┘       └────┬─────┘  └────┬─────┘
         │            │                 │             │
         │ HTTP API   │ REST API        │ SOAP API    │
         ↓            ↓                 ↓             ↓
    ┌──────────────────────────────────────────────────────────┐
    │           ERP-SYSTEME (Externe Datenquellen)            │
    │  - Bestandsverwaltung                                    │
    │  - Preisberechnung                                       │
    │  - Verfügbarkeitsprüfung                                 │
    │  - Lieferantenmanagement                                 │
    └──────────────────────────────────────────────────────────┘
```

### Provider Interface Definition

**File**: `backend/services/Catalog/Core/Domain/Providers/IInventoryProvider.cs`

```csharp
namespace B2X.Catalog.Core.Domain.Providers
{
    /// <summary>
    /// Provider interface for retrieving pricing and availability data
    /// from external ERP systems
    /// </summary>
    public interface IInventoryProvider
    {
        /// <summary>
        /// Provider-ID (z.B. "sap", "oracle", "nav")
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Priorität für diesen Provider (höher = bevorzugt)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Ob dieser Provider verfügbar/konfiguriert ist
        /// </summary>
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Hole Preis und Verfügbarkeit für ein Produkt
        /// </summary>
        Task<ProductAvailabilityData> GetProductAvailabilityAsync(
            string productSku,
            string tenantId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Hole Verfügbarkeitsdaten für mehrere Produkte (Batch)
        /// </summary>
        Task<IEnumerable<ProductAvailabilityData>> GetProductsAvailabilityAsync(
            IEnumerable<string> productSkus,
            string tenantId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Fehlerbehandlung und Retry-Logik
        /// </summary>
        Task<bool> HealthCheckAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Verfügbarkeitsdaten von einem Provider
    /// </summary>
    public class ProductAvailabilityData
    {
        public string ProductSku { get; init; }
        public decimal Price { get; init; }
        public decimal? DiscountPrice { get; init; }
        public string Currency { get; init; } = "EUR";
        public int AvailableQuantity { get; init; }
        public bool IsInStock => AvailableQuantity > 0;
        public DateTime? ExpectedRestock { get; init; }
        public string ProviderId { get; init; }
        public DateTime FetchedAt { get; init; }
        public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(5);
    }
}
```

### Provider Implementierungen

#### SAP ERP Provider

**File**: `backend/services/Catalog/Infrastructure/Providers/SapErpProvider.cs`

```csharp
namespace B2X.Catalog.Infrastructure.Providers
{
    public class SapErpProvider : IInventoryProvider
    {
        public string ProviderId => "sap-erp";
        public int Priority => 100; // Höchste Priorität

        private readonly HttpClient _httpClient;
        private readonly SapErpConfiguration _config;
        private readonly ICache _cache;
        private readonly ILogger<SapErpProvider> _logger;

        public SapErpProvider(
            HttpClient httpClient,
            IOptions<SapErpConfiguration> config,
            ICache cache,
            ILogger<SapErpProvider> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await HealthCheckAsync(cancellationToken);
                return response;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ProductAvailabilityData> GetProductAvailabilityAsync(
            string productSku,
            string tenantId,
            CancellationToken cancellationToken)
        {
            // Versuche aus Cache zu lesen
            var cacheKey = $"inventory:{ProviderId}:{tenantId}:{productSku}";
            if (_cache.TryGet(cacheKey, out ProductAvailabilityData cached))
            {
                _logger.LogDebug("Inventory cache hit for {ProductSku}", productSku);
                return cached;
            }

            try
            {
                _logger.LogInformation(
                    "Fetching availability from SAP for SKU: {Sku}, Tenant: {TenantId}",
                    productSku,
                    tenantId);

                // SAP REST API aufrufen
                var request = new
                {
                    sku = productSku,
                    tenantId = tenantId,
                    includePrice = true,
                    includeStock = true
                };

                var response = await _httpClient.PostAsync(
                    $"{_config.BaseUrl}/api/products/availability",
                    new StringContent(
                        JsonSerializer.Serialize(request),
                        Encoding.UTF8,
                        "application/json"),
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<SapAvailabilityResponse>(content);

                var result = new ProductAvailabilityData
                {
                    ProductSku = productSku,
                    Price = data.Price,
                    DiscountPrice = data.DiscountPrice,
                    Currency = data.Currency,
                    AvailableQuantity = data.Stock,
                    ProviderId = ProviderId,
                    FetchedAt = DateTime.UtcNow,
                    ExpectedRestock = data.ExpectedRestockDate,
                    CacheDuration = TimeSpan.FromMinutes(_config.CacheDurationMinutes)
                };

                // Cache result
                _cache.Set(cacheKey, result, result.CacheDuration);

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SAP API error for SKU: {Sku}", productSku);
                throw new ProviderException($"SAP provider failed for {productSku}", ex);
            }
        }

        public async Task<IEnumerable<ProductAvailabilityData>> GetProductsAvailabilityAsync(
            IEnumerable<string> productSkus,
            string tenantId,
            CancellationToken cancellationToken)
        {
            // Batch-Anfrage für bessere Performance
            var skuList = productSkus.ToList();
            var results = new List<ProductAvailabilityData>();

            foreach (var sku in skuList)
            {
                try
                {
                    var data = await GetProductAvailabilityAsync(sku, tenantId, cancellationToken);
                    results.Add(data);
                }
                catch (ProviderException ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch availability for SKU: {Sku}", sku);
                    // Continue with next SKU
                }
            }

            return results;
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_config.BaseUrl}/health",
                    cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class SapAvailabilityResponse
    {
        [JsonPropertyName("price")]
        public decimal Price { get; init; }

        [JsonPropertyName("discountPrice")]
        public decimal? DiscountPrice { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("stock")]
        public int Stock { get; init; }

        [JsonPropertyName("expectedRestockDate")]
        public DateTime? ExpectedRestockDate { get; init; }
    }

    public class SapErpConfiguration
    {
        public string BaseUrl { get; init; }
        public string AuthToken { get; init; }
        public int TimeoutSeconds { get; init; } = 10;
        public int CacheDurationMinutes { get; init; } = 5;
        public int MaxRetries { get; init; } = 3;
    }
}
```

#### Oracle ERP Provider

**File**: `backend/services/Catalog/Infrastructure/Providers/OracleErpProvider.cs`

```csharp
namespace B2X.Catalog.Infrastructure.Providers
{
    public class OracleErpProvider : IInventoryProvider
    {
        public string ProviderId => "oracle-erp";
        public int Priority => 90; // Mittlere Priorität

        private readonly OracleDbConnection _connection;
        private readonly ICache _cache;
        private readonly ILogger<OracleErpProvider> _logger;

        public OracleErpProvider(
            OracleDbConnection connection,
            ICache cache,
            ILogger<OracleErpProvider> logger)
        {
            _connection = connection;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ProductAvailabilityData> GetProductAvailabilityAsync(
            string productSku,
            string tenantId,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"inventory:{ProviderId}:{tenantId}:{productSku}";
            
            if (_cache.TryGet(cacheKey, out ProductAvailabilityData cached))
            {
                return cached;
            }

            try
            {
                // Oracle DB-Abfrage über direkte Datenbankverbindung
                var query = @"
                    SELECT 
                        price,
                        discount_price,
                        currency,
                        available_stock,
                        restock_date
                    FROM products_inventory
                    WHERE sku = :sku AND tenant_id = :tenantId";

                using var cmd = _connection.CreateCommand(query);
                cmd.Parameters.AddWithValue(":sku", productSku);
                cmd.Parameters.AddWithValue(":tenantId", tenantId);

                var result = await cmd.ExecuteReaderAsync(cancellationToken);

                if (await result.ReadAsync(cancellationToken))
                {
                    var data = new ProductAvailabilityData
                    {
                        ProductSku = productSku,
                        Price = result.GetDecimal(0),
                        DiscountPrice = result.IsDBNull(1) ? null : result.GetDecimal(1),
                        Currency = result.GetString(2),
                        AvailableQuantity = result.GetInt32(3),
                        ProviderId = ProviderId,
                        FetchedAt = DateTime.UtcNow,
                        ExpectedRestock = result.IsDBNull(4) ? null : result.GetDateTime(4),
                        CacheDuration = TimeSpan.FromMinutes(5)
                    };

                    _cache.Set(cacheKey, data, data.CacheDuration);
                    return data;
                }

                throw new ProviderException($"Product {productSku} not found in Oracle");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oracle provider error for SKU: {Sku}", productSku);
                throw new ProviderException("Oracle provider failed", ex);
            }
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var cmd = _connection.CreateCommand("SELECT 1");
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        // ... weitere Methoden
    }
}
```

#### enventa Trade ERP Provider

**File**: `backend/services/Catalog/Infrastructure/Providers/EventaTradeErpProvider.cs`

```csharp
namespace B2X.Catalog.Infrastructure.Providers
{
    /// <summary>
    /// Provider for enventa Trade ERP integration
    /// Connects to enventa API for real-time pricing and inventory data
    /// </summary>
    public class EventaTradeErpProvider : IInventoryProvider
    {
        public string ProviderId => "eventa-trade";
        public int Priority => 85; // Hohe Priorität

        private readonly HttpClient _httpClient;
        private readonly EventaTradeConfiguration _config;
        private readonly ICache _cache;
        private readonly ILogger<EventaTradeErpProvider> _logger;

        public EventaTradeErpProvider(
            HttpClient httpClient,
            IOptions<EventaTradeConfiguration> config,
            ICache cache,
            ILogger<EventaTradeErpProvider> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await HealthCheckAsync(cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "enventa Trade provider health check failed");
                return false;
            }
        }

        public async Task<ProductAvailabilityData> GetProductAvailabilityAsync(
            string productSku,
            string tenantId,
            CancellationToken cancellationToken)
        {
            // Versuche aus Cache zu lesen
            var cacheKey = $"inventory:{ProviderId}:{tenantId}:{productSku}";
            if (_cache.TryGet(cacheKey, out ProductAvailabilityData cached))
            {
                _logger.LogDebug("Inventory cache hit for {ProductSku} from enventa", productSku);
                return cached;
            }

            try
            {
                _logger.LogInformation(
                    "Fetching availability from enventa Trade for SKU: {Sku}, Tenant: {TenantId}",
                    productSku,
                    tenantId);

                // Authentifizierung via API-Key
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _config.ApiKey);

                // enventa Trade API aufrufen
                var request = new EventaProductRequest
                {
                    Sku = productSku,
                    TenantId = tenantId,
                    IncludePrice = true,
                    IncludeStock = true,
                    IncludeBundle = false
                };

                var response = await _httpClient.PostAsync(
                    $"{_config.BaseUrl}/api/v2/products/availability",
                    new StringContent(
                        JsonSerializer.Serialize(request),
                        Encoding.UTF8,
                        "application/json"),
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("Product not found in enventa: {Sku}", productSku);
                        throw new ProviderException($"Product {productSku} not found in enventa");
                    }

                    _logger.LogError(
                        "enventa API error: {StatusCode} - {Message}",
                        response.StatusCode,
                        await response.Content.ReadAsStringAsync(cancellationToken));

                    throw new HttpRequestException($"enventa API returned {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<EventaAvailabilityResponse>(content);

                if (data == null)
                {
                    throw new ProviderException("Invalid response from enventa");
                }

                var result = new ProductAvailabilityData
                {
                    ProductSku = productSku,
                    Price = data.Price.NetPrice,
                    DiscountPrice = data.Price.GrossPrice != data.Price.NetPrice ? data.Price.GrossPrice : null,
                    Currency = data.Price.Currency ?? "EUR",
                    AvailableQuantity = data.Inventory.AvailableQuantity,
                    ProviderId = ProviderId,
                    FetchedAt = DateTime.UtcNow,
                    ExpectedRestock = data.Inventory.ExpectedRestockDate,
                    CacheDuration = TimeSpan.FromMinutes(_config.CacheDurationMinutes)
                };

                // Cache result
                _cache.Set(cacheKey, result, result.CacheDuration);

                _logger.LogInformation(
                    "Successfully fetched availability from enventa for {Sku}: {Quantity} in stock",
                    productSku,
                    result.AvailableQuantity);

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "enventa API communication error for SKU: {Sku}", productSku);
                throw new ProviderException($"enventa provider failed for {productSku}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse enventa response for SKU: {Sku}", productSku);
                throw new ProviderException("Invalid response format from enventa", ex);
            }
        }

        public async Task<IEnumerable<ProductAvailabilityData>> GetProductsAvailabilityAsync(
            IEnumerable<string> productSkus,
            string tenantId,
            CancellationToken cancellationToken)
        {
            // enventa Trade unterstützt auch Batch-Abfragen
            var skuList = productSkus.ToList();
            var results = new List<ProductAvailabilityData>();

            try
            {
                _logger.LogInformation(
                    "Fetching batch availability from enventa for {Count} products",
                    skuList.Count);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _config.ApiKey);

                var batchRequest = new EventaBatchRequest
                {
                    Skus = skuList,
                    TenantId = tenantId,
                    IncludePrice = true,
                    IncludeStock = true
                };

                var response = await _httpClient.PostAsync(
                    $"{_config.BaseUrl}/api/v2/products/availability/batch",
                    new StringContent(
                        JsonSerializer.Serialize(batchRequest),
                        Encoding.UTF8,
                        "application/json"),
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var batchResponse = JsonSerializer.Deserialize<EventaBatchResponse>(content);

                foreach (var item in batchResponse.Items)
                {
                    var cacheKey = $"inventory:{ProviderId}:{tenantId}:{item.Sku}";
                    var data = new ProductAvailabilityData
                    {
                        ProductSku = item.Sku,
                        Price = item.Price.NetPrice,
                        DiscountPrice = item.Price.GrossPrice != item.Price.NetPrice ? item.Price.GrossPrice : null,
                        Currency = item.Price.Currency ?? "EUR",
                        AvailableQuantity = item.Inventory.AvailableQuantity,
                        ProviderId = ProviderId,
                        FetchedAt = DateTime.UtcNow,
                        ExpectedRestock = item.Inventory.ExpectedRestockDate,
                        CacheDuration = TimeSpan.FromMinutes(_config.CacheDurationMinutes)
                    };

                    _cache.Set(cacheKey, data, data.CacheDuration);
                    results.Add(data);
                }

                _logger.LogInformation(
                    "Successfully fetched batch availability from enventa: {Count} products",
                    results.Count);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Batch availability from enventa failed, falling back to individual requests");

                // Fallback auf einzelne Anfragen
                foreach (var sku in skuList)
                {
                    try
                    {
                        var data = await GetProductAvailabilityAsync(sku, tenantId, cancellationToken);
                        results.Add(data);
                    }
                    catch (ProviderException pex)
                    {
                        _logger.LogWarning(pex, "Failed to fetch availability for SKU: {Sku}", sku);
                        // Continue mit nächstem Produkt
                    }
                }

                return results;
            }
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _config.ApiKey);

                var response = await _httpClient.GetAsync(
                    $"{_config.BaseUrl}/health",
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var healthCheck = JsonSerializer.Deserialize<EventaHealthResponse>(content);
                    return healthCheck?.Status == "healthy";
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "enventa Trade health check failed");
                return false;
            }
        }
    }

    /// <summary>
    /// enventa Trade API Request/Response Models
    /// </summary>
    public class EventaProductRequest
    {
        [JsonPropertyName("sku")]
        public string Sku { get; init; }

        [JsonPropertyName("tenantId")]
        public string TenantId { get; init; }

        [JsonPropertyName("includePrice")]
        public bool IncludePrice { get; init; }

        [JsonPropertyName("includeStock")]
        public bool IncludeStock { get; init; }

        [JsonPropertyName("includeBundle")]
        public bool IncludeBundle { get; init; }
    }

    public class EventaBatchRequest
    {
        [JsonPropertyName("skus")]
        public List<string> Skus { get; init; }

        [JsonPropertyName("tenantId")]
        public string TenantId { get; init; }

        [JsonPropertyName("includePrice")]
        public bool IncludePrice { get; init; }

        [JsonPropertyName("includeStock")]
        public bool IncludeStock { get; init; }
    }

    public class EventaAvailabilityResponse
    {
        [JsonPropertyName("sku")]
        public string Sku { get; init; }

        [JsonPropertyName("price")]
        public EventaPriceInfo Price { get; init; }

        [JsonPropertyName("inventory")]
        public EventaInventoryInfo Inventory { get; init; }
    }

    public class EventaBatchResponse
    {
        [JsonPropertyName("items")]
        public List<EventaAvailabilityResponse> Items { get; init; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; init; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; init; }
    }

    public class EventaPriceInfo
    {
        [JsonPropertyName("netPrice")]
        public decimal NetPrice { get; init; }

        [JsonPropertyName("grossPrice")]
        public decimal GrossPrice { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("validFrom")]
        public DateTime ValidFrom { get; init; }

        [JsonPropertyName("validTo")]
        public DateTime? ValidTo { get; init; }

        [JsonPropertyName("discount")]
        public EventaDiscount Discount { get; init; }
    }

    public class EventaDiscount
    {
        [JsonPropertyName("percentage")]
        public decimal? Percentage { get; init; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; init; }

        [JsonPropertyName("validUntil")]
        public DateTime? ValidUntil { get; init; }
    }

    public class EventaInventoryInfo
    {
        [JsonPropertyName("availableQuantity")]
        public int AvailableQuantity { get; init; }

        [JsonPropertyName("reservedQuantity")]
        public int ReservedQuantity { get; init; }

        [JsonPropertyName("totalQuantity")]
        public int TotalQuantity => AvailableQuantity + ReservedQuantity;

        [JsonPropertyName("warehouseCode")]
        public string WarehouseCode { get; init; }

        [JsonPropertyName("location")]
        public string Location { get; init; }

        [JsonPropertyName("expectedRestockDate")]
        public DateTime? ExpectedRestockDate { get; init; }

        [JsonPropertyName("restockQuantity")]
        public int? RestockQuantity { get; init; }
    }

    public class EventaHealthResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }

        [JsonPropertyName("version")]
        public string Version { get; init; }
    }

    public class EventaTradeConfiguration
    {
        public string BaseUrl { get; init; }
        public string ApiKey { get; init; }
        public int TimeoutSeconds { get; init; } = 10;
        public int CacheDurationMinutes { get; init; } = 5;
        public int MaxRetries { get; init; } = 3;
        public string DefaultWarehouse { get; init; } = "DEFAULT";
        public bool EnableBatchRequests { get; init; } = true;
    }
}
```

### Provider Resolver (CQRS ReadCommand)

**File**: `backend/services/Catalog/Application/ReadCommands/GetProductAvailabilityReadCommand.cs`

```csharp
namespace B2X.Catalog.Application.ReadCommands
{
    /// <summary>
    /// Read command that fetches product pricing & availability
    /// from configured providers with fallback strategy
    /// </summary>
    public class GetProductAvailabilityReadCommand : IRequest<ProductAvailabilityDto>
    {
        public string ProductId { get; init; }
        public string Sku { get; init; }
        public string TenantId { get; init; }
    }

    public class GetProductAvailabilityHandler 
        : IRequestHandler<GetProductAvailabilityReadCommand, ProductAvailabilityDto>
    {
        private readonly IEnumerable<IInventoryProvider> _providers;
        private readonly IProviderConfigurationService _providerConfig;
        private readonly ILogger<GetProductAvailabilityHandler> _logger;

        public GetProductAvailabilityHandler(
            IEnumerable<IInventoryProvider> providers,
            IProviderConfigurationService providerConfig,
            ILogger<GetProductAvailabilityHandler> logger)
        {
            _providers = providers;
            _providerConfig = providerConfig;
            _logger = logger;
        }

        public async Task<ProductAvailabilityDto> Handle(
            GetProductAvailabilityReadCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching availability for Product: {ProductId}, SKU: {Sku}, Tenant: {TenantId}",
                request.ProductId,
                request.Sku,
                request.TenantId);

            // Hole tenant-spezifische Provider-Konfiguration
            var providerOrder = await _providerConfig.GetProviderOrderAsync(
                request.TenantId,
                cancellationToken);

            // Versuche Provider der Reihe nach
            foreach (var providerId in providerOrder)
            {
                var provider = _providers.FirstOrDefault(p => p.ProviderId == providerId);
                
                if (provider == null)
                {
                    _logger.LogWarning("Provider not found: {ProviderId}", providerId);
                    continue;
                }

                try
                {
                    if (!await provider.IsAvailableAsync(cancellationToken))
                    {
                        _logger.LogWarning("Provider {ProviderId} is not available", providerId);
                        continue;
                    }

                    var availability = await provider.GetProductAvailabilityAsync(
                        request.Sku,
                        request.TenantId,
                        cancellationToken);

                    _logger.LogInformation(
                        "Successfully fetched availability from {ProviderId}",
                        providerId);

                    return MapToDto(availability, request.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Provider {ProviderId} failed, trying next provider",
                        providerId);
                    // Fallback zum nächsten Provider
                    continue;
                }
            }

            // Kein Provider erfolgreich
            _logger.LogError(
                "All providers failed for Product: {ProductId}",
                request.ProductId);

            throw new NoProviderAvailableException(
                $"No provider available for product {request.ProductId}");
        }

        private ProductAvailabilityDto MapToDto(
            ProductAvailabilityData data,
            string productId)
        {
            return new ProductAvailabilityDto
            {
                ProductId = productId,
                Price = data.Price,
                DiscountPrice = data.DiscountPrice,
                Currency = data.Currency,
                AvailableQuantity = data.AvailableQuantity,
                IsInStock = data.IsInStock,
                ExpectedRestock = data.ExpectedRestock,
                ProviderId = data.ProviderId,
                FetchedAt = data.FetchedAt,
                CacheExpiresAt = data.FetchedAt.Add(data.CacheDuration)
            };
        }
    }
}
```

### Frontend Integration

#### Typ-Definition

**File**: `src/types/availability.ts`

```typescript
export interface ProductAvailability {
  productId: string;
  sku: string;
  price: number;
  discountPrice?: number;
  currency: string;
  availableQuantity: number;
  isInStock: boolean;
  expectedRestock?: string; // ISO date
  providerId: string;
  fetchedAt: string; // ISO timestamp
  cacheExpiresAt: string; // ISO timestamp
}

export interface PriceInfo {
  originalPrice: number;
  currentPrice: number;
  discount?: {
    amount: number;
    percentage: number;
  };
  currency: string;
}

export interface StockInfo {
  quantity: number;
  status: 'in-stock' | 'low-stock' | 'out-of-stock';
  expectedRestock?: string;
  message: string;
}
```

#### API Service

**File**: `src/services/api/availability.ts`

```typescript
import { client } from '@/services/client';
import type { ProductAvailability } from '@/types/availability';

export const availabilityApi = {
  /**
   * Fetch pricing and availability from configured providers
   * Uses fallback strategy if primary provider fails
   */
  async getProductAvailability(
    productId: string,
    sku: string
  ): Promise<ProductAvailability> {
    return client.get(`/api/products/${productId}/availability`, {
      params: { sku }
    });
  },

  /**
   * Batch fetch availability for multiple products
   * Optimized for product listing pages
   */
  async getProductsAvailability(
    products: Array<{ id: string; sku: string }>
  ): Promise<ProductAvailability[]> {
    return client.post('/api/products/availability/batch', {
      products
    });
  },

  /**
   * Check if provider data is still fresh
   * Uses cache if within TTL
   */
  async isCacheFresh(productId: string): Promise<boolean> {
    const availability = await this.getProductAvailability(productId, '');
    const expiresAt = new Date(availability.cacheExpiresAt);
    return expiresAt > new Date();
  }
};
```

#### Composable für Verfügbarkeitsverwaltung

**File**: `src/composables/useAvailability.ts`

```typescript
import { ref, computed } from 'vue';
import { availabilityApi } from '@/services/api/availability';
import type { ProductAvailability, StockInfo } from '@/types/availability';

export function useAvailability() {
  const availability = ref<ProductAvailability | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const stockInfo = computed((): StockInfo | null => {
    if (!availability.value) return null;

    const quantity = availability.value.availableQuantity;
    let status: 'in-stock' | 'low-stock' | 'out-of-stock';
    let message: string;

    if (quantity === 0) {
      status = 'out-of-stock';
      if (availability.value.expectedRestock) {
        const restockDate = new Date(availability.value.expectedRestock);
        message = `Back in stock on ${restockDate.toLocaleDateString()}`;
      } else {
        message = 'Currently out of stock';
      }
    } else if (quantity < 5) {
      status = 'low-stock';
      message = `Only ${quantity} left in stock`;
    } else {
      status = 'in-stock';
      message = `${quantity} available`;
    }

    return {
      quantity,
      status,
      message,
      expectedRestock: availability.value.expectedRestock
    };
  });

  const fetchAvailability = async (productId: string, sku: string) => {
    loading.value = true;
    error.value = null;

    try {
      availability.value = await availabilityApi.getProductAvailability(
        productId,
        sku
      );
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch availability';
    } finally {
      loading.value = false;
    }
  };

  const refreshIfNeeded = async (productId: string, sku: string) => {
    if (!availability.value) {
      await fetchAvailability(productId, sku);
      return;
    }

    const isFresh = await availabilityApi.isCacheFresh(productId);
    if (!isFresh) {
      await fetchAvailability(productId, sku);
    }
  };

  return {
    availability,
    loading,
    error,
    stockInfo,
    fetchAvailability,
    refreshIfNeeded
  };
}
```

#### Component Beispiel

```vue
<!-- src/components/products/ProductPrice.vue -->

<template>
  <div class="price-section">
    <!-- Loading State -->
    <div v-if="loading" class="animate-pulse">
      <div class="h-8 bg-gray-200 rounded w-24 mb-2"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="text-red-600 text-sm">
      {{ error }}
    </div>

    <!-- Price Display -->
    <div v-else-if="availability" class="space-y-2">
      <!-- Price -->
      <div class="flex items-baseline gap-2">
        <span class="text-2xl font-bold">
          {{ formatCurrency(availability.price, availability.currency) }}
        </span>
        <span v-if="availability.discountPrice" class="text-sm text-gray-500 line-through">
          {{ formatCurrency(availability.discountPrice, availability.currency) }}
        </span>
      </div>

      <!-- Stock Status -->
      <div v-if="stockInfo" :class="stockStatusClass">
        <span class="text-sm font-medium">{{ stockInfo.message }}</span>
      </div>

      <!-- Provider Info (optional) -->
      <div class="text-xs text-gray-500 mt-2">
        <span>Powered by {{ availability.providerId }}</span>
        <span class="ml-2">Updated: {{ formatTime(availability.fetchedAt) }}</span>
      </div>

      <!-- Add to Cart Button -->
      <button
        :disabled="!availability.isInStock"
        @click="$emit('add-to-cart')"
        :class="addToCartButtonClass"
      >
        {{ availability.isInStock ? 'Add to Cart' : 'Out of Stock' }}
      </button>

      <!-- Expected Restock Info -->
      <div v-if="!availability.isInStock && stockInfo?.expectedRestock" class="bg-blue-50 p-3 rounded">
        <p class="text-sm text-blue-900">
          ✓ Expected to be back in stock on {{ formatDate(stockInfo.expectedRestock) }}
        </p>
        <button class="text-blue-600 text-sm hover:underline mt-1">
          Notify me when available
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useAvailability } from '@/composables/useAvailability';
import { formatCurrency, formatTime, formatDate } from '@/utils/format';

interface Props {
  productId: string;
  sku: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'add-to-cart': [];
}>();

const { availability, loading, error, stockInfo, fetchAvailability } = useAvailability();

onMounted(() => {
  fetchAvailability(props.productId, props.sku);
});

const stockStatusClass = computed(() => {
  if (!stockInfo.value) return '';
  const baseClass = 'text-sm font-medium';
  switch (stockInfo.value.status) {
    case 'in-stock':
      return `${baseClass} text-green-600`;
    case 'low-stock':
      return `${baseClass} text-orange-600`;
    case 'out-of-stock':
      return `${baseClass} text-red-600`;
  }
});

const addToCartButtonClass = computed(() => {
  const baseClass = 'w-full px-4 py-2 rounded font-semibold transition';
  if (!availability.value?.isInStock) {
    return `${baseClass} bg-gray-300 text-gray-500 cursor-not-allowed`;
  }
  return `${baseClass} bg-blue-600 text-white hover:bg-blue-700`;
});
</script>
```

### Provider-Konfiguration pro Tenant

**File**: `backend/services/Catalog/Application/Configuration/ProviderConfiguration.cs`

```csharp
namespace B2X.Catalog.Application.Configuration
{
    /// <summary>
    /// Service to manage tenant-specific provider configurations
    /// Determines which providers to use and in what order
    /// </summary>
    public interface IProviderConfigurationService
    {
        /// <summary>
        /// Get ordered list of provider IDs for a tenant
        /// </summary>
        Task<IEnumerable<string>> GetProviderOrderAsync(
            string tenantId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Set provider order for a tenant
        /// </summary>
        Task SetProviderOrderAsync(
            string tenantId,
            IEnumerable<string> providerIds,
            CancellationToken cancellationToken);
    }

    public class ProviderConfigurationService : IProviderConfigurationService
    {
        private readonly IRepository<TenantProviderConfiguration> _repository;
        private readonly ILogger<ProviderConfigurationService> _logger;

        public async Task<IEnumerable<string>> GetProviderOrderAsync(
            string tenantId,
            CancellationToken cancellationToken)
        {
            var config = await _repository.GetAsync(
                c => c.TenantId == tenantId,
                cancellationToken);

            if (config == null)
            {
                _logger.LogWarning("No provider config found for tenant {TenantId}", tenantId);
                // Return default provider order
                return new[] { "sap-erp", "oracle-erp", "nav-erp" };
            }

            return config.ProviderOrder.Split(',').Select(p => p.Trim());
        }

        public async Task SetProviderOrderAsync(
            string tenantId,
            IEnumerable<string> providerIds,
            CancellationToken cancellationToken)
        {
            var config = await _repository.GetAsync(
                c => c.TenantId == tenantId,
                cancellationToken) ?? new TenantProviderConfiguration { TenantId = tenantId };

            config.ProviderOrder = string.Join(",", providerIds);
            
            await _repository.UpsertAsync(config, cancellationToken);

            _logger.LogInformation(
                "Updated provider configuration for tenant {TenantId}: {Providers}",
                tenantId,
                string.Join(",", providerIds));
        }
    }

    public class TenantProviderConfiguration
    {
        public string TenantId { get; set; }
        public string ProviderOrder { get; set; } // Comma-separated provider IDs
        public bool EnableFallback { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 10;
        public DateTime ConfiguredAt { get; set; } = DateTime.UtcNow;
    }
}
```

### Fehlerbehandlung & Fallback-Strategie

```
Provider-Versuch 1 (SAP ERP)
    ↓
    ├─ Erfolg → Rückgabe Daten
    ├─ Timeout (> 10s) → Fallback
    ├─ HTTP 500 → Fallback
    └─ Verbindungsfehler → Fallback
        ↓
        Provider-Versuch 2 (Oracle ERP)
            ↓
            ├─ Erfolg → Rückgabe Daten
            └─ Fehler → Fallback
                ↓
                Provider-Versuch 3 (NAV ERP)
                    ↓
                    ├─ Erfolg → Rückgabe Daten
                    └─ Fehler → NoProviderAvailableException
```

### Vorteile des Provider-Modells

| Vorteil | Beschreibung |
|---------|-------------|
| **Flexibilität** | Verschiedene ERP-Systeme pro Tenant |
| **Fehlertoleranz** | Automatisches Fallback bei Ausfällen |
| **Skalierbarkeit** | Unabhängig von Produkt-Schema erweiterbar |
| **Wartbarkeit** | Einfache Hinzufügung neuer Provider |
| **Performance** | Cache-Strategien und Batch-Operationen |
| **Tenancy** | Mandant-spezifische Provider-Konfiguration |

---

## Feature Development Workflow

### Step 1: Define Feature Requirements

Create a feature specification with:

```markdown
## Feature: [Feature Name]

### User Stories
- As a [user type], I want [action] so that [benefit]

### Requirements
- [ ] Functional requirements
- [ ] User interface mockups
- [ ] API endpoints needed
- [ ] Data models

### Success Criteria
- [ ] All user stories completed
- [ ] Mobile responsive
- [ ] Performance < 2s load time
- [ ] Unit tests with >80% coverage
- [ ] E2E tests for critical paths
```

### Step 2: Create Type Definitions

**File**: `src/types/[feature].ts`

```typescript
// src/types/product.ts

export interface Product {
  id: string;
  tenantId: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  originalPrice?: number;
  sku: string;
  stock: number;
  category: Category;
  images: ProductImage[];
  rating: number;
  reviewCount: number;
  slug: string;
  status: 'active' | 'inactive';
  createdAt: string;
  updatedAt: string;
}

export interface ProductImage {
  id: string;
  url: string;
  thumbnail: string;
  alt: string;
  isPrimary: boolean;
  order: number;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  icon?: string;
}

export interface ProductFilter {
  search?: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  rating?: number;
  inStock?: boolean;
  sort?: 'newest' | 'price-asc' | 'price-desc' | 'rating';
}

export interface ProductResponse {
  items: Product[];
  total: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
}
```

### Step 3: Create API Service

**File**: `src/services/api/[feature].ts`

```typescript
// src/services/api/products.ts

import { client } from '@/services/client';
import type { 
  Product, 
  ProductResponse, 
  ProductFilter 
} from '@/types/product';

export const productsApi = {
  async list(
    page: number = 1,
    pageSize: number = 20,
    filters?: ProductFilter
  ): Promise<ProductResponse> {
    return client.get('/api/products', {
      params: { page, pageSize, ...filters }
    });
  },

  async get(slug: string): Promise<Product> {
    return client.get(`/api/products/${slug}`);
  },

  async search(query: string, limit: number = 10): Promise<Product[]> {
    return client.get('/api/products/search', {
      params: { q: query, limit }
    });
  },

  async getRelated(productId: string, limit: number = 4): Promise<Product[]> {
    return client.get(`/api/products/${productId}/related`, {
      params: { limit }
    });
  },

  async getCategories(): Promise<Category[]> {
    return client.get('/api/categories');
  }
};
```

### Step 4: Create Pinia Store

**File**: `src/stores/[feature].ts`

```typescript
// src/stores/products.ts

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { productsApi } from '@/services/api/products';
import type { Product, ProductFilter, ProductResponse } from '@/types/product';

export const useProductsStore = defineStore('products', () => {
  // State
  const products = ref<Product[]>([]);
  const total = ref(0);
  const page = ref(1);
  const pageSize = ref(20);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const filters = ref<ProductFilter>({});
  const selectedProduct = ref<Product | null>(null);

  // Computed
  const isLoading = computed(() => loading.value);
  const hasError = computed(() => error.value !== null);
  const totalPages = computed(() => Math.ceil(total.value / pageSize.value));
  const hasNextPage = computed(() => page.value < totalPages.value);
  const isEmpty = computed(() => products.value.length === 0);

  // Actions
  async function fetchProducts() {
    loading.value = true;
    error.value = null;
    try {
      const response = await productsApi.list(
        page.value,
        pageSize.value,
        filters.value
      );
      products.value = response.items;
      total.value = response.total;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load products';
    } finally {
      loading.value = false;
    }
  }

  async function getProduct(slug: string) {
    try {
      const product = await productsApi.get(slug);
      selectedProduct.value = product;
      return product;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Product not found';
      throw err;
    }
  }

  async function search(query: string) {
    filters.value.search = query;
    page.value = 1;
    await fetchProducts();
  }

  function setFilters(newFilters: ProductFilter) {
    filters.value = { ...filters.value, ...newFilters };
    page.value = 1;
  }

  function clearFilters() {
    filters.value = {};
    page.value = 1;
  }

  function goToPage(newPage: number) {
    if (newPage >= 1 && newPage <= totalPages.value) {
      page.value = newPage;
      fetchProducts();
    }
  }

  return {
    // State
    products,
    total,
    page,
    pageSize,
    loading,
    error,
    filters,
    selectedProduct,
    // Computed
    isLoading,
    hasError,
    totalPages,
    hasNextPage,
    isEmpty,
    // Actions
    fetchProducts,
    getProduct,
    search,
    setFilters,
    clearFilters,
    goToPage
  };
});
```

### Step 5: Create Components

**File**: `src/components/[feature]/[Component].vue`

```vue
<!-- src/components/products/ProductCard.vue -->

<template>
  <div class="product-card group cursor-pointer hover:shadow-lg transition-shadow">
    <!-- Image -->
    <div class="relative overflow-hidden bg-gray-200 rounded-lg h-64 mb-4">
      <img 
        v-lazy="product.images[0]?.url"
        :alt="product.images[0]?.alt || product.name"
        class="w-full h-full object-cover group-hover:scale-105 transition-transform"
        loading="lazy"
      />
      
      <!-- Discount badge -->
      <div v-if="discount" class="absolute top-2 right-2 bg-red-500 text-white px-2 py-1 rounded text-sm">
        -{{ discount }}%
      </div>

      <!-- Quick actions -->
      <div class="absolute bottom-0 left-0 right-0 bg-black bg-opacity-0 group-hover:bg-opacity-50 transition-all flex gap-2 justify-center py-3">
        <button 
          @click="$emit('add-to-cart')"
          class="bg-blue-600 text-white px-4 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity"
        >
          Add to Cart
        </button>
        <button 
          @click="toggleFavorite"
          class="bg-white text-gray-800 px-3 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity"
        >
          ❤️
        </button>
      </div>
    </div>

    <!-- Info -->
    <div>
      <!-- Category -->
      <span class="text-xs text-gray-500 uppercase">{{ product.category.name }}</span>
      
      <!-- Name -->
      <h3 class="font-semibold text-gray-800 line-clamp-2">{{ product.name }}</h3>
      
      <!-- Rating -->
      <div class="flex items-center gap-1 my-2">
        <RatingStars :rating="product.rating" :count="product.reviewCount" />
      </div>

      <!-- Price -->
      <div class="flex items-baseline gap-2">
        <span class="text-lg font-bold text-gray-900">{{ formatCurrency(product.price) }}</span>
        <span v-if="product.originalPrice" class="text-sm text-gray-500 line-through">
          {{ formatCurrency(product.originalPrice) }}
        </span>
      </div>

      <!-- Stock status -->
      <p v-if="product.stock === 0" class="text-xs text-red-600 mt-2">Out of Stock</p>
      <p v-else-if="product.stock < 5" class="text-xs text-orange-600 mt-2">Only {{ product.stock }} left</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Product } from '@/types/product';
import { formatCurrency } from '@/utils/currency';
import RatingStars from '@/components/common/RatingStars.vue';

interface Props {
  product: Product;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'add-to-cart': [];
  'toggle-favorite': [];
}>();

const isFavorite = ref(false);

const discount = computed(() => {
  if (!props.product.originalPrice) return null;
  return Math.round(
    ((props.product.originalPrice - props.product.price) / props.product.originalPrice) * 100
  );
});

const toggleFavorite = () => {
  isFavorite.value = !isFavorite.value;
  emit('toggle-favorite');
};
</script>

<style scoped>
.product-card {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
```

### Step 6: Create Routes

**File**: `src/router/routes.ts`

```typescript
// Add to routes array
{
  path: '/products',
  component: () => import('@/views/Products.vue'),
  meta: {
    title: 'Shop',
    breadcrumb: 'Products'
  }
},
{
  path: '/products/:slug',
  component: () => import('@/views/ProductDetail.vue'),
  meta: {
    title: 'Product Details'
  }
}
```

### Step 7: Create Tests

**File**: `tests/unit/stores/products.spec.ts`

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useProductsStore } from '@/stores/products';
import * as productsApi from '@/services/api/products';

vi.mock('@/services/api/products');

describe('Products Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it('loads products successfully', async () => {
    const store = useProductsStore();
    const mockResponse = {
      items: [
        { 
          id: '1',
          name: 'Product 1',
          price: 99.99,
          tenantId: 'test'
        }
      ],
      total: 1,
      page: 1,
      pageSize: 20,
      hasMore: false
    };

    vi.mocked(productsApi.productsApi.list).mockResolvedValue(mockResponse);

    await store.fetchProducts();

    expect(store.products).toEqual(mockResponse.items);
    expect(store.total).toBe(1);
    expect(store.isLoading).toBe(false);
  });

  it('handles API errors', async () => {
    const store = useProductsStore();
    const error = new Error('API Error');

    vi.mocked(productsApi.productsApi.list).mockRejectedValue(error);

    await store.fetchProducts();

    expect(store.error).toBe('API Error');
    expect(store.products).toEqual([]);
  });

  it('filters products', async () => {
    const store = useProductsStore();
    
    store.setFilters({ search: 'test', minPrice: 10 });

    expect(store.filters.search).toBe('test');
    expect(store.filters.minPrice).toBe(10);
    expect(store.page).toBe(1);
  });
});
```

---

## Core Modules

### Shopping Cart Composable

```typescript
// src/composables/useCart.ts

import { computed } from 'vue';
import { useCartStore } from '@/stores/cart';
import { useNotification } from '@/composables/useNotification';

export function useCart() {
  const store = useCartStore();
  const { success, error } = useNotification();

  const cartTotal = computed(() =>
    store.items.reduce((sum, item) => sum + item.price * item.quantity, 0)
  );

  const itemCount = computed(() =>
    store.items.reduce((sum, item) => sum + item.quantity, 0)
  );

  const addItem = (productId: string, quantity: number = 1) => {
    try {
      store.addItem(productId, quantity);
      success('Added to cart');
    } catch (err) {
      error('Failed to add item');
    }
  };

  const removeItem = (productId: string) => {
    store.removeItem(productId);
  };

  const updateQuantity = (productId: string, quantity: number) => {
    if (quantity <= 0) {
      removeItem(productId);
    } else {
      store.updateQuantity(productId, quantity);
    }
  };

  const clear = () => {
    store.clear();
  };

  return {
    items: computed(() => store.items),
    cartTotal,
    itemCount,
    addItem,
    removeItem,
    updateQuantity,
    clear
  };
}
```

### Search with Debounce

```typescript
// src/composables/useSearch.ts

import { ref, computed } from 'vue';
import { useProductsStore } from '@/stores/products';

export function useSearch(delay: number = 300) {
  const store = useProductsStore();
  const query = ref('');
  const debounceTimer = ref<NodeJS.Timeout>();
  const searching = ref(false);

  const search = (value: string) => {
    query.value = value;
    clearTimeout(debounceTimer.value);

    if (!value) {
      store.clearFilters();
      return;
    }

    searching.value = true;
    debounceTimer.value = setTimeout(async () => {
      try {
        await store.search(value);
      } finally {
        searching.value = false;
      }
    }, delay);
  };

  const clear = () => {
    query.value = '';
    store.clearFilters();
    clearTimeout(debounceTimer.value);
  };

  return {
    query,
    searching,
    search,
    clear
  };
}
```

### Infinite Scroll Composable

```typescript
// src/composables/useInfiniteScroll.ts

import { ref, onMounted, onUnmounted } from 'vue';

export function useInfiniteScroll(callback: () => void, threshold: number = 0.8) {
  const container = ref<HTMLElement>();

  const handleScroll = () => {
    if (!container.value) return;

    const { scrollHeight, scrollTop, clientHeight } = container.value;
    const scrollPercent = (scrollTop + clientHeight) / scrollHeight;

    if (scrollPercent > threshold) {
      callback();
    }
  };

  onMounted(() => {
    container.value?.addEventListener('scroll', handleScroll);
  });

  onUnmounted(() => {
    container.value?.removeEventListener('scroll', handleScroll);
  });

  return { container };
}
```

---

## Integration Patterns

### Pattern 1: Product Listing with Filters

```vue
<template>
  <div class="products-page">
    <!-- Sidebar filters (desktop) -->
    <aside class="hidden lg:block w-64">
      <ProductFilter @apply="applyFilters" />
    </aside>

    <!-- Main content -->
    <main class="flex-1">
      <!-- Mobile filter button -->
      <button 
        @click="showMobileFilters = true"
        class="lg:hidden mb-4"
      >
        Filters
      </button>

      <!-- Search bar -->
      <SearchBar @search="handleSearch" />

      <!-- Products grid -->
      <div v-if="store.isLoading" class="text-center py-12">
        <LoadingSpinner />
      </div>

      <div v-else-if="store.isEmpty" class="text-center py-12">
        <p class="text-gray-500">No products found</p>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <ProductCard 
          v-for="product in store.products"
          :key="product.id"
          :product="product"
          @add-to-cart="handleAddToCart(product)"
        />
      </div>

      <!-- Pagination -->
      <Pagination 
        :current="store.page"
        :total="store.totalPages"
        @change="store.goToPage"
      />
    </main>

    <!-- Mobile filters modal -->
    <Modal v-if="showMobileFilters" @close="showMobileFilters = false">
      <ProductFilter @apply="applyFilters" />
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useProductsStore } from '@/stores/products';
import { useCart } from '@/composables/useCart';
import type { ProductFilter } from '@/types/product';

const store = useProductsStore();
const { addItem } = useCart();
const showMobileFilters = ref(false);

onMounted(() => {
  store.fetchProducts();
});

const applyFilters = (filters: ProductFilter) => {
  store.setFilters(filters);
  store.fetchProducts();
  showMobileFilters.value = false;
};

const handleSearch = (query: string) => {
  store.search(query);
};

const handleAddToCart = (product: any) => {
  addItem(product.id, 1);
};
</script>
```

### Pattern 2: Shopping Cart with Checkout

```vue
<template>
  <div class="cart-page">
    <h1 class="text-3xl font-bold mb-6">Shopping Cart</h1>

    <div v-if="cart.items.value.length === 0" class="text-center py-12">
      <p class="text-gray-500 mb-4">Your cart is empty</p>
      <router-link to="/products" class="text-blue-600">Continue shopping</router-link>
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Cart items -->
      <div class="lg:col-span-2 space-y-4">
        <CartItem 
          v-for="item in cart.items.value"
          :key="item.id"
          :item="item"
          @update-quantity="cart.updateQuantity(item.id, $event)"
          @remove="cart.removeItem(item.id)"
        />
      </div>

      <!-- Cart summary -->
      <CartSummary 
        :total="cart.cartTotal.value"
        :item-count="cart.itemCount.value"
        @checkout="proceedToCheckout"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCart } from '@/composables/useCart';
import { useRouter } from 'vue-router';
import CartItem from '@/components/cart/CartItem.vue';
import CartSummary from '@/components/cart/CartSummary.vue';

const cart = useCart();
const router = useRouter();

const proceedToCheckout = () => {
  router.push('/checkout');
};
</script>
```

### Pattern 3: Responsive Product Detail

```vue
<template>
  <div v-if="store.isLoading" class="text-center py-12">
    <LoadingSpinner />
  </div>

  <div v-else-if="product" class="grid grid-cols-1 lg:grid-cols-2 gap-8 py-8">
    <!-- Images -->
    <div>
      <ImageGallery :images="product.images" />
    </div>

    <!-- Details -->
    <div>
      <h1 class="text-3xl font-bold mb-4">{{ product.name }}</h1>
      
      <RatingStars :rating="product.rating" :count="product.reviewCount" />
      
      <!-- Price -->
      <div class="text-3xl font-bold text-gray-900 my-4">
        {{ formatCurrency(product.price) }}
      </div>

      <!-- Description -->
      <p class="text-gray-700 mb-6">{{ product.description }}</p>

      <!-- Stock -->
      <div class="mb-6">
        <p v-if="product.stock === 0" class="text-red-600 font-semibold">Out of Stock</p>
        <p v-else class="text-green-600">In Stock</p>
      </div>

      <!-- Add to cart -->
      <button 
        @click="handleAddToCart"
        :disabled="product.stock === 0"
        class="w-full bg-blue-600 text-white px-6 py-3 rounded-lg font-semibold hover:bg-blue-700 disabled:opacity-50"
      >
        Add to Cart
      </button>

      <!-- Related products -->
      <div class="mt-12">
        <h2 class="text-xl font-semibold mb-4">You might also like</h2>
        <div class="grid grid-cols-2 gap-4">
          <ProductCard 
            v-for="product in relatedProducts"
            :key="product.id"
            :product="product"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useProductsStore } from '@/stores/products';
import { useCart } from '@/composables/useCart';
import { formatCurrency } from '@/utils/currency';

const route = useRoute();
const store = useProductsStore();
const { addItem } = useCart();
const relatedProducts = ref([]);

const product = computed(() => store.selectedProduct);

onMounted(async () => {
  const slug = route.params.slug as string;
  await store.getProduct(slug);
  
  if (product.value) {
    relatedProducts.value = await productsApi.getRelated(product.value.id, 4);
  }
});

const handleAddToCart = () => {
  if (product.value) {
    addItem(product.value.id, 1);
  }
};
</script>
```

---

## API Integration

### HTTP Client Configuration

```typescript
// src/services/client.ts

import axios from 'axios';
import { useAuthStore } from '@/stores/auth';
import { useNotification } from '@/composables/useNotification';

export const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor
client.interceptors.request.use((config) => {
  const auth = useAuthStore();
  const tenantId = sessionStorage.getItem('tenantId');

  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`;
  }

  if (tenantId) {
    config.headers['X-Tenant-ID'] = tenantId;
  }

  return config;
});

// Response interceptor
client.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const { error: notifyError } = useNotification();

    if (error.response?.status === 401) {
      useAuthStore().logout();
    } else if (error.response?.status >= 400) {
      const message = error.response?.data?.message || 'Something went wrong';
      notifyError(message);
    }

    return Promise.reject(error);
  }
);
```

---

## State Management

### Cart Store Example

```typescript
// src/stores/cart.ts

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([]);

  const addItem = (productId: string, quantity: number = 1) => {
    const existing = items.value.find(i => i.id === productId);

    if (existing) {
      existing.quantity += quantity;
    } else {
      items.value.push({
        id: productId,
        quantity,
        addedAt: new Date()
      });
    }

    // Persist to localStorage
    saveToLocalStorage();
  };

  const removeItem = (productId: string) => {
    items.value = items.value.filter(i => i.id !== productId);
    saveToLocalStorage();
  };

  const updateQuantity = (productId: string, quantity: number) => {
    const item = items.value.find(i => i.id === productId);
    if (item) {
      item.quantity = quantity;
      saveToLocalStorage();
    }
  };

  const clear = () => {
    items.value = [];
    localStorage.removeItem('cart');
  };

  const saveToLocalStorage = () => {
    localStorage.setItem('cart', JSON.stringify(items.value));
  };

  const loadFromLocalStorage = () => {
    const saved = localStorage.getItem('cart');
    if (saved) {
      items.value = JSON.parse(saved);
    }
  };

  // Load on initialization
  loadFromLocalStorage();

  return {
    items,
    addItem,
    removeItem,
    updateQuantity,
    clear
  };
}, {
  persist: true // Enable persistence
});
```

---

## Performance Optimization

### Image Optimization

```typescript
// src/utils/image.ts

export function getOptimizedImageUrl(
  url: string,
  width: number,
  height: number,
  quality: number = 80
): string {
  // Use image CDN parameters
  const params = new URLSearchParams({
    w: width.toString(),
    h: height.toString(),
    q: quality.toString(),
    fit: 'cover'
  });

  return `${url}?${params}`;
}

// Usage in component
<img 
  :src="getOptimizedImageUrl(product.image, 400, 400)"
  :srcset="`
    ${getOptimizedImageUrl(product.image, 400, 400)} 1x,
    ${getOptimizedImageUrl(product.image, 800, 800)} 2x
  `"
/>
```

### Code Splitting

```typescript
// src/router/routes.ts

// Lazy load heavy routes
{
  path: '/products',
  component: () => import('@/views/Products.vue'), // Code split
  meta: { preload: true } // Preload hint
}
```

### Virtual Scrolling for Long Lists

```vue
<template>
  <VirtualList
    :items="products"
    :item-height="300"
    :buffer-size="5"
  >
    <template #default="{ item }">
      <ProductCard :product="item" />
    </template>
  </VirtualList>
</template>
```

---

## SEO & Meta Tags

### Dynamic Meta Tags

```typescript
// src/router/guards.ts

router.beforeEach((to, from, next) => {
  // Update document title
  document.title = (to.meta.title as string) || 'B2X Shop';

  // Update meta tags
  const metaDescription = document.querySelector('meta[name="description"]');
  if (metaDescription && to.meta.description) {
    metaDescription.setAttribute('content', to.meta.description as string);
  }

  // Update Open Graph tags for sharing
  updateOpenGraphTags(to);

  next();
});

function updateOpenGraphTags(route: RouteLocationNormalized) {
  const ogTitle = document.querySelector('meta[property="og:title"]');
  const ogDescription = document.querySelector('meta[property="og:description"]');
  const ogImage = document.querySelector('meta[property="og:image"]');

  if (ogTitle && route.meta.ogTitle) {
    ogTitle.setAttribute('content', route.meta.ogTitle as string);
  }
  // ... update other OG tags
}
```

### Product Schema Markup

```typescript
// src/utils/schema.ts

export function generateProductSchema(product: Product) {
  return {
    '@context': 'https://schema.org/',
    '@type': 'Product',
    name: product.name,
    description: product.description,
    image: product.images.map(img => img.url),
    price: product.price,
    priceCurrency: product.currency,
    aggregateRating: {
      '@type': 'AggregateRating',
      ratingValue: product.rating,
      reviewCount: product.reviewCount
    },
    availability: product.stock > 0 ? 'InStock' : 'OutOfStock'
  };
}

// Use in component
<script type="application/ld+json" v-html="JSON.stringify(generateProductSchema(product))"></script>
```

---

## Testing Strategy

### Unit Tests for Composables

```typescript
// tests/unit/composables/useCart.spec.ts

describe('useCart Composable', () => {
  it('adds item to cart', () => {
    const { addItem, items } = useCart();
    addItem('product-1', 1);
    expect(items.value).toHaveLength(1);
  });

  it('updates quantity', () => {
    const { addItem, updateQuantity, items } = useCart();
    addItem('product-1', 1);
    updateQuantity('product-1', 2);
    expect(items.value[0].quantity).toBe(2);
  });

  it('removes item', () => {
    const { addItem, removeItem, items } = useCart();
    addItem('product-1', 1);
    removeItem('product-1');
    expect(items.value).toHaveLength(0);
  });
});
```

### E2E Tests for Critical Flows

```typescript
// tests/e2e/shopping.spec.ts

test.describe('Shopping Flow', () => {
  test('should complete purchase', async ({ page }) => {
    // Navigate to products
    await page.goto('/products');
    
    // Search for product
    await page.fill('[placeholder="Search..."]', 'Test Product');
    await page.click('button:has-text("Search")');
    
    // Add to cart
    await page.click('text=Add to Cart');
    await expect(page).toContainText('Added to cart');
    
    // Go to cart
    await page.click('[data-testid="cart-icon"]');
    
    // Checkout
    await page.click('button:has-text("Proceed to Checkout")');
    
    // Fill shipping info
    await page.fill('[name="email"]', 'test@example.com');
    await page.fill('[name="address"]', '123 Main St');
    
    // Submit order
    await page.click('button:has-text("Place Order")');
    
    // Verify success
    await expect(page).toContainText('Order confirmed');
  });
});
```

---

## Common Use Cases

### Use Case 1: Add Product Review Feature

1. Create review types: `src/types/review.ts`
2. Create review API: `src/services/api/reviews.ts`
3. Create review store: `src/stores/reviews.ts`
4. Create review components
5. Add review route
6. Write tests

### Use Case 2: Implement Wishlist

```typescript
// src/composables/useWishlist.ts

export function useWishlist() {
  const store = useWishlistStore();

  const toggleWishlist = (productId: string) => {
    if (store.isInWishlist(productId)) {
      store.removeItem(productId);
    } else {
      store.addItem(productId);
    }
  };

  const isInWishlist = (productId: string) =>
    store.isInWishlist(productId);

  return {
    items: computed(() => store.items),
    toggleWishlist,
    isInWishlist
  };
}
```

### Use Case 3: Add Price Comparison

```vue
<template>
  <div class="price-comparison">
    <h3>Compare Prices</h3>
    <table>
      <tr>
        <th>Retailer</th>
        <th>Price</th>
        <th>Action</th>
      </tr>
      <tr v-for="price in priceHistory" :key="price.date">
        <td>{{ formatDate(price.date) }}</td>
        <td>{{ formatCurrency(price.amount) }}</td>
        <td>{{ price.change }}%</td>
      </tr>
    </table>
  </div>
</template>
```

---

## Troubleshooting

### Common Issues

#### 1. Images Not Loading

**Problem**: Product images show broken image icon

**Solution**: Check image CDN configuration
```typescript
// .env
VITE_IMAGE_CDN_URL=https://images.example.com

// Use in components
:src="`${import.meta.env.VITE_IMAGE_CDN_URL}/${product.image}`"
```

#### 2. Cart Data Lost on Refresh

**Problem**: Cart items disappear after page reload

**Solution**: Persist cart to localStorage
```typescript
// In store
const persistCart = () => {
  localStorage.setItem('cart', JSON.stringify(items.value));
};

const loadCart = () => {
  const saved = localStorage.getItem('cart');
  if (saved) items.value = JSON.parse(saved);
};
```

#### 3. Slow Page Load

**Problem**: Product listing takes too long to load

**Solution**: Implement pagination and lazy loading
```vue
<img v-lazy="product.image" /> <!-- Only load when visible -->

<!-- Implement infinite scroll -->
<InfiniteScroll @load-more="loadMoreProducts" />
```

#### 4. Mobile Layout Issues

**Problem**: Components don't stack properly on mobile

**Solution**: Use responsive grid classes
```vue
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
  <!-- Grid adjusts columns by breakpoint -->
</div>
```

---

## References

- [Vue 3 Documentation](https://vuejs.org)
- [Pinia Store Documentation](https://pinia.vuejs.org)
- [Vue Router Documentation](https://router.vuejs.org)
- [Tailwind CSS](https://tailwindcss.com)
- [Vite Documentation](https://vitejs.dev)
- [Web Vitals](https://web.dev/vitals)

---

**Maintained by**: B2X Development Team  
**Last Updated**: December 26, 2025  
**Status**: Production Ready
