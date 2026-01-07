using System.Linq.Expressions;

namespace B2X.ERP.Abstractions
{
    /// <summary>
    /// Improved query builder using Specification pattern with better performance and composability
    /// </summary>
    public abstract class ErpQuerySpecification<T>
    {
        private readonly List<QueryFilter> _filters = new();
        private readonly List<SortField> _sorting = new();
        private int? _skip;
        private int? _take;

        protected void AddFilter(QueryFilter filter) => _filters.Add(filter);
        protected void AddSorting(SortField sort) => _sorting.Add(sort);

        protected void SetSkip(int skip) => _skip = skip;
        protected void SetTake(int take) => _take = take;

        public QueryRequest ToQueryRequest() => new()
        {
            Filters = _filters,
            Sorting = _sorting,
            Skip = _skip,
            Take = _take
        };

        // Composition methods
        public ErpQuerySpecification<T> And(ErpQuerySpecification<T> other)
        {
            var combined = new CompositeSpecification<T>(this, other, CompositeOperator.And);
            combined._filters.AddRange(_filters);
            combined._filters.AddRange(other._filters);
            combined._sorting.AddRange(_sorting.Any() ? _sorting : other._sorting);
            combined._skip = _skip ?? other._skip;
            combined._take = _take ?? other._take;
            return combined;
        }

        public ErpQuerySpecification<T> Or(ErpQuerySpecification<T> other)
        {
            var combined = new CompositeSpecification<T>(this, other, CompositeOperator.Or);
            combined._filters.AddRange(_filters);
            combined._filters.AddRange(other._filters);
            combined._sorting.AddRange(_sorting.Any() ? _sorting : other._sorting);
            combined._skip = _skip ?? other._skip;
            combined._take = _take ?? other._take;
            return combined;
        }
    }

    public enum CompositeOperator { And, Or }

    internal class CompositeSpecification<T> : ErpQuerySpecification<T>
    {
        public CompositeSpecification(ErpQuerySpecification<T> left, ErpQuerySpecification<T> right, CompositeOperator @operator)
        {
            // Composite logic could be extended here if needed
        }
    }

    // Property accessor cache for better performance
    public static class PropertyCache<T>
    {
        private static readonly Dictionary<string, Func<T, object>> _accessors = new();

        public static Func<T, object> GetAccessor(string propertyName)
        {
            if (_accessors.TryGetValue(propertyName, out var accessor))
                return accessor;

            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(param, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(convert, param);

            accessor = lambda.Compile();
            _accessors[propertyName] = accessor;
            return accessor;
        }
    }

    // Filter builders with better type safety
    public static class ErpFilters
    {
        public static QueryFilter Equals(string propertyName, object value) => new() { PropertyName = propertyName, Operator = FilterOperator.Equals, Value = value };

        public static QueryFilter NotEquals(string propertyName, object value) => new() { PropertyName = propertyName, Operator = FilterOperator.NotEquals, Value = value };

        public static QueryFilter GreaterThan(string propertyName, object value) => new() { PropertyName = propertyName, Operator = FilterOperator.GreaterThan, Value = value };

        public static QueryFilter LessThan(string propertyName, object value) => new() { PropertyName = propertyName, Operator = FilterOperator.LessThan, Value = value };

        public static QueryFilter Contains(string propertyName, string value) => new() { PropertyName = propertyName, Operator = FilterOperator.Contains, Value = value };

        public static QueryFilter In(string propertyName, IEnumerable<object> values) => new() { PropertyName = propertyName, Operator = FilterOperator.In, Value = values };

        public static QueryFilter IsNull(string propertyName) => new() { PropertyName = propertyName, Operator = FilterOperator.IsNull };

        public static QueryFilter IsNotNull(string propertyName) => new() { PropertyName = propertyName, Operator = FilterOperator.IsNotNull };
    }

    // Sorting helpers
    public static class ErpSorting
    {
        public static SortField Ascending(string propertyName) => new() { PropertyName = propertyName, Order = SortOrder.Ascending };

        public static SortField Descending(string propertyName) => new() { PropertyName = propertyName, Order = SortOrder.Descending };
    }

    // Specialized specifications with domain-specific methods
    public class ArticleSpecification : ErpQuerySpecification<ArticleDto>
    {
        public ArticleSpecification WhereArticleId(int articleId)
        {
            AddFilter(ErpFilters.Equals(nameof(ArticleDto.ArticleId), articleId));
            return this;
        }

        public ArticleSpecification WhereNameContains(string name)
        {
            AddFilter(ErpFilters.Contains(nameof(ArticleDto.Name), name));
            return this;
        }

        public ArticleSpecification WhereArticleState(ArticleState state)
        {
            AddFilter(ErpFilters.Equals(nameof(ArticleDto.ArticleState), state));
            return this;
        }

        public ArticleSpecification WhereECommerceEnabled()
        {
            AddFilter(ErpFilters.Equals(nameof(ArticleDto.NoECommerce), false));
            return this;
        }

        public ArticleSpecification WhereECommerceDisabled()
        {
            AddFilter(ErpFilters.Equals(nameof(ArticleDto.NoECommerce), true));
            return this;
        }

        public ArticleSpecification WherePriceGreaterThan(decimal price)
        {
            AddFilter(ErpFilters.GreaterThan(nameof(ArticleDto.Price), price));
            return this;
        }

        public ArticleSpecification WhereStockGreaterThan(int stock)
        {
            AddFilter(ErpFilters.GreaterThan(nameof(ArticleDto.StockQuantity), stock));
            return this;
        }

        public ArticleSpecification OrderByName()
        {
            AddSorting(ErpSorting.Ascending(nameof(ArticleDto.Name)));
            return this;
        }

        public ArticleSpecification OrderByPriceDescending()
        {
            AddSorting(ErpSorting.Descending(nameof(ArticleDto.Price)));
            return this;
        }

        public ArticleSpecification Skip(int count)
        {
            SetSkip(count);
            return this;
        }

        public ArticleSpecification Take(int count)
        {
            SetTake(count);
            return this;
        }
    }

    public class CustomerSpecification : ErpQuerySpecification<CustomerDto>
    {
        public CustomerSpecification WhereCustomerNumber(string customerNumber)
        {
            AddFilter(ErpFilters.Equals(nameof(CustomerDto.CustomerNumber), customerNumber));
            return this;
        }

        public CustomerSpecification WhereCompanyNameContains(string companyName)
        {
            AddFilter(ErpFilters.Contains(nameof(CustomerDto.CompanyName), companyName));
            return this;
        }

        public CustomerSpecification WhereEmail(string email)
        {
            AddFilter(ErpFilters.Equals(nameof(CustomerDto.Email), email));
            return this;
        }

        public CustomerSpecification WhereCity(string city)
        {
            AddFilter(ErpFilters.Equals(nameof(CustomerDto.City), city));
            return this;
        }

        public CustomerSpecification WhereCountry(string country)
        {
            AddFilter(ErpFilters.Equals(nameof(CustomerDto.Country), country));
            return this;
        }

        public CustomerSpecification WhereActive()
        {
            AddFilter(ErpFilters.Equals(nameof(CustomerDto.IsActive), true));
            return this;
        }

        public CustomerSpecification OrderByCompanyName()
        {
            AddSorting(ErpSorting.Ascending(nameof(CustomerDto.CompanyName)));
            return this;
        }

        public CustomerSpecification OrderByCustomerNumber()
        {
            AddSorting(ErpSorting.Ascending(nameof(CustomerDto.CustomerNumber)));
            return this;
        }
    }

    public class OrderSpecification : ErpQuerySpecification<OrderDto>
    {
        public OrderSpecification WhereOrderNumber(string orderNumber)
        {
            AddFilter(ErpFilters.Equals(nameof(OrderDto.OrderNumber), orderNumber));
            return this;
        }

        public OrderSpecification WhereCustomerNumber(string customerNumber)
        {
            AddFilter(ErpFilters.Equals(nameof(OrderDto.CustomerNumber), customerNumber));
            return this;
        }

        public OrderSpecification WhereOrderDateAfter(DateTime date)
        {
            AddFilter(ErpFilters.GreaterThan(nameof(OrderDto.OrderDate), date));
            return this;
        }

        public OrderSpecification WhereOrderDateBefore(DateTime date)
        {
            AddFilter(ErpFilters.LessThan(nameof(OrderDto.OrderDate), date));
            return this;
        }

        public OrderSpecification WhereTotalAmountGreaterThan(decimal amount)
        {
            AddFilter(ErpFilters.GreaterThan(nameof(OrderDto.TotalAmount), amount));
            return this;
        }

        public OrderSpecification WhereStatus(OrderStatus status)
        {
            AddFilter(ErpFilters.Equals(nameof(OrderDto.Status), status));
            return this;
        }

        public OrderSpecification WhereStatusIn(params OrderStatus[] statuses)
        {
            AddFilter(ErpFilters.In(nameof(OrderDto.Status), statuses.Cast<object>()));
            return this;
        }

        public OrderSpecification OrderByOrderDateDescending()
        {
            AddSorting(ErpSorting.Descending(nameof(OrderDto.OrderDate)));
            return this;
        }

        public OrderSpecification OrderByTotalAmountDescending()
        {
            AddSorting(ErpSorting.Descending(nameof(OrderDto.TotalAmount)));
            return this;
        }
    }

    // Factory methods for easy creation
    public static class ErpQuery
    {
        public static ArticleSpecification ForArticles() => new();
        public static CustomerSpecification ForCustomers() => new();
        public static OrderSpecification ForOrders() => new();
    }
}
