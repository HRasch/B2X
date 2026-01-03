using System;
using System.Configuration;
using B2Connect.ErpConnector.Infrastructure;
using B2Connect.ErpConnector.Infrastructure.Erp;
using B2Connect.ErpConnector.Services;
using Microsoft.Owin.Hosting;
using NLog;

namespace B2Connect.ErpConnector
{
    /// <summary>
    /// Windows Service implementation using TopShelf.
    /// Hosts the OWIN Web API and manages ERP actor pool.
    /// 
    /// Infrastructure initialization order (based on eGate patterns):
    /// 1. EnventaGlobalFactory - connection pool management
    /// 2. EnventaActorPool - thread-safe actor pattern
    /// 3. OWIN Web API - HTTP endpoints
    /// </summary>
    public class ErpConnectorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IDisposable _webApp;

        public void Start()
        {
            Logger.Info("Starting ERP Connector Service...");

            try
            {
                // Initialize global factory with pool size from config
                // Based on FSGlobalFactory.Initialize() pattern from eGate
                var poolSize = int.Parse(ConfigurationManager.AppSettings["ErpPoolSize"] ?? "5");
                var globalObjectFactory = new MockEnventaGlobalObjectFactory();
                EnventaGlobalFactory.Initialize(globalObjectFactory, poolSize);
                Logger.Info("EnventaGlobalFactory initialized with pool size: {0}", poolSize);

                // Initialize actor pool (singleton)
                var actorPool = EnventaActorPool.Instance;
                Logger.Info("ERP Actor Pool initialized");

                // Get base address from config or use default
                var baseAddress = ConfigurationManager.AppSettings["BaseAddress"] ?? "http://localhost:5080";

                // Start OWIN self-host
                _webApp = WebApp.Start<Startup>(baseAddress);
                Logger.Info($"ERP Connector HTTP API started at {baseAddress}");

                // Log available endpoints
                Logger.Info("Available endpoints:");
                Logger.Info($"  GET  {baseAddress}/api/health");
                Logger.Info($"  GET  {baseAddress}/api/articles/{{id}}");
                Logger.Info($"  POST {baseAddress}/api/articles/query");
                Logger.Info($"  POST {baseAddress}/api/articles/sync");
                Logger.Info($"  GET  {baseAddress}/api/customers/{{id}}");
                Logger.Info($"  POST {baseAddress}/api/customers/query");
                Logger.Info($"  GET  {baseAddress}/api/orders/{{id}}");
                Logger.Info($"  POST {baseAddress}/api/orders");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to start ERP Connector service");
                throw;
            }
        }

        public void Stop()
        {
            Logger.Info("Stopping ERP Connector Service...");

            try
            {
                // Dispose web app
                _webApp?.Dispose();
                _webApp = null;

                // Dispose actor pool
                EnventaActorPool.Instance.Dispose();

                // Dispose global factory (closes all connections and pools)
                // Based on FSGlobalFactory.Dispose() pattern from eGate
                EnventaGlobalFactory.Dispose();
                Logger.Info("EnventaGlobalFactory disposed");

                Logger.Info("ERP Connector Service stopped successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error during service shutdown");
            }
        }
    }
}
