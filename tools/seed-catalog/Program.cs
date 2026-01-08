using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using B2X.Admin.Infrastructure.Data;
using B2X.Shared.Tenancy.Infrastructure.Context;

namespace B2X.Tools.SeedCatalog
{
    class Program
    {
        static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var conn = builder["ConnectionString"] ?? builder.GetValue<string>("Catalog:ConnectionString");
            if (string.IsNullOrEmpty(conn))
            {
                Console.WriteLine("Connection string required. Pass --ConnectionString='<conn>' or set Catalog__ConnectionString env var.");
                return 1;
            }

            var count = builder.GetValue<int?>("Count") ?? 100;
            Console.WriteLine($"Seeding demo catalog with {count} products to: {conn}");

            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseNpgsql(conn)
                .Options;

            var tenantContext = new DemoTenantContext();

            using var context = new CatalogDbContext(options, tenantContext);
            context.Database.EnsureCreated();

            var (categories, brands, products) = CatalogDemoDataGenerator.GenerateDemoCatalog(count);

            context.Categories.AddRange(categories);
            context.Brands.AddRange(brands);
            context.Products.AddRange(products);
            context.SaveChanges();

            Console.WriteLine("Seeding complete.");
            return 0;
        }

        private class DemoTenantContext : ITenantContext
        {
            public Guid TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }
}
