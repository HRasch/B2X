namespace B2X.Shared.Core.Authorization;

/// <summary>
/// Centralized permission constants for the application.
/// Organized by feature area for easy discovery and maintenance.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Store access and visibility permissions.
    /// </summary>
    public static class Store
    {
        /// <summary>
        /// Permission to access the store frontend.
        /// </summary>
        public const string Access = "Store.Access";

        /// <summary>
        /// Permission to browse products without authentication.
        /// </summary>
        public const string BrowseAnonymous = "Store.BrowseAnonymous";

        /// <summary>
        /// Permission to view product prices.
        /// </summary>
        public const string ViewPrices = "Store.ViewPrices";

        /// <summary>
        /// Permission to add items to cart.
        /// </summary>
        public const string AddToCart = "Store.AddToCart";

        /// <summary>
        /// Permission to checkout as guest.
        /// </summary>
        public const string GuestCheckout = "Store.GuestCheckout";
    }

    /// <summary>
    /// Account-related permissions.
    /// </summary>
    public static class Account
    {
        /// <summary>
        /// Permission to manage own addresses.
        /// </summary>
        public const string ManageAddresses = "Account.ManageAddresses";

        /// <summary>
        /// Permission to select shipping address.
        /// </summary>
        public const string SelectShippingAddress = "Account.SelectShippingAddress";

        /// <summary>
        /// Permission to select invoice address.
        /// </summary>
        public const string SelectInvoiceAddress = "Account.SelectInvoiceAddress";

        /// <summary>
        /// Permission to manage sub-accounts (master account feature).
        /// </summary>
        public const string ManageSubAccounts = "Account.ManageSubAccounts";
    }

    /// <summary>
    /// Order-related permissions.
    /// </summary>
    public static class Order
    {
        /// <summary>
        /// Permission to view order history.
        /// </summary>
        public const string ViewOrders = "Order.ViewOrders";

        /// <summary>
        /// Permission to place orders.
        /// </summary>
        public const string PlaceOrder = "Order.PlaceOrder";

        /// <summary>
        /// Permission to approve orders (for approval workflows).
        /// </summary>
        public const string ApproveOrders = "Order.ApproveOrders";

        /// <summary>
        /// Permission to view purchase information.
        /// </summary>
        public const string ViewPurchaseInfo = "Order.ViewPurchaseInfo";
    }

    /// <summary>
    /// Cart-related permissions.
    /// </summary>
    public static class Cart
    {
        /// <summary>
        /// Permission to approve cart (for approval workflows).
        /// </summary>
        public const string Approve = "Cart.Approve";

        /// <summary>
        /// Permission to modify cart contents.
        /// </summary>
        public const string Modify = "Cart.Modify";
    }

    /// <summary>
    /// Catalog-related permissions.
    /// </summary>
    public static class Catalog
    {
        /// <summary>
        /// Permission to view product prices.
        /// </summary>
        public const string ShowPrices = "Catalog.ShowPrices";

        /// <summary>
        /// Permission to view product stock levels.
        /// </summary>
        public const string ShowStock = "Catalog.ShowStock";

        /// <summary>
        /// Permission to view product availability.
        /// </summary>
        public const string ShowAvailability = "Catalog.ShowAvailability";
    }

    /// <summary>
    /// Integration-specific permissions (OCI, cXML, IDS Connect).
    /// </summary>
    public static class Integration
    {
        /// <summary>
        /// Permission for OCI punchout access.
        /// </summary>
        public const string OciAccess = "Integration.OCI.Access";

        /// <summary>
        /// Permission for cXML punchout access.
        /// </summary>
        public const string CxmlAccess = "Integration.CXML.Access";

        /// <summary>
        /// Permission for IDS Connect access.
        /// </summary>
        public const string IdsConnectAccess = "Integration.IDSConnect.Access";
    }
}
