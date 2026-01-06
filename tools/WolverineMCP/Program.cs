using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McpSharp.Server;
using B2Connect.Tools.WolverineMCP.Services;
using B2Connect.Tools.WolverineMCP.Tools;

// Register MSBuild before anything else
MSBuildLocator.RegisterDefaults();

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<WolverineAnalysisService>();
builder.Services.AddSingleton<QueryOptimizationService>();
builder.Services.AddSingleton<DependencyInjectionService>();

// Register MCP server with tools
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

await app.RunAsync();