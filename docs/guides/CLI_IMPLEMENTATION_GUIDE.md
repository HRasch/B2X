# B2Connect CLI Implementation Guide

**Status**: 🟢 Architecture Defined | Ready for Implementation  
**Last Updated**: 28. Dezember 2025

---

## Overview

Die **B2Connect CLI** ist ein dotnet-Tool, das alle Backend-Operationen über die Kommandozeile ermöglicht. Dies ermöglicht:

- ✅ **Automation**: CI/CD Pipelines, Deployment Scripts
- ✅ **Local Development**: Schneller Setup, Datenmigrationen
- ✅ **Operations**: Tenants verwalten, User erstellen, Datenbank-Operationen
- ✅ **Debugging**: Service-Status prüfen, Health Checks
- ✅ **Scripting**: Batch-Operationen, Daten-Import

---

## Projektstruktur

```
backend/CLI/B2Connect.CLI/
├── B2Connect.CLI.csproj              # CLI Tool Definition
├── Program.cs                        # Entry Point
├── Commands/
│   ├── AuthCommands/
│   │   ├── CreateUserCommand.cs
│   │   ├── GenerateTokenCommand.cs
│   │   ├── ListUsersCommand.cs
│   │   └── ResetPasswordCommand.cs
│   ├── TenantCommands/
│   │   ├── CreateTenantCommand.cs
│   │   ├── ListTenantsCommand.cs
│   │   ├── ShowTenantCommand.cs
│   │   └── DeleteTenantCommand.cs
│   ├── ProductCommands/
│   │   ├── CreateProductCommand.cs
│   │   ├── ListProductsCommand.cs
│   │   ├── ImportCsvCommand.cs
│   │   └── DeleteProductCommand.cs
│   ├── ContentCommands/
│   │   ├── CreatePageCommand.cs
│   │   ├── PublishPageCommand.cs
│   │   └── ListPagesCommand.cs
│   └── SystemCommands/
│       ├── MigrateCommand.cs
│       ├── SeedCommand.cs
│       ├── StatusCommand.cs
│       └── HealthCommand.cs
├── Services/
│   ├── CliHttpClient.cs              # HTTP Client für Microservices
│   ├── ConfigurationService.cs       # Lädt Service-Endpoints
│   ├── ConsoleOutputService.cs       # Formatierte Ausgabe
│   └── CommandExecutionService.cs    # Command-Ausführungs-Logik
└── Models/
    ├── CommandResult.cs
    ├── ServiceEndpoint.cs
    └── OutputFormatter.cs
```

---

## Installation & Verwendung

### 1. Als Global Tool Installieren

```bash
# Build CLI
cd backend/CLI/B2Connect.CLI
dotnet build -c Release

# Pack as NuGet Package
dotnet pack -c Release --output ../../nupkg

# Install global tool
dotnet tool install --global --add-source ./nupkg B2Connect.CLI

# Verify installation
b2connect --version
```

### 2. Als Local Tool Installieren

```bash
# Add tool manifest to repo
dotnet new tool-manifest

# Install local tool
dotnet tool install B2Connect.CLI --version <latest>

# Use via dotnet
dotnet b2connect --help
```

### 3. Direktes Ausführen

```bash
dotnet run --project backend/CLI/B2Connect.CLI -- <command> <options>
```

---

## Befehl-Struktur

### Format
```bash
b2connect <group> <command> [options] [arguments]
```

### Beispiele
```bash
b2connect auth create-user john@example.com --password secret123
b2connect tenant create "Acme Corp" --admin email@acme.com
b2connect product list --tenant-id <guid> --limit 10
b2connect migrate --service Identity --latest
```

---

## Implementierungs-Patterns

### Pattern 1: Einfacher Befehl (String Parameter)

**Beispiel: CreateUserCommand**

```csharp
namespace B2Connect.CLI.Commands.AuthCommands;

using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Invocation;

public class CreateUserCommand : Command
{
    public CreateUserCommand() : base("create-user", "Create a new user account")
    {
        // Arguments
        AddArgument(new Argument<string>(
            "email",
            "User email address"));
        
        // Options
        AddOption(new Option<string>(
            ["--password", "-p"],
            "User password (prompted if not provided)"));
        
        AddOption(new Option<Guid>(
            ["--tenant-id", "-t"],
            () => Guid.Empty,
            "Tenant ID (required)"));
        
        this.SetHandler(ExecuteAsync);
    }
    
    private async Task<int> ExecuteAsync(InvocationContext context)
    {
        var email = context.ParseResult.GetValueForArgument<string>("email");
        var password = context.ParseResult.GetValueForOption<string>("--password")
            ?? AnsiConsole.Prompt(new TextPrompt<string>("Password: ").Secret());
        var tenantId = context.ParseResult.GetValueForOption<Guid>("--tenant-id");
        
        if (tenantId == Guid.Empty)
        {
            AnsiConsole.MarkupLine("[red]✗[/] Tenant ID is required");
            return 1;
        }
        
        try
        {
            var client = new CliHttpClient("http://localhost:7002");
            var result = await client.PostAsync("/auth/create-user", new
            {
                email,
                password,
                tenantId
            });
            
            if (result.Success)
            {
                AnsiConsole.MarkupLine("[green]✓[/] User created successfully");
                AnsiConsole.WriteLine($"User ID: {result.Data?.userId}");
                return 0;
            }
            
            AnsiConsole.MarkupLine($"[red]✗[/] {result.Error}");
            return 1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
```

### Pattern 2: Befehl mit Datei-Input (CSV Import)

**Beispiel: ImportCsvCommand**

```csharp
public class ImportCsvCommand : Command
{
    public ImportCsvCommand() : base("import-csv", "Import products from CSV file")
    {
        AddArgument(new Argument<FileInfo>(
            "file",
            "CSV file path"));
        
        AddOption(new Option<Guid>(
            ["--tenant-id", "-t"],
            "Tenant ID (required)"));
        
        AddOption(new Option<bool>(
            ["--dry-run"],
            "Preview changes without saving"));
        
        this.SetHandler(ExecuteAsync);
    }
    
    private async Task<int> ExecuteAsync(InvocationContext context)
    {
        var file = context.ParseResult.GetValueForArgument<FileInfo>("file");
        var tenantId = context.ParseResult.GetValueForOption<Guid>("--tenant-id");
        var dryRun = context.ParseResult.GetValueForOption<bool>("--dry-run");
        
        if (!file.Exists)
        {
            AnsiConsole.MarkupLine($"[red]✗[/] File not found: {file.FullName}");
            return 1;
        }
        
        try
        {
            // Show progress
            AnsiConsole.Status()
                .Start("[yellow]Processing CSV...[/]", async ctx =>
                {
                    var lines = await File.ReadAllLinesAsync(file.FullName);
                    var total = lines.Length - 1; // Exclude header
                    
                    AnsiConsole.MarkupLine($"[cyan]Found {total} products[/]");
                    
                    if (dryRun)
                    {
                        AnsiConsole.MarkupLine("[yellow]Dry run mode: no data will be saved[/]");
                    }
                    
                    // Process CSV...
                    var client = new CliHttpClient("http://localhost:7005");
                    var result = await client.PostAsync("/catalog/import", new
                    {
                        tenantId,
                        fileName = file.Name,
                        lineCount = total,
                        dryRun
                    });
                    
                    if (result.Success)
                    {
                        AnsiConsole.MarkupLine($"[green]✓[/] Imported {result.Data?.imported} products");
                        if (result.Data?.skipped > 0)
                        {
                            AnsiConsole.MarkupLine($"[yellow]⚠[/] Skipped {result.Data.skipped} rows");
                        }
                        return 0;
                    }
                    
                    AnsiConsole.MarkupLine($"[red]✗[/] {result.Error}");
                    return 1;
                });
            
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
```

### Pattern 3: Status/Health-Check Befehl

**Beispiel: StatusCommand**

```csharp
public class StatusCommand : Command
{
    public StatusCommand() : base("status", "Check status of all services")
    {
        AddOption(new Option<bool>(
            ["--all"],
            "Check all services (default: only required)"));
        
        this.SetHandler(ExecuteAsync);
    }
    
    private async Task<int> ExecuteAsync(InvocationContext context)
    {
        var checkAll = context.ParseResult.GetValueForOption<bool>("--all");
        
        var services = new[]
        {
            ("Identity", "http://localhost:7002"),
            ("Tenancy", "http://localhost:7003"),
            ("Catalog", "http://localhost:7005"),
            ("CMS", "http://localhost:7006"),
            ("Theming", "http://localhost:7008"),
            ("Search", "http://localhost:9300")
        };
        
        var table = new Table()
            .AddColumn("Service")
            .AddColumn("Port")
            .AddColumn("Status")
            .AddColumn("Response Time");
        
        foreach (var (name, url) in services)
        {
            try
            {
                var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await client.GetAsync($"{url}/health");
                sw.Stop();
                
                var status = response.IsSuccessStatusCode 
                    ? "[green]✓ Healthy[/]" 
                    : "[red]✗ Error[/]";
                
                var port = new Uri(url).Port;
                table.AddRow(name, port.ToString(), status, $"{sw.ElapsedMilliseconds}ms");
            }
            catch
            {
                var port = new Uri(url).Port;
                table.AddRow(name, port.ToString(), "[red]✗ Offline[/]", "-");
            }
        }
        
        AnsiConsole.Write(table);
        return 0;
    }
}
```

---

## HTTP Client Service

**Datei: Services/CliHttpClient.cs**

```csharp
using System.Text.Json;

namespace B2Connect.CLI.Services;

public class CliHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public CliHttpClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30),
            BaseAddress = new Uri(baseUrl)
        };
    }
    
    public async Task<CliResult<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(content);
                return new CliResult<T> { Success = true, Data = data };
            }
            
            return new CliResult<T> 
            { 
                Success = false, 
                Error = $"HTTP {(int)response.StatusCode}: {content}" 
            };
        }
        catch (Exception ex)
        {
            return new CliResult<T> { Success = false, Error = ex.Message };
        }
    }
    
    public async Task<CliResult<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<T>(responseContent);
                return new CliResult<T> { Success = true, Data = result };
            }
            
            return new CliResult<T> 
            { 
                Success = false, 
                Error = $"HTTP {(int)response.StatusCode}: {responseContent}" 
            };
        }
        catch (Exception ex)
        {
            return new CliResult<T> { Success = false, Error = ex.Message };
        }
    }
}

public class CliResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
}
```

---

## Program.cs Aufbau

```csharp
using System.CommandLine;
using Spectre.Console;
using B2Connect.CLI.Commands.AuthCommands;
using B2Connect.CLI.Commands.TenantCommands;
using B2Connect.CLI.Commands.ProductCommands;
using B2Connect.CLI.Commands.SystemCommands;

var rootCommand = new RootCommand("B2Connect CLI - Manage B2Connect platform")
{
    TreatUnmatchedTokensAsErrors = true
};

// Auth Commands
var authGroup = new Command("auth", "User authentication & management");
authGroup.AddCommand(new CreateUserCommand());
authGroup.AddCommand(new GenerateTokenCommand());
authGroup.AddCommand(new ListUsersCommand());
authGroup.AddCommand(new ResetPasswordCommand());
rootCommand.AddCommand(authGroup);

// Tenant Commands
var tenantGroup = new Command("tenant", "Tenant management");
tenantGroup.AddCommand(new CreateTenantCommand());
tenantGroup.AddCommand(new ListTenantsCommand());
tenantGroup.AddCommand(new ShowTenantCommand());
tenantGroup.AddCommand(new DeleteTenantCommand());
rootCommand.AddCommand(tenantGroup);

// Product Commands
var productGroup = new Command("product", "Product catalog management");
productGroup.AddCommand(new CreateProductCommand());
productGroup.AddCommand(new ListProductsCommand());
productGroup.AddCommand(new ImportCsvCommand());
productGroup.AddCommand(new DeleteProductCommand());
rootCommand.AddCommand(productGroup);

// System Commands
var systemGroup = new Command("system", "System operations");
systemGroup.AddCommand(new MigrateCommand());
systemGroup.AddCommand(new SeedCommand());
systemGroup.AddCommand(new StatusCommand());
systemGroup.AddCommand(new HealthCommand());
rootCommand.AddCommand(systemGroup);

// Version command
rootCommand.AddOption(new Option<bool>(
    ["--version", "-v"],
    "Show version"));

return await rootCommand.InvokeAsync(args);
```

---

## Häufige Operationen

### User Management

```bash
# Create user
b2connect auth create-user john@example.com --password secret123 --tenant-id <guid>

# Generate token for testing
b2connect auth generate-token john@example.com --password secret123

# List all users in tenant
b2connect auth list-users --tenant-id <guid>

# Reset user password
b2connect auth reset-password john@example.com --tenant-id <guid>
```

### Tenant Management

```bash
# Create new tenant
b2connect tenant create "Company Name" --admin-email admin@company.com --admin-password initial123

# List all tenants
b2connect tenant list

# Show tenant details
b2connect tenant show <tenant-id>

# Delete tenant (with confirmation)
b2connect tenant delete <tenant-id> --force
```

### Product Management

```bash
# Create product
b2connect product create "SKU-001" "Product Name" \
  --price 99.99 \
  --category "Electronics" \
  --tenant-id <guid>

# Import from CSV
b2connect product import-csv products.csv --tenant-id <guid>

# Dry run (preview changes)
b2connect product import-csv products.csv --tenant-id <guid> --dry-run

# List products
b2connect product list --tenant-id <guid> --limit 50
```

### System Operations

```bash
# Run database migrations
b2connect system migrate --service Identity --latest

# Seed database with test data
b2connect system seed --service Catalog --file data.json

# Check service status
b2connect system status --all

# Health check
b2connect system health
```

---

## Configuration

### appsettings.json (für CLI)

```json
{
  "Services": {
    "Identity": "http://localhost:7002",
    "Tenancy": "http://localhost:7003",
    "Catalog": "http://localhost:7005",
    "CMS": "http://localhost:7006",
    "Theming": "http://localhost:7008",
    "Search": "http://localhost:9300"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Environment Variables

```bash
export B2CONNECT_IDENTITY_URL=http://localhost:7002
export B2CONNECT_CATALOG_URL=http://localhost:7005
export B2CONNECT_ADMIN_PASSWORD=cli-admin-password
```

---

## CI/CD Integration

### GitHub Actions Example

```yaml
name: CLI Operations

on: [workflow_dispatch]

jobs:
  seed-database:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0'
      
      - name: Install CLI
        run: |
          cd backend/CLI/B2Connect.CLI
          dotnet tool install --global --add-source ./nupkg B2Connect.CLI
      
      - name: Run migrations
        run: b2connect system migrate --service Identity --latest
      
      - name: Seed test data
        run: b2connect system seed --service Catalog --file test-data.json
      
      - name: Verify services
        run: b2connect system status --all
```

---

## Roadmap

### Phase 1 (Current)
- ✅ Projektstruktur definiert
- ✅ Pattern dokumentiert
- ⏳ Basis-Befehle implementieren (Create User, Tenant CRUD)

### Phase 2
- ⏳ Product Import (CSV, BMEcat)
- ⏳ Data Enrichment Commands
- ⏳ Migration Tools

### Phase 3
- ⏳ Analytics/Reporting Commands
- ⏳ Backup/Restore Tools
- ⏳ Performance Tuning Commands

---

## Testing

```bash
# Build CLI
dotnet build backend/CLI/B2Connect.CLI/B2Connect.CLI.csproj

# Run help
dotnet run --project backend/CLI/B2Connect.CLI -- --help

# Test create user
dotnet run --project backend/CLI/B2Connect.CLI -- auth create-user test@example.com --help

# Test status
dotnet run --project backend/CLI/B2Connect.CLI -- system status
```

---

**Status**: 🟢 Ready for Implementation  
**Next**: Starten Sie mit AuthCommands und TenantCommands

