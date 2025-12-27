using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;

namespace B2Connect.Shared.User.Infrastructure.ModelBinding;

/// <summary>
/// Custom Input Formatter für IExtensibleEntity
/// Ermöglicht application/json Content-Type mit Custom Properties
/// </summary>
public class ExtensibleEntityJsonInputFormatter : TextInputFormatter
{
    private readonly IEntityExtensionService _extensionService;
    private readonly ILogger<ExtensibleEntityJsonInputFormatter> _logger;

    public ExtensibleEntityJsonInputFormatter(
        IEntityExtensionService extensionService,
        ILogger<ExtensibleEntityJsonInputFormatter> logger)
    {
        _extensionService = extensionService;
        _logger = logger;

        SupportedMediaTypes.Add("application/json");
        SupportedMediaTypes.Add("text/json");
        SupportedEncodings.Add(System.Text.Encoding.UTF8);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(context);

        var request = context.HttpContext.Request;
        var modelType = context.ModelType;

        // Check ob Type IExtensibleEntity implementiert
        if (!typeof(IExtensibleEntity).IsAssignableFrom(modelType))
        {
            return await InputFormatterResult.FailureAsync();
        }

        try
        {
            using var reader = new StreamReader(request.Body, encoding);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
            {
                return await InputFormatterResult.NoValueAsync();
            }

            // Parse JSON
            using var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;

            // Erstelle Model
            var model = DeserializeModel(root, modelType, context);

            return await InputFormatterResult.SuccessAsync(model);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON format for {ModelType}", modelType.Name);
            context.ModelState.AddModelError(context.FieldName, "Invalid JSON format");
            return await InputFormatterResult.FailureAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading request body for {ModelType}", modelType.Name);
            context.ModelState.AddModelError(context.FieldName, ex.Message);
            return await InputFormatterResult.FailureAsync();
        }
    }

    protected override bool CanReadType(Type type)
    {
        return typeof(IExtensibleEntity).IsAssignableFrom(type);
    }

    private object DeserializeModel(
        JsonElement root,
        Type modelType,
        InputFormatterContext context)
    {
        var model = Activator.CreateInstance(modelType)
            ?? throw new InvalidOperationException($"Cannot create instance of {modelType.Name}");

        var extModel = model as IExtensibleEntity;

        // Deserialize alle Properties
        var properties = modelType.GetProperties();

        foreach (var property in properties)
        {
            if (property.Name == nameof(IExtensibleEntity.CustomProperties))
                continue;

            var jsonPropName = property.Name[0].ToString().ToLower() + property.Name.Substring(1);

            if (root.TryGetProperty(jsonPropName, out var element))
            {
                try
                {
                    var value = JsonSerializer.Deserialize(
                        element.GetRawText(),
                        property.PropertyType);

                    if (value != null && property.CanWrite)
                    {
                        property.SetValue(model, value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize property {PropertyName}", jsonPropName);
                }
            }
        }

        // Deserialize Custom Properties
        if (extModel != null && root.TryGetProperty("customProperties", out var customPropsElement))
        {
            var customProps = JsonSerializer.Deserialize<Dictionary<string, object?>>(
                customPropsElement.GetRawText())
                ?? new Dictionary<string, object?>();

            var customPropsJson = JsonSerializer.Serialize(customProps);
            extModel.CustomProperties = customPropsJson;
        }

        return model;
    }
}
