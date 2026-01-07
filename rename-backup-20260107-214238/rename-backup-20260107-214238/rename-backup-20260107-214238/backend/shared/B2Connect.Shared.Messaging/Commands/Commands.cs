namespace B2X.Shared.Messaging.Commands;

/// <summary>
/// Command to index a product in the search engine
/// </summary>
public record IndexProductCommand(
    Guid TenantId,
    Guid ProductId,
    string ProductName,
    string Description,
    string Sku,
    decimal Price,
    Dictionary<string, object> Attributes);

/// <summary>
/// Command to remove a product from the search index
/// </summary>
public record RemoveProductFromIndexCommand(
    Guid TenantId,
    Guid ProductId);

/// <summary>
/// Command to reindex all products for a tenant
/// </summary>
public record ReindexAllProductsCommand(
    Guid TenantId);

/// <summary>
/// Command to send a notification email
/// </summary>
public record SendEmailCommand(
    Guid TenantId,
    string To,
    string Subject,
    string Body,
    bool IsHtml = true);

/// <summary>
/// Command to generate an invoice
/// </summary>
public record GenerateInvoiceCommand(
    Guid TenantId,
    Guid OrderId,
    Guid CustomerId);
