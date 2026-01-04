namespace B2Connect.ErpConnector.Infrastructure.Erp
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using B2Connect.ErpConnector.Infrastructure.Identity;

    /// <summary>
    /// Pool of enventa global objects per identity.
    /// Based on FSGlobalPool from eGate.
    /// 
    /// Key features:
    /// - Maintains a pool of pre-authenticated connections
    /// - Thread-safe access via ConcurrentBag
    /// - Automatic pool replenishment
    /// - Connection reuse for performance
    /// </summary>
    internal class EnventaGlobalPool : IDisposable
    {
        private static readonly object _lock = new object();

        private readonly ConcurrentBag<IFSGlobalObjects> _poolGlobals = new ConcurrentBag<IFSGlobalObjects>();
        private readonly ConcurrentBag<IFSGlobalObjects> _availableGlobals = new ConcurrentBag<IFSGlobalObjects>();

        private readonly EnventaIdentity _identity;
        private readonly int _poolSize;
        private readonly EventWaitHandle _globalWaitHandle;
        private readonly IEnventaGlobalObjectFactory _globalObjectFactory;

        private bool _disposed;

        public EnventaGlobalPool(
            EnventaIdentity identity,
            int poolSize,
            EventWaitHandle globalWaitHandle,
            IEnventaGlobalObjectFactory globalObjectFactory)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _poolSize = poolSize > 0 ? poolSize : 1;
            _globalWaitHandle = globalWaitHandle ?? throw new ArgumentNullException(nameof(globalWaitHandle));
            _globalObjectFactory = globalObjectFactory ?? throw new ArgumentNullException(nameof(globalObjectFactory));
        }

        /// <summary>
        /// Initializes the pool with pre-created global objects.
        /// </summary>
        public void Initialize()
        {
            Console.WriteLine($"Initializing EnventaGlobalPool for {_identity} with pool size {_poolSize}");

            for (int i = 0; i < _poolSize; i++)
            {
                Task.Run(() => CreateGlobal());
            }
        }

        /// <summary>
        /// Creates a new authenticated global object and adds it to the pool.
        /// </summary>
        public void CreateGlobal()
        {
            var globalId = Guid.NewGuid();

            try
            {
                Console.WriteLine($"Creating new global object {globalId} for {_identity}");

                var global = _globalObjectFactory.CreateGlobalObject(globalId);

                // Login must be synchronized - enventa is not thread-safe
                lock (_lock)
                {
                    _globalObjectFactory.Login(global, _identity);
                }

                // Validate login status immediately after login
                // This checks oMsg.oMsgItemColl for login-specific errors
                _globalObjectFactory.ValidateLoginStatus(global, _identity);

                // Validate the global object state
                _globalObjectFactory.Validate(global);

                // Enable caching for better performance
                _globalObjectFactory.EnableCaching(global);

                _poolGlobals.Add(global);

                // Return to available pool
                PutGlobal(global);

                // Close connection (will be reopened on next use)
                global.CloseConnection();

                Console.WriteLine($"Global object {globalId} created and pooled for {_identity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create global object {globalId} for {_identity}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets an available global object from the pool.
        /// If none available, waits for one or creates a new one.
        /// </summary>
        public IFSGlobalObjects GetGlobal()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EnventaGlobalPool));

            if (!_availableGlobals.TryTake(out var global))
            {
                // Pool exhausted, create new one in background
                Task.Run(() => CreateGlobal());

                // Wait for one to become available
                while (!_availableGlobals.TryTake(out global))
                {
                    _globalWaitHandle.WaitOne(TimeSpan.FromSeconds(30));

                    if (_disposed)
                        throw new ObjectDisposedException(nameof(EnventaGlobalPool));
                }
            }

            Console.WriteLine($"Got global object from pool for {_identity}");
            return global;
        }

        /// <summary>
        /// Returns a global object to the pool.
        /// </summary>
        public bool PutGlobal(IFSGlobalObjects global)
        {
            if (global == null)
                return false;

            if (_disposed)
            {
                try { global.Dispose(); } catch { }
                return false;
            }

            // Close connection before returning to pool
            global.CloseConnection();

            _availableGlobals.Add(global);
            _globalWaitHandle.Set();

            Console.WriteLine($"Returned global object to pool for {_identity}");
            return true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Console.WriteLine($"Disposing EnventaGlobalPool for {_identity}");

            // Dispose all pooled globals
            while (_poolGlobals.TryTake(out var global))
            {
                try
                {
                    global.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing global object: {ex.Message}");
                }
            }

            _globalWaitHandle.Set(); // Wake up any waiting threads
        }
    }
}
