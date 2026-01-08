using B2X.Shared.Kernel;

namespace B2X.PatternAnalysis.Core.Domain;

public class Incident : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public IncidentSeverity Severity { get; private set; }
    public string Impact { get; private set; } = string.Empty;
    public string? Resolution { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Guid? RepositoryId { get; private set; }
    public string? FilePath { get; private set; }
    public int? LineNumber { get; private set; }

    private readonly List<IncidentPattern> _patterns = new();
    public IReadOnlyCollection<IncidentPattern> Patterns => _patterns.AsReadOnly();

    private Incident() { } // EF

    public Incident(string title, string description, DateTime occurredAt, IncidentSeverity severity, string impact)
    {
        Title = title;
        Description = description;
        OccurredAt = occurredAt;
        Severity = severity;
        Impact = impact;
        Status = IncidentStatus.Open;
    }

    public void AddPattern(Guid patternId, string detectedCode, double confidence)
    {
        _patterns.Add(new IncidentPattern(Id, patternId, detectedCode, confidence));
    }

    public void Resolve(string resolution)
    {
        Resolution = resolution;
        Status = IncidentStatus.Resolved;
    }

    public void SetLocation(Guid repositoryId, string filePath, int lineNumber)
    {
        RepositoryId = repositoryId;
        FilePath = filePath;
        LineNumber = lineNumber;
    }
}

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum IncidentStatus
{
    Open,
    Investigating,
    Resolved,
    Closed
}

public class IncidentPattern
{
    public string IncidentId { get; private set; } = string.Empty;
    public Guid PatternId { get; private set; }
    public string DetectedCode { get; private set; } = string.Empty;
    public double Confidence { get; private set; }

    private IncidentPattern() { } // EF

    public IncidentPattern(string incidentId, Guid patternId, string detectedCode, double confidence)
    {
        IncidentId = incidentId;
        PatternId = patternId;
        DetectedCode = detectedCode;
        Confidence = confidence;
    }
}
