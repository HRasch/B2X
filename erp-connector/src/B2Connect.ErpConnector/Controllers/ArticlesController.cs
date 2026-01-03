using System.Threading.Tasks;
using System.Web.Http;
using B2Connect.ErpConnector.Models;
using B2Connect.ErpConnector.Services;
using NLog;

namespace B2Connect.ErpConnector.Controllers
{
    /// <summary>
    /// API endpoints for article operations.
    /// </summary>
    [RoutePrefix("api/articles")]
    public class ArticlesController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EnventaErpService _erpService;

        public ArticlesController()
        {
            _erpService = new EnventaErpService();
        }

        /// <summary>
        /// Get a single article by ID.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetArticle(string id)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"GET /api/articles/{id} - tenant: {tenantId}, businessUnit: {businessUnit}");

            var article = await _erpService.GetArticleAsync(tenantId, id, businessUnit);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }

        /// <summary>
        /// Query articles with filters, sorting, and pagination.
        /// </summary>
        [HttpPost]
        [Route("query")]
        public async Task<IHttpActionResult> QueryArticles([FromBody] QueryRequest request)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"POST /api/articles/query - tenant: {tenantId}, businessUnit: {businessUnit}");

            if (request == null)
            {
                request = new QueryRequest();
            }

            var result = await _erpService.QueryArticlesAsync(tenantId, request, businessUnit);
            return Ok(result);
        }

        /// <summary>
        /// Delta sync articles - get changes since last sync.
        /// </summary>
        [HttpPost]
        [Route("sync")]
        public async Task<IHttpActionResult> SyncArticles([FromBody] DeltaSyncRequest request)
        {
            var tenantId = GetTenantId();
            var businessUnit = GetBusinessUnit();
            Logger.Debug($"POST /api/articles/sync - tenant: {tenantId}, businessUnit: {businessUnit}, since: {request?.Since}");

            if (request == null)
            {
                request = new DeltaSyncRequest();
            }

            var result = await _erpService.SyncArticlesAsync(tenantId, request, businessUnit);
            return Ok(result);
        }

        /// <summary>
        /// Stream articles (for large datasets).
        /// </summary>
        [HttpGet]
        [Route("stream")]
        public async Task<IHttpActionResult> StreamArticles([FromUri] int? pageSize = 100, [FromUri] string? cursor = null)
        {
            var tenantId = GetTenantId();
            Logger.Debug($"GET /api/articles/stream - tenant: {tenantId}, pageSize: {pageSize}");

            var request = new QueryRequest
            {
                Take = pageSize ?? 100
            };

            // Decode cursor if provided
            if (!string.IsNullOrEmpty(cursor))
            {
                try
                {
                    var skip = System.BitConverter.ToInt32(System.Convert.FromBase64String(cursor), 0);
                    request.Skip = skip;
                }
                catch
                {
                    // Invalid cursor, start from beginning
                }
            }

            var result = await _erpService.QueryArticlesAsync(tenantId, request);
            return Ok(result);
        }

        private string GetTenantId()
        {
            // Get tenant ID from header or default
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
