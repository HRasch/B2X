namespace B2X.ErpConnector.Infrastructure.Erp
{
    using System;
    using B2X.ErpConnector.Infrastructure.Identity;

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
        /// Validates the login status immediately after login.
        /// In production: Checks global.oMsg for login-specific errors (Level 3 = error)
        /// Must be called right after Login() to catch authentication failures.
        /// </summary>
        void ValidateLoginStatus(IFSGlobalObjects global, EnventaIdentity identity);

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

        public void ValidateLoginStatus(IFSGlobalObjects global, EnventaIdentity identity)
        {
            // Check login result from global message collection (oMsg.oMsgItemColl)
            // enventa logs initialization errors to this collection
            var msg = global.Message;
            if (msg == null)
            {
                throw new InvalidOperationException(
                    $"Login validation failed for {identity.Name}: Message object is null");
            }

            // Check message level (3 = Error)
            if (msg.Level >= 3 || msg.HasErrors)
            {
                identity.IsAuthenticated = false;

                // Collect all error messages from oMsgItemColl
                var errorText = msg.Text;
                if (msg.Items != null && msg.Items.Count > 0)
                {
                    var errors = new System.Text.StringBuilder();
                    for (int i = 0; i < msg.Items.Count; i++)
                    {
                        var item = msg.Items[i];
                        if (item.Level >= 3) // Error level
                        {
                            if (errors.Length > 0) errors.Append("; ");
                            errors.Append($"[{item.Code}] {item.Text}");
                        }
                    }
                    if (errors.Length > 0)
                    {
                        errorText = errors.ToString();
                    }
                }

                throw new UnauthorizedAccessException(
                    $"Login failed for {identity.Name}: {errorText}");
            }

            // Verify identity was authenticated
            if (!identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException(
                    $"Login failed for {identity.Name}: Identity not authenticated after login");
            }
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
        public IFSMessageItemCollection Items => new MockFSMessageItemCollection();
        public bool HasErrors => Level >= 3 || (Items?.Count > 0 && HasErrorItems());

        private bool HasErrorItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Level >= 3) return true;
            }
            return false;
        }
    }

    internal class MockFSMessageItemCollection : IFSMessageItemCollection
    {
        private readonly IFSMessageItem[] _items = Array.Empty<IFSMessageItem>();

        public int Count => _items.Length;
        public IFSMessageItem this[int index] => _items[index];
    }

    internal class MockFSMessageItem : IFSMessageItem
    {
        public int Level { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
