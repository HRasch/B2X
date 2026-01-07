namespace B2Connect.Catalog.Models;

/// <summary>
/// Represents an available shipping method with cost information
/// </summary>
public class ShippingMethod
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty; // DHL, DPD, PostNL
    public decimal BaseCost { get; set; }
    public string Description { get; set; } = string.Empty;
    public int EstimatedDaysMin { get; set; }
    public int EstimatedDaysMax { get; set; }
    public decimal? MaxWeight { get; set; } // kg, null = unlimited
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Shipping method with country-specific cost
/// </summary>
public class ShippingMethodDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Description { get; set; } = string.Empty;
    public int EstimatedDaysMin { get; set; }
    public int EstimatedDaysMax { get; set; }
    public string CurrencyCode { get; set; } = "EUR";
}

/// <summary>
/// Request to calculate shipping cost
/// </summary>
public class GetShippingMethodsRequest
{
    public string DestinationCountry { get; set; } = string.Empty;
    public decimal? TotalWeight { get; set; }
    public decimal? OrderTotal { get; set; }
}

/// <summary>
/// Response with available shipping methods
/// </summary>
public class GetShippingMethodsResponse
{
    public bool Success { get; set; }
    public List<ShippingMethodDto> Methods { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
