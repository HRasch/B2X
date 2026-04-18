using System;
using B2X.ErpConnector.Infrastructure.Identity;

namespace B2X.ErpConnector.Infrastructure.Erp
{
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
    /// Represents enventa's oMsg with oMsgItemColl.
    /// </summary>
    public interface IFSMessage
    {
        /// <summary>
        /// Highest error level: 0 = OK, 1 = Info, 2 = Warning, 3 = Error
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Summary text of the message.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Collection of message items (oMsgItemColl).
        /// Contains detailed error/warning/info messages.
        /// </summary>
        IFSMessageItemCollection Items { get; }

        /// <summary>
        /// Checks if there are any error-level messages.
        /// </summary>
        bool HasErrors { get; }
    }

    /// <summary>
    /// Message item collection interface.
    /// Represents enventa's oMsgItemColl.
    /// </summary>
    public interface IFSMessageItemCollection
    {
        int Count { get; }
        IFSMessageItem this[int index] { get; }
    }

    /// <summary>
    /// Individual message item.
    /// Represents an entry in oMsgItemColl.
    /// </summary>
    public interface IFSMessageItem
    {
        /// <summary>
        /// Message level: 0 = OK, 1 = Info, 2 = Warning, 3 = Error
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Message code/ID for programmatic handling.
        /// </summary>
        string Code { get; }
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
