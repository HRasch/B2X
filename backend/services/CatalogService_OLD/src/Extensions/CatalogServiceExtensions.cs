using FluentValidation;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Validators;
using B2Connect.Shared.Extensions;

namespace B2Connect.CatalogService.Extensions;

/// <summary>
/// Extension methods for setting up Catalog Service dependencies
/// Configures validation, AOP filters, and services
/// </summary>
public static class CatalogServiceExtensions
{
    /// <summary>
    /// Adds all Catalog Service infrastructure
    /// - Validates FluentValidation setup
    /// - Configures AOP filters
    /// - Registers service dependencies
    /// </summary>
    public static IServiceCollection AddCatalogServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Validators
        AddValidators(services);

        // Add AOP Filters and Validation
        services.AddAopAndValidation(typeof(CatalogServiceExtensions));

        return services;
    }

    /// <summary>
    /// Registers all FluentValidation validators for the Catalog Service
    /// </summary>
    private static void AddValidators(IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
        services.AddScoped<IValidator<CreateCategoryRequest>, CreateCategoryRequestValidator>();
        services.AddScoped<IValidator<CreateBrandRequest>, CreateBrandRequestValidator>();
    }
}
