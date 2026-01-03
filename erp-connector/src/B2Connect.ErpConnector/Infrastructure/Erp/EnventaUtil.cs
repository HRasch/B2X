namespace B2Connect.ErpConnector.Infrastructure.Erp
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using B2Connect.ErpConnector.Infrastructure.Identity;
    using NLog;

    /// <summary>
    /// Utility class for common enventa operations.
    /// Based on FSUtil from eGate.
    /// 
    /// Provides:
    /// - Scope creation for ERP operations
    /// - Query building helpers
    /// - Branch/business unit filtering
    /// - File operations
    /// </summary>
    public class EnventaUtil : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly IEnventaIdentityProvider _provider;
        private bool _disposed;

        public EnventaUtil(IEnventaIdentityProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Creates a new scope for ERP operations.
        /// </summary>
        public EnventaScope Scope()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EnventaUtil));

            return new EnventaScope(_provider.Get);
        }

        /// <summary>
        /// Creates a component within a new scope.
        /// </summary>
        public T Create<T>() where T : class, IDevFrameworkObject
        {
            using (var scope = Scope())
            {
                return scope.Create<T>();
            }
        }

        /// <summary>
        /// Creates a component within an existing scope.
        /// </summary>
        public T Create<T>(EnventaScope scope) where T : class, IDevFrameworkObject
        {
            return scope.Create<T>();
        }

        /// <summary>
        /// Gets the WHERE clause for branch/business unit filtering.
        /// </summary>
        public string GetWhereBranch<TFSEntity>(TFSEntity entity, string prefix = "AND", string alias = null, bool native = false)
            where TFSEntity : class, IDevFrameworkDataObject
        {
            // In production, this would call:
            // scope.FSGlobalContext.FSGlobal.ocGlobal.oBusinessUnit.ExtendLoadCondition(ref whereBranch, entity);
            // 
            // For now, return empty (no branch filtering in mock)
            return string.Empty;
        }

        /// <summary>
        /// Gets the WHERE clause for branch filtering by table name.
        /// </summary>
        public string GetWhereBranch(string tableName, string prefix = "AND", string alias = null, bool native = false)
        {
            // In production, this would build the branch filter
            return string.Empty;
        }

        /// <summary>
        /// Gets the MIME type for a file path.
        /// </summary>
        public string GetMimeType(string path)
        {
            using (var scope = Scope())
            {
                // In production:
                // var fileManager = scope.GlobalContext.FSGlobal.ocGlobal.oFileManager;
                // return fileManager.GetMimeTypeByFileName(path);

                // Mock implementation based on extension
                var ext = Path.GetExtension(path)?.ToLowerInvariant();
                return ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".xml" => "application/xml",
                    ".json" => "application/json",
                    _ => "application/octet-stream"
                };
            }
        }

        /// <summary>
        /// Checks if a path is a server file path.
        /// </summary>
        public bool IsServerFile(string path)
        {
            // In production, checks against server file paths
            return !string.IsNullOrEmpty(path) &&
                   (path.StartsWith("\\\\") || path.StartsWith("//"));
        }

        /// <summary>
        /// Gets the bytes of a server file.
        /// </summary>
        public byte[] GetServerFileBytes(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            // In production, uses enventa file manager
            // For mock, try local file system
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        /// <summary>
        /// Authenticates an identity against enventa.
        /// </summary>
        public static bool Authenticate(EnventaIdentity identity)
        {
            try
            {
                var context = EnventaGlobalFactory.Get(identity);
                if (context != null)
                {
                    identity.IsAuthenticated = true;
                    EnventaGlobalFactory.Put(context);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Warn(ex, "Authentication failed for {0}: {1}", identity?.Name, ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            EnventaGlobalFactory.Dispose();
        }
    }
}
