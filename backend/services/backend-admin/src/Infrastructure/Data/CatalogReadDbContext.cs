using Microsoft.EntityFrameworkCore;
using B2Connect.Admin.Infrastructure.Data.ReadModel;

namespace B2Connect.Admin.Infrastructure.Data;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

