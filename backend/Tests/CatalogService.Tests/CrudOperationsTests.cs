using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Functional tests for CRUD operations
    /// Tests that CRUD operations work correctly when authorized
    /// </summary>
    public class CrudOperationsTests
    {
        #region Product CRUD Tests

        /// <summary>
        /// Test that CreateProduct creates and returns product
        /// </summary>
        [Fact]
        public async Task CreateProduct_WithValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productDto = new CreateProductDto
            {
                Sku = "TEST-001",
                Slug = "test-product",
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m
            };

            var createdProduct = new ProductDto
            {
                Id = Guid.NewGuid(),
                Sku = productDto.Sku,
                Slug = productDto.Slug,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price
            };

            mockService
                .Setup(s => s.CreateProductAsync(It.IsAny<CreateProductDto>()))
                .ReturnsAsync(createdProduct);

            // Act
            var result = await controller.CreateProduct(productDto);

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ProductsController.GetProduct), createdResult.ActionName);
            var returnedProduct = Assert.IsType<ProductDto>(createdResult.Value);
            Assert.Equal(createdProduct.Id, returnedProduct.Id);
        }

        /// <summary>
        /// Test that UpdateProduct updates and returns product
        /// </summary>
        [Fact]
        public async Task UpdateProduct_WithValidData_ShouldReturnUpdatedProduct()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto
            {
                Sku = "TEST-001",
                Slug = "test-product",
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 149.99m
            };

            var updatedProduct = new ProductDto
            {
                Id = productId,
                Sku = updateDto.Sku,
                Slug = updateDto.Slug,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Price = updateDto.Price
            };

            mockService
                .Setup(s => s.UpdateProductAsync(productId, It.IsAny<UpdateProductDto>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await controller.UpdateProduct(productId, updateDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
            Assert.Equal("Updated Product", returnedProduct.Name);
        }

        /// <summary>
        /// Test that DeleteProduct deletes successfully
        /// </summary>
        [Fact]
        public async Task DeleteProduct_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productId = Guid.NewGuid();
            mockService
                .Setup(s => s.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
        }

        /// <summary>
        /// Test that DeleteProduct returns NotFound for missing product
        /// </summary>
        [Fact]
        public async Task DeleteProduct_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productId = Guid.NewGuid();
            mockService
                .Setup(s => s.DeleteProductAsync(productId))
                .ReturnsAsync(false);

            // Act
            var result = await controller.DeleteProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Category CRUD Tests

        /// <summary>
        /// Test that CreateCategory creates and returns category
        /// </summary>
        [Fact]
        public async Task CreateCategory_WithValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            var categoryDto = new CreateCategoryDto
            {
                Slug = "electronics",
                Name = "Electronics",
                Description = "Electronic products"
            };

            var createdCategory = new CategoryDto
            {
                Id = Guid.NewGuid(),
                Slug = categoryDto.Slug,
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            mockService
                .Setup(s => s.CreateCategoryAsync(It.IsAny<CreateCategoryDto>()))
                .ReturnsAsync(createdCategory);

            // Act
            var result = await controller.CreateCategory(categoryDto);

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(createdResult.Value);
            Assert.Equal(createdCategory.Id, returnedCategory.Id);
        }

        /// <summary>
        /// Test that UpdateCategory updates and returns category
        /// </summary>
        [Fact]
        public async Task UpdateCategory_WithValidData_ShouldReturnUpdatedCategory()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            var categoryId = Guid.NewGuid();
            var updateDto = new UpdateCategoryDto
            {
                Slug = "electronics",
                Name = "Updated Electronics",
                Description = "Updated description"
            };

            var updatedCategory = new CategoryDto
            {
                Id = categoryId,
                Slug = updateDto.Slug,
                Name = updateDto.Name,
                Description = updateDto.Description
            };

            mockService
                .Setup(s => s.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ReturnsAsync(updatedCategory);

            // Act
            var result = await controller.UpdateCategory(categoryId, updateDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(categoryId, returnedCategory.Id);
        }

        /// <summary>
        /// Test that DeleteCategory deletes successfully
        /// </summary>
        [Fact]
        public async Task DeleteCategory_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            var categoryId = Guid.NewGuid();
            mockService
                .Setup(s => s.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(true);

            // Act
            var result = await controller.DeleteCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region Brand CRUD Tests

        /// <summary>
        /// Test that CreateBrand creates and returns brand
        /// </summary>
        [Fact]
        public async Task CreateBrand_WithValidData_ShouldReturnCreatedResult()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            var brandDto = new CreateBrandDto
            {
                Slug = "apple",
                Name = "Apple",
                Description = "Apple Inc."
            };

            var createdBrand = new BrandDto
            {
                Id = Guid.NewGuid(),
                Slug = brandDto.Slug,
                Name = brandDto.Name,
                Description = brandDto.Description
            };

            mockService
                .Setup(s => s.CreateBrandAsync(It.IsAny<CreateBrandDto>()))
                .ReturnsAsync(createdBrand);

            // Act
            var result = await controller.CreateBrand(brandDto);

            // Assert
            Assert.NotNull(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedBrand = Assert.IsType<BrandDto>(createdResult.Value);
            Assert.Equal(createdBrand.Id, returnedBrand.Id);
        }

        /// <summary>
        /// Test that UpdateBrand updates and returns brand
        /// </summary>
        [Fact]
        public async Task UpdateBrand_WithValidData_ShouldReturnUpdatedBrand()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            var brandId = Guid.NewGuid();
            var updateDto = new UpdateBrandDto
            {
                Slug = "apple",
                Name = "Apple Inc.",
                Description = "Updated description"
            };

            var updatedBrand = new BrandDto
            {
                Id = brandId,
                Slug = updateDto.Slug,
                Name = updateDto.Name,
                Description = updateDto.Description
            };

            mockService
                .Setup(s => s.UpdateBrandAsync(brandId, It.IsAny<UpdateBrandDto>()))
                .ReturnsAsync(updatedBrand);

            // Act
            var result = await controller.UpdateBrand(brandId, updateDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBrand = Assert.IsType<BrandDto>(okResult.Value);
            Assert.Equal(brandId, returnedBrand.Id);
        }

        /// <summary>
        /// Test that DeleteBrand deletes successfully
        /// </summary>
        [Fact]
        public async Task DeleteBrand_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            var brandId = Guid.NewGuid();
            mockService
                .Setup(s => s.DeleteBrandAsync(brandId))
                .ReturnsAsync(true);

            // Act
            var result = await controller.DeleteBrand(brandId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region Read Operations Tests

        /// <summary>
        /// Test that GetProduct returns product without authorization
        /// </summary>
        [Fact]
        public async Task GetProduct_PublicAccess_ShouldReturnProduct()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productId = Guid.NewGuid();
            var product = new ProductDto { Id = productId, Name = "Test Product" };

            mockService
                .Setup(s => s.GetProductAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await controller.GetProduct(productId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        /// <summary>
        /// Test that GetCategory returns category without authorization
        /// </summary>
        [Fact]
        public async Task GetCategory_PublicAccess_ShouldReturnCategory()
        {
            // Arrange
            var mockService = new Mock<ICategoryService>();
            var mockLogger = new Mock<ILogger<CategoriesController>>();
            var controller = new CategoriesController(mockService.Object, mockLogger.Object);

            var categoryId = Guid.NewGuid();
            var category = new CategoryDto { Id = categoryId, Name = "Electronics" };

            mockService
                .Setup(s => s.GetCategoryAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            var result = await controller.GetCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(categoryId, returnedCategory.Id);
        }

        /// <summary>
        /// Test that GetBrand returns brand without authorization
        /// </summary>
        [Fact]
        public async Task GetBrand_PublicAccess_ShouldReturnBrand()
        {
            // Arrange
            var mockService = new Mock<IBrandService>();
            var mockLogger = new Mock<ILogger<BrandsController>>();
            var controller = new BrandsController(mockService.Object, mockLogger.Object);

            var brandId = Guid.NewGuid();
            var brand = new BrandDto { Id = brandId, Name = "Apple" };

            mockService
                .Setup(s => s.GetBrandAsync(brandId))
                .ReturnsAsync(brand);

            // Act
            var result = await controller.GetBrand(brandId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBrand = Assert.IsType<BrandDto>(okResult.Value);
            Assert.Equal(brandId, returnedBrand.Id);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// Test that UpdateProduct returns NotFound for non-existent product
        /// </summary>
        [Fact]
        public async Task UpdateProduct_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var productId = Guid.NewGuid();
            var updateDto = new UpdateProductDto();

            mockService
                .Setup(s => s.UpdateProductAsync(productId, It.IsAny<UpdateProductDto>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await controller.UpdateProduct(productId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Test that CreateProduct returns BadRequest for validation error
        /// </summary>
        [Fact]
        public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(mockService.Object, mockLogger.Object);

            var invalidDto = new CreateProductDto();

            mockService
                .Setup(s => s.CreateProductAsync(It.IsAny<CreateProductDto>()))
                .ThrowsAsync(new ArgumentException("Invalid product data"));

            // Act
            var result = await controller.CreateProduct(invalidDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion
    }
}
