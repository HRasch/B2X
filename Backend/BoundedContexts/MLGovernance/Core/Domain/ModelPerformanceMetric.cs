using B2X.Shared.Kernel;

namespace B2X.MLGovernance.Core.Domain;

public class ModelPerformanceMetric : Entity
{
    public Guid ModelId { get; private set; }
    public required string MetricName { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; private set; }
    public string? AdditionalData { get; set; }

    private ModelPerformanceMetric() { } // EF

    public ModelPerformanceMetric(Guid modelId, string metricName, double value, DateTime timestamp)
    {
        ModelId = modelId;
        MetricName = metricName;
        Value = value;
        Timestamp = timestamp;
    }
}