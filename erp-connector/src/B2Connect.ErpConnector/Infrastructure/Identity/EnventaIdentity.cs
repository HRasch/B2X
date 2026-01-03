namespace B2Connect.ErpConnector.Infrastructure.Identity
{
    using System;

    /// <summary>
    /// Represents an enventa ERP identity for authentication.
    /// Based on NVIdentity pattern from eGate.
    /// </summary>
    public class EnventaIdentity
    {
        public EnventaIdentity(string name, string password, string businessUnit)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            Name = name;
            Password = password ?? string.Empty;
            BusinessUnit = businessUnit ?? "10"; // Default business unit
        }

        /// <summary>
        /// Username for ERP authentication.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Password for ERP authentication.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Business unit / tenant identifier.
        /// </summary>
        public string BusinessUnit { get; }

        /// <summary>
        /// Whether authentication was successful.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets a unique token for pool identification.
        /// Used to maintain separate connection pools per identity.
        /// NOTE: Does not include password for security - use GetToken() for pool keys only.
        /// </summary>
        public string GetToken()
        {
            return $"{Name}:{BusinessUnit}".ToLowerInvariant();
        }

        /// <summary>
        /// String representation - NEVER includes password for security.
        /// </summary>
        public override string ToString()
        {
            return $"EnventaIdentity[{Name}@{BusinessUnit}]";
        }

        /// <summary>
        /// Check if this identity has valid credentials set.
        /// Does NOT expose the actual password value.
        /// </summary>
        public bool HasCredentials => !string.IsNullOrEmpty(Password);
    }
}
