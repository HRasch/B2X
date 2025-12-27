using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Text.Json;

namespace B2Connect.Shared.User.Infrastructure.ModelBinding;

/// <summary>
/// Custom Model Binder für IExtensibleEntity
/// Ermöglicht automatische Bindung von Custom Properties aus Request
/// 
/// Beispiel Request Body:
/// {
///   "email": "john@example.com",
///   "firstName": "John",
///   "lastName": "Doe",
///   "customProperties": {
///     "erp_customer_id": "CUST-123456",
///     "warehouse_code": "WH-001"
///   }
/// }
/// </summary>
public class ExtensibleEntityModelBinder : IModelBinder
{
    private readonly IEntityExtensionService _extensionService;
    private readonly ILogger<ExtensibleEntityModelBinder> _logger;

    public ExtensibleEntityModelBinder(
        IEntityExtensionService extensionService,
        ILogger<ExtensibleEntityModelBinder> logger)
    {
        _extensionService = extensionService;
        _logger = logger;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelType = bindingContext.ModelType;

        // Check ob Type IExtensibleEntity implementiert
        if (!typeof(IExtensibleEntity).IsAssignableFrom(modelType))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        try
        {
            // Lese Request Body als Stream
            var request = bindingContext.HttpContext.Request;
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            // Parse JSON
            using var jsonDocument = JsonDocument.Parse(body);
            var root = jsonDocument.RootElement;

            // Erstelle Model Instance
            var model = Activator.CreateInstance(modelType)
                ?? throw new InvalidOperationException($"Cannot create instance of {modelType.Name}");

            // Bind standard properties
            BindStandardProperties(model, root, modelType);

            // Bind custom properties
            if (root.TryGetProperty("customProperties", out var customPropsElement))
            {
                await BindCustomPropertiesAsync(model as IExtensibleEntity, customPropsElement, bindingContext);
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON for {ModelType}", modelType.Name);
            bindingContext.ModelState.AddModelError(
                bindingContext.ModelName,
                "Invalid JSON format");
            bindingContext.Result = ModelBindingResult.Failed();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Model binding error for {ModelType}", modelType.Name);
            bindingContext.ModelState.AddModelError(
                bindingContext.ModelName,
                "An error occurred while binding the model");
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }

    private static void BindStandardProperties(
        object model,
        JsonElement root,
        Type modelType)
    {
        // Binde alle Properties die NICHT custom sind
        var properties = modelType.GetProperties(
            BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

        var customPropsProperty = modelType.GetProperty(nameof(IExtensibleEntity.CustomProperties));

        foreach (var property in properties)
        {
            // Skip custom properties - diese werden separat behandelt
            if (property.Name.Equals(nameof(IExtensibleEntity.CustomProperties),
                StringComparison.OrdinalIgnoreCase))
                continue;

            var jsonPropertyName = ToCamelCase(property.Name);

            if (!root.TryGetProperty(jsonPropertyName, out var element))
                continue;

            try
            {
                var value = JsonSerializer.Deserialize(
                    element.GetRawText(),
                    property.PropertyType,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (value != null && property.CanWrite)
                {
                    property.SetValue(model, value);
                }
            }
            catch
            {
                // Skip invalid properties
            }
        }
    }

    private async Task BindCustomPropertiesAsync(
        IExtensibleEntity? model,
        JsonElement customPropsElement,
        ModelBindingContext bindingContext)
    {
        if (model == null)
            return;

        var tenantIdValue = bindingContext.HttpContext.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (string.IsNullOrEmpty(tenantIdValue) || !Guid.TryParse(tenantIdValue, out var tenantId))
        {
            _logger.LogWarning("Missing or invalid X-Tenant-ID header");
            return;
        }

        var customProps = JsonSerializer.Deserialize<Dictionary<string, object?>>(
            customPropsElement.GetRawText())
            ?? new Dictionary<string, object?>();

        // Validiere jeden Custom Property
        foreach (var kvp in customProps)
        {
            var isValid = await _extensionService.ValidateCustomPropertyAsync(
                tenantId,
                model.GetType().Name,
                kvp.Key,
                kvp.Value);

            if (!isValid)
            {
                bindingContext.ModelState.AddModelError(
                    $"customProperties.{kvp.Key}",
                    $"Invalid value for field '{kvp.Key}'");
                continue;
            }

            // Setze Custom Property
            _extensionService.SetCustomProperty(model, kvp.Key, kvp.Value);
        }
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}

/// <summary>
/// Provider für ExtensibleEntityModelBinder
/// </summary>
public class ExtensibleEntityModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (typeof(IExtensibleEntity).IsAssignableFrom(context.Metadata.ModelType))
        {
            var extensionService = context.Services.GetRequiredService<IEntityExtensionService>();
            var logger = context.Services.GetRequiredService<ILogger<ExtensibleEntityModelBinder>>();

            return new ExtensibleEntityModelBinder(extensionService, logger);
        }

        return null;
    }
}
