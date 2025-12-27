using Wolverine;
using B2Connect.Admin.Application.Commands.Categories;
using B2Connect.Admin.Core.Interfaces;

namespace B2Connect.Admin.Application.Handlers.Categories;

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Create Category
// ─────────────────────────────────────────────────────────────────────────────
public class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(ICategoryRepository repository, ILogger<CreateCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CategoryResult> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Creating category '{Name}' (Slug: {Slug}) for tenant {TenantId}",
            command.Name, command.Slug, command.TenantId);

        // Validation
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Category name is required");

        if (string.IsNullOrWhiteSpace(command.Slug))
            throw new ArgumentException("Category slug is required");

        // Business Logic
        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            ParentId = command.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category, ct);

        _logger.LogInformation("Category {CategoryId} created successfully", category.Id);

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Update Category
// ─────────────────────────────────────────────────────────────────────────────
public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(ICategoryRepository repository, ILogger<UpdateCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CategoryResult> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Updating category {CategoryId} for tenant {TenantId}",
            command.CategoryId, command.TenantId);

        var category = await _repository.GetByIdAsync(command.TenantId, command.CategoryId, ct);
        if (category == null)
            throw new KeyNotFoundException($"Category {command.CategoryId} not found");

        category.Name = command.Name;
        category.Slug = command.Slug;
        category.Description = command.Description;
        category.ParentId = command.ParentId;

        await _repository.UpdateAsync(category, ct);

        _logger.LogInformation("Category {CategoryId} updated successfully", command.CategoryId);

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Delete Category
// ─────────────────────────────────────────────────────────────────────────────
public class DeleteCategoryHandler : ICommandHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(ICategoryRepository repository, ILogger<DeleteCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Deleting category {CategoryId} from tenant {TenantId}",
            command.CategoryId, command.TenantId);

        var category = await _repository.GetByIdAsync(command.TenantId, command.CategoryId, ct);
        if (category == null)
            return false;

        await _repository.DeleteAsync(command.TenantId, command.CategoryId, ct);

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

    public GetCategoryHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CategoryResult?> Handle(GetCategoryQuery query, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(query.TenantId, query.CategoryId, ct);

        if (category == null)
            return null;

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Category by Slug
// ─────────────────────────────────────────────────────────────────────────────
public class GetCategoryBySlugHandler : IQueryHandler<GetCategoryBySlugQuery, CategoryResult?>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryBySlugHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CategoryResult?> Handle(GetCategoryBySlugQuery query, CancellationToken ct)
    {
        var category = await _repository.GetBySlugAsync(query.TenantId, query.Slug, ct);

        if (category == null)
            return null;

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Root Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetRootCategoriesHandler : IQueryHandler<GetRootCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;

    public GetRootCategoriesHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetRootCategoriesQuery query, CancellationToken ct)
    {
        var categories = await _repository.GetRootCategoriesAsync(query.TenantId, ct);

        return categories.Select(c => new CategoryResult(
            c.Id, c.TenantId, c.Name, c.Slug,
            c.Description, c.ParentId, c.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Child Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetChildCategoriesHandler : IQueryHandler<GetChildCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;

    public GetChildCategoriesHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetChildCategoriesQuery query, CancellationToken ct)
    {
        var categories = await _repository.GetChildCategoriesAsync(query.TenantId, query.ParentId, ct);

        return categories.Select(c => new CategoryResult(
            c.Id, c.TenantId, c.Name, c.Slug,
            c.Description, c.ParentId, c.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Category Hierarchy
// ─────────────────────────────────────────────────────────────────────────────
public class GetCategoryHierarchyHandler : IQueryHandler<GetCategoryHierarchyQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryHierarchyHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetCategoryHierarchyQuery query, CancellationToken ct)
    {
        var categories = await _repository.GetHierarchyAsync(query.TenantId, ct);

        return categories.Select(c => new CategoryResult(
            c.Id, c.TenantId, c.Name, c.Slug,
            c.Description, c.ParentId, c.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Active Categories
// ─────────────────────────────────────────────────────────────────────────────
public class GetActiveCategoriesHandler : IQueryHandler<GetActiveCategoriesQuery, IEnumerable<CategoryResult>>
{
    private readonly ICategoryRepository _repository;

    public GetActiveCategoriesHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<CategoryResult>> Handle(GetActiveCategoriesQuery query, CancellationToken ct)
    {
        var categories = await _repository.GetActiveCategoriesAsync(query.TenantId, ct);

        return categories.Select(c => new CategoryResult(
            c.Id, c.TenantId, c.Name, c.Slug,
            c.Description, c.ParentId, c.CreatedAt));
    }
}
