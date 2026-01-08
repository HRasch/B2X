using B2X.Shared.Kernel;

namespace B2X.MLGovernance.Core.Domain;

public class MLModel : AggregateRoot
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Description { get; set; }
    public MLModelType Type { get; set; }
    public MLModelStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastTrainedAt { get; private set; }
    public DateTime? LastDeployedAt { get; private set; }
    public required string TrainingDataHash { get; set; }
    public required string ModelHash { get; set; }
    public ComplianceStatus ComplianceStatus { get; private set; }
    public BiasDetectionResult? BiasDetectionResult { get; private set; }
    public EthicalAIAssessment? EthicalAssessment { get; private set; }
    public ICollection<ModelAuditTrail> AuditTrails { get; private set; } = new List<ModelAuditTrail>();
    public ICollection<ModelPerformanceMetric> PerformanceMetrics { get; private set; } = new List<ModelPerformanceMetric>();

    private MLModel() { } // EF

    public MLModel(string name, string version, string description, MLModelType type, string trainingDataHash, string modelHash)
    {
        Name = name;
        Version = version;
        Description = description;
        Type = type;
        Status = MLModelStatus.Training;
        CreatedAt = DateTime.UtcNow;
        TrainingDataHash = trainingDataHash;
        ModelHash = modelHash;
        ComplianceStatus = ComplianceStatus.Pending;
    }

    public void MarkAsTrained()
    {
        Status = MLModelStatus.Trained;
        LastTrainedAt = DateTime.UtcNow;
    }

    public void Deploy()
    {
        Status = MLModelStatus.Deployed;
        LastDeployedAt = DateTime.UtcNow;
    }

    public void Retire()
    {
        Status = MLModelStatus.Retired;
    }

    public void UpdateComplianceStatus(ComplianceStatus status)
    {
        ComplianceStatus = status;
    }

    public void PerformBiasDetection(BiasDetectionResult result)
    {
        BiasDetectionResult = result;
        AddAuditEntry("BiasDetection", "System", $"Bias detection completed. Score: {result.BiasScore}");
    }

    public void ConductEthicalAssessment(EthicalAIAssessment assessment)
    {
        EthicalAssessment = assessment;
        ComplianceStatus = assessment.IsEthical ? ComplianceStatus.Compliant : ComplianceStatus.NonCompliant;
        AddAuditEntry("EthicalAssessment", "System", $"Ethical assessment completed. Ethical: {assessment.IsEthical}");
    }

    public void AddAuditEntry(string action, string userId, string details)
    {
        var auditEntry = new ModelAuditTrail(this.Id, action, userId, details);
        AuditTrails.Add(auditEntry);
    }

    public void RecordPerformanceMetric(string metricName, double value, DateTime timestamp)
    {
        var metric = new ModelPerformanceMetric(this.Id, metricName, value, timestamp);
        PerformanceMetrics.Add(metric);
    }
}

public enum MLModelType
{
    Classification,
    Regression,
    Clustering,
    Recommendation,
    NaturalLanguageProcessing,
    ComputerVision
}

public enum MLModelStatus
{
    Training,
    Trained,
    Deployed,
    Retired
}

public enum ComplianceStatus
{
    Pending,
    Compliant,
    NonCompliant,
    UnderReview
}