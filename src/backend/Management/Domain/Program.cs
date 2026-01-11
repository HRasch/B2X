using B2X.CMS.API.Endpoints;
using B2X.CMS.Application.Pages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
// builder.Services.AddOpenApi();  // TODO: Requires Microsoft.AspNetCore.OpenApi setup
builder.Services.AddScoped<ITemplateValidationService, DefaultTemplateValidationService>();

// Database configuration (if needed)
// builder.Services.AddDbContext<CmsDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();  // TODO: Requires Microsoft.AspNetCore.OpenApi setup
}

// Map template validation endpoints
app.MapTemplateValidationEndpoints();

app.Run();
