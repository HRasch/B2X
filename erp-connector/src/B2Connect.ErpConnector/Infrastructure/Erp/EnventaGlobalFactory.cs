namespace B2Connect.ErpConnector.Infrastructure.Erp
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Threading;
    using B2Connect.ErpConnector.Infrastructure.Identity;
    using NLog;

    /// <summary>
    /// Static factory for managing enventa global object pools.
    /// Based on FSGlobalFactory from eGate.
    /// 
    /// Key features:
    /// - Maintains separate pools per identity (tenant isolation)
    /// - Lazy initialization of pools
    /// - Thread-safe pool management
    /// - Automatic cleanup on dispose
    /// </summary>
    public static class EnventaGlobalFactory
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private static readonly ConcurrentDictionary<string, Lazy<EnventaGlobalPool>> _globalPools
            = new ConcurrentDictionary<string, Lazy<EnventaGlobalPool>>();
        private static readonly EventWaitHandle _globalWaitHandle
            = new EventWaitHandle(false, EventResetMode.AutoReset);

        private static IEnventaGlobalObjectFactory _globalObjectFactory;
        private static int _poolSize = 1;
        private static bool _initialized;

        /// <summary>
        /// Initializes the factory with configuration.
        /// Must be called before using Get/Put.
        /// </summary>
        public static void Initialize(IEnventaGlobalObjectFactory globalObjectFactory, int poolSize = 1)
        {
            if (_initialized)
            {
                Log.Warn("EnventaGlobalFactory already initialized");
                return;
            }

            _globalObjectFactory = globalObjectFactory ?? throw new ArgumentNullException(nameof(globalObjectFactory));
            _poolSize = poolSize > 0 ? poolSize : 1;
            _initialized = true;

            Log.Info("EnventaGlobalFactory initialized with pool size {0}", _poolSize);
        }

        /// <summary>
        /// Gets a global context for the given identity.
        /// Returns a context from the identity's pool.
        /// </summary>
        public static EnventaGlobalContext Get(EnventaIdentity identity)
        {
            if (!_initialized)
            {
                // Auto-initialize with mock factory for development
                Initialize(new MockEnventaGlobalObjectFactory(), 1);
            }

            if (identity == null || string.IsNullOrWhiteSpace(identity.Name))
            {
                throw new ArgumentException("Invalid identity: Name is required");
            }

            var token = identity.GetToken();

            var pool = _globalPools.GetOrAdd(token, x => new Lazy<EnventaGlobalPool>(() => CreateGlobalPool(identity)));

            var global = pool.Value.GetGlobal();

            return new EnventaGlobalContext(identity, global);
        }

        /// <summary>
        /// Returns a global context to its pool.
        /// </summary>
        public static void Put(EnventaGlobalContext context)
        {
            if (context == null)
                return;

            if (_globalPools.TryGetValue(context.Identity.GetToken(), out var pool))
            {
                pool.Value.PutGlobal(context.FSGlobal);
            }
        }

        private static EnventaGlobalPool CreateGlobalPool(EnventaIdentity identity)
        {
            Log.Info("Creating new global pool for {0}", identity);

            var pool = new EnventaGlobalPool(identity, _poolSize, _globalWaitHandle, _globalObjectFactory);
            pool.Initialize();

            return pool;
        }

        /// <summary>
        /// Disposes all pools and cleans up resources.
        /// </summary>
        public static void Dispose()
        {
            Log.Info("Disposing EnventaGlobalFactory");

            foreach (var key in _globalPools.Keys)
            {
                if (_globalPools.TryRemove(key, out var pool))
                {
                    if (pool.IsValueCreated)
                    {
                        pool.Value.Dispose();
                    }
                }
            }

            _initialized = false;
        }
    }
}
