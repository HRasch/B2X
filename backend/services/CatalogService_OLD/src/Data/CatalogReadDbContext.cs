using Microsoft.EntityFrameworkCore;
using B2Connect.CatalogService.Data.ReadModel;

namespace B2Connect.CatalogService.Data;

/// <summary>
/// CQRS Read Model Database Context
/// Separate from CatalogDbContext (write model)
/// 
/// This context handles:
/// - Denormalized product views optimized for querying
/// - Asynchronous updates from domain events
/// - Eventual consistency between write and read models
/// - Multi-tenant isolation at database level
/// 
/// Scalability Benefits:
/// - No expensive joins for million-product queries
/// - Simple WHERE filters on indexed columns
/// - ElasticSearch integration for full-text search
/// - Easy horizontal scaling (read replicas)
/// </summary>
public class CatalogReadDbContext : DbContext
{
    public CatalogReadDbContext(DbContextOptions<CatalogReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductReadModel> ProductsReadModel { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new ProductReadModelConfiguration());

        // Global tenant filter (if needed)
        // This ensures queries always filter by tenant
    }

    /// <summary>
    /// Rebuild read model from write model
    /// Used for:
    /// - Initial sync
    /// - Data recovery after corruption
    /// - Periodic consistency checks
    /// Performs batch insert for performance on large datasets
    /// </summary>
    public async Task RebuildReadModelAsync(CatalogDbContext writeContext, CancellationToken cancellationToken)
    {
        if (writeContext == null)
            throw new ArgumentNullException(nameof(writeContext));

        try
        {
            // Clear existing read model
            await ProductsReadModel.ExecuteDeleteAsync(cancellationToken);

            // Populate from write model using batch inserts
            // Process in batches to avoid memory issues with large datasets
            const int batchSize = 1000;
            var products = writeContext.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted) // Only active products
                .Select(p => new ProductReadModel
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    Sku = p.Sku,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = p.Category ?? "Uncategorized",
                    IsAvailable = p.IsActive,
                    StockQuantity = p.StockQuantity,
                    SearchText = $"{p.Name} {p.Description} {p.Sku}".ToLower(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsDeleted = false
                });

            // Batch insert to improve performance
            var batch = new List<ProductReadModel>(batchSize);
            await foreach (var product in products.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                batch.Add(product);

                if (batch.Count >= batchSize)
                {
                    await ProductsReadModel.AddRangeAsync(batch, cancellationToken);
                    await SaveChangesAsync(cancellationToken);
                    batch.Clear();
                }
            }

            // Insert remaining products
            if (batch.Count > 0)
            {
                await ProductsReadModel.AddRangeAsync(batch, cancellationToken);
                await SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to rebuild read model from write model", ex);
        }
    }
}
