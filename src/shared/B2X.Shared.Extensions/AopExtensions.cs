using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace B2X.Shared.Extensions;

/// <summary>
/// Extension methods for configuring AOP and Validation
/// Centralizes setup of filters, validators, and cross-cutting concerns
/// </summary>
public static class AopExtensions
{
    /// <summary>
    /// Adds AOP filters to the MVC builder
    /// Registers automatic validation, exception handling, and logging
    /// </summary>
    public static IServiceCollection AddAopFilters(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options =>
        {
            // Add AOP filters globally
            options.Filters.Add<ExceptionHandlingAttribute>();
            options.Filters.Add<ValidateModelAttribute>();
            options.Filters.Add<RequestLoggingAttribute>();
        });

        return services;
    }

    /// <summary>
    /// Adds FluentValidation to the service collection
    /// Auto-registers all validators from specified assemblies
    /// </summary>
    public static IServiceCollection AddFluentValidationForCatalog(
        this IServiceCollection services,
        params Type[] assemblyMarkers)
    {
        foreach (var marker in assemblyMarkers)
        {
            services.AddValidatorsFromAssembly(marker.Assembly);
        }

        return services;
    }

    /// <summary>
    /// Adds Event Validation infrastructure
    /// Registers validators and interceptors for domain events before publishing
    /// </summary>
    public static IServiceCollection AddEventValidation(
        this IServiceCollection services,
        params Type[] assemblyMarkers)
    {
        // Register validators from specified assemblies
        foreach (var marker in assemblyMarkers)
        {
            services.AddValidatorsFromAssembly(marker.Assembly);
        }

        // Register event validation service
        services.AddScoped<IEventValidationService, EventValidationService>();

        // Register event publisher with validation
        services.AddScoped<IEventPublisher, ValidatedEventPublisher>();

        // Register event validator factory
        services.AddScoped<EventValidatorFactory>();

        return services;
    }

    /// <summary>
    /// Adds Event Validation Middleware to the pipeline
    /// Validates events before they are processed
    /// </summary>
    public static void UseEventValidation(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventValidationMiddleware>();
    }

    /// <summary>
    /// Adds all AOP and Validation infrastructure in one call
    /// Recommended approach for initial setup
    /// </summary>
    public static IServiceCollection AddAopAndValidation(
        this IServiceCollection services,
        params Type[] assemblyMarkers)
    {
        services.AddAopFilters();
        services.AddFluentValidationForCatalog(assemblyMarkers);
        services.AddEventValidation(assemblyMarkers);

        return services;
    }
}
