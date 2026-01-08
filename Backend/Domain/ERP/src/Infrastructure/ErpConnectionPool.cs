using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using B2X.ERP.Abstractions;
using Microsoft.Extensions.Logging;
using NVShop.Data.NV;
using NVShop.Data.NV.Model;

namespace B2X.Domain.ERP.Infrastructure
{
    /// <summary>
    /// ERP Connection Pool - based on eGate FSGlobalPool pattern
    /// Manages reusable ERP connections with proper cleanup and thread safety
    /// </summary>
    public interface IErpConnectionPool
    {
        ValueTask<IErpConnection> RentAsync(string tenantId, CancellationToken ct = default);
        ValueTask ReturnAsync(IErpConnection connection);
        Task WarmupAsync(string tenantId, int count = 5, CancellationToken ct = default);
        Task HealthCheckAsync(string tenantId, CancellationToken ct = default);
        PoolStatistics GetStatistics(string tenantId);
    }

    /// <summary>
    /// ERP Connection interface - abstracts the underlying ERP connection
    /// </summary>
    public interface IErpConnection : IAsyncDisposable
    {
        string TenantId { get; }
        INVContext Context { get; }
        bool IsHealthy { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset LastUsedAt { get; }

        Task<T> ExecuteAsync<T>(Func<INVContext, Task<T>> operation, CancellationToken ct = default);
        void CloseActiveTransaction();
    }

    /// <summary>
    /// Connection Pool Implementation - based on eGate FSGlobalPool
    /// Manages per-tenant connection pools for thread safety
    /// </summary>
    public class ErpConnectionPool : IErpConnectionPool, IAsyncDisposable
    {
        private readonly IErpConnectionFactory _connectionFactory;
        private readonly ErpConnectionPoolOptions _options;
        private readonly ILogger<ErpConnectionPool> _logger;
        private readonly ConcurrentDictionary<string, TenantConnectionPool> _tenantPools;
        private readonly Timer _healthCheckTimer;
        private bool _disposed;

        public ErpConnectionPool(
            IErpConnectionFactory connectionFactory,
            ErpConnectionPoolOptions? options = null,
            ILogger<ErpConnectionPool>? logger = null)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? new ErpConnectionPoolOptions();
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ErpConnectionPool>.Instance;
            _tenantPools = new ConcurrentDictionary<string, TenantConnectionPool>();

            // Health check timer (like eGate's periodic validation)
            _healthCheckTimer = new Timer(
                _ => Task.Run(() => PerformGlobalHealthCheckAsync()),
                null,
                _options.HealthCheckInterval,
                _options.HealthCheckInterval);
        }

        public async ValueTask<IErpConnection> RentAsync(string tenantId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentException("Tenant ID cannot be null or empty", nameof(tenantId));

            var pool = _tenantPools.GetOrAdd(tenantId, _ => new TenantConnectionPool(tenantId, _options, _logger));

            try
            {
                var connection = await pool.RentAsync(_connectionFactory, ct);
                _logger.LogDebug("Rented connection for tenant {TenantId}. Pool size: {PoolSize}", tenantId, pool.Size);
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rent connection for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async ValueTask ReturnAsync(IErpConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var pool = _tenantPools.GetValueOrDefault(connection.TenantId);
            if (pool == null)
            {
                _logger.LogWarning("Attempted to return connection for unknown tenant {TenantId}", connection.TenantId);
                await connection.DisposeAsync();
                return;
            }

            await pool.ReturnAsync(connection);
            _logger.LogDebug("Returned connection for tenant {TenantId}. Pool size: {PoolSize}", connection.TenantId, pool.Size);
        }

        public async Task WarmupAsync(string tenantId, int count = 5, CancellationToken ct = default)
        {
            var pool = _tenantPools.GetOrAdd(tenantId, _ => new TenantConnectionPool(tenantId, _options, _logger));
            await pool.WarmupAsync(_connectionFactory, count, ct);
            _logger.LogInformation("Warmed up {Count} connections for tenant {TenantId}", count, tenantId);
        }

        public async Task HealthCheckAsync(string tenantId, CancellationToken ct = default)
        {
            var pool = _tenantPools.GetValueOrDefault(tenantId);
            if (pool != null)
            {
                await pool.HealthCheckAsync(ct);
            }
        }

        public PoolStatistics GetStatistics(string tenantId)
        {
            var pool = _tenantPools.GetValueOrDefault(tenantId);
            return pool?.GetStatistics() ?? new PoolStatistics { TenantId = tenantId };
        }

        private async Task PerformGlobalHealthCheckAsync()
        {
            foreach (var kvp in _tenantPools)
            {
                try
                {
                    await kvp.Value.HealthCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Health check failed for tenant {TenantId}", kvp.Key);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            GC.SuppressFinalize(this);

            _healthCheckTimer?.Dispose();

            foreach (var pool in _tenantPools.Values)
            {
                await pool.DisposeAsync();
            }
            _tenantPools.Clear();
        }
    }

    /// <summary>
    /// Per-tenant connection pool
    /// </summary>
    internal class TenantConnectionPool : IAsyncDisposable
    {
        private readonly string _tenantId;
        private readonly ErpConnectionPoolOptions _options;
        private readonly ILogger _logger;

        private readonly Channel<IErpConnection> _availableConnections;
        private readonly ConcurrentDictionary<string, IErpConnection> _activeConnections;
        private readonly SemaphoreSlim _creationLock;
        private bool _disposed;

        public int Size => _availableConnections.Reader.Count + _activeConnections.Count;

        public TenantConnectionPool(string tenantId, ErpConnectionPoolOptions options, ILogger logger)
        {
            _tenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _availableConnections = Channel.CreateBounded<IErpConnection>(
                new BoundedChannelOptions(_options.MaxPoolSize)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

            _activeConnections = new ConcurrentDictionary<string, IErpConnection>();
            _creationLock = new SemaphoreSlim(1, 1);
        }

        public async Task<IErpConnection> RentAsync(IErpConnectionFactory factory, CancellationToken ct)
        {
            // Try to get existing connection from pool
            if (_availableConnections.Reader.TryRead(out var connection))
            {
                if (await IsConnectionValidAsync(connection, ct))
                {
                    _activeConnections.TryAdd(connection.GetHashCode().ToString(), connection);
                    return connection;
                }
                else
                {
                    // Connection is invalid, dispose and create new one
                    await connection.DisposeAsync();
                }
            }

            // Create new connection (with lock to prevent stampede)
            await _creationLock.WaitAsync(ct);
            try
            {
                var newConnection = await factory.CreateConnectionAsync(_tenantId, ct);
                _activeConnections.TryAdd(newConnection.GetHashCode().ToString(), newConnection);
                return newConnection;
            }
            finally
            {
                _creationLock.Release();
            }
        }

        public async Task ReturnAsync(IErpConnection connection)
        {
            if (!await IsConnectionValidAsync(connection, default))
            {
                await connection.DisposeAsync();
                return;
            }

            // Reset connection state (close transactions, etc.)
            connection.CloseActiveTransaction();

            // Try to return to pool
            if (_activeConnections.TryRemove(connection.GetHashCode().ToString(), out _))
            {
                await _availableConnections.Writer.WriteAsync(connection);
            }
            else
            {
                // Connection not in active list, dispose
                await connection.DisposeAsync();
            }
        }

        public async Task WarmupAsync(IErpConnectionFactory factory, int count, CancellationToken ct)
        {
            // Pre-create connections like eGate's GlobalWarmup()
            var warmupTasks = new List<Task>();
            for (int i = 0; i < Math.Min(count, _options.MaxPoolSize); i++)
            {
                warmupTasks.Add(Task.Run(async () =>
                {
                    var connection = await RentAsync(factory, ct);
                    await ReturnAsync(connection);
                }, ct));
            }

            await Task.WhenAll(warmupTasks);
        }

        public async Task HealthCheckAsync(CancellationToken ct = default)
        {
            // Check all connections in pool
            var invalidConnections = new List<IErpConnection>();

            await foreach (var connection in _availableConnections.Reader.ReadAllAsync(ct))
            {
                if (!await IsConnectionValidAsync(connection, ct))
                {
                    invalidConnections.Add(connection);
                }
            }

            // Remove invalid connections
            foreach (var invalid in invalidConnections)
            {
                await invalid.DisposeAsync();
            }

            // Check active connections (mark for cleanup on return)
            foreach (var kvp in _activeConnections)
            {
                if (!await IsConnectionValidAsync(kvp.Value, ct))
                {
                    // Mark as unhealthy - will be disposed on return
                    // We don't dispose here as it might be in use
                }
            }
        }

        public PoolStatistics GetStatistics()
        {
            return new PoolStatistics
            {
                TenantId = _tenantId,
                TotalConnections = Size,
                AvailableConnections = _availableConnections.Reader.Count,
                LeasedConnections = _activeConnections.Count,
                MaxConnections = _options.MaxPoolSize
            };
        }

        private async Task<bool> IsConnectionValidAsync(IErpConnection connection, CancellationToken ct)
        {
            if (!connection.IsHealthy)
                return false;

            // Check age
            var age = DateTimeOffset.UtcNow - connection.CreatedAt;
            if (age > _options.MaxConnectionAge)
                return false;

            // Check idle time
            var idleTime = DateTimeOffset.UtcNow - connection.LastUsedAt;
            if (idleTime > _options.MaxIdleTime)
                return false;

            // Optional: Perform actual health check (ping ERP)
            if (_options.EnableHealthChecks)
            {
                try
                {
                    await connection.ExecuteAsync(ctx => Task.FromResult(true), ct);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            // Dispose all active connections
            foreach (var kvp in _activeConnections)
            {
                await kvp.Value.DisposeAsync();
            }
            _activeConnections.Clear();

            // Dispose all available connections
            await foreach (var connection in _availableConnections.Reader.ReadAllAsync())
            {
                await connection.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Connection Pool Options
    /// </summary>
    public class ErpConnectionPoolOptions
    {
        public int MaxPoolSize { get; set; } = 10;
        public TimeSpan MaxConnectionAge { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan MaxIdleTime { get; set; } = TimeSpan.FromMinutes(30);
        public bool EnableHealthChecks { get; set; } = true;
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Factory for creating ERP connections
    /// </summary>
    public interface IErpConnectionFactory
    {
        Task<IErpConnection> CreateConnectionAsync(string tenantId, CancellationToken ct = default);
    }

    /// <summary>
    /// Pool statistics
    /// </summary>
    public class PoolStatistics
    {
        public string TenantId { get; set; } = string.Empty;
        public int TotalConnections { get; set; }
        public int AvailableConnections { get; set; }
        public int LeasedConnections { get; set; }
        public int MaxConnections { get; set; }
    }
}
