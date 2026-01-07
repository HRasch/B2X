using B2X.IdsConnectAdapter.Formatters;
using B2X.IdsConnectAdapter.Middleware;
using B2X.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Add XML input/output formatters for IDS Connect protocol
    options.InputFormatters.Add(new XmlSerializerInputFormatter());
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
})
.AddXmlSerializerFormatters();

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add IDS Connect middleware
app.UseMiddleware<IdsConnectMiddleware>();

// Map controllers
app.MapControllers();

app.Run();
