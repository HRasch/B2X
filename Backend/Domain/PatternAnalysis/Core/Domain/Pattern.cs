using B2Connect.Shared.Kernel;

namespace B2Connect.PatternAnalysis.Core.Domain;

public class Pattern : AggregateRoot
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public PatternType Type { get; private set; }
    public SeverityLevel Severity { get; private set; }
    public PatternCategory Category { get; private set; }
    public required string DetectionRule { get; set; } // Rule or AI prompt for detection
    public required string Suggestion { get; set; } // Automated suggestion
    public bool IsActive { get; private set; }

    private Pattern() { } // EF

    public Pattern(string name, string description, PatternType type, SeverityLevel severity, PatternCategory category, string detectionRule, string suggestion)
    {
        Name = name;
        Description = description;
        Type = type;
        Severity = severity;
        Category = category;
        DetectionRule = detectionRule;
        Suggestion = suggestion;
        IsActive = true;
    }

    public void Update(string description, string detectionRule, string suggestion)
    {
        Description = description;
        DetectionRule = detectionRule;
        Suggestion = suggestion;
    }

    public void Deactivate() => IsActive = false;
}

public enum PatternType
{
    AntiPattern,
    GoodPattern
}

public enum SeverityLevel
{
    Low,
    Medium,
    High,
    Critical
}

public enum PatternCategory
{
    CodeSmell,
    Performance,
    Security,
    Maintainability,
    Testability
}
