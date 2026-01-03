using System;
using System.Web.Http;
using B2Connect.ErpConnector.Models;

namespace B2Connect.ErpConnector.Controllers
{
    /// <summary>
    /// Health check endpoint for the ERP Connector.
    /// </summary>
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        /// <summary>
        /// Get connector health status.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var response = new ConnectorHealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                Services =
                {
                    ["ActorPool"] = "Running",
                    ["WebApi"] = "Running",
                    // TODO: Add actual ERP connection status
                    ["ErpConnection"] = "Mock"
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Simple ping endpoint for load balancers.
        /// </summary>
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
