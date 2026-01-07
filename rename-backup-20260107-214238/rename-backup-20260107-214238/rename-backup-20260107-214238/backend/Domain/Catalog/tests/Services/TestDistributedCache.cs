using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Test implementation of IDistributedCache for VAT validation tests
/// </summary>
public class TestDistributedCache : IDistributedCache
{
    private readonly Dictionary<string, byte[]> _data = new(StringComparer.Ordinal);

    public bool ContainsKey(string key) => _data.ContainsKey(key);

    public byte[]? Get(string key)
    {
        _data.TryGetValue(key, out var value);
        return value;
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        return Get(key);
    }

    public void Refresh(string key) { }

    public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;

    public void Remove(string key)
    {
        _data.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions? options)
    {
        _data[key] = value;
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions? options, CancellationToken token = default)
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task SetStringAsync(string key, string? value, DistributedCacheEntryOptions? options = null, CancellationToken token = default)
    {
        if (value == null)
        {
            return Task.CompletedTask;
        }

        Set(key, Encoding.UTF8.GetBytes(value), null);
        return Task.CompletedTask;
    }

    public Task<string?> GetStringAsync(string key, CancellationToken token = default)
    {
        var bytes = Get(key);
        return Task.FromResult(bytes == null ? null : Encoding.UTF8.GetString(bytes));
    }
}
