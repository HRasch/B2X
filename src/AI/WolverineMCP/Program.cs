using B2X.Tools.WolverineMCP.Services;
using B2X.Tools.WolverineMCP.Tools;
using McpDotNet;
using McpDotNet.Server;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Register MSBuild before anything else
MSBuildLocator.RegisterDefaults();

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<WolverineAnalysisService>();
builder.Services.AddSingleton<QueryOptimizationService>();
builder.Services.AddSingleton<DependencyInjectionService>();

// Register MCP tool classes
builder.Services.AddSingleton<AnalyzeHandlersTool>();
builder.Services.AddSingleton<AnalyzeQueriesTool>();
builder.Services.AddSingleton<ValidateDITool>();

// Register MCP server with tools
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools();

var app = builder.Build();

await app.RunAsync();
