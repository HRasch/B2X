using Wolverine;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.CQRS.Commands;

/// <summary>
/// Create a new product
/// This command is synchronous and returns immediately with the created product ID
/// </summary>
public class CreateProductCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; } = 0;
    public string[]? CategoryIds { get; set; }
    public Dictionary<string, string>? LocalizedData { get; set; }
}

/// <summary>
/// Update an existing product
/// </summary>
public class UpdateProductCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Delete a product
/// </summary>
public class DeleteProductCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
}

/// <summary>
/// Bulk import products from CSV/data
/// Asynchronous operation that processes in the background
/// </summary>
public class BulkImportProductsCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public required ProductImportData[] Products { get; set; }
    public string? ImportId { get; set; } = Guid.NewGuid().ToString();
}

public class ProductImportData
{
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
}
