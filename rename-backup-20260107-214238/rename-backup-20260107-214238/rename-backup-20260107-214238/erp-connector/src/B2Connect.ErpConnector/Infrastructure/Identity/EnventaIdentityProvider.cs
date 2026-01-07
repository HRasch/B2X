namespace B2X.ErpConnector.Infrastructure.Identity
{
    using System;

    /// <summary>
    /// Provider for the current identity context.
    /// Allows different identity sources (HTTP header, configuration, etc.)
    /// </summary>
    public interface IEnventaIdentityProvider
    {
        /// <summary>
        /// Gets the current identity for ERP operations.
        /// </summary>
        EnventaIdentity Get { get; }

        /// <summary>
        /// Gets identity for specific business unit.
        /// </summary>
        /// <param name="businessUnit">Business unit identifier</param>
        EnventaIdentity GetForBusinessUnit(string businessUnit);
    }

    /// <summary>
    /// Identity provider that uses a fixed configured identity.
    /// </summary>
    public class ConfiguredIdentityProvider : IEnventaIdentityProvider
    {
        private readonly EnventaIdentity _identity;

        public ConfiguredIdentityProvider(string name, string password, string businessUnit)
        {
            _identity = new EnventaIdentity(name, password, businessUnit);
        }

        public EnventaIdentity Get => _identity;

        public EnventaIdentity GetForBusinessUnit(string businessUnit)
        {
            return new EnventaIdentity(_identity.Name, _identity.Password, businessUnit);
        }
    }

    /// <summary>
    /// Identity provider that extracts identity from HTTP context (tenant header).
    /// </summary>
    public class HttpContextIdentityProvider : IEnventaIdentityProvider
    {
        private readonly Func<EnventaIdentity> _identityFactory;

        public HttpContextIdentityProvider(Func<EnventaIdentity> identityFactory)
        {
            _identityFactory = identityFactory ?? throw new ArgumentNullException(nameof(identityFactory));
        }

        public EnventaIdentity Get => _identityFactory();

        public EnventaIdentity GetForBusinessUnit(string businessUnit)
        {
            var baseIdentity = _identityFactory();
            return new EnventaIdentity(baseIdentity.Name, baseIdentity.Password, businessUnit);
        }
    }
}
