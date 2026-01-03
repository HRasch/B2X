using System.Threading.Tasks;
using System.Web.Http;
using B2Connect.ErpConnector.Models;
using B2Connect.ErpConnector.Services;
using NLog;

namespace B2Connect.ErpConnector.Controllers
{
    /// <summary>
    /// API endpoints for order operations.
    /// </summary>
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EnventaErpService _erpService;

        public OrdersController()
        {
            _erpService = new EnventaErpService();
        }

        /// <summary>
        /// Get a single order by order number.
        /// </summary>
        [HttpGet]
        [Route("{orderNumber}")]
        public async Task<IHttpActionResult> GetOrder(string orderNumber)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"GET /api/orders/{orderNumber} - tenant: {tenantId}, businessUnit: {businessUnit}");

            var order = await _erpService.GetOrderAsync(tenantId, orderNumber, businessUnit);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        /// <summary>
        /// Create a new order.
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Info($"POST /api/orders - tenant: {tenantId}, businessUnit: {businessUnit}, customer: {request?.CustomerNumber}");

            if (request == null)
            {
                return BadRequest("Request body is required");
            }

            if (string.IsNullOrEmpty(request.CustomerNumber))
            {
                return BadRequest("CustomerNumber is required");
            }

            var order = await _erpService.CreateOrderAsync(tenantId, request, businessUnit);
            return Created($"api/orders/{order.OrderNumber}", order);
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
