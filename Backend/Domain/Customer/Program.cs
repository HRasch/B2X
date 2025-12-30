using FluentValidation;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
using Serilog;
using Wolverine;
using Wolverine.Http;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
using B2Connect.Customer.Data;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Services;
using B2Connect.Customer.Validators;
using B2Connect.Customer.Models;

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
builder.Host.AddServiceDefaults();

// Add Wolverine with HTTP Endpoints
var rabbitMqUri = builder.Configuration["RabbitMq:Uri"] ?? "amqp://guest:guest@localhost:5672";
var useRabbitMq = builder.Configuration.GetValue<bool>("Messaging:UseRabbitMq");

builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "CustomerService";

    // Enable HTTP Endpoints (Wolverine Mediator)
    // opts.Http.EnableEndpoints = true;  // TODO: Enable when Wolverine HTTP is properly configured

    // Discovery configuration
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // Add RabbitMQ if enabled (requires Wolverine.RabbitMq package)
    // if (useRabbitMq)
    // {
    //     opts.UseRabbitMq(rabbitMqUri);
    // }
});

// Add Wolverine HTTP support (REQUIRED for MapWolverineEndpoints)
builder.Services.AddWolverineHttp();

// Remove Controllers - using Wolverine HTTP Endpoints instead
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Database Context (Issue #32: Invoice Management)
// Uses PostgreSQL with snake_case naming convention
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=b2connect_customer;Username=postgres;Password=postgres";
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// Issue #32: Invoice Modification for Reverse Charge
// Add Invoice Services
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IValidator<GenerateInvoiceCommand>, GenerateInvoiceCommandValidator>();
builder.Services.AddScoped<IValidator<ModifyInvoiceCommand>, ModifyInvoiceCommandValidator>();

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
app.UseServiceDefaults();

// Auto-discover and register Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();
