using System.Text.Json.Serialization;
using B2X.Orders.Core.Interfaces;
using B2X.Orders.Infrastructure.Data;
using B2X.Orders.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references between Order and OrderItem
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add Database Context (in-memory for testing)
builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseInMemoryDatabase("B2X_orders_inmemory");
});

// Discover FluentValidation validators in this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Orders services
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

// Add Wolverine for messaging
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "OrdersService";
    opts.UseFluentValidation();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

app.MapGet("/test", () => "Orders API is running!");

app.Run();
