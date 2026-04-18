# CMS Testing Guide

Complete testing suite for the CMS Widget System including unit tests, integration tests, E2E tests, and test data seeding.

## Test Structure

```
backend/Tests/B2X.CMS.Tests/
├── WidgetRegistryTests.cs              # Widget registry unit tests
├── PageDefinitionTests.cs              # Page domain model tests
├── GetPageDefinitionQueryHandlerTests.cs # Query handler tests
└── CmsEndToEndTests.cs                 # End-to-end workflow tests

frontend/tests/
├── components/cms/
│   ├── WidgetRenderer.spec.ts         # Widget renderer component tests
│   ├── RegionRenderer.spec.ts         # Region renderer component tests
│   ├── HeroBanner.spec.ts             # Hero banner widget tests
│   └── Testimonials.spec.ts           # Testimonials widget tests
└── e2e/cms/
    ├── cms-pages.spec.ts              # Page loading and navigation tests
    └── cms-api.spec.ts                # API integration tests

backend/services/CMS/Infrastructure/Seeding/
└── CmsTestDataSeeder.cs               # Test data factory methods
```

## Backend Unit Tests

### WidgetRegistry Tests (8 tests)

Tests the widget registry service that manages available widgets.

```bash
# Run widget registry tests
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj -k WidgetRegistryTests
```

**Test Coverage:**
- ✅ Register valid widget
- ✅ Reject empty widget ID
- ✅ Get non-existent widget throws exception
- ✅ Get all widgets returns all
- ✅ Filter widgets by category
- ✅ Filter widgets by page type
- ✅ Check widget availability
- ✅ Sort widgets by sort order

### PageDefinition Tests (10 tests)

Tests the page definition aggregate root and its entities.

```bash
# Run page definition tests
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj -k PageDefinitionTests
```

**Test Coverage:**

**PageDefinition Tests:**
- ✅ Create page with valid data
- ✅ Add unique region
- ✅ Reject duplicate region names
- ✅ Remove existing region
- ✅ Safe removal of non-existent region
- ✅ Publish page sets flags
- ✅ Unpublish page clears published flag

**PageRegion Tests:**
- ✅ Add widget to region
- ✅ Auto-increment widget order
- ✅ Respect max widget limit
- ✅ Remove widget and reorder

**WidgetInstance Tests:**
- ✅ Create with defaults
- ✅ Store widget settings

### Query Handler Tests (4 tests)

Tests the CQRS query handler for fetching page definitions.

```bash
# Run query handler tests
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj -k GetPageDefinitionQueryHandlerTests
```

**Test Coverage:**
- ✅ Return page definition DTO
- ✅ Reject unpublished pages
- ✅ Handle non-existent pages
- ✅ Map widget component paths
- ✅ Filter disabled widgets

### End-to-End Tests (4 tests)

Tests complete workflows from page creation to query execution.

```bash
# Run E2E tests
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj -k CmsEndToEndTests
```

**Test Coverage:**
- ✅ Create page with multiple widgets and query
- ✅ Create multiple independent pages
- ✅ Dynamic widget loading and path resolution
- ✅ Widget filtering by category

### Run All Backend Tests

```bash
# Run all CMS tests
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj

# With verbose output
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj -v normal

# With coverage report
dotnet test backend/Tests/B2X.CMS.Tests/B2X.CMS.Tests.csproj /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Frontend Component Tests

### WidgetRenderer Tests (5 tests)

Tests the dynamic widget loader component.

```bash
# Run WidgetRenderer tests
npm run test -- tests/components/cms/WidgetRenderer.spec.ts
```

**Test Coverage:**
- ✅ Mount component successfully
- ✅ Pass settings to widget component
- ✅ Apply widget type CSS class
- ✅ Handle different widget types
- ✅ Accept various widget IDs

### RegionRenderer Tests (7 tests)

Tests the region/slot renderer component.

```bash
# Run RegionRenderer tests
npm run test -- tests/components/cms/RegionRenderer.spec.ts
```

**Test Coverage:**
- ✅ Render region successfully
- ✅ Render all widgets in region
- ✅ Apply region CSS classes
- ✅ Apply region settings as styles
- ✅ Show empty state in development
- ✅ Set data-region-id attribute
- ✅ Handle region settings (background, padding, maxWidth)

### HeroBanner Tests (6 tests)

Tests the hero banner widget component.

```bash
# Run HeroBanner tests
npm run test -- tests/components/cms/HeroBanner.spec.ts
```

**Test Coverage:**
- ✅ Render with title and subtitle
- ✅ Apply background image styles
- ✅ Render CTA button when link provided
- ✅ Hide button without link
- ✅ Use default CTA text
- ✅ Navigate on button click

### Testimonials Tests (7 tests)

Tests the testimonials carousel widget component.

```bash
# Run Testimonials tests
npm run test -- tests/components/cms/Testimonials.spec.ts
```

**Test Coverage:**
- ✅ Render carousel successfully
- ✅ Display first testimonial by default
- ✅ Navigate to next testimonial
- ✅ Navigate to previous testimonial
- ✅ Cycle through testimonials
- ✅ Show testimonial counter
- ✅ Hide navigation with single testimonial

### Run All Frontend Component Tests

```bash
# Run all component tests
npm run test tests/components/cms

# Watch mode
npm run test:watch tests/components/cms

# With coverage
npm run test:coverage tests/components/cms
```

## Frontend E2E Tests

### CMS Pages Tests (14 tests)

Tests page loading, widget rendering, and interactions.

```bash
# Run page tests
npm run test:e2e tests/e2e/cms/cms-pages.spec.ts
```

**Test Coverage:**
- ✅ Load home page successfully
- ✅ Display hero banner
- ✅ Proper page structure with regions
- ✅ Navigate to products page
- ✅ Render product grid widget
- ✅ Render testimonials carousel
- ✅ Render feature grid widget
- ✅ Render call-to-action widget
- ✅ Render newsletter signup form
- ✅ Navigate hero banner CTA
- ✅ Navigate testimonials carousel
- ✅ Interact with newsletter form
- ✅ Navigate to about page
- ✅ Navigate to contact page
- ✅ Proper meta tags on pages
- ✅ Update page title on navigation
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Performance (load time < 5 seconds)

### CMS API Tests (9 tests)

Tests backend API endpoints.

```bash
# Run API tests
npm run test:e2e tests/e2e/cms/cms-api.spec.ts
```

**Test Coverage:**
- ✅ Fetch page definition for home
- ✅ Fetch available widgets
- ✅ Fetch widgets for page type
- ✅ Fetch widgets by category
- ✅ Page definition has required properties
- ✅ Regions contain widgets
- ✅ Widgets have component paths
- ✅ Widget definitions have settings metadata
- ✅ Widget categories are consistent
- ✅ Support different page types
- ✅ Product listing has product grid

### Run All E2E Tests

```bash
# Run all E2E tests
npm run test:e2e tests/e2e/cms

# Headed mode (see browser)
npm run test:e2e tests/e2e/cms --headed

# Debug mode
npm run test:e2e tests/e2e/cms --debug

# Specific browser
npm run test:e2e tests/e2e/cms --project=chromium
```

## Test Data Seeding

### Using CmsTestDataSeeder

The `CmsTestDataSeeder` provides factory methods for creating test data.

#### In Unit Tests

```csharp
// Create a single page
var homePage = CmsTestDataSeeder.CreateHomePage("tenant-1");

// Create multiple pages
var pages = CmsTestDataSeeder.CreateSamplePages("tenant-1");

// Use in tests
[Fact]
public void TestWithSeededData()
{
    var page = CmsTestDataSeeder.CreateProductListingPage();
    // Use page in test
}
```

#### In Integration Tests

```csharp
[Fact]
public async Task TestWithDatabaseSeeding()
{
    // Create in-memory database
    var options = new DbContextOptionsBuilder<CmsDbContext>()
        .UseInMemoryDatabase("test-db")
        .Options;
    
    using var context = new CmsDbContext(options);
    await context.Database.EnsureCreatedAsync();
    
    // Seed data
    var pages = CmsTestDataSeeder.CreateSamplePages();
    context.Pages.AddRange(pages);
    await context.SaveChangesAsync();
    
    // Test with seeded data
}
```

### Available Test Data

#### Pages

1. **Home Page**
   - 3 regions: header, main, footer
   - Widgets: hero-banner, feature-grid, product-grid, testimonials, CTA, newsletter

2. **Product Listing Page**
   - 3 regions: header, main, sidebar
   - Widgets: hero-banner, product-grid, CTA
   - Sidebar max widgets: 2

3. **About Page**
   - 1 region: main
   - Widgets: hero-banner, text-block, feature-grid

4. **Contact Page**
   - 1 region: main
   - Widgets: text-block

#### Widgets

All default widget definitions with complete settings:

- **hero-banner** - Full-width hero section
- **product-grid** - Product display grid
- **feature-grid** - Feature/benefits grid
- **testimonials** - Customer testimonials carousel
- **call-to-action** - CTA button section
- **text-block** - Rich text content
- **video** - Video embed
- **newsletter-signup** - Email subscription form

### Create Custom Test Data

```csharp
public class CustomTestDataSeeder
{
    public static PageDefinition CreateCustomPage()
    {
        var page = new PageDefinition(
            "tenant-1",
            "custom",
            "/custom",
            "Custom Page",
            "full-width");

        var region = new PageRegion
        {
            Name = "main",
            Order = 1
        };

        region.AddWidget("hero-banner", new Dictionary<string, object>
        {
            { "title", "Custom Title" },
            { "backgroundImage", "/images/custom.jpg" }
        });

        page.AddRegion(region);
        page.PublishPage();

        return page;
    }
}
```

## Running All Tests

### Run Everything

```bash
# Backend tests
dotnet test backend/Tests/B2X.CMS.Tests/

# Frontend component tests
npm run test tests/components/cms

# Frontend E2E tests
npm run test:e2e tests/e2e/cms
```

### Continuous Integration

```bash
# Run tests with CI flags
# Backend
dotnet test backend/Tests/B2X.CMS.Tests/ /p:CI=true

# Frontend
npm run test:ci tests/

# E2E
npm run test:e2e:ci tests/e2e/
```

## Test Metrics

### Backend Test Coverage

- **WidgetRegistry**: 100% coverage
- **PageDefinition**: 95% coverage
- **Query Handler**: 90% coverage
- **Overall**: 95% coverage target

### Frontend Test Coverage

- **Components**: 90% coverage target
- **E2E**: Key user flows covered
- **Integration**: API + UI interactions

## Best Practices

### Writing Tests

1. **Arrange-Act-Assert Pattern**
   ```csharp
   [Fact]
   public void Should_Do_Something()
   {
       // Arrange
       var data = new TestData();
       
       // Act
       var result = Service.Process(data);
       
       // Assert
       result.ShouldBe(expected);
   }
   ```

2. **Use Meaningful Names**
   ```csharp
   public void Should_ThrowException_WhenGivenInvalidInput()
   ```

3. **One Assert Per Test (When Possible)**
   ```csharp
   [Fact]
   public void Should_ReturnWidget()
   {
       var widget = _registry.GetWidget("hero-banner");
       widget.ShouldNotBeNull();
   }
   ```

4. **Use Test Data Builders**
   ```csharp
   var page = CmsTestDataSeeder.CreateHomePage();
   ```

### Test Organization

1. Group related tests with `describe` blocks
2. Use clear test names describing expected behavior
3. Arrange test data at the beginning
4. Clean up resources in `afterEach` or `IAsyncLifetime`

## Troubleshooting

### Backend Test Issues

```bash
# Clear build cache
dotnet clean backend/Tests/B2X.CMS.Tests/

# Rebuild
dotnet build backend/Tests/B2X.CMS.Tests/

# Run with verbose logging
dotnet test backend/Tests/B2X.CMS.Tests/ -v normal
```

### Frontend Test Issues

```bash
# Clear dependencies
rm -rf node_modules
npm install

# Clear cache
npm run test -- --clearCache

# Run with debugging
npm run test -- --inspect-brk
```

### E2E Test Issues

```bash
# Run with headed mode to see browser
npm run test:e2e -- --headed

# Run in debug mode
npm run test:e2e -- --debug

# Check traces and videos
# Located in: tests/e2e/test-results/
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: CMS Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0'
      
      - name: Backend Tests
        run: dotnet test backend/Tests/B2X.CMS.Tests/
      
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install Frontend Dependencies
        run: npm ci
      
      - name: Frontend Tests
        run: npm run test:ci tests/
      
      - name: E2E Tests
        run: npm run test:e2e:ci tests/e2e/cms/
```

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Vitest Documentation](https://vitest.dev/)
- [Playwright Documentation](https://playwright.dev/)
- [Test Best Practices](https://martinfowler.com/bliki/TestPyramid.html)
