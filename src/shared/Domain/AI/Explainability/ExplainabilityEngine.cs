// B2X.AI.Explainability
// AI Model Explainability Framework Implementation
// DocID: SPR-027-AI-EXPLAIN

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2X.AI.Explainability
{
    /// <summary>
    /// AI Model Explainability Engine using SHAP/LIME approaches
    /// </summary>
    public class ExplainabilityEngine
    {
        private readonly IModelInterpreter _interpreter;
        private readonly IAuditLogger _auditLogger;

        public ExplainabilityEngine(IModelInterpreter interpreter, IAuditLogger auditLogger)
        {
            _interpreter = interpreter;
            _auditLogger = auditLogger;
        }

        /// <summary>
        /// Generate explanation for model prediction
        /// </summary>
        public async Task<ModelExplanation> ExplainPredictionAsync(
            string modelId,
            object inputData,
            object prediction,
            string userId)
        {
            var explanation = await _interpreter.GenerateExplanationAsync(modelId, inputData, prediction);

            await _auditLogger.LogExplanationAsync(new AuditEntry
            {
                ModelId = modelId,
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                InputData = inputData,
                Prediction = prediction,
                Explanation = explanation
            });

            return explanation;
        }
    }

    public interface IModelInterpreter
    {
        Task<ModelExplanation> GenerateExplanationAsync(string modelId, object inputData, object prediction);
    }

    public interface IAuditLogger
    {
        Task LogExplanationAsync(AuditEntry entry);
    }

    public class ModelExplanation
    {
        public string? ModelId { get; set; }
        public Dictionary<string, double>? FeatureImportances { get; set; }
        public string? ExplanationText { get; set; }
        public double Confidence { get; set; }
    }

    public class AuditEntry
    {
        public string? ModelId { get; set; }
        public string? UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public object? InputData { get; set; }
        public object? Prediction { get; set; }
        public ModelExplanation? Explanation { get; set; }
    }
}
