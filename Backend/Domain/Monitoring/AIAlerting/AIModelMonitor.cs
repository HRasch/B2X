// B2Connect.Monitoring.AIAlerting
// Advanced Alerting for AI Model Performance Degradation
// DocID: SPR-027-AI-ALERTING

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2Connect.Monitoring.AIAlerting
{
    /// <summary>
    /// AI Model Performance Monitoring and Alerting System
    /// Detects performance degradation within 5 minutes with automated escalation
    /// </summary>
    public class AIModelMonitor
    {
        private readonly IPerformanceMetricsCollector _metricsCollector;
        private readonly IAnomalyDetector _anomalyDetector;
        private readonly IAlertEscalator _alertEscalator;
        private readonly IModelHealthRepository _healthRepo;

        public AIModelMonitor(
            IPerformanceMetricsCollector metricsCollector,
            IAnomalyDetector anomalyDetector,
            IAlertEscalator alertEscalator,
            IModelHealthRepository healthRepo)
        {
            _metricsCollector = metricsCollector;
            _anomalyDetector = anomalyDetector;
            _alertEscalator = alertEscalator;
            _healthRepo = healthRepo;
        }

        /// <summary>
        /// Monitor model performance and detect degradation
        /// </summary>
        public async Task<ModelHealthStatus> MonitorModelAsync(string modelId)
        {
            var metrics = await _metricsCollector.CollectMetricsAsync(modelId);
            var health = await _healthRepo.GetHealthStatusAsync(modelId);

            // Detect anomalies
            var anomalies = await _anomalyDetector.DetectAnomaliesAsync(metrics, health.Baseline);

            if (anomalies.Any())
            {
                var alert = new PerformanceAlert
                {
                    ModelId = modelId,
                    Anomalies = anomalies,
                    Severity = DetermineSeverity(anomalies),
                    Timestamp = DateTime.UtcNow
                };

                await _alertEscalator.EscalateAlertAsync(alert);
                health.LastAlert = alert.Timestamp;
            }

            // Update health status
            health.LastCheck = DateTime.UtcNow;
            health.IsHealthy = !anomalies.Any(a => a.Severity == "Critical");
            health.PerformanceScore = CalculatePerformanceScore(metrics);

            await _healthRepo.UpdateHealthStatusAsync(health);

            return health;
        }

        /// <summary>
        /// Get model performance dashboard data
        /// </summary>
        public async Task<ModelPerformanceDashboard> GetPerformanceDashboardAsync(string modelId)
        {
            var health = await _healthRepo.GetHealthStatusAsync(modelId);
            var recentMetrics = await _metricsCollector.GetRecentMetricsAsync(modelId, TimeSpan.FromDays(7));

            return new ModelPerformanceDashboard
            {
                ModelId = modelId,
                HealthStatus = health,
                Metrics = recentMetrics,
                Alerts = await _alertEscalator.GetRecentAlertsAsync(modelId, TimeSpan.FromDays(1))
            };
        }

        private string DetermineSeverity(IEnumerable<Anomaly> anomalies)
        {
            if (anomalies.Any(a => a.Type == "AccuracyDrop" && a.Deviation > 0.1))
                return "Critical";
            if (anomalies.Any(a => a.Deviation > 0.05))
                return "High";
            return "Medium";
        }

        private double CalculatePerformanceScore(IEnumerable<PerformanceMetric> metrics)
        {
            // Simplified scoring based on key metrics
            var accuracy = metrics.FirstOrDefault(m => m.Name == "Accuracy")?.Value ?? 0;
            var latency = metrics.FirstOrDefault(m => m.Name == "Latency")?.Value ?? 0;

            // Higher score is better (normalized 0-100)
            return Math.Max(0, Math.Min(100, (accuracy * 100) - (latency / 10)));
        }
    }

    public interface IPerformanceMetricsCollector
    {
        Task<IEnumerable<PerformanceMetric>> CollectMetricsAsync(string modelId);
        Task<IEnumerable<PerformanceMetric>> GetRecentMetricsAsync(string modelId, TimeSpan period);
    }

    public interface IAnomalyDetector
    {
        Task<IEnumerable<Anomaly>> DetectAnomaliesAsync(
            IEnumerable<PerformanceMetric> current,
            ModelBaseline baseline);
    }

    public interface IAlertEscalator
    {
        Task EscalateAlertAsync(PerformanceAlert alert);
        Task<IEnumerable<PerformanceAlert>> GetRecentAlertsAsync(string modelId, TimeSpan period);
    }

    public interface IModelHealthRepository
    {
        Task<ModelHealthStatus> GetHealthStatusAsync(string modelId);
        Task UpdateHealthStatusAsync(ModelHealthStatus status);
    }

    public class PerformanceMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Anomaly
    {
        public string Type { get; set; } // AccuracyDrop, LatencySpike, etc.
        public double Deviation { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
    }

    public class PerformanceAlert
    {
        public string ModelId { get; set; }
        public IEnumerable<Anomaly> Anomalies { get; set; }
        public string Severity { get; set; }
        public DateTime Timestamp { get; set; }
        public string EscalationStatus { get; set; }
    }

    public class ModelHealthStatus
    {
        public string ModelId { get; set; }
        public bool IsHealthy { get; set; }
        public double PerformanceScore { get; set; }
        public ModelBaseline Baseline { get; set; }
        public DateTime LastCheck { get; set; }
        public DateTime? LastAlert { get; set; }
    }

    public class ModelBaseline
    {
        public Dictionary<string, double> ExpectedMetrics { get; set; }
        public Dictionary<string, double> Thresholds { get; set; }
    }

    public class ModelPerformanceDashboard
    {
        public string ModelId { get; set; }
        public ModelHealthStatus HealthStatus { get; set; }
        public IEnumerable<PerformanceMetric> Metrics { get; set; }
        public IEnumerable<PerformanceAlert> Alerts { get; set; }
    }
}