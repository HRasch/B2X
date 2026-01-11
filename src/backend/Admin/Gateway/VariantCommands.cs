namespace B2X.Admin.Application.Commands.Variants;

/// <summary>
/// Query to get a variant by ID
/// </summary>
public record GetVariantByIdQuery(Guid Id, Guid TenantId);

/// <summary>
/// Query to get a variant by SKU
/// </summary>
public record GetVariantBySkuQuery(string Sku, Guid TenantId);

/// <summary>
/// Query to get variants by product with pagination
/// </summary>
public record GetVariantsByProductQuery(Guid ProductId, Guid TenantId, int Page = 1, int PageSize = 20);

/// <summary>
/// Query to search variants with pagination
/// </summary>
public record SearchVariantsQuery(string SearchTerm, Guid TenantId, int Page = 1, int PageSize = 20);

/// <summary>
/// Query to get all variants with pagination
/// </summary>
public record GetAllVariantsQuery(Guid TenantId, int Page = 1, int PageSize = 20);
