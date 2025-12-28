# B2Connect CLI Tool

**Microservices Management & Operations Interface**

## Installation

### Build as Global Tool

```bash
cd backend/CLI/B2Connect.CLI

# Build and pack
dotnet build
dotnet pack -c Release

# Install globally
dotnet tool install --global --add-source ./bin/Release B2Connect.CLI

# Verify installation
b2connect --help
```

### Build from Source

```bash
dotnet build backend/CLI/B2Connect.CLI/B2Connect.CLI.csproj
dotnet run --project backend/CLI/B2Connect.CLI/B2Connect.CLI.csproj -- --help
```

## Configuration

Configuration is stored in `~/.b2connect/config.json`:

```json
{
  "services": {
    "identity": {
      "url": "http://localhost:7002",
      "description": "Identity Service"
    },
    "tenancy": {
      "url": "http://localhost:7003",
      "description": "Tenancy Service"
    },
    "catalog": {
      "url": "http://localhost:7005",
      "description": "Catalog Service"
    }
  },
  "environment": "development",
  "timeout": 30
}
```

## Environment Variables

```bash
# Authentication token
export B2CONNECT_TOKEN="eyJhbGc..."

# Default tenant ID
export B2CONNECT_TENANT="00000000-0000-0000-0000-000000000000"
```

## Usage

### Help

```bash
b2connect --help
b2connect auth --help
b2connect auth create-user --help
```

### Authentication Commands

#### Create User

```bash
# Interactive password prompt
b2connect auth create-user john@example.com \
  --first-name John \
  --last-name Doe \
  --tenant-id 00000000-0000-0000-0000-000000000000

# With password
b2connect auth create-user john@example.com \
  --password "SecurePassword123!" \
  --first-name John \
  --last-name Doe
```

#### Login

```bash
# Interactive password prompt
b2connect auth login john@example.com

# With password
b2connect auth login john@example.com -p "SecurePassword123!"

# Save token for future use
b2connect auth login john@example.com --save
```

### Tenant Commands

#### Create Tenant

```bash
b2connect tenant create "Acme Corp" \
  --admin-email admin@acme.com \
  --admin-password "AdminPassword123!"
```

### System Commands

#### Check Service Status

```bash
# All services
b2connect system status

# Specific service
b2connect system status --service identity

# All services verbose
b2connect system status --service all
```

#### Show Configuration

```bash
b2connect info
```

## Command Structure

```
b2connect/
├── auth                    # Authentication & User Management
│   ├── create-user        # Create new user
│   └── login              # Get JWT token
│
├── tenant                  # Tenant Management
│   ├── create             # Create new tenant
│   ├── list               # List all tenants
│   └── show               # Show tenant details
│
├── product                # Product Management (Catalog)
│   ├── create             # Create product
│   ├── import             # Bulk import
│   └── list               # List products
│
├── content                # Content Management (CMS)
│   ├── create-page        # Create page
│   └── publish            # Publish content
│
├── system                 # System Operations
│   ├── status             # Service health check
│   ├── migrate            # Database migrations
│   └── seed               # Seed data
│
└── info                   # Show configuration
```

## Examples

### Full Workflow

```bash
# 1. Create tenant
TENANT_ID=$(b2connect tenant create "My Shop" \
  --admin-email admin@myshop.com \
  --admin-password "AdminPassword123!" | grep "Tenant ID" | awk '{print $NF}')

export B2CONNECT_TENANT=$TENANT_ID

# 2. Create users
b2connect auth create-user john@myshop.com \
  --first-name John \
  --last-name Doe

# 3. Login
b2connect auth login admin@myshop.com --save

# 4. Check services
b2connect system status

# 5. Create product
b2connect product create "SKU-001" "Product Name" \
  --price 99.99 \
  --category "Electronics"
```

### Scripting

```bash
#!/bin/bash

# Setup new tenant
TENANT_NAME="$1"
ADMIN_EMAIL="$2"

echo "Creating tenant: $TENANT_NAME"
b2connect tenant create "$TENANT_NAME" \
  --admin-email "$ADMIN_EMAIL" \
  --admin-password "$(openssl rand -base64 12)"

# Wait for service
sleep 2

# Health check
b2connect system status || exit 1

echo "Tenant setup complete!"
```

## Advanced Features

### Custom Service Endpoints

Edit `~/.b2connect/config.json`:

```json
{
  "services": {
    "custom-service": {
      "url": "http://custom-service:9000",
      "description": "Custom Service"
    }
  }
}
```

### Authentication Token Management

```bash
# Save token after login
b2connect auth login user@example.com --save

# Use saved token in requests
b2connect auth create-user newuser@example.com
# Token from B2CONNECT_TOKEN env var is automatically used

# Clear token
unset B2CONNECT_TOKEN
```

### Batch Operations

```bash
# Create multiple users from file
cat users.txt | while read email; do
  b2connect auth create-user "$email" \
    --password "TempPassword123!" \
    --tenant-id "$B2CONNECT_TENANT"
done
```

## Development

### Project Structure

```
backend/CLI/B2Connect.CLI/
├── Commands/
│   ├── AuthCommands/           # User & auth commands
│   ├── TenantCommands/         # Tenant management
│   ├── ProductCommands/        # Catalog operations
│   ├── ContentCommands/        # CMS operations
│   └── SystemCommands/         # System operations
│
├── Services/
│   ├── CliHttpClient.cs        # HTTP communication
│   ├── ConfigurationService.cs # Config management
│   └── ConsoleOutputService.cs # Formatted output
│
├── Program.cs                  # Entry point
└── B2Connect.CLI.csproj
```

### Adding New Commands

1. Create command file in appropriate folder: `Commands/{Group}/{Name}Command.cs`

```csharp
using System.CommandLine;
using B2Connect.CLI.Services;

namespace B2Connect.CLI.Commands.{Group};

public static class {Name}Command
{
    public static Command Create()
    {
        var command = new Command("{name}", "Description");
        
        // Add arguments and options
        var argument = new Argument<string>("name", "Description");
        command.AddArgument(argument);
        
        command.SetHandler(ExecuteAsync, argument);
        return command;
    }
    
    private static async Task ExecuteAsync(string arg)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();
        
        // Implementation
        console.Success("Done!");
    }
}
```

2. Register in `Program.cs`:

```csharp
var customCommand = new Command("custom", "Custom operations");
customCommand.AddCommand({Name}Command.Create());
rootCommand.AddCommand(customCommand);
```

## Troubleshooting

### Service Connection Failed

```bash
# Check if service is running
b2connect system status

# Verify config
b2connect info

# Test URL directly
curl http://localhost:7002/health
```

### Authentication Errors

```bash
# Clear saved token
unset B2CONNECT_TOKEN

# Login again
b2connect auth login user@example.com --save
```

### Configuration Issues

```bash
# Reset to defaults
rm -rf ~/.b2connect

# Reconfigure services in config.json
```

## CI/CD Integration

### GitHub Actions

```yaml
- name: Create Test Tenant
  run: |
    dotnet tool install --global --add-source ./nupkg B2Connect.CLI
    b2connect tenant create "CI-Test-${{ github.run_number }}" \
      --admin-email ci@test.com \
      --admin-password "${{ secrets.ADMIN_PASSWORD }}"
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install CLI
COPY . /src
WORKDIR /src/backend/CLI/B2Connect.CLI
RUN dotnet build && dotnet pack -c Release && \
    dotnet tool install --global --add-source ./bin/Release B2Connect.CLI

ENTRYPOINT ["b2connect"]
```

## Roadmap

### Phase 1 (Current)
- [x] Auth commands (create-user, login)
- [x] Tenant commands (create)
- [x] System commands (status)
- [x] HTTP client & configuration

### Phase 2
- [ ] Product commands (create, import, list)
- [ ] Content commands (create-page, publish)
- [ ] Database migration commands
- [ ] Data seeding commands
- [ ] Bulk operations

### Phase 3
- [ ] Interactive shell mode
- [ ] Configuration UI wizard
- [ ] Export/import configurations
- [ ] Webhook management
- [ ] Advanced analytics commands

## License

MIT

---

**Documentation**: [CLI_IMPLEMENTATION_GUIDE.md](../../CLI_IMPLEMENTATION_GUIDE.md)
