using B2X.Admin.Infrastructure.Data.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace B2X.Admin.Infrastructure.Data;

/// <summary>
/// CQRS Read Model Database Context
/// Currently disabled - implement when scaling to millions of products
/// For MVP: Use main CatalogDbContext with optimized queries
/// </summary>
public class CatalogReadDbContext : DbContext
{
    public CatalogReadDbContext(DbContextOptions<CatalogReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductReadModel> ProductsReadModel { get; set; } = null!;
}
