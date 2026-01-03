using System;
using B2Connect.ErpConnector.Infrastructure.Erp;
using B2Connect.ErpConnector.Infrastructure.Identity;

namespace B2Connect.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Unit of Work / Context that exposes all repositories.
    /// Based on eGate NVContext pattern.
    /// 
    /// Usage:
    /// <code>
    /// using (var context = new EnventaContext(identityProvider))
    /// {
    ///     var articles = context.Articles.Query()
    ///         .ActiveOnly()
    ///         .WithCategory("Schrauben")
    ///         .Take(100)
    ///         .Execute();
    ///     
    ///     var customer = context.Customers.Find("CUST0001");
    ///     
    ///     var order = context.Orders.CreateOrder(request);
    /// }
    /// </code>
    /// </summary>
    public class EnventaContext : IDisposable
    {
        private readonly EnventaScope _scope;
        private bool _disposed;

        // Lazy-loaded repositories
        private EnventaArticleRepository _articles;
        private EnventaCustomerRepository _customers;
        private EnventaOrderRepository _orders;

        /// <summary>
        /// Create context with identity provider.
        /// </summary>
        public EnventaContext(IEnventaIdentityProvider identityProvider)
        {
            if (identityProvider == null)
                throw new ArgumentNullException(nameof(identityProvider));

            var util = new EnventaUtil(identityProvider);
            _scope = util.Scope();
        }

        /// <summary>
        /// Create context with existing scope.
        /// </summary>
        public EnventaContext(EnventaScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <summary>
        /// Article repository.
        /// </summary>
        public EnventaArticleRepository Articles
        {
            get
            {
                ThrowIfDisposed();
                return _articles ?? (_articles = new EnventaArticleRepository(_scope));
            }
        }

        /// <summary>
        /// Customer repository.
        /// </summary>
        public EnventaCustomerRepository Customers
        {
            get
            {
                ThrowIfDisposed();
                return _customers ?? (_customers = new EnventaCustomerRepository(_scope));
            }
        }

        /// <summary>
        /// Order repository.
        /// </summary>
        public EnventaOrderRepository Orders
        {
            get
            {
                ThrowIfDisposed();
                return _orders ?? (_orders = new EnventaOrderRepository(_scope));
            }
        }

        /// <summary>
        /// The underlying scope (for advanced operations).
        /// </summary>
        public EnventaScope Scope
        {
            get
            {
                ThrowIfDisposed();
                return _scope;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EnventaContext));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _scope?.Dispose();
                _disposed = true;
            }
        }
    }
}
