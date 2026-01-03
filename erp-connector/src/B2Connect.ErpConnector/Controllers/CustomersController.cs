using System.Threading.Tasks;
using System.Web.Http;
using B2Connect.ErpConnector.Models;
using B2Connect.ErpConnector.Services;
using NLog;

namespace B2Connect.ErpConnector.Controllers
{
    /// <summary>
    /// API endpoints for customer operations.
    /// </summary>
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EnventaErpService _erpService;

        public CustomersController()
        {
            _erpService = new EnventaErpService();
        }

        /// <summary>
        /// Get a single customer by customer number.
        /// </summary>
        [HttpGet]
        [Route("{customerNumber}")]
        public async Task<IHttpActionResult> GetCustomer(string customerNumber)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"GET /api/customers/{customerNumber} - tenant: {tenantId}, businessUnit: {businessUnit}");

            var customer = await _erpService.GetCustomerAsync(tenantId, customerNumber, businessUnit);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// Query customers with filters, sorting, and pagination.
        /// </summary>
        [HttpPost]
        [Route("query")]
        public async Task<IHttpActionResult> QueryCustomers([FromBody] QueryRequest request)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"POST /api/customers/query - tenant: {tenantId}, businessUnit: {businessUnit}");

            if (request == null)
            {
                request = new QueryRequest();
            }

            var result = await _erpService.QueryCustomersAsync(tenantId, request, businessUnit);
            return Ok(result);
        }

        private string GetTenantId()
        {
            if (Request.Headers.TryGetValues("X-Tenant-Id", out var values))
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }

            return "default";
        }

        private string? GetBusinessUnit()
        {
            // Get business unit from header or return null (uses default)
            if (Request.Headers.TryGetValues("X-Business-Unit", out var values))
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }

            return null; // Use default business unit
        }
    }
}
