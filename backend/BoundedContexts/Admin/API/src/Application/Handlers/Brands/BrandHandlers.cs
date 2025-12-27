using Wolverine;
using B2Connect.Admin.Application.Commands.Brands;
using B2Connect.Admin.Core.Interfaces;

namespace B2Connect.Admin.Application.Handlers.Brands;

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Create Brand
// ─────────────────────────────────────────────────────────────────────────────
public class CreateBrandHandler : ICommandHandler<CreateBrandCommand, BrandResult>
{
    private readonly IBrandRepository _repository;
    private readonly ILogger<CreateBrandHandler> _logger;

    public CreateBrandHandler(IBrandRepository repository, ILogger<CreateBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BrandResult> Handle(CreateBrandCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Creating brand '{Name}' (Slug: {Slug}) for tenant {TenantId}",
            command.Name, command.Slug, command.TenantId);

        // Validation
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Brand name is required");

        if (string.IsNullOrWhiteSpace(command.Slug))
            throw new ArgumentException("Brand slug is required");

        // Business Logic
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Slug = command.Slug,
            Logo = command.Logo,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(brand, ct);

        _logger.LogInformation("Brand {BrandId} created successfully", brand.Id);

        return new BrandResult(
            brand.Id, brand.TenantId, brand.Name, brand.Slug,
            brand.Logo, brand.Description, brand.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Update Brand
// ─────────────────────────────────────────────────────────────────────────────
public class UpdateBrandHandler : ICommandHandler<UpdateBrandCommand, BrandResult>
{
    private readonly IBrandRepository _repository;
    private readonly ILogger<UpdateBrandHandler> _logger;

    public UpdateBrandHandler(IBrandRepository repository, ILogger<UpdateBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BrandResult> Handle(UpdateBrandCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Updating brand {BrandId} for tenant {TenantId}",
            command.BrandId, command.TenantId);

        var brand = await _repository.GetByIdAsync(command.TenantId, command.BrandId, ct);
        if (brand == null)
            throw new KeyNotFoundException($"Brand {command.BrandId} not found");

        brand.Name = command.Name;
        brand.Slug = command.Slug;
        brand.Logo = command.Logo;
        brand.Description = command.Description;

        await _repository.UpdateAsync(brand, ct);

        _logger.LogInformation("Brand {BrandId} updated successfully", command.BrandId);

        return new BrandResult(
            brand.Id, brand.TenantId, brand.Name, brand.Slug,
            brand.Logo, brand.Description, brand.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Command Handler für Delete Brand
// ─────────────────────────────────────────────────────────────────────────────
public class DeleteBrandHandler : ICommandHandler<DeleteBrandCommand, bool>
{
    private readonly IBrandRepository _repository;
    private readonly ILogger<DeleteBrandHandler> _logger;

    public DeleteBrandHandler(IBrandRepository repository, ILogger<DeleteBrandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteBrandCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Deleting brand {BrandId} from tenant {TenantId}",
            command.BrandId, command.TenantId);

        var brand = await _repository.GetByIdAsync(command.TenantId, command.BrandId, ct);
        if (brand == null)
            return false;

        await _repository.DeleteAsync(command.TenantId, command.BrandId, ct);

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

    public GetBrandHandler(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BrandResult?> Handle(GetBrandQuery query, CancellationToken ct)
    {
        var brand = await _repository.GetByIdAsync(query.TenantId, query.BrandId, ct);

        if (brand == null)
            return null;

        return new BrandResult(
            brand.Id, brand.TenantId, brand.Name, brand.Slug,
            brand.Logo, brand.Description, brand.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Brand by Slug
// ─────────────────────────────────────────────────────────────────────────────
public class GetBrandBySlugHandler : IQueryHandler<GetBrandBySlugQuery, BrandResult?>
{
    private readonly IBrandRepository _repository;

    public GetBrandBySlugHandler(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BrandResult?> Handle(GetBrandBySlugQuery query, CancellationToken ct)
    {
        var brand = await _repository.GetBySlugAsync(query.TenantId, query.Slug, ct);

        if (brand == null)
            return null;

        return new BrandResult(
            brand.Id, brand.TenantId, brand.Name, brand.Slug,
            brand.Logo, brand.Description, brand.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Active Brands
// ─────────────────────────────────────────────────────────────────────────────
public class GetActiveBrandsHandler : IQueryHandler<GetActiveBrandsQuery, IEnumerable<BrandResult>>
{
    private readonly IBrandRepository _repository;

    public GetActiveBrandsHandler(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<BrandResult>> Handle(GetActiveBrandsQuery query, CancellationToken ct)
    {
        var brands = await _repository.GetActiveBrandsAsync(query.TenantId, ct);

        return brands.Select(b => new BrandResult(
            b.Id, b.TenantId, b.Name, b.Slug,
            b.Logo, b.Description, b.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für Get Brands Paged
// ─────────────────────────────────────────────────────────────────────────────
public class GetBrandsPagedHandler : IQueryHandler<GetBrandsPagedQuery, (IEnumerable<BrandResult>, int)>
{
    private readonly IBrandRepository _repository;

    public GetBrandsPagedHandler(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<(IEnumerable<BrandResult>, int)> Handle(GetBrandsPagedQuery query, CancellationToken ct)
    {
        var (brands, total) = await _repository.GetPagedAsync(
            query.TenantId, query.PageNumber, query.PageSize, ct);

        var results = brands.Select(b => new BrandResult(
            b.Id, b.TenantId, b.Name, b.Slug,
            b.Logo, b.Description, b.CreatedAt));

        return (results, total);
    }
}
