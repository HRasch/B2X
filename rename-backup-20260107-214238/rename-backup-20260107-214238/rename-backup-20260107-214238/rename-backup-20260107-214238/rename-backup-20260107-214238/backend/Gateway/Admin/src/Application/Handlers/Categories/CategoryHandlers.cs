using B2Connect.Admin.Application.Commands.Categories;
using B2Connect.Admin.Application.Handlers;
using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Shared.Middleware;
using B2Connect.Types.Localization;
using Wolverine;

namespace B2Connect.Admin.Application.Handlers.Categories;

/// <summary>
/// Helper method for converting Category entities to CategoryResult DTOs
/// </summary>
internal static class CategoryMapper
{
    public static CategoryResult ToResult(Category category)
        => new CategoryResult(
            category.Id,
            category.TenantId ?? Guid.Empty,
            category.Name?.Get("en") ?? string.Empty,
            category.Slug,
            category.Description?.Get("en"),
            category.ParentCategoryId,
            category.CreatedAt);
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Create Category
// ─────────────────────────────────────────────────────────────────────────────
public class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(
        ICategoryRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<CreateCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Creating category '{Name}' (Slug: {Slug}) for tenant {TenantId}",
            command.Name, command.Slug, tenantId);

        // Validation
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Category name is required");
        }

        if (string.IsNullOrWhiteSpace(command.Slug))
        {
            throw new ArgumentException("Category slug is required");
        }

        // Business Logic - convert string to LocalizedContent
        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", command.Name),
            Slug = command.Slug,
            Description = command.Description != null
                ? new LocalizedContent().Set("en", command.Description)
                : null,
            ParentCategoryId = command.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Category {CategoryId} created successfully", category.Id);

        return CategoryMapper.ToResult(category);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Update Category
// ─────────────────────────────────────────────────────────────────────────────
public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(
        ICategoryRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<UpdateCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Updating category {CategoryId} for tenant {TenantId}",
            command.CategoryId, tenantId);

        var category = await _repository.GetByIdAsync(tenantId, command.CategoryId, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category {command.CategoryId} not found");
        }

        // Update fields - convert string to LocalizedContent
        category.Name = new LocalizedContent().Set("en", command.Name);
        category.Slug = command.Slug;
        category.Description = command.Description != null
            ? new LocalizedContent().Set("en", command.Description)
            : null;
        category.ParentCategoryId = command.ParentId;

        await _repository.UpdateAsync(category, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Category {CategoryId} updated successfully", command.CategoryId);

        return CategoryMapper.ToResult(category);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Delete Category
// ─────────────────────────────────────────────────────────────────────────────
public class DeleteCategoryHandler : ICommandHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(
        ICategoryRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<DeleteCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Deleting category {CategoryId} from tenant {TenantId}",
            command.CategoryId, tenantId);

        var category = await _repository.GetByIdAsync(tenantId, command.CategoryId, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return false;
        }

        await _repository.DeleteAsync(tenantId, command.CategoryId, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Category {CategoryId} deleted successfully", command.CategoryId);
        return true;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Category by ID
// ─────────────────────────────────────────────────────────────────────────────
public class GetCategoryHandler : IQueryHandler<GetCategoryQuery, CategoryResult?>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetCategoryHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<CategoryResult?> Handle(GetCategoryQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var category = await _repository.GetByIdAsync(tenantId, query.CategoryId, cancellationToken).ConfigureAwait(false);

        if (category == null)
        {
            return null;
        }

        return CategoryMapper.ToResult(category);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Category by Slug
// ─────────────────────────────────────────────────────────────────────────────
public class GetCategoryBySlugHandler : IQueryHandler<GetCategoryBySlugQuery, CategoryResult?>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetCategoryBySlugHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<CategoryResult?> Handle(GetCategoryBySlugQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var category = await _repository.GetBySlugAsync(tenantId, query.Slug, cancellationToken).ConfigureAwait(false);

        if (category == null)
        {
            return null;
        }

        return CategoryMapper.ToResult(category);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Root Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetRootCategoriesHandler : IQueryHandler<GetRootCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetRootCategoriesHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetRootCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var categories = await _repository.GetRootCategoriesAsync(tenantId, cancellationToken).ConfigureAwait(false);

        return categories.Select(CategoryMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Child Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetChildCategoriesHandler : IQueryHandler<GetChildCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetChildCategoriesHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetChildCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var categories = await _repository.GetChildCategoriesAsync(tenantId, query.ParentId, cancellationToken).ConfigureAwait(false);

        return categories.Select(CategoryMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Category Hierarchy
// ─────────────────────────────────────────────────────────────────────────────
public class GetCategoryHierarchyHandler : IQueryHandler<GetCategoryHierarchyQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetCategoryHierarchyHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetCategoryHierarchyQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var categories = await _repository.GetHierarchyAsync(tenantId, cancellationToken).ConfigureAwait(false);

        return categories.Select(CategoryMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Active Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetActiveCategoriesHandler : IQueryHandler<GetActiveCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetActiveCategoriesHandler(ICategoryRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetActiveCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        var categories = await _repository.GetActiveCategoriesAsync(tenantId, cancellationToken).ConfigureAwait(false);

        return categories.Select(CategoryMapper.ToResult);
    }
}
