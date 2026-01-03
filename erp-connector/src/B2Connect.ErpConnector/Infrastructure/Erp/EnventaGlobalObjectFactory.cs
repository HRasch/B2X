namespace B2Connect.ErpConnector.Infrastructure.Erp
{
    using System;
    using B2Connect.ErpConnector.Infrastructure.Identity;

    /// <summary>
    /// Factory for creating and managing enventa global objects.
    /// Abstracts the actual enventa framework integration.
    /// </summary>
    public interface IEnventaGlobalObjectFactory
    {
        /// <summary>
        /// Creates a new global object instance.
        /// In production: GlobalObjectManager.CreateGlobalObject(globalId) as IFSGlobalObjects
        /// </summary>
        IFSGlobalObjects CreateGlobalObject(Guid globalId);

        /// <summary>
        /// Authenticates a global object with the given identity.
        /// In production: Uses cLoginFactory.Create() and LoginServiceHost()
        /// </summary>
        void Login(IFSGlobalObjects global, EnventaIdentity identity);

        /// <summary>
        /// Validates the global object state.
        /// In production: Checks global.oMsg.oMsgItemColl for errors
        /// </summary>
        void Validate(IFSGlobalObjects global);

        /// <summary>
        /// Enables caching on the global object.
        /// In production: Calls EnableCaching() on the global
        /// </summary>
        void EnableCaching(IFSGlobalObjects global);
    }

    /// <summary>
    /// Mock implementation for development/testing.
    /// Replace with actual enventa integration in production.
    /// </summary>
    public class MockEnventaGlobalObjectFactory : IEnventaGlobalObjectFactory
    {
        public IFSGlobalObjects CreateGlobalObject(Guid globalId)
        {
            return new MockFSGlobalObjects(globalId);
        }

        public void Login(IFSGlobalObjects global, EnventaIdentity identity)
        {
            // Mock: Always succeed
            identity.IsAuthenticated = true;
        }

        public void Validate(IFSGlobalObjects global)
        {
            // Mock: Always valid
            if (global.Message?.Level == 3)
            {
                throw new InvalidOperationException($"Global validation failed: {global.Message.Text}");
            }
        }

        public void EnableCaching(IFSGlobalObjects global)
        {
            // Mock: No-op
        }
    }

    /// <summary>
    /// Mock implementation of IFSGlobalObjects for development.
    /// </summary>
    internal class MockFSGlobalObjects : IFSGlobalObjects
    {
        private readonly Guid _id;
        private bool _disposed;

        public MockFSGlobalObjects(Guid id)
        {
            _id = id;
        }

        public T CreateComponent<T>() where T : class, IDevFrameworkObject
        {
            // In production: return Global.CreateComponent(componentName) as T
            return default(T);
        }

        public void CloseConnection()
        {
            // Mock: No-op
        }

        public IFSMessage Message => new MockFSMessage();
        public IFSGlobalContext GlobalContext => null;

        public void Dispose()
        {
            _disposed = true;
        }
    }

    internal class MockFSMessage : IFSMessage
    {
        public int Level => 0; // 0 = OK, 3 = Error
        public string Text => string.Empty;
    }
}
