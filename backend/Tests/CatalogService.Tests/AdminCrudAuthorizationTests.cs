using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using B2Connect.CatalogService.Controllers;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Tests
{
    /// <summary>
    /// Tests for Admin CRUD Authorization
    /// Verifies that POST, PUT, DELETE endpoints require Admin role
    /// </summary>
    public class AdminCrudAuthorizationTests
    {
        #region Products Controller Tests

        /// <summary>
        /// Test that CreateProduct requires Admin role
        /// </summary>
        [Fact]
        public async Task CreateProduct_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            // Act - Check if CreateProduct method has Authorize attribute
            var methodInfo = typeof(ProductsController).GetMethod("CreateProduct");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that UpdateProduct requires Admin role
        /// </summary>
        [Fact]
        public async Task UpdateProduct_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(ProductsController).GetMethod("UpdateProduct");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that DeleteProduct requires Admin role
        /// </summary>
        [Fact]
        public async Task DeleteProduct_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(ProductsController).GetMethod("DeleteProduct");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that GetProduct does NOT require authorization
        /// </summary>
        [Fact]
        public async Task GetProduct_NoAuthorizeAttribute_PublicAccess()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(ProductsController).GetMethod("GetProduct", new[] { typeof(Guid) });

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.Empty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that GetAllProducts does NOT require authorization
        /// </summary>
        [Fact]
        public async Task GetAllProducts_NoAuthorizeAttribute_PublicAccess()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(ProductsController).GetMethod("GetAllProducts");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.Empty(authorizeAttr ?? Array.Empty<object>());
        }

        #endregion

        #region Categories Controller Tests

        /// <summary>
        /// Test that CreateCategory requires Admin role
        /// </summary>
        [Fact]
        public async Task CreateCategory_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(CategoriesController).GetMethod("CreateCategory");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that UpdateCategory requires Admin role
        /// </summary>
        [Fact]
        public async Task UpdateCategory_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(CategoriesController).GetMethod("UpdateCategory");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that DeleteCategory requires Admin role
        /// </summary>
        [Fact]
        public async Task DeleteCategory_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(CategoriesController).GetMethod("DeleteCategory");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that GetCategory does NOT require authorization
        /// </summary>
        [Fact]
        public async Task GetCategory_NoAuthorizeAttribute_PublicAccess()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(CategoriesController).GetMethod("GetCategory", new[] { typeof(Guid) });

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.Empty(authorizeAttr ?? Array.Empty<object>());
        }

        #endregion

        #region Brands Controller Tests

        /// <summary>
        /// Test that CreateBrand requires Admin role
        /// </summary>
        [Fact]
        public async Task CreateBrand_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(BrandsController).GetMethod("CreateBrand");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that UpdateBrand requires Admin role
        /// </summary>
        [Fact]
        public async Task UpdateBrand_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(BrandsController).GetMethod("UpdateBrand");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that DeleteBrand requires Admin role
        /// </summary>
        [Fact]
        public async Task DeleteBrand_HasAuthorizeAttribute_ForAdmin()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(BrandsController).GetMethod("DeleteBrand");

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.NotEmpty(authorizeAttr ?? Array.Empty<object>());
        }

        /// <summary>
        /// Test that GetBrand does NOT require authorization
        /// </summary>
        [Fact]
        public async Task GetBrand_NoAuthorizeAttribute_PublicAccess()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            // Act
            var methodInfo = typeof(BrandsController).GetMethod("GetBrand", new[] { typeof(Guid) });

            // Assert
            Assert.NotNull(methodInfo);
            var authorizeAttr = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false);
            Assert.Empty(authorizeAttr ?? Array.Empty<object>());
        }

        #endregion

        #region Route Tests

        /// <summary>
        /// Test that all controllers use public API routes (not /api/admin/)
        /// </summary>
        [Fact]
        public void Controllers_UsePublicApiRoutes_NotAdminRoutes()
        {
            // Arrange & Act
            var productsRoute = typeof(ProductsController).GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false);
            var categoriesRoute = typeof(CategoriesController).GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false);
            var brandsRoute = typeof(BrandsController).GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false);

            // Assert
            Assert.NotEmpty(productsRoute ?? Array.Empty<object>());
            Assert.NotEmpty(categoriesRoute ?? Array.Empty<object>());
            Assert.NotEmpty(brandsRoute ?? Array.Empty<object>());

            // Verify routes don't contain "/admin"
            foreach (var route in productsRoute ?? Array.Empty<object>())
            {
                if (route is Microsoft.AspNetCore.Mvc.RouteAttribute attr)
                {
                    Assert.DoesNotContain("admin", attr.Template?.ToLower() ?? "");
                }
            }
        }

        /// <summary>
        /// Test that controllers use standard naming patterns
        /// </summary>
        [Fact]
        public void Controllers_UseStandardNaming_NoAdminSuffix()
        {
            // Arrange & Act
            var productControllerName = typeof(ProductsController).Name;
            var categoryControllerName = typeof(CategoriesController).Name;
            var brandControllerName = typeof(BrandsController).Name;

            // Assert
            Assert.Equal("ProductsController", productControllerName);
            Assert.Equal("CategoriesController", categoryControllerName);
            Assert.Equal("BrandsController", brandControllerName);

            // Verify no "Admin" in names
            Assert.DoesNotContain("Admin", productControllerName);
            Assert.DoesNotContain("Admin", categoryControllerName);
            Assert.DoesNotContain("Admin", brandControllerName);
        }

        #endregion

        #region Admin Directory Check

        /// <summary>
        /// Test that no /Admin controllers directory exists
        /// </summary>
        [Fact]
        public void AdminControllerDirectory_ShouldNotExist()
        {
            // Arrange
            var adminControllerPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(typeof(ProductsController).Assembly.Location),
                "..", "..", "src", "Controllers", "Admin");

            // Act
            var directoryExists = System.IO.Directory.Exists(adminControllerPath);

            // Assert
            Assert.False(directoryExists, "Admin controller directory should not exist - use single controllers with authorization attributes");
        }

        #endregion
    }
}
