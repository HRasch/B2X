---
docid: DOC-APPHOST-QUICKSTART
title: AppHost Quick Start Guide
owner: @DevOps
status: Active
created: 2026-01-08
---

# AppHost Quick Start

## Prerequisites
- .NET 10 SDK
- PostgreSQL (for data persistence)
- Elasticsearch (for search functionality)

## Running Locally
1. Navigate to the AppHost directory:
   ```bash
   cd AppHost
   ```

2. Run the AppHost:
   ```bash
   dotnet run --project B2X.AppHost.csproj
   ```

3. Open the Aspire dashboard at http://localhost:15500

## Development Mode
For development with in-memory databases:
```bash
dotnet run --project B2X.AppHost.csproj --environment Development
```

## Services Included
- API Gateway
- Catalog Service
- Identity Service
- Search Service
- CMS Service
- Email Service

## Troubleshooting
- Check the Aspire dashboard for service health
- Review logs in the terminal output
- Ensure all required services are running

For detailed specifications, see [DOC-APPHOST-SPEC].