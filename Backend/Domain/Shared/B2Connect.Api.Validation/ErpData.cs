namespace B2Connect.Api.Models.Erp;

public class ErpData
{
    public string Id { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ErpType { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
}