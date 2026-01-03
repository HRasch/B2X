using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.ErpConnector.Infrastructure.Erp;
using B2Connect.ErpConnector.Infrastructure.Identity;
using B2Connect.ErpConnector.Infrastructure.Repository;
using B2Connect.ErpConnector.Models;
using NLog;

namespace B2Connect.ErpConnector.Services
{
    /// <summary>
    /// ERP Service implementation using the repository pattern.
    /// Based on eGate ECArticleService/NVContext patterns.
    /// 
    /// Key patterns:
    /// - Uses EnventaContext (Unit of Work) for repository access
    /// - Uses Actor pattern for thread-safe access (enventa is not thread-safe)
    /// - Uses Query Builders for type-safe filtering
    /// - Supports multi-tenant through identity provider
    /// </summary>
    public class EnventaErpService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EnventaActorPool _actorPool;
        private readonly IEnventaIdentityProvider _identityProvider;

        public EnventaErpService(IEnventaIdentityProvider identityProvider)
        {
            _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
            _actorPool = EnventaActorPool.Instance;
        }

        public EnventaErpService() : this(new ConfiguredIdentityProvider("sysadm", "sysadm", "10"))
        {
        }

        /// <summary>
        /// Creates an identity provider for a specific business unit.
        /// </summary>
        private IEnventaIdentityProvider GetIdentityProviderForBusinessUnit(string? businessUnit)
        {
            return businessUnit != null
                ? new ConfiguredIdentityProvider(_identityProvider.Get.Name, _identityProvider.Get.Password, businessUnit)
                : _identityProvider;
        }

        #region Articles

        /// <summary>
        /// Get article by ID with cancellation support.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="articleId">Article ID to retrieve</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<ArticleDto> GetArticleAsync(string tenantId, string articleId, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("GetArticle: tenant={0}, articleId={1}, businessUnit={2}", tenantId, articleId, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    return context.Articles.Find(articleId);
                }
            }, ct);
        }

        /// <summary>
        /// Query articles with filtering, sorting and pagination.
        /// Supports cancellation for long-running ADO.NET operations.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="query">Query parameters (filters, sorting, pagination)</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<CursorPageResponse<ArticleDto>> QueryArticlesAsync(string tenantId, QueryRequest query, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("QueryArticles: tenant={0}, filters={1}, businessUnit={2}", tenantId, query.Filters?.Count ?? 0, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    // Build query using type-safe query builder
                    var queryBuilder = context.Articles.Query();

                    // Apply filters from request
                    ApplyArticleFilters(queryBuilder, query.Filters);

                    // Apply sorting
                    ApplyArticleSorting(queryBuilder, query.Sorting);

                    // Get total count before pagination
                    var totalCount = queryBuilder.Count();

                    // Apply pagination
                    if (query.Skip.HasValue)
                        queryBuilder.Skip(query.Skip.Value);
                    if (query.Take.HasValue)
                        queryBuilder.Take(query.Take.Value);

                    // Execute query
                    var articles = queryBuilder.Execute().ToList();

                    var skip = query.Skip ?? 0;
                    var take = query.Take ?? 100;

                    return new CursorPageResponse<ArticleDto>
                    {
                        Items = articles,
                        TotalCount = totalCount,
                        HasMore = skip + take < totalCount,
                        NextCursor = skip + take < totalCount
                            ? Convert.ToBase64String(BitConverter.GetBytes(skip + take))
                            : null
                    };
                }
            }, ct);
        }

        /// <summary>
        /// Sync articles with delta support (modified since timestamp).
        /// Supports cancellation for long-running ADO.NET operations.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="request">Delta sync request with optional since timestamp</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<DeltaSyncResponse<ArticleDto>> SyncArticlesAsync(string tenantId, DeltaSyncRequest request, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("SyncArticles: tenant={0}, since={1}, businessUnit={2}", tenantId, request.Since, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    var queryBuilder = context.Articles.Query();

                    // Filter by modification date
                    if (request.Since.HasValue)
                    {
                        queryBuilder.ModifiedSince(request.Since.Value);
                    }

                    // Apply other filters from request
                    if (!string.IsNullOrWhiteSpace(request.Category))
                    {
                        queryBuilder.WithCategory(request.Category);
                    }

                    // Apply business unit filter if specified
                    if (!string.IsNullOrWhiteSpace(request.BusinessUnit))
                    {
                        queryBuilder.WithBusinessUnit(request.BusinessUnit);
                    }

                    queryBuilder.OrderByModifiedDescending();

                    token.ThrowIfCancellationRequested();
                    var articles = queryBuilder.Execute().ToList();

                    return new DeltaSyncResponse<ArticleDto>
                    {
                        Items = articles,
                        TotalCount = articles.Count,
                        HasMore = false,
                        LastModified = DateTime.UtcNow,
                        Watermark = Convert.ToBase64String(BitConverter.GetBytes(DateTime.UtcNow.Ticks))
                    };
                }
            }, ct);
        }

        private void ApplyArticleFilters(ArticleQueryBuilder queryBuilder, List<QueryFilter> filters)
        {
            if (filters == null) return;

            foreach (var filter in filters)
            {
                switch (filter.PropertyName.ToLowerInvariant())
                {
                    case "name":
                        if (filter.Operator == FilterOperator.Contains && filter.Value is string nameValue)
                            queryBuilder.WithNameContains(nameValue);
                        break;

                    case "articlestate":
                        if (filter.Value is int stateValue)
                            queryBuilder.WithState((ArticleState)stateValue);
                        break;

                    case "category":
                        if (filter.Value is string categoryValue)
                            queryBuilder.WithCategory(categoryValue);
                        break;

                    case "price":
                        if (filter.Value is decimal priceValue)
                        {
                            switch (filter.Operator)
                            {
                                case FilterOperator.GreaterThan:
                                    queryBuilder.WithPriceRange(priceValue, null);
                                    break;
                                case FilterOperator.LessThan:
                                    queryBuilder.WithPriceRange(null, priceValue);
                                    break;
                            }
                        }
                        break;

                    case "noecommerce":
                        if (filter.Value is bool noEcomValue && !noEcomValue)
                            queryBuilder.ECommerceEnabled();
                        break;

                    case "active":
                        if (filter.Value is bool activeValue && activeValue)
                            queryBuilder.ActiveOnly();
                        break;
                }
            }
        }

        private void ApplyArticleSorting(ArticleQueryBuilder queryBuilder, List<SortField> sorting)
        {
            if (sorting == null || !sorting.Any()) return;

            var sort = sorting.First();
            switch (sort.PropertyName.ToLowerInvariant())
            {
                case "name":
                    if (sort.Order == SortOrder.Ascending)
                        queryBuilder.OrderByName();
                    else
                        queryBuilder.OrderByNameDescending();
                    break;

                case "price":
                    if (sort.Order == SortOrder.Ascending)
                        queryBuilder.OrderByPrice();
                    else
                        queryBuilder.OrderByPriceDescending();
                    break;

                case "modifiedat":
                    queryBuilder.OrderByModifiedDescending();
                    break;
            }
        }

        #endregion

        #region Customers

        /// <summary>
        /// Get customer by number with cancellation support.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="customerNumber">Customer number to retrieve</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<CustomerDto> GetCustomerAsync(string tenantId, string customerNumber, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("GetCustomer: tenant={0}, customerNumber={1}, businessUnit={2}", tenantId, customerNumber, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    return context.Customers.Find(customerNumber);
                }
            }, ct);
        }

        /// <summary>
        /// Query customers with filtering, sorting and pagination.
        /// Supports cancellation for long-running ADO.NET operations.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="query">Query parameters (filters, sorting, pagination)</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<CursorPageResponse<CustomerDto>> QueryCustomersAsync(string tenantId, QueryRequest query, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("QueryCustomers: tenant={0}, businessUnit={1}", tenantId, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    var queryBuilder = context.Customers.Query();

                    // Apply filters
                    ApplyCustomerFilters(queryBuilder, query.Filters);

                    // Apply sorting
                    ApplyCustomerSorting(queryBuilder, query.Sorting);

                    var totalCount = queryBuilder.Count();

                    if (query.Skip.HasValue)
                        queryBuilder.Skip(query.Skip.Value);
                    if (query.Take.HasValue)
                        queryBuilder.Take(query.Take.Value);

                    token.ThrowIfCancellationRequested();
                    var customers = queryBuilder.Execute().ToList();

                    var skip = query.Skip ?? 0;
                    var take = query.Take ?? 100;

                    return new CursorPageResponse<CustomerDto>
                    {
                        Items = customers,
                        TotalCount = totalCount,
                        HasMore = skip + take < totalCount
                    };
                }
            }, ct);
        }

        private void ApplyCustomerFilters(CustomerQueryBuilder queryBuilder, List<QueryFilter> filters)
        {
            if (filters == null) return;

            foreach (var filter in filters)
            {
                switch (filter.PropertyName.ToLowerInvariant())
                {
                    case "companyname":
                        if (filter.Operator == FilterOperator.Contains && filter.Value is string nameValue)
                            queryBuilder.WithCompanyNameContains(nameValue);
                        break;

                    case "country":
                        if (filter.Value is string countryValue)
                            queryBuilder.WithCountry(countryValue);
                        break;

                    case "city":
                        if (filter.Value is string cityValue)
                            queryBuilder.WithCity(cityValue);
                        break;

                    case "active":
                        if (filter.Value is bool activeValue && activeValue)
                            queryBuilder.ActiveOnly();
                        break;
                }
            }
        }

        private void ApplyCustomerSorting(CustomerQueryBuilder queryBuilder, List<SortField> sorting)
        {
            if (sorting == null || !sorting.Any()) return;

            var sort = sorting.First();
            switch (sort.PropertyName.ToLowerInvariant())
            {
                case "companyname":
                    queryBuilder.OrderByCompanyName();
                    break;

                case "customernumber":
                    queryBuilder.OrderByCustomerNumber();
                    break;
            }
        }

        #endregion

        #region Orders

        /// <summary>
        /// Get order by number with cancellation support.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="orderNumber">Order number to retrieve</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<OrderDto> GetOrderAsync(string tenantId, string orderNumber, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("GetOrder: tenant={0}, orderNumber={1}, businessUnit={2}", tenantId, orderNumber, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    return context.Orders.Find(orderNumber);
                }
            }, ct);
        }

        /// <summary>
        /// Create a new order with cancellation support.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="request">Order creation request</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET operations</param>
        public Task<OrderDto> CreateOrderAsync(string tenantId, CreateOrderRequest request, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Info("CreateOrder: tenant={0}, customer={1}, businessUnit={2}", tenantId, request.CustomerNumber, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(_identityProvider))
                {
                    var order = context.Orders.CreateOrder(request);
                    Logger.Info("Order created: {0}", order.OrderNumber);
                    return order;
                }
            }, ct);
        }

        /// <summary>
        /// Query orders with filtering, sorting and pagination.
        /// Supports cancellation for long-running ADO.NET operations.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="query">Query parameters (filters, sorting, pagination)</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<CursorPageResponse<OrderDto>> QueryOrdersAsync(string tenantId, QueryRequest query, string? businessUnit = null, CancellationToken ct = default)
        {
            Logger.Debug("QueryOrders: tenant={0}, businessUnit={1}", tenantId, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(_identityProvider))
                {
                    var queryBuilder = context.Orders.Query();

                    ApplyOrderFilters(queryBuilder, query.Filters);
                    ApplyOrderSorting(queryBuilder, query.Sorting);

                    var totalCount = queryBuilder.Count();

                    if (query.Skip.HasValue)
                        queryBuilder.Skip(query.Skip.Value);
                    if (query.Take.HasValue)
                        queryBuilder.Take(query.Take.Value);

                    token.ThrowIfCancellationRequested();
                    var orders = queryBuilder.Execute().ToList();

                    var skip = query.Skip ?? 0;
                    var take = query.Take ?? 100;

                    return new CursorPageResponse<OrderDto>
                    {
                        Items = orders,
                        TotalCount = totalCount,
                        HasMore = skip + take < totalCount
                    };
                }
            }, ct);
        }

        /// <summary>
        /// Get orders for a specific customer with cancellation support.
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="customerNumber">Customer number</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="limit">Optional limit on results</param>
        /// <param name="ct">Cancellation token for ADO.NET DataReader operations</param>
        public Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(string tenantId, string customerNumber, string? businessUnit = null, int? limit = null, CancellationToken ct = default)
        {
            Logger.Debug("GetCustomerOrders: tenant={0}, customer={1}, businessUnit={2}", tenantId, customerNumber, businessUnit);

            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    return context.Orders.GetCustomerOrders(customerNumber, limit);
                }
            }, ct);
        }

        private void ApplyOrderFilters(OrderQueryBuilder queryBuilder, List<QueryFilter> filters)
        {
            if (filters == null) return;

            foreach (var filter in filters)
            {
                switch (filter.PropertyName.ToLowerInvariant())
                {
                    case "customernumber":
                        if (filter.Value is string customerValue)
                            queryBuilder.WithCustomerNumber(customerValue);
                        break;

                    case "status":
                        if (filter.Value is int statusValue)
                            queryBuilder.WithStatus((OrderStatus)statusValue);
                        break;

                    case "minamount":
                        if (filter.Value is decimal amountValue)
                            queryBuilder.WithMinAmount(amountValue);
                        break;
                }
            }
        }

        private void ApplyOrderSorting(OrderQueryBuilder queryBuilder, List<SortField> sorting)
        {
            if (sorting == null || !sorting.Any())
            {
                queryBuilder.OrderByDateDescending();
                return;
            }

            var sort = sorting.First();
            switch (sort.PropertyName.ToLowerInvariant())
            {
                case "orderdate":
                    queryBuilder.OrderByDateDescending();
                    break;

                case "ordernumber":
                    queryBuilder.OrderByOrderNumber();
                    break;

                case "totalamount":
                    queryBuilder.OrderByAmountDescending();
                    break;
            }
        }

        #endregion

        #region Licensing

        /// <summary>
        /// Check if Steel license is enabled (async).
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token</param>
        public Task<bool> CheckSteelLicenseEnabledAsync(string tenantId, string? businessUnit = null, CancellationToken ct = default)
        {
            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                using (var context = new EnventaContext(_identityProvider))
                {
                    // In production:
                    // var license = context.Scope.Create<IcECLicense>();
                    // return license.SteelEnabled();
                    return false;
                }
            }, ct);
        }

        /// <summary>
        /// Check if Datanorm license is enabled (async).
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="businessUnit">Optional business unit (defaults to configured)</param>
        /// <param name="ct">Cancellation token</param>
        public Task<bool> CheckDatanormLicenseEnabledAsync(string tenantId, string? businessUnit = null, CancellationToken ct = default)
        {
            var actor = _actorPool.GetActor(tenantId, businessUnit);
            return actor.ExecuteAsync((connection, token) =>
            {
                token.ThrowIfCancellationRequested();
                var identityProvider = GetIdentityProviderForBusinessUnit(businessUnit);
                using (var context = new EnventaContext(identityProvider))
                {
                    // In production:
                    // var license = context.Scope.Create<IcECLicense>();
                    // return license.DatanormEnabled();
                    return false;
                }
            }, ct);
        }

        #endregion
    }
}
