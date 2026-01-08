using ModelContextProtocol;
using B2X.Api.Validation;
using B2X.Api.Connectors;

namespace B2X.Api.Mcp;

public class ErpValidationTool : ITool
{
    private readonly ErpSpecificValidator _validator;
    private readonly IEnumerable<IErpConnector> _connectors;

    public ErpValidationTool(
        ErpSpecificValidator validator,
        IEnumerable<IErpConnector> connectors)
    {
        _validator = validator;
        _connectors = connectors;
    }

    public string Name => "erp_validation";
    public string Description => "Validate ERP data using connector-specific rules";

    public async Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken ct)
    {
        var erpType = call.Arguments["erp_type"]?.ToString();
        var data = JsonSerializer.Deserialize<ErpData>(call.Arguments["data"]?.ToString());
        var useConnector = call.Arguments["use_connector"]?.ToString() == "true";

        IEnumerable<ValidationResult> results;

        if (useConnector)
        {
            var connector = _connectors.FirstOrDefault(c => c.ErpType == erpType);
            if (connector == null)
            {
                return new ToolResult
                {
                    Content = new[] { new TextContent { Text = $"Connector for {erpType} not found" } }
                };
            }
            results = await connector.ValidateDataAsync(data, ct);
        }
        else
        {
            results = new[] { await _validator.ValidateAsync(data, ct) };
        }

        return new ToolResult
        {
            Content = new[] { new TextContent { Text = JsonSerializer.Serialize(results) } }
        };
    }
}