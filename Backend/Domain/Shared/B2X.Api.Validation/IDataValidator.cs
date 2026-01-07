using System.Threading;
using System.Threading.Tasks;

namespace B2X.Api.Validation;

public interface IDataValidator<T>
{
    Task<ValidationResult> ValidateAsync(T data, CancellationToken ct = default);
    Task<IEnumerable<ValidationResult>> ValidateCollectionAsync(
        IEnumerable<T> items,
        CancellationToken ct = default);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string FieldPath { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}

public enum ValidationSeverity
{
    Warning,    // Continue but log
    Error,      // Quarantine record
    Critical    // Reject entire batch
}
