namespace B2X.Compliance.Jurisdictions;

public interface IJurisdictionRepository
{
    Task<IEnumerable<string>> GetSupportedJurisdictionsAsync();
    Task<JurisdictionData?> GetJurisdictionDataAsync(string code);
    Task<bool> IsJurisdictionSupportedAsync(string code);
}

public interface IRegulatoryUpdateService
{
    Task<IEnumerable<string>> GetUpdatesAsync(string jurisdictionCode);
    Task<bool> HasPendingUpdatesAsync(string jurisdictionCode);
}

public interface IJurisdictionHandler
{
    Task<ComplianceResult> AssessComplianceAsync(ComplianceRequest request);
}

public class JurisdictionComplianceEngine
{
    private readonly IJurisdictionRepository _repository;
    private readonly IRegulatoryUpdateService _regulatoryService;
    private readonly Dictionary<string, IJurisdictionHandler> _handlers = new();

    public JurisdictionComplianceEngine(
        IJurisdictionRepository repository,
        IRegulatoryUpdateService regulatoryService)
    {
        _repository = repository;
        _regulatoryService = regulatoryService;
    }

    public async Task<IEnumerable<string>> GetSupportedJurisdictions()
    {
        return await _repository.GetSupportedJurisdictionsAsync();
    }

    public void RegisterHandler(string jurisdictionCode, IJurisdictionHandler handler)
    {
        _handlers[jurisdictionCode] = handler;
    }

    public async Task<ComplianceResult> AssessComplianceAsync(string jurisdictionCode, ComplianceRequest request)
    {
        if (!_handlers.TryGetValue(jurisdictionCode, out var handler))
        {
            throw new NotSupportedException($"Jurisdiction '{jurisdictionCode}' is not supported");
        }

        var result = await handler.AssessComplianceAsync(request);
        var updates = await _regulatoryService.GetUpdatesAsync(jurisdictionCode);

        if (updates.Any())
        {
            result.RegulatoryNotes = updates;
        }

        return result;
    }
}

public class ComplianceRequest
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Dictionary<string, object>? AdditionalData { get; set; }
}

public class ComplianceResult
{
    public bool IsCompliant { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public decimal RiskScore { get; set; }
    public IEnumerable<string>? Violations { get; set; }
    public IEnumerable<string>? RegulatoryNotes { get; set; }
}

public class JurisdictionData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object>? Regulations { get; set; }
}
