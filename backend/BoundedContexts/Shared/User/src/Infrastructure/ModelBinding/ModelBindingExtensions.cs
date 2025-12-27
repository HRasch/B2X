using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Shared.User.Infrastructure.ModelBinding;

/// <summary>
/// Extension Methods für ASP.NET Core MVC Options
/// Registriert Custom Model Binder und Input Formatter
/// </summary>
public static class ModelBindingExtensions
{
    /// <summary>
    /// Aktiviere Model Binding für IExtensibleEntity
    /// </summary>
    public static MvcOptions AddExtensibleEntityModelBinding(this MvcOptions options)
    {
        // Model Binder Provider (für Parameter Binding)
        options.ModelBinderProviders.Insert(0, new ExtensibleEntityModelBinderProvider());

        // Input Formatter (für JSON Content)
        options.InputFormatters.Insert(0,
            new ExtensibleEntityJsonInputFormatter(
                new ServiceCollection()
                    .AddScoped<IEntityExtensionService, EntityExtensionService>()
                    .BuildServiceProvider()
                    .GetRequiredService<IEntityExtensionService>(),
                new ServiceCollection()
                    .AddLogging()
                    .BuildServiceProvider()
                    .GetRequiredService<ILogger<ExtensibleEntityJsonInputFormatter>>()
            ));

        return options;
    }
}

/// <summary>
/// Service Collection Extension
/// </summary>
public static class ModelBindingServiceCollectionExtensions
{
    /// <summary>
    /// Registriere Model Binding Services
    /// </summary>
    public static IServiceCollection AddExtensibleEntityModelBinding(
        this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.AddExtensibleEntityModelBinding();
        });

        return services;
    }
}
