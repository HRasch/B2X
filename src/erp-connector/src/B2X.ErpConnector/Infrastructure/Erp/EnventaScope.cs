using System;
using B2X.ErpConnector.Infrastructure.Identity;

namespace B2X.ErpConnector.Infrastructure.Erp
{
    /// <summary>
    /// Disposable scope for enventa ERP operations.
    /// Based on FSScope from eGate.
    /// 
    /// Usage pattern:
    /// <code>
    /// using (var scope = new EnventaScope(identity))
    /// {
    ///     var article = scope.Create&lt;IcECArticle&gt;();
    ///     // ... use article ...
    /// }
    /// // Connection automatically returned to pool
    /// </code>
    /// 
    /// Key features:
    /// - Acquires connection from pool on creation
    /// - Returns connection to pool on dispose
    /// - Provides factory methods for enventa components
    /// - Thread-safe when used with Actor pattern
    /// </summary>
    public class EnventaScope : IDisposable
    {
        private EnventaGlobalContext _context;
        private bool _disposed;

        /// <summary>
        /// Creates a new scope using the given identity.
        /// </summary>
        public EnventaScope(EnventaIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            _context = EnventaGlobalFactory.Get(identity);
            Console.WriteLine($"Created scope for {identity}");
        }

        /// <summary>
        /// Creates a new scope from an existing context.
        /// Used for nested operations.
        /// </summary>
        public EnventaScope(EnventaGlobalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Get a new context from the pool using the same identity
            _context = EnventaGlobalFactory.Get(context.Identity);
            Console.WriteLine($"Created nested scope for {context.Identity}");
        }

        /// <summary>
        /// Gets the underlying global context.
        /// </summary>
        public EnventaGlobalContext GlobalContext
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(EnventaScope));
                return _context;
            }
        }

        /// <summary>
        /// Creates an enventa component instance.
        /// </summary>
        /// <typeparam name="T">Component type (e.g., IcECArticle)</typeparam>
        public T Create<T>() where T : class, IDevFrameworkObject
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EnventaScope));

            return _context.Create<T>();
        }

        /// <summary>
        /// Disposes the scope, returning the connection to the pool.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_context != null)
            {
                Console.WriteLine($"Disposing scope for {_context.Identity}");
                _context.CloseConnection();
                EnventaGlobalFactory.Put(_context);
                _context = null;
            }
        }
    }
}
