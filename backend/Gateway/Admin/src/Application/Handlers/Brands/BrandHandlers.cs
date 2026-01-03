using B2Connect.Admin.Application.Commands.Brands;
using B2Connect.Admin.Application.Handlers;
using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Middleware;
using B2Connect.Types.Localization;
using Wolverine;

namespace B2Connect.Admin.Application.Handlers.Brands;

/// <summary>
/// Helper method for converting Brand entities to BrandResult DTOs
/// </summary>
internal static class BrandMapper
{
    public static BrandResult ToResult(Brand brand)
        => new BrandResult(
            brand.Id,
            brand.TenantId ?? Guid.Empty,
            brand.Name?.Get("en") ?? string.Empty,
            brand.Slug,
            brand.LogoUrl,
            brand.Description?.Get("en"),
            brand.CreatedAt);
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Create Brand
// ─────────────────────────────────────────────────────────────────────────────
public class CreateBrandHandler : ICommandHandler<CreateBrandCommand, BrandResult>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateBrandHandler(
        IBrandRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<CreateBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BrandResult> Handle(CreateBrandCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Creating brand '{Name}' (Slug: {Slug}) for tenant {TenantId}",
            command.Name, command.Slug, tenantId);

        // Validation
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Brand name is required");
        }

        if (string.IsNullOrWhiteSpace(command.Slug))
        {
            throw new ArgumentException("Brand slug is required");
        }

        // Business Logic - convert string to LocalizedContent
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", command.Name),
            Slug = command.Slug,
            LogoUrl = command.Logo,
            Description = command.Description != null
                ? new LocalizedContent().Set("en", command.Description)
                : null,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(brand, ct);

        _logger.LogInformation("Brand {BrandId} created successfully", brand.Id);

        return BrandMapper.ToResult(brand);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Update Brand
// ─────────────────────────────────────────────────────────────────────────────
public class UpdateBrandHandler : ICommandHandler<UpdateBrandCommand, BrandResult>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<UpdateBrandHandler> _logger;

    public UpdateBrandHandler(
        IBrandRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<UpdateBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BrandResult> Handle(UpdateBrandCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Updating brand {BrandId} for tenant {TenantId}",
            command.BrandId, tenantId);

        var brand = await _repository.GetByIdAsync(tenantId, command.BrandId, ct);
        if (brand == null)
        {
            throw new KeyNotFoundException($"Brand {command.BrandId} not found");
        }

        // Update fields - convert string to LocalizedContent
        brand.Name = new LocalizedContent().Set("en", command.Name);
        brand.Slug = command.Slug;
        brand.LogoUrl = command.Logo;
        brand.Description = command.Description != null
            ? new LocalizedContent().Set("en", command.Description)
            : null;
        brand.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(brand, ct);

        _logger.LogInformation("Brand {BrandId} updated successfully", command.BrandId);

        return BrandMapper.ToResult(brand);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Delete Brand
// ─────────────────────────────────────────────────────────────────────────────
public class DeleteBrandHandler : ICommandHandler<DeleteBrandCommand, bool>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<DeleteBrandHandler> _logger;

    public DeleteBrandHandler(
        IBrandRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<DeleteBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteBrandCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Deleting brand {BrandId} from tenant {TenantId}",
            command.BrandId, tenantId);

        var brand = await _repository.GetByIdAsync(tenantId, command.BrandId, ct);
        if (brand == null)
        {
            return false;
        }

        await _repository.DeleteAsync(tenantId, command.BrandId, ct);

        _logger.LogInformation("Brand {BrandId} deleted successfully", command.BrandId);
        return true;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Brand by ID
// ─────────────────────────────────────────────────────────────────────────────
public class GetBrandHandler : IQueryHandler<GetBrandQuery, BrandResult?>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetBrandHandler(IBrandRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<BrandResult?> Handle(GetBrandQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var brand = await _repository.GetByIdAsync(tenantId, query.BrandId, ct);

        if (brand == null)
        {
            return null;
        }

        return BrandMapper.ToResult(brand);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Brand by Slug
// ─────────────────────────────────────────────────────────────────────────────
public class GetBrandBySlugHandler : IQueryHandler<GetBrandBySlugQuery, BrandResult?>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetBrandBySlugHandler(IBrandRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<BrandResult?> Handle(GetBrandBySlugQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var brand = await _repository.GetBySlugAsync(tenantId, query.Slug, ct);

        if (brand == null)
        {
            return null;
        }

        return BrandMapper.ToResult(brand);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Active Brands
// ─────────────────────────────────────────────────────────────────────────────
public class GetActiveBrandsHandler : IQueryHandler<GetActiveBrandsQuery, IEnumerable<BrandResult>>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetActiveBrandsHandler(IBrandRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<BrandResult>> Handle(GetActiveBrandsQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var brands = await _repository.GetActiveBrandsAsync(tenantId, ct);

        return brands.Select(BrandMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Brands Paged
// ─────────────────────────────────────────────────────────────────────────────
public class GetBrandsPagedHandler : IQueryHandler<GetBrandsPagedQuery, (IEnumerable<BrandResult>, int)>
{
    private readonly IBrandRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetBrandsPagedHandler(IBrandRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<(IEnumerable<BrandResult>, int)> Handle(GetBrandsPagedQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var (brands, total) = await _repository.GetPagedAsync(
            tenantId,
            query.PageNumber,
            query.PageSize,
            ct);

        var results = brands.Select(BrandMapper.ToResult);

        return (results, total);
    }
}
