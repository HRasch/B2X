using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace B2Connect.ErpConnector.Services
{
    /// <summary>
    /// Thread-safe ERP access using Actor pattern.
    /// All ERP operations are serialized through a single worker thread per tenant.
    /// This is required because enventa ERP assemblies are NOT thread-safe.
    /// </summary>
    public class EnventaErpActor : IDisposable
    {
        private readonly BlockingCollection<ErpOperation> _operationQueue;
        private readonly Task _workerTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly string _tenantId;
        private readonly string? _businessUnit;
        private bool _disposed;

        // TODO: Replace with actual enventa connection type
        private readonly object _erpConnection;

        public EnventaErpActor(string tenantId, string? businessUnit = null)
        {
            _tenantId = tenantId;
            _businessUnit = businessUnit;

            _operationQueue = new BlockingCollection<ErpOperation>(boundedCapacity: 1000);

            // Initialize ERP connection
            _erpConnection = InitializeErpConnection(tenantId, businessUnit);

            // Start the single-threaded worker
            _workerTask = Task.Factory.StartNew(
                () => ProcessOperationsAsync(_cts.Token),
                TaskCreationOptions.LongRunning);

            Console.WriteLine($"ERP Actor created for tenant {tenantId}, business unit {businessUnit ?? "default"}");
        }

        /// <summary>
        /// Enqueue an operation and wait for result.
        /// Thread-safe: can be called from multiple HTTP request threads.
        /// Supports cancellation for long-running ADO.NET operations.
        /// </summary>
        /// <param name="operation">The operation to execute with connection and cancellation token</param>
        /// <param name="ct">Cancellation token to cancel the operation</param>
        public Task<TResult> ExecuteAsync<TResult>(Func<object, CancellationToken, TResult> operation, CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<TResult>();

            // Register cancellation callback to cancel the task
            ct.Register(() => tcs.TrySetCanceled(ct), useSynchronizationContext: false);

            var erpOp = new ErpOperation(
                (conn, token) =>
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        var result = operation(conn, token);
                        tcs.TrySetResult(result);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled(ct);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                },
                ct);

            // Use synchronous write for .NET 4.8 compatibility
            if (_operationQueue.TryAdd(erpOp))
            {
                return tcs.Task;
            }

            // Queue is full, wait asynchronously
            return EnqueueAndWaitAsync(erpOp, tcs, ct);
        }

        /// <summary>
        /// Simplified overload without cancellation token for backwards compatibility.
        /// </summary>
        public Task<TResult> ExecuteAsync<TResult>(Func<object, TResult> operation)
        {
            return ExecuteAsync((conn, ct) => operation(conn), CancellationToken.None);
        }

        private async Task<TResult> EnqueueAndWaitAsync<TResult>(ErpOperation op, TaskCompletionSource<TResult> tcs, CancellationToken ct)
        {
            _operationQueue.Add(op);
            return await tcs.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Single-threaded worker processing all ERP operations.
        /// </summary>
        private async Task ProcessOperationsAsync(CancellationToken ct)
        {
            Console.WriteLine($"ERP Actor worker started for tenant {_tenantId}");

            try
            {
                foreach (var operation in _operationQueue.GetConsumingEnumerable(ct))
                {
                    try
                    {
                        // Execute on single thread - thread-safe for enventa
                        operation.Execute(_erpConnection);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing ERP operation for tenant {_tenantId}: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"ERP Actor worker cancelled for tenant {_tenantId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error in ERP Actor worker for tenant {_tenantId}: {ex.Message}");
            }
        }

        private object InitializeErpConnection(string tenantId, string? businessUnit)
        {
            Console.WriteLine($"Initializing ERP connection for tenant {tenantId}, business unit {businessUnit ?? "default"}");

            // TODO: Replace with actual enventa connection
            // Example with real enventa:
            // var identity = new NVIdentity(tenantId, username, password, businessUnit);
            // var connection = FSGlobalPool.GetConnection(identity);
            // return connection;

            // For now, return a mock connection
            return new MockErpConnection(tenantId, businessUnit);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Console.WriteLine($"Disposing ERP Actor for tenant {_tenantId}...");

            // Signal cancellation
            _cts.Cancel();

            // Signal that no more items will be added
            _operationQueue.CompleteAdding();

            // Wait for worker to complete
            try
            {
                _workerTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch (AggregateException)
            {
                // Expected when cancelled
            }

            // Dispose ERP connection
            if (_erpConnection is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _cts.Dispose();

            Console.WriteLine($"ERP Actor disposed for tenant {_tenantId}");
        }
    }

    /// <summary>
    /// Manages per-tenant ERP actors with thread isolation.
    /// Singleton pattern for use across the application.
    /// </summary>
    public class EnventaActorPool : IDisposable
    {
        private static readonly Lazy<EnventaActorPool> _instance = new Lazy<EnventaActorPool>(() => new EnventaActorPool());
        private readonly ConcurrentDictionary<string, Lazy<EnventaErpActor>> _actors = new ConcurrentDictionary<string, Lazy<EnventaErpActor>>();
        private bool _disposed;

        public static EnventaActorPool Instance => _instance.Value;

        private EnventaActorPool()
        {
            Console.WriteLine("ERP Actor Pool initialized");
        }

        /// <summary>
        /// Get or create actor for tenant and business unit. Thread-safe.
        /// Each tenant/business-unit combination gets dedicated single-threaded ERP access.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="businessUnit">Business unit identifier (optional, uses configured default if null)</param>
        public EnventaErpActor GetActor(string tenantId, string? businessUnit = null)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                tenantId = "default";
            }

            // Create composite key for tenant + business unit isolation
            var actorKey = businessUnit != null ? $"{tenantId}:{businessUnit}" : tenantId;

            return _actors.GetOrAdd(
                actorKey,
                key => new Lazy<EnventaErpActor>(() =>
                {
                    Console.WriteLine($"Creating new ERP actor for tenant {tenantId}, business unit {businessUnit ?? "default"}");
                    return new EnventaErpActor(tenantId, businessUnit);
                })
            ).Value;
        }

        /// <summary>
        /// Get or create actor for tenant (backwards compatibility).
        /// Uses configured default business unit.
        /// </summary>
        public EnventaErpActor GetActor(string tenantId)
        {
            return GetActor(tenantId, null);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Console.WriteLine("Disposing ERP Actor Pool...");

            foreach (var kvp in _actors)
            {
                if (kvp.Value.IsValueCreated)
                {
                    try
                    {
                        kvp.Value.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error disposing actor for tenant {kvp.Key}: {ex.Message}");
                    }
                }
            }

            _actors.Clear();
            Console.WriteLine("ERP Actor Pool disposed");
        }
    }

    /// <summary>
    /// Represents an operation to be executed on the ERP actor.
    /// Supports cancellation for ADO.NET DataReader operations.
    /// </summary>
    internal class ErpOperation
    {
        private readonly Action<object, CancellationToken> _executeAction;
        private readonly CancellationToken _cancellationToken;

        public ErpOperation(Action<object, CancellationToken> executeAction, CancellationToken cancellationToken = default)
        {
            _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
            _cancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken => _cancellationToken;

        public void Execute(object connection)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            _executeAction(connection, _cancellationToken);
        }
    }

    /// <summary>
    /// Mock ERP connection for development/testing.
    /// Replace with actual enventa connection in production.
    /// </summary>
    internal class MockErpConnection : IDisposable
    {
        private readonly string _tenantId;
        private readonly string? _businessUnit;

        public string TenantId => _tenantId;
        public string? BusinessUnit => _businessUnit;

        public MockErpConnection(string tenantId, string? businessUnit = null)
        {
            _tenantId = tenantId;
            _businessUnit = businessUnit;
            Console.WriteLine($"Mock ERP connection created for tenant {_tenantId}, business unit {_businessUnit ?? "default"}");
        }

        public void Dispose()
        {
            Console.WriteLine($"Mock ERP connection disposed for tenant {_tenantId}, business unit {_businessUnit ?? "default"}");
        }
    }
}
