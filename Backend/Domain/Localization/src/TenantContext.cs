using System.Runtime.CompilerServices;

namespace B2Connect.LocalizationService;

/// <summary>
/// Ambient context for tenant information
/// Provides implicit access to current tenant without explicit parameter passing
/// </summary>
public static class TenantContext
{
    private static readonly AsyncLocal<Guid?> _currentTenantId = new();

    /// <summary>
    /// Gets or sets the current tenant ID for the execution context
    /// </summary>
    public static Guid? CurrentTenantId
    {
        get => _currentTenantId.Value;
        set => _currentTenantId.Value = value;
    }

    /// <summary>
    /// Sets the tenant context for the current execution scope
    /// </summary>
    /// <param name="tenantId">The tenant ID to set</param>
    /// <returns>An IDisposable that restores the previous context when disposed</returns>
    public static IDisposable SetTenantId(Guid tenantId)
    {
        var previousTenantId = CurrentTenantId;
        CurrentTenantId = tenantId;

        return new TenantScope(previousTenantId);
    }

    /// <summary>
    /// Clears the current tenant context
    /// </summary>
    public static void Clear()
    {
        CurrentTenantId = null;
    }

    /// <summary>
    /// Ensures a tenant ID is set, throws if not
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no tenant context is available</exception>
    public static Guid RequireTenantId([CallerMemberName] string? caller = null)
    {
        return CurrentTenantId ?? throw new InvalidOperationException(
            $"Tenant context is required but not set. Called from: {caller}");
    }

    private class TenantScope : IDisposable
    {
        private readonly Guid? _previousTenantId;

        public TenantScope(Guid? previousTenantId)
        {
            _previousTenantId = previousTenantId;
        }

        public void Dispose()
        {
            CurrentTenantId = _previousTenantId;
        }
    }
}