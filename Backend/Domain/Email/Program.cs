using FluentValidation;
using B2Connect.Shared.Messaging.Extensions;
using B2Connect.ServiceDefaults;
using Serilog;
using Wolverine;
using Wolverine.Http;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
using B2Connect.Email.Data;
using B2Connect.Email.Interfaces;
using B2Connect.Email.Services;
using B2Connect.Email.Validators;
using B2Connect.Email.Models;

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
    opts.ServiceName = "EmailService";

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

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=b2connect_email;Username=postgres;Password=postgres";
builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// Add Email Services
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IValidator<SendEmailCommand>, SendEmailCommandValidator>();

// Add SMTP Configuration
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Add Caching
builder.Services.AddMemoryCache();

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseServiceDefaults();

// Auto-discover and register Wolverine HTTP endpoints
app.MapWolverineEndpoints();

app.Run();