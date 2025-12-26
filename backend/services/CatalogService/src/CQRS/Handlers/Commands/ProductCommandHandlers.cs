using WolverineFx;
using FluentValidation;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS.Events;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Repositories;

namespace B2Connect.CatalogService.CQRS.Handlers.Commands;

/// <summary>
/// Handler for CreateProductCommand
/// Wolverine automatically discovers this because:
/// 1. Class name ends with "Handler"
/// 2. Implements ICommandHandler<CreateProductCommand>
/// 3. Has async Handle() method
///
/// Wolverine will:
/// - Run FluentValidation automatically
/// - Invoke this handler synchronously
/// - Return CommandResult immediately
/// - Then publish ProductCreatedEvent asynchronously
/// </summary>
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly IMessageBus _messageBus;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        CatalogDbContext context,
        IProductRepository productRepository,
        IMessageBus messageBus,
        IValidator<CreateProductCommand> validator,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _productRepository = productRepository;
        _messageBus = messageBus;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Creating product {Sku} for tenant {TenantId}",
                command.Sku, command.TenantId);

            // Validation is run automatically by Wolverine
            // If validation fails, an exception is thrown
            // and handled by Wolverine's error policies

            // Create product entity
            var product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = command.TenantId,
                Sku = command.Sku,
                Name = command.Name,
                Price = command.Price,
                Description = command.Description,
                StockQuantity = command.StockQuantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish domain event asynchronously
            // Wolverine uses transactional outbox to guarantee delivery
            var @event = new ProductCreatedEvent
            {
                ProductId = product.Id,
                TenantId = command.TenantId,
                Sku = command.Sku,
                Name = command.Name,
                Price = command.Price,
                Description = command.Description,
                StockQuantity = command.StockQuantity
            };

            await _messageBus.PublishAsync(@event, cancellation: cancellationToken);

            _logger.LogInformation(
                "Product created successfully: {ProductId} ({Sku})",
                product.Id, command.Sku);

            return CommandResult.Ok(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product {Sku}", command.Sku);
            throw;  // Let Wolverine's error handling take over
        }
    }
}

/// <summary>
/// Handler for UpdateProductCommand
/// </summary>
public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IMessageBus messageBus,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(
                command.TenantId, command.ProductId, cancellationToken);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found: {ProductId} (Tenant: {TenantId})",
                    command.ProductId, command.TenantId);

                return CommandResult.Fail("Product not found");
            }

            // Update fields
            if (!string.IsNullOrEmpty(command.Name))
                product.Name = command.Name;

            if (command.Price.HasValue)
                product.Price = command.Price.Value;

            if (!string.IsNullOrEmpty(command.Description))
                product.Description = command.Description;

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product, cancellationToken);

            // Publish event
            var @event = new ProductUpdatedEvent
            {
                ProductId = product.Id,
                TenantId = command.TenantId,
                Name = command.Name,
                Price = command.Price,
                Description = command.Description
            };

            await _messageBus.PublishAsync(@event, cancellation: cancellationToken);

            _logger.LogInformation(
                "Product updated: {ProductId}",
                command.ProductId);

            return CommandResult.Ok(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", command.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Handler for DeleteProductCommand
/// </summary>
public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IMessageBus messageBus,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(
                command.TenantId, command.ProductId, cancellationToken);

            if (product == null)
            {
                return CommandResult.Fail("Product not found");
            }

            await _productRepository.DeleteAsync(product, cancellationToken);

            // Publish deletion event
            var @event = new ProductDeletedEvent
            {
                ProductId = product.Id,
                TenantId = command.TenantId,
                Sku = product.Sku
            };

            await _messageBus.PublishAsync(@event, cancellation: cancellationToken);

            _logger.LogInformation(
                "Product deleted: {ProductId} ({Sku})",
                command.ProductId, product.Sku);

            return CommandResult.Ok(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", command.ProductId);
            throw;
        }
    }
}
