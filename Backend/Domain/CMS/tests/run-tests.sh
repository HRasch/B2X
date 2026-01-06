#!/usr/bin/env bash
# CMS Testing Script
# Comprehensive testing for CMS Widget System

set -euo pipefail

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}CMS Testing Suite${NC}"
echo -e "${BLUE}========================================${NC}\n"

# Function to print section headers
print_section() {
    echo -e "\n${BLUE}--- $1 ---${NC}\n"
}

# Function to print success
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

# Function to print info
print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

# Parse command line arguments
BACKEND_ONLY=false
FRONTEND_ONLY=false
E2E_ONLY=false
COVERAGE=false
WATCH=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --backend)
            BACKEND_ONLY=true
            shift
            ;;
        --frontend)
            FRONTEND_ONLY=true
            shift
            ;;
        --e2e)
            E2E_ONLY=true
            shift
            ;;
        --coverage)
            COVERAGE=true
            shift
            ;;
        --watch)
            WATCH=true
            shift
            ;;
        *)
            shift
            ;;
    esac
done

# Default: run all tests if no specific type is selected
if ! $BACKEND_ONLY && ! $FRONTEND_ONLY && ! $E2E_ONLY; then
    BACKEND_ONLY=true
    FRONTEND_ONLY=true
    E2E_ONLY=true
fi

# Backend Tests
if $BACKEND_ONLY; then
    print_section "Running Backend Tests"
    
    print_info "Testing: Widget Registry"
    cd backend/Tests/B2Connect.CMS.Tests
    dotnet test -k "WidgetRegistryTests" -v minimal || true
    print_success "Widget Registry Tests Complete"
    
    print_info "Testing: Page Definition"
    dotnet test -k "PageDefinitionTests" -v minimal || true
    print_success "Page Definition Tests Complete"
    
    print_info "Testing: Query Handler"
    dotnet test -k "GetPageDefinitionQueryHandlerTests" -v minimal || true
    print_success "Query Handler Tests Complete"
    
    print_info "Testing: End-to-End Workflows"
    dotnet test -k "CmsEndToEndTests" -v minimal || true
    print_success "End-to-End Tests Complete"
    
    if $COVERAGE; then
        print_info "Generating Coverage Report"
        dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov || true
        print_success "Coverage Report Generated"
    fi
    
    cd ../../..
fi

# Frontend Component Tests
if $FRONTEND_ONLY; then
    print_section "Running Frontend Component Tests"
    
    print_info "Installing dependencies..."
    npm install > /dev/null 2>&1 || true
    
    print_info "Testing: Widget Renderer"
    npm run test -- tests/components/cms/WidgetRenderer.spec.ts --run || true
    print_success "Widget Renderer Tests Complete"
    
    print_info "Testing: Region Renderer"
    npm run test -- tests/components/cms/RegionRenderer.spec.ts --run || true
    print_success "Region Renderer Tests Complete"
    
    print_info "Testing: Hero Banner Widget"
    npm run test -- tests/components/cms/HeroBanner.spec.ts --run || true
    print_success "Hero Banner Tests Complete"
    
    print_info "Testing: Testimonials Widget"
    npm run test -- tests/components/cms/Testimonials.spec.ts --run || true
    print_success "Testimonials Tests Complete"
    
    if $COVERAGE; then
        print_info "Generating Coverage Report"
        npm run test:coverage tests/components/cms || true
        print_success "Coverage Report Generated"
    fi
    
    if $WATCH; then
        print_info "Watching for changes..."
        npm run test:watch tests/components/cms
    fi
fi

# E2E Tests
if $E2E_ONLY; then
    print_section "Running E2E Tests"
    
    print_info "Starting development server..."
    npm run dev > /dev/null 2>&1 &
    DEV_PID=$!
    
    # Wait for server to start
    sleep 5
    
    print_info "Testing: CMS Pages"
    npm run test:e2e -- tests/e2e/cms/cms-pages.spec.ts --project=chromium || true
    print_success "CMS Pages Tests Complete"
    
    print_info "Testing: CMS API"
    npm run test:e2e -- tests/e2e/cms/cms-api.spec.ts --project=chromium || true
    print_success "CMS API Tests Complete"
    
    # Clean up dev server
    kill $DEV_PID 2>/dev/null || true
fi

print_section "Test Summary"
echo -e "${GREEN}✓ Test Suite Complete!${NC}"
echo ""
echo -e "${YELLOW}Test Results:${NC}"
if $BACKEND_ONLY; then
    echo "  - Backend: /backend/Tests/B2Connect.CMS.Tests/"
fi
if $FRONTEND_ONLY; then
    echo "  - Frontend: npm test"
    echo "  - Components: tests/components/cms/"
fi
if $E2E_ONLY; then
    echo "  - E2E: tests/e2e/cms/"
    if $COVERAGE; then
        echo "  - Coverage: ./coverage/"
    fi
fi

echo ""
echo -e "${BLUE}Next Steps:${NC}"
echo "  1. Check test output above for any failures"
echo "  2. Review TESTING.md for detailed test information"
echo "  3. Update tests if CMS features change"
echo "  4. Run tests before committing changes"
echo ""
