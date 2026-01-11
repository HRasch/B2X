using System;
using System.Collections.Generic;
using System.Linq;
using B2X.ErpConnector.Infrastructure.Erp;
using B2X.ErpConnector.Models;

namespace B2X.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Customer query builder with type-safe filter methods.
    /// </summary>
    public class CustomerQueryBuilder : EnventaQueryBuilderBase<CustomerDto>
    {
        public CustomerQueryBuilder(IEnventaSelectRepository<CustomerDto> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// Filter by customer number.
        /// </summary>
        public CustomerQueryBuilder WithCustomerNumber(string customerNumber)
        {
            AddWhere($"CustomerNumber = {EscapeString(customerNumber)}");
            return this;
        }

        /// <summary>
        /// Filter by company name (contains).
        /// </summary>
        public CustomerQueryBuilder WithCompanyNameContains(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                AddWhere($"CompanyName LIKE '%{name.Replace("'", "''")}%'");
            }
            return this;
        }

        /// <summary>
        /// Filter by country code.
        /// </summary>
        public CustomerQueryBuilder WithCountry(string countryCode)
        {
            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                AddWhere($"Country = {EscapeString(countryCode)}");
            }
            return this;
        }

        /// <summary>
        /// Filter by city.
        /// </summary>
        public CustomerQueryBuilder WithCity(string city)
        {
            if (!string.IsNullOrWhiteSpace(city))
            {
                AddWhere($"City = {EscapeString(city)}");
            }
            return this;
        }

        /// <summary>
        /// Filter active customers only.
        /// </summary>
        public CustomerQueryBuilder ActiveOnly()
        {
            AddWhere("IsActive = 1");
            return this;
        }

        /// <summary>
        /// Filter by email domain.
        /// </summary>
        public CustomerQueryBuilder WithEmailDomain(string domain)
        {
            if (!string.IsNullOrWhiteSpace(domain))
            {
                AddWhere($"Email LIKE '%@{domain.Replace("'", "''")}'");
            }
            return this;
        }

        /// <summary>
        /// Order by company name.
        /// </summary>
        public CustomerQueryBuilder OrderByCompanyName()
        {
            OrderByClause = "CompanyName ASC";
            return this;
        }

        /// <summary>
        /// Order by customer number.
        /// </summary>
        public CustomerQueryBuilder OrderByCustomerNumber()
        {
            OrderByClause = "CustomerNumber ASC";
            return this;
        }
    }

    /// <summary>
    /// Customer repository implementation.
    /// </summary>
    public class EnventaCustomerRepository : EnventaBaseRepository<CustomerDto, IDevFrameworkDataObject>,
        IEnventaQueryRepository<CustomerDto, CustomerQueryBuilder>
    {
        public EnventaCustomerRepository(EnventaScope scope) : base(scope)
        {
        }

        /// <summary>
        /// Get type-safe query builder.
        /// </summary>
        public CustomerQueryBuilder Query()
        {
            return new CustomerQueryBuilder(this);
        }

        #region Mapping Implementation

        protected override CustomerDto ToDto(IDevFrameworkDataObject entity)
        {
            // In production with actual enventa assemblies:
            // var customer = entity as IcECCustomer;
            // return new CustomerDto { ... };
            return null;
        }

        protected override IDevFrameworkDataObject ToEntity(CustomerDto dto)
        {
            throw new NotImplementedException("Mock implementation");
        }

        protected override IDevFrameworkDataObject LoadEntity(string key)
        {
            return null;
        }

        protected override IEnumerable<IDevFrameworkDataObject> GetEntities(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null)
        {
            return Enumerable.Empty<IDevFrameworkDataObject>();
        }

        protected override int CountEntities(string where = "")
        {
            return 20;
        }

        protected override bool ExistsEntity(string where = "")
        {
            return true;
        }

        #endregion

        #region Override for Mock Data

        public override CustomerDto Find(string key)
        {
            Console.WriteLine($"Find Customer by key: {key}");

            return new CustomerDto
            {
                CustomerNumber = key,
                CompanyName = $"Company {key}",
                Email = $"contact@{key.ToLowerInvariant()}.example.com",
                City = "Stuttgart",
                Country = "DE",
                IsActive = true
            };
        }

        public override IEnumerable<CustomerDto> Select(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            System.Threading.CancellationToken ct = default)
        {
            Console.WriteLine($"Select Customers: where='{where}', orderBy='{orderBy}'");

            var customers = new List<CustomerDto>();
            for (int i = 1; i <= 20; i++)
            {
                customers.Add(new CustomerDto
                {
                    CustomerNumber = $"CUST{i:D4}",
                    CompanyName = $"Company {i}",
                    Email = $"contact{i}@example.com",
                    City = i % 2 == 0 ? "Stuttgart" : "München",
                    Country = "DE",
                    IsActive = i % 5 != 0
                });
            }

            var result = customers.AsEnumerable();
            if (offset.HasValue)
                result = result.Skip(offset.Value);
            if (limit.HasValue)
                result = result.Take(limit.Value);

            return result.ToList();
        }

        #endregion
    }
}
