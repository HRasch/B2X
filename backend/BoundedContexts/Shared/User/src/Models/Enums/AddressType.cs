namespace B2Connect.Shared.User.Models.Enums;

/// <summary>
/// Address type enumeration for customer addresses
/// </summary>
public enum AddressType
{
    /// <summary>Shipping/Delivery address</summary>
    Shipping = 0,

    /// <summary>Billing address</summary>
    Billing = 1,

    /// <summary>Both shipping and billing address</summary>
    Both = 2,

    /// <summary>Residential address</summary>
    Residential = 3,

    /// <summary>Commercial/Business address</summary>
    Commercial = 4,

    /// <summary>Other type</summary>
    Other = 5
}
