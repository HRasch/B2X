using B2X.Returns.Core.Interfaces;
using B2X.Returns.Infrastructure.Data;
using B2X.Returns.Infrastructure.Repositories;
using B2X.Returns.Application.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SERVICE REGISTRATION: Database
// ============================================================================

builder.Services.AddDbContext<ReturnsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured");
    
    options.UseNpgsql(connectionString);
});

// ============================================================================
// SERVICE REGISTRATION: Repositories
// ============================================================================

builder.Services.AddScoped<IReturnRepository, ReturnRepository>();

// ============================================================================
// SERVICE REGISTRATION: Validation
// ============================================================================

builder.Services.AddValidatorsFromAssemblyContaining<CreateReturnCommandValidator>();

// ============================================================================
// SERVICE REGISTRATION: Wolverine HTTP
// ============================================================================

builder.UseWolverine(opts =>
{
    // Returns bounded context endpoints
    // Wolverine auto-discovers public async methods in Service classes
    
    // Endpoint: POST /createreturn
    // Handler: ReturnManagementService.CreateReturn()
    
    opts.Services.AddScoped<ReturnManagementService>();
});

// ============================================================================
// HTTP PIPELINE
// ============================================================================

var app = builder.Build();

app.UseRouting();

app.Run();
