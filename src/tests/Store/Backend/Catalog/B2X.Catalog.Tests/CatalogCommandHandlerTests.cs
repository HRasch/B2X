using B2X.Catalog.Application.Commands;
using B2X.Catalog.Application.Handlers;
using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Models;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Application.Handlers;

/// <summary>
/// Tests for CatalogCommandHandler demonstrating current tenant handling
/// Note: Future refactoring could use Wolverine's TenantId injection (v3.6+ feature)
/// when the project upgrades to compatible Wolverine version
/// </summary>
public class CatalogCommandHandlerTests
{
    private readonly Mock<ICatalogRepository> _repositoryMock;
    private readonly Guid _tenantId;

    public CatalogCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICatalogRepository>();
        _tenantId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_CreateCategoryCommand_WithTenantIdInCommand()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            TenantId: _tenantId,
            Name: "Test Category",
            Description: "Test Description",
            Slug: "test-category",
            ParentId: null,
            ImageUrl: null,
            Icon: null,
            DisplayOrder: 1,
            MetaTitle: null,
            MetaDescription: null,
            IsActive: true,
            IsVisible: true
        );

        var expectedCategory = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Name = command.Name,
            Description = command.Description,
            Slug = command.Slug,
            IsActive = command.IsActive,
            IsVisible = command.IsVisible
        };

        _repositoryMock
            .Setup(r => r.AddCategoryAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask)
            .Callback<Category>(c => expectedCategory = c);

        // Act
        var result = await CatalogCommandHandler.Handle(command, _repositoryMock.Object);

        // Assert
        result.ShouldNotBeNull();
        result.TenantId.ShouldBe(_tenantId);
        result.Name.ShouldBe(command.Name);
        result.IsActive.ShouldBeTrue();

        _repositoryMock.Verify(r => r.AddCategoryAsync(It.Is<Category>(c => c.TenantId == _tenantId)), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCategoryCommand_WithTenantIdInCommand()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = categoryId,
            TenantId = _tenantId,
            Name = "Old Name",
            IsActive = false
        };

        var command = new UpdateCategoryCommand(
            Id: categoryId,
            TenantId: _tenantId,
            Name: "Updated Category",
            Description: "Updated Description",
            Slug: "updated-category",
            ParentId: null,
            ImageUrl: null,
            Icon: null,
            DisplayOrder: 2,
            MetaTitle: null,
            MetaDescription: null,
            IsActive: true,
            IsVisible: true
        );

        _repositoryMock
            .Setup(r => r.GetCategoryByIdAsync(categoryId, _tenantId))
            .ReturnsAsync(existingCategory);

        _repositoryMock
            .Setup(r => r.UpdateCategoryAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await CatalogCommandHandler.Handle(command, _repositoryMock.Object);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(categoryId);
        result.TenantId.ShouldBe(_tenantId);
        result.Name.ShouldBe(command.Name);
        result.IsActive.ShouldBeTrue();

        _repositoryMock.Verify(r => r.GetCategoryByIdAsync(categoryId, _tenantId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateCategoryAsync(It.Is<Category>(c => c.TenantId == _tenantId)), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCategoryCommand_WithTenantIdInCommand()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId, _tenantId);

        _repositoryMock
            .Setup(r => r.DeleteCategoryAsync(categoryId, _tenantId))
            .Returns(Task.CompletedTask);

        // Act
        await CatalogCommandHandler.Handle(command, _repositoryMock.Object);

        // Assert
        _repositoryMock.Verify(r => r.DeleteCategoryAsync(categoryId, _tenantId), Times.Once);
    }
}