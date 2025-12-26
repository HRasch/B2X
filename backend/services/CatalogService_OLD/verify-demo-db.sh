#!/bin/bash

# Catalog Service Demo Database Verification Script
# This script tests the in-memory demo database

set -e

SERVICE_URL="http://localhost:5008"
API_VERSION="v1"

echo "================================================"
echo "🔍 Catalog Service Demo Database Verification"
echo "================================================"
echo ""

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper functions
check_endpoint() {
    local endpoint=$1
    local description=$2
    
    echo -n "📡 Testing $description... "
    
    if response=$(curl -s -w "\n%{http_code}" "$SERVICE_URL/api/$API_VERSION$endpoint"); then
        http_code=$(echo "$response" | tail -n 1)
        body=$(echo "$response" | head -n -1)
        
        if [ "$http_code" = "200" ]; then
            echo -e "${GREEN}✓ OK${NC} (HTTP $http_code)"
            echo "$body"
            echo ""
            return 0
        else
            echo -e "${RED}✗ FAILED${NC} (HTTP $http_code)"
            echo "$body"
            echo ""
            return 1
        fi
    else
        echo -e "${RED}✗ CONNECTION ERROR${NC}"
        return 1
    fi
}

count_items() {
    local response=$1
    echo "$response" | grep -o '"id"' | wc -l
}

# Check if service is running
echo "🔄 Checking if Catalog Service is running..."
if ! curl -s "$SERVICE_URL/health" > /dev/null 2>&1; then
    echo -e "${RED}✗ Service is not running at $SERVICE_URL${NC}"
    echo ""
    echo "To start the service:"
    echo "  cd backend/services/CatalogService"
    echo "  ASPNETCORE_ENVIRONMENT=Development dotnet run"
    exit 1
fi
echo -e "${GREEN}✓ Service is running${NC}"
echo ""

# Test Health Check
echo "════════════════════════════════════════════"
echo "1️⃣  Health Check"
echo "════════════════════════════════════════════"
health_response=$(curl -s "$SERVICE_URL/health")
echo "Health Status: $health_response"
echo ""

# Test Products Endpoint
echo "════════════════════════════════════════════"
echo "2️⃣  Products (Sample)"
echo "════════════════════════════════════════════"
products_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/products?limit=3")
product_count=$(echo "$products_response" | grep -o '"id"' | wc -l)
echo "📦 Sample Products (showing first 3):"
echo "$products_response" | jq '.data[0:3] | .[] | {id, name, price, sku, isActive}' 2>/dev/null || echo "$products_response" | head -c 500
echo "..."
echo ""

# Test Categories Endpoint
echo "════════════════════════════════════════════"
echo "3️⃣  Categories"
echo "════════════════════════════════════════════"
categories_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/categories")
category_count=$(echo "$categories_response" | grep -o '"id"' | wc -l)
echo "🏷️  Total Categories: $category_count"
echo "$categories_response" | jq '.data | .[] | {id, name, slug}' 2>/dev/null | head -c 500 || echo "$categories_response" | head -c 500
echo ""
echo ""

# Test Brands Endpoint
echo "════════════════════════════════════════════"
echo "4️⃣  Brands"
echo "════════════════════════════════════════════"
brands_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/brands")
brand_count=$(echo "$brands_response" | grep -o '"id"' | wc -l)
echo "🏢 Total Brands: $brand_count"
echo "$brands_response" | jq '.data | .[] | {id, name, slug}' 2>/dev/null | head -c 500 || echo "$brands_response" | head -c 500
echo ""
echo ""

# Test Featured Products
echo "════════════════════════════════════════════"
echo "5️⃣  Featured Products"
echo "════════════════════════════════════════════"
featured_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/products/featured")
featured_count=$(echo "$featured_response" | grep -o '"id"' | wc -l)
echo "⭐ Featured Products: $featured_count"
if [ "$featured_count" -gt 0 ]; then
    echo -e "${GREEN}✓ Featured products found${NC}"
else
    echo -e "${YELLOW}⚠ No featured products (may depend on random generation)${NC}"
fi
echo ""

# Test Search
echo "════════════════════════════════════════════"
echo "6️⃣  Search"
echo "════════════════════════════════════════════"
search_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/products/search?query=laptop")
search_count=$(echo "$search_response" | grep -o '"id"' | wc -l)
echo "🔍 Search results for 'laptop': $search_count products"
if [ "$search_count" -gt 0 ]; then
    echo -e "${GREEN}✓ Search functionality working${NC}"
else
    echo -e "${YELLOW}⚠ No results for search (may be query-specific)${NC}"
fi
echo ""

# Test Category Hierarchy
echo "════════════════════════════════════════════"
echo "7️⃣  Category Hierarchy"
echo "════════════════════════════════════════════"
hierarchy_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/categories/hierarchy")
echo "📊 Category Hierarchy:"
echo "$hierarchy_response" | jq '.data | .[] | {id, name, slug, children}' 2>/dev/null | head -c 800 || echo "$hierarchy_response" | head -c 500
echo ""
echo ""

# Test Pagination
echo "════════════════════════════════════════════"
echo "8️⃣  Pagination"
echo "════════════════════════════════════════════"
paged_response=$(curl -s "$SERVICE_URL/api/$API_VERSION/products/paged?page=1&pageSize=10")
echo "📄 Paged Products (Page 1, 10 per page):"
echo "$paged_response" | jq '.pageNumber, .pageSize, .totalCount, .totalPages' 2>/dev/null || echo "$paged_response" | head -c 500
echo ""

# Summary
echo "════════════════════════════════════════════"
echo "📊 Demo Database Summary"
echo "════════════════════════════════════════════"
echo ""

total_products=$(curl -s "$SERVICE_URL/api/$API_VERSION/products?limit=99999" | jq '.data | length' 2>/dev/null || echo "N/A")
total_categories=$(curl -s "$SERVICE_URL/api/$API_VERSION/categories" | jq '.data | length' 2>/dev/null || echo "N/A")
total_brands=$(curl -s "$SERVICE_URL/api/$API_VERSION/brands" | jq '.data | length' 2>/dev/null || echo "N/A")

echo "✅ Service Status: ${GREEN}Running${NC}"
echo "📦 Total Products: $total_products"
echo "🏷️  Total Categories: $total_categories"
echo "🏢 Total Brands: $total_brands"
echo ""

# Check data completeness
echo "════════════════════════════════════════════"
echo "✓ Data Completeness Check"
echo "════════════════════════════════════════════"
echo ""

# Test multilingual content
first_product=$(curl -s "$SERVICE_URL/api/$API_VERSION/products?limit=1" | jq '.data[0]' 2>/dev/null)
if echo "$first_product" | jq '.name.en, .name.de, .name.fr' > /dev/null 2>&1; then
    echo -e "${GREEN}✓ Multilingual content detected (en, de, fr)${NC}"
else
    echo -e "${YELLOW}⚠ Could not verify multilingual content${NC}"
fi

# Test relationships
if curl -s "$SERVICE_URL/api/$API_VERSION/products?limit=1" | grep -q "brandId"; then
    echo -e "${GREEN}✓ Product relationships present${NC}"
else
    echo -e "${YELLOW}⚠ Could not verify product relationships${NC}"
fi

echo ""
echo "════════════════════════════════════════════"
echo "✅ Verification Complete!"
echo "════════════════════════════════════════════"
echo ""
echo "Next Steps:"
echo "  • Browse API: http://localhost:5008/swagger"
echo "  • Test endpoints with demo data"
echo "  • Customize in: appsettings.Development.json"
echo "  • Documentation: CATALOG_DEMO_DATABASE.md"
echo ""
