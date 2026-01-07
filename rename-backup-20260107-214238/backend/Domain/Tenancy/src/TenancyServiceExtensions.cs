using B2X.Tenancy.Infrastructure.Data;
using B2X.Tenancy.Repositories;
using B2X.Tenancy.Services;
using DnsClient;
using Microsoft.EntityFrameworkCore;

namespace B2X.Tenancy;

/// <summary>
/// Extension methods for configuring Tenancy services in DI container.
/// </summary>
public static class TenancyServiceExtensions
{
    /// <summary>
    /// Adds Tenancy services to the service collection.
    /// </summary>
    public static IServiceCollection AddTenancyServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database context
        var connectionString = configuration.GetConnectionString("TenancyDb")
            ?? configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<TenancyDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "tenancy");
                });
                options.UseSnakeCaseNamingConvention();
            });
        }

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantDomainRepository, TenantDomainRepository>();

        // Services
        services.AddScoped<IDomainLookupService, DomainLookupService>();
        services.AddScoped<IDnsVerificationService, DnsVerificationService>();

        // DNS client for domain verification
        services.AddSingleton<ILookupClient>(sp =>
        {
            var options = new LookupClientOptions
            {
                UseCache = true,
                Timeout = TimeSpan.FromSeconds(10),
                Retries = 2
            };
            return new LookupClient(options);
        });

        // Memory cache (if not already registered)
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds authorization policies for tenant management.
    /// </summary>
    public static IServiceCollection AddTenancyAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Platform admin policy - can manage all tenants
            options.AddPolicy("PlatformAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("role", "platform_admin", "super_admin");
            });

            // Tenant admin policy - can manage own tenant
            options.AddPolicy("TenantAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("role", "tenant_admin", "platform_admin", "super_admin");
            });
        });

        return services;
    }
}
