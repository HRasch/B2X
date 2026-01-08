---
docid: DOC-APPHOST-SPEC
title: AppHost Specifications
owner: @DevOps
status: Active
created: 2026-01-08
---

# AppHost Specifications

## Overview
The AppHost is the central orchestration component for the B2X platform, providing unified service management and deployment.

## Key Features
- Service orchestration using .NET Aspire
- Cross-platform compatibility (Windows, macOS, Linux)
- Zero external dependencies
- Integrated logging and monitoring

## Architecture
- Built on .NET Aspire framework
- Uses Wolverine for messaging
- PostgreSQL for data persistence
- Elasticsearch for search functionality

## Configuration
See [AppHost README](../../AppHost/README-Testing-Configuration.md) for detailed configuration options.

## Deployment
The AppHost can be run locally for development or deployed to production environments.

For more details, see the [AppHost Dashboard](../../AppHost/DASHBOARD.md).</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\APPHOST_QUICKSTART.md