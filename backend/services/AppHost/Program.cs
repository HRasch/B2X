using Serilog;
using System.Diagnostics;

// Configure Serilog
var serilogConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "AppHost")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

Log.Logger = serilogConfig.CreateLogger();

try
{
    var appHostBinDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        ?? AppContext.BaseDirectory;
    var servicesDir = Path.GetFullPath(Path.Combine(appHostBinDir, "..", "..", "..", ".."));

    Log.Information("🚀 B2Connect Application Host - Starting");
    Log.Information($"Services directory: {servicesDir}");
    Log.Information("");

    var services = new List<(string name, string path, int port)>
    {
        ("Auth Service", Path.Combine(servicesDir, "auth-service"), 9002),
        ("Tenant Service", Path.Combine(servicesDir, "tenant-service"), 9003),
        ("Localization Service", Path.Combine(servicesDir, "LocalizationService"), 9004),
    };

    var processes = new List<Process>();

    foreach (var (name, path, port) in services)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Log.Warning($"Service directory not found: {path}");
                continue;
            }

            Log.Information($"▶ Starting {name} on port {port}...");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run",
                WorkingDirectory = path,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };

            var process = Process.Start(psi);
            if (process != null)
            {
                processes.Add(process);
                Log.Information($"  ✓ {name} started (PID: {process.Id})");
            }

            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to start {name}");
        }
    }

    Log.Information("");
    Log.Information("✅ B2Connect Application Host initialized");
    Log.Information("");
    Log.Information("📊 Services:");
    Log.Information("  • Auth Service:         http://localhost:9002");
    Log.Information("  • Tenant Service:       http://localhost:9003");
    Log.Information("  • Localization Service: http://localhost:9004");
    Log.Information("");
    Log.Information("🎨 Frontend Services:");
    Log.Information("  • Customer App:  Port 5173 (npm run dev)");
    Log.Information("  • Admin App:     Port 5174 (npm run dev -- --port 5174)");
    Log.Information("");
    Log.Information("⏸  Press Ctrl+C to stop all services");
    Log.Information("");

    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (s, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
    };

    await Task.Delay(Timeout.Infinite, cts.Token).ConfigureAwait(false);
}
catch (OperationCanceledException)
{
    Log.Information("🛑 Shutting down services...");
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}

