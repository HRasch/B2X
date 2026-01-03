namespace B2Connect.ErpConnector.Infrastructure.Erp
{
    using System;
    using B2Connect.ErpConnector.Infrastructure.Identity;

    // NOTE: These interfaces represent enventa ERP framework types.
    // In production, these would be replaced with actual enventa assembly references:
    // - FS.Hosting.Shared.IDevFrameworkObject
    // - FS.Hosting.Shared.IDevFrameworkDataObject
    // - FSGeneral.GlobalObjects.IFSGlobalObjects
    // - NV.ERP.MM.ECommerce.ECComponents.IcECArticle
    // etc.

    /// <summary>
    /// Base interface for enventa framework objects.
    /// Placeholder for FS.Hosting.Shared.IDevFrameworkObject
    /// </summary>
    public interface IDevFrameworkObject
    {
    }

    /// <summary>
    /// Interface for enventa data objects with ROWID/ROWVERSION.
    /// Placeholder for FS.Hosting.Shared.IDevFrameworkDataObject
    /// </summary>
    public interface IDevFrameworkDataObject : IDevFrameworkObject
    {
        string ROWID { get; }
        long ROWVERSION { get; }
    }

    /// <summary>
    /// Interface representing enventa's global objects context.
    /// Placeholder for FSGeneral.GlobalObjects.IFSGlobalObjects
    /// </summary>
    public interface IFSGlobalObjects : IDisposable
    {
        /// <summary>
        /// Creates a component instance.
        /// </summary>
        T CreateComponent<T>() where T : class, IDevFrameworkObject;

        /// <summary>
        /// Closes the database connection.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Gets the message/error state.
        /// </summary>
        IFSMessage Message { get; }

        /// <summary>
        /// Gets the global context object.
        /// </summary>
        IFSGlobalContext GlobalContext { get; }
    }

    /// <summary>
    /// Message interface for error handling.
    /// </summary>
    public interface IFSMessage
    {
        int Level { get; }
        string Text { get; }
    }

    /// <summary>
    /// Global context interface.
    /// </summary>
    public interface IFSGlobalContext
    {
        // Business unit context, file manager, etc.
    }

    /// <summary>
    /// Context wrapper around IFSGlobalObjects with identity.
    /// Based on FSGlobalContext from eGate.
    /// </summary>
    public class EnventaGlobalContext
    {
        public EnventaGlobalContext(EnventaIdentity identity, IFSGlobalObjects global)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            FSGlobal = global ?? throw new ArgumentNullException(nameof(global));
        }

        /// <summary>
        /// The identity used for this context.
        /// </summary>
        public EnventaIdentity Identity { get; }

        /// <summary>
        /// The underlying enventa global objects.
        /// </summary>
        internal IFSGlobalObjects FSGlobal { get; }

        /// <summary>
        /// Creates an enventa component.
        /// </summary>
        internal T Create<T>() where T : class, IDevFrameworkObject
        {
            return FSGlobal.CreateComponent<T>();
        }

        /// <summary>
        /// Closes the database connection.
        /// </summary>
        internal void CloseConnection()
        {
            FSGlobal.CloseConnection();
        }
    }
}
