namespace B2Connect.ERP.Abstractions;

// Dummy implementations for compilation when actual enventa assemblies are not available
// The ERP Connector (.NET 4.8) contains the actual DAL implementation

/// <summary>
/// Helper for empty async enumerables.
/// </summary>
public static class AsyncEnumerable
{
    public static async IAsyncEnumerable<T> Empty<T>()
    {
        await Task.CompletedTask;
        yield break;
    }
}

/// <summary>
/// Dummy NVContext for compilation when enventa assemblies are not available.
/// The actual implementation lives in the .NET 4.8 ERP Connector.
/// </summary>
public class DummyNVContext : NVShop.Data.NV.Model.INVContext
{
    public IQueryable<NVShop.Data.NV.Model.NVArticle> Articles =>
        new List<NVShop.Data.NV.Model.NVArticle>().AsQueryable();

    public IQueryable<NVShop.Data.NV.Model.NVCustomer> Customers =>
        new List<NVShop.Data.NV.Model.NVCustomer>().AsQueryable();

    public IQueryable<NVShop.Data.NV.Model.NVOrder> Orders =>
        new List<NVShop.Data.NV.Model.NVOrder>().AsQueryable();

    public IQueryable<NVShop.Data.NV.Model.NVAddress> Addresses =>
        new List<NVShop.Data.NV.Model.NVAddress>().AsQueryable();

    public void Dispose()
    {
        // No-op for dummy implementation
    }
}

/// <summary>
/// Connection lease for pooled ERP connections.
/// </summary>
public class ConnectionLease : IDisposable
{
    public NVShop.Data.NV.Model.INVContext Context { get; }

    public ConnectionLease(NVShop.Data.NV.Model.INVContext context)
    {
        Context = context;
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

/// <summary>
/// Connection pool for ERP connections.
/// Provides pooled access to NVContext instances per tenant.
/// </summary>
public class ErpConnectionPool : IDisposable
{
    public ErpConnectionPool()
    {
    }

    public Task<NVShop.Data.NV.Model.INVContext> GetContextAsync(string tenantId) =>
        Task.FromResult<NVShop.Data.NV.Model.INVContext>(new DummyNVContext());

    public Task<ConnectionLease> RentAsync(string tenantId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ConnectionLease(new DummyNVContext()));

    public Task<T> ExecuteAsync<T>(string tenantId, Func<NVShop.Data.NV.Model.INVContext, Task<T>> operation) =>
        Task.FromResult(default(T)!);

    public void Dispose()
    {
        // Cleanup pooled connections
    }
}
