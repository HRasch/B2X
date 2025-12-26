using Wolverine;
using B2Connect.CatalogService.CQRS.Events;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Data.ReadModel;
using Microsoft.Extensions.Caching.Distributed;

namespace B2Connect.CatalogService.CQRS.Handlers.Events;

/// <summary>
/// Event handler for ProductCreatedEvent
/// Wolverine automatically discovers and invokes this when ProductCreatedEvent is published
/// Multiple handlers can subscribe to the same event
/// This handler updates the read model for optimized queries
/// </summary>
public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        CatalogReadDbContext readDb,
        ILogger<ProductCreatedEventHandler> logger)
    {
        _readDb = readDb ?? throw new ArgumentNullException(nameof(readDb));
        _logger = logger;
    }

    public async Task Handle(
        ProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            // Create denormalized read model entry
            var searchText = string.Concat(
                @event.Name, " ",
                @event.Description ?? "", " ",
                @event.Sku
            ).ToLower();

            var readModel = new ProductReadModel
            {
                Id = @event.ProductId,
                TenantId = @event.TenantId,
                Sku = @event.Sku,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price,
                Category = @event.Category,
                IsAvailable = @event.IsAvailable,
                StockQuantity = @event.StockQuantity,
                SearchText = searchText,
                CreatedAt = @event.CreatedAt,
                UpdatedAt = @event.CreatedAt
            };

            await _readDb.ProductsReadModel.AddAsync(readModel, cancellationToken);
            await _readDb.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product created event processed: {ProductId} ({Sku}) for tenant {TenantId}",
                @event.ProductId, @event.Sku, @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductCreatedEvent for {ProductId}", @event.ProductId);
            throw;  // Wolverine will handle retry/DLQ based on config
        }
    }
}

/// <summary>
/// Event handler for ProductUpdatedEvent
/// Updates the read model when a product is modified
/// </summary>
public class ProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(
        CatalogReadDbContext readDb,
        ILogger<ProductUpdatedEventHandler> logger)
    {
        _readDb = readDb ?? throw new ArgumentNullException(nameof(readDb));
        _logger = logger;
    }

    public async Task Handle(
        ProductUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _readDb.ProductsReadModel
                .FirstOrDefaultAsync(p => p.Id == @event.ProductId, cancellationToken);

            if (existing == null)
            {
                _logger.LogWarning(
                    "ProductUpdatedEvent received but read model entry not found: {ProductId}",
                    @event.ProductId);
                return;
            }

            // Update only changed fields
            if (@event.Name != null)
                existing.Name = @event.Name;
            if (@event.Price.HasValue)
                existing.Price = @event.Price.Value;
            if (@event.Description != null)
                existing.Description = @event.Description;
            if (@event.Category != null)
                existing.Category = @event.Category;
            if (@event.IsAvailable.HasValue)
                existing.IsAvailable = @event.IsAvailable.Value;
            if (@event.StockQuantity.HasValue)
                existing.StockQuantity = @event.StockQuantity.Value;

            // Rebuild search text
            if (@event.Name != null || @event.Description != null)
            {
                existing.SearchText = string.Concat(
                    existing.Name, " ",
                    existing.Description ?? "", " ",
                    existing.Sku
                ).ToLower();
            }

            existing.UpdatedAt = @event.UpdatedAt;

            _readDb.ProductsReadModel.Update(existing);
            await _readDb.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product updated event processed: {ProductId}",
                @event.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductUpdatedEvent for {ProductId}", @event.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Event handler for ProductDeletedEvent
/// Removes or archives the product in read model
/// </summary>
public class ProductDeletedEventHandler : IEventHandler<ProductDeletedEvent>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(
        CatalogReadDbContext readDb,
        ILogger<ProductDeletedEventHandler> logger)
    {
        _readDb = readDb ?? throw new ArgumentNullException(nameof(readDb));
        _logger = logger;
    }

    public async Task Handle(
        ProductDeletedEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _readDb.ProductsReadModel
                .FirstOrDefaultAsync(p => p.Id == @event.ProductId, cancellationToken);

            if (existing == null)
            {
                _logger.LogWarning(
                    "ProductDeletedEvent received but read model entry not found: {ProductId}",
                    @event.ProductId);
                return;
            }

            // Soft delete (mark as deleted)
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;

            _readDb.ProductsReadModel.Update(existing);
            await _readDb.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Product deleted event processed: {ProductId} ({Sku})",
                @event.ProductId, @event.Sku);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductDeletedEvent for {ProductId}", @event.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Event handler for ProductsBulkImportedEvent
/// Called after bulk import completes
/// Can trigger read model rebuilds or cache invalidation
/// </summary>
public class ProductsBulkImportedEventHandler : IEventHandler<ProductsBulkImportedEvent>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductsBulkImportedEventHandler> _logger;

    public ProductsBulkImportedEventHandler(
        CatalogReadDbContext readDb,
        IDistributedCache cache,
        ILogger<ProductsBulkImportedEventHandler> logger)
    {
        _readDb = readDb ?? throw new ArgumentNullException(nameof(readDb));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger;
    }

    public async Task Handle(
        ProductsBulkImportedEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            // Mark all products imported in this batch with ImportId
            // This allows tracking which products came from which import
            var productsToUpdate = await _readDb.ProductsReadModel
                .Where(p => p.TenantId == @event.TenantId && p.ImportId == @event.ImportId)
                .ToListAsync(cancellationToken);

            foreach (var product in productsToUpdate)
            {
                product.ImportId = @event.ImportId;
                product.UpdatedAt = DateTime.UtcNow;
            }

            if (productsToUpdate.Count > 0)
            {
                _readDb.ProductsReadModel.UpdateRange(productsToUpdate);
                await _readDb.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Bulk import completed: ImportId={ImportId}, Count={ProductCount}, Tenant={TenantId}, ReadModelCount={UpdatedCount}",
                @event.ImportId, @event.ProductCount, @event.TenantId, productsToUpdate.Count);

            // Invalidate catalog stats cache for this tenant
            var cacheKey = $"catalog_stats_{@event.TenantId}";
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogInformation("Invalidated cache for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling ProductsBulkImportedEvent for ImportId={ImportId}",
                @event.ImportId);
            throw;
        }
    }
}
