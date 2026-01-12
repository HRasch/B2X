using B2X.Customer.Data;
using B2X.Customer.Interfaces;
using B2X.Customer.Models;
using B2X.Customer.Services;
using B2X.Customer.Validators;
using B2X.ServiceDefaults;
using B2X.Shared.Messaging.Extensions;
using EFCore.NamingConventions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Service Defaults (Health checks, Service Discovery)
// builder.Host.AddServiceDefaults();

// Add Wolverine with HTTP Endpoints
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "CustomerService";

    // Discovery configuration
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Register Controllers to enable model binding support
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Database Context (Issue #32: Invoice Management)
// Uses PostgreSQL with snake_case naming convention
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=B2X_customer;Username=postgres;Password=postgres";

// Use in-memory database for development/testing
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CustomerDbContext>(options =>
        options.UseInMemoryDatabase("CustomerDb"));
}
else
{
    builder.Services.AddDbContext<CustomerDbContext>(options =>
        options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());
}

// Issue #32: Invoice Modification for Reverse Charge
// Add Invoice Services
// builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
// builder.Services.AddScoped<IInvoiceService, InvoiceService>();
// builder.Services.AddScoped<IValidator<GenerateInvoiceCommand>, GenerateInvoiceCommandValidator>();
// builder.Services.AddScoped<IValidator<ModifyInvoiceCommand>, ModifyInvoiceCommandValidator>();

// Add TaxRateService (from Catalog domain for VAT calculations)
// TODO: Inject ITaxRateService from Catalog service via HTTP call
// For now, using internal dependency

// Add Caching (if needed for future features)
builder.Services.AddMemoryCache();

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add Authorization (REQUIRED for [Authorize] attributes)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
// app.UseServiceDefaults();

// Auto-discover and register Wolverine HTTP endpoints
app.MapWolverineEndpoints();

// Map Controllers
app.MapControllers();

// Add a simple test endpoint
app.MapGet("/test", () => "Customer API is running!");

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Customer API" }));

app.Run();
