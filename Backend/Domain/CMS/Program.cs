using B2X.CMS.API.Endpoints;
using B2X.CMS.Application.Pages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITemplateValidationService, DefaultTemplateValidationService>();

// Database configuration (if needed)
// builder.Services.AddDbContext<CmsDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map template validation endpoints
app.MapTemplateValidationEndpoints();

app.Run();
