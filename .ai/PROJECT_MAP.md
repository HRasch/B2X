# B2Connect Project Map

Generated: 2026-01-11

## Project Structure Overview

### Backend Projects (src/backend/)

| Project | Path |
|---------|------|
| B2X.AppHost | src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj |
| B2X.ServiceDefaults | src/backend/Infrastructure/Hosting/ServiceDefaults/B2X.ServiceDefaults.csproj |
| B2X.Admin (Gateway) | src/backend/Admin/Gateway/B2X.Admin.csproj |
| B2X.Admin.API | src/backend/Admin/API/Gateway/B2X.Admin.csproj |
| B2X.CLI | src/backend/Management/CLI/B2X.CLI/B2X.CLI.csproj |
| B2X.CLI.Administration | src/backend/Admin/CLI/B2X.CLI.Administration/B2X.CLI.Administration.csproj |
| B2X.Store | src/backend/Store/API/B2X.Store.csproj |
| B2X.Catalog.API | src/backend/Store/Domain/Catalog/B2X.Catalog.API.csproj |
| B2X.Categories | src/backend/Store/Domain/Categories/B2X.Categories.csproj |
| B2X.Domain.Search | src/backend/Store/Domain/Search/B2X.Domain.Search.csproj |
| B2X.Variants | src/backend/Store/Domain/Variants/src/B2X.Variants.csproj |
| B2X.Store.Search | src/backend/Store/Infrastructure/Gateway/Search/B2X.Store.Search.csproj |
| B2X.Shared.Core | src/backend/Shared/Domain/B2X.Shared.Core/B2X.Shared.Core.csproj |
| B2X.Shared.Kernel | src/backend/Shared/Domain/B2X.Shared.Kernel/B2X.Shared.Kernel.csproj |
| B2X.Types | src/backend/Shared/Domain/B2X.Types/B2X.Types.csproj |
| B2X.Utils | src/backend/Shared/Domain/B2X.Utils/B2X.Utils.csproj |
| B2X.Identity.API | src/backend/Shared/Domain/Identity/B2X.Identity.API.csproj |
| B2X.Identity | src/backend/Shared/Domain/Identity/B2X.Identity.csproj |
| B2X.Localization.API | src/backend/Shared/Domain/Localization/B2X.Localization.API.csproj |
| B2X.PatternAnalysis | src/backend/Shared/Domain/PatternAnalysis/B2X.PatternAnalysis.csproj |
| B2X.Search | src/backend/Shared/Domain/Search/B2X.Search.csproj |
| B2X.Api.Validation | src/backend/Shared/Domain/Shared/B2X.Api.Validation/B2X.Api.Validation.csproj |
| B2X.SmartDataIntegration | src/backend/Shared/Domain/SmartDataIntegration/src/B2X.SmartDataIntegration.csproj |
| B2X.SmartDataIntegration.API | src/backend/Shared/Domain/SmartDataIntegration/API/B2X.SmartDataIntegration.API.csproj |
| B2X.Tenancy.API | src/backend/Shared/Domain/Tenancy/B2X.Tenancy.API.csproj |
| B2X.Theming.API | src/backend/Shared/Domain/Theming/B2X.Theming.API.csproj |
| B2X.Theming.Layout | src/backend/Shared/Domain/Theming/Layout/B2X.Theming.Layout.csproj |
| B2X.ERP.Abstractions | src/backend/Shared/Domain/ERP/Abstractions/B2X.ERP.Abstractions.csproj |
| B2X.ERP (impl) | src/backend/Shared/Domain/ERP/impl/B2X.ERP.csproj |
| B2X.ERP (src) | src/backend/Shared/Domain/ERP/src/B2X.ERP.csproj |
| B2X.Shared.Infrastructure | src/backend/Shared/Infrastructure/B2X.Shared.Infrastructure/B2X.Shared.Infrastructure.csproj |
| B2X.Shared.Messaging | src/backend/Shared/Infrastructure/Messaging/B2X.Shared.Messaging.csproj |
| B2X.Shared.Middleware | src/backend/Shared/Infrastructure/middleware/B2X.Shared.Middleware.csproj |
| B2X.Shared.Monitoring | src/backend/Shared/Infrastructure/Monitoring/B2X.Shared.Monitoring.csproj |
| B2X.Shared.Search | src/backend/Shared/Infrastructure/Search/B2X.Shared.Search.csproj |
| B2X.Shared.Tools | src/backend/Shared/Infrastructure/tools/B2X.Shared.Tools.csproj |
| B2X.Connectors | src/backend/Infrastructure/Connectors/B2X.Connectors.csproj |
| B2X.Hosting | src/backend/Services/Hosting/B2X.Hosting.csproj |
| B2X.CMS | src/backend/Management/Domain/B2X.CMS.csproj |
| B2X.Email | src/backend/Management/Domain/B2X.Email.csproj |
| B2X.Compliance.API | src/backend/Admin/Domain/Compliance/B2X.Compliance.API.csproj |
| B2X.Legal.API | src/backend/Admin/Domain/Legal/B2X.Legal.API.csproj |

### Shared Projects (src/shared/)

| Project | Path |
|---------|------|
| B2X.CLI.Shared | src/shared/B2X.CLI.Shared/B2X.CLI.Shared.csproj |
| B2X.Shared.Erp.Core | src/shared/B2X.Shared.Erp.Core/B2X.Shared.Erp.Core.csproj |
| B2X.Shared.Erp.Validation | src/shared/B2X.Shared.Erp.Validation/B2X.Shared.Erp.Validation.csproj |
| B2X.Shared.ErpConnector | src/shared/B2X.Shared.ErpConnector/B2X.Shared.ErpConnector.csproj |
| B2X.Shared.Tools (Extensions) | src/shared/B2X.Shared.Extensions/B2X.Shared.Tools.csproj |
| B2X.AI | src/shared/Domain/AI/B2X.AI.csproj |
| B2X.Types (shared) | src/shared/types/B2X.Types.csproj |
| B2X.Utils (shared) | src/shared/utils/B2X.Utils.csproj |

### Services (src/services/)

| Project | Path |
|---------|------|
| B2X.SearchService | src/services/Search/B2X.SearchService.csproj |
| B2X.IdsConnectAdapter | src/services/PunchoutAdapters/IdsConnect/B2X.IdsConnectAdapter.csproj |

### Test Projects (src/tests/)

| Project | Path |
|---------|------|
| B2X.AppHost.Tests | src/tests/Hosting/AppHost.Tests/B2X.AppHost.Tests.csproj |
| B2X.Architecture.Tests | src/tests/Shared/B2X.Architecture.Tests/B2X.Architecture.Tests.csproj |
| B2X.Gateway.Integration.Tests | src/tests/Shared/B2X.Gateway.Integration.Tests/B2X.Gateway.Integration.Tests.csproj |
| B2X.AI.Tests | src/tests/Shared/Domain/AI/tests/B2X.AI.Tests.csproj |
| B2X.ERP.Tests | src/tests/Shared/Domain/ERP/tests/B2X.ERP.Tests.csproj |
| B2X.Identity.Tests | src/tests/Shared/Domain/Identity/tests/B2X.Identity.Tests.csproj |
| B2X.Localization.Tests | src/tests/Shared/Domain/Localization/tests/B2X.Localization.Tests.csproj |
| B2X.PatternAnalysis.Tests | src/tests/Shared/Domain/PatternAnalysis/tests/B2X.PatternAnalysis.Tests.csproj |
| B2X.Returns.Tests | src/tests/Shared/Domain/Returns/tests/B2X.Returns.Tests.csproj |
| B2X.Search.Integration | src/tests/Shared/Domain/Search/B2X.Search.Integration/B2X.Search.Integration.csproj |
| B2X.Search.UnitTests | src/tests/Shared/Domain/Search/B2X.Search.UnitTests/B2X.Search.UnitTests.csproj |
| B2X.Security.Tests | src/tests/Shared/Domain/Security/tests/B2X.Security.Tests.csproj |
| B2X.SmartDataIntegration.Tests | src/tests/Shared/Domain/SmartDataIntegration/tests/B2X.SmartDataIntegration.Tests.csproj |
| B2X.Tenancy.Tests | src/tests/Shared/Domain/Tenancy/tests/B2X.Tenancy.Tests.csproj |
| B2X.Validation.Tests | src/tests/Shared/Validation/B2X.Validation.Tests.csproj |
| B2X.Catalog.Tests | src/tests/Store/Backend/Catalog/tests/B2X.Catalog.Tests.csproj |
| B2X.Customer.Tests | src/tests/Store/Backend/Customer/tests/B2X.Customer.Tests.csproj |
| B2X.Orders.Tests | src/tests/Store/Backend/Orders/tests/B2X.Orders.Tests.csproj |
| B2X.CMS.Tests | src/tests/Management/Backend/CMS/tests/B2X.CMS.Tests.csproj |
| B2X.Email.Tests | src/tests/Management/Email/B2X.Email.Tests.csproj |
| B2X.Compliance.Tests | src/tests/Admin/Backend/Compliance/tests/B2X.Compliance.Tests.csproj |
| B2X.Legal.Tests | src/tests/Admin/Backend/Legal/tests/B2X.Legal.Tests.csproj |
| B2X.IdsConnectAdapter.Tests | src/tests/Services/PunchoutAdapters/IdsConnect/tests/B2X.IdsConnectAdapter.Tests.csproj |

### Other Projects

| Project | Path |
|---------|------|
| B2X.ErpConnector | src/erp-connector/src/B2X.ErpConnector/B2X.ErpConnector.csproj |
| B2X.Admin.MCP | src/AI/MCP/B2X.Admin.MCP/B2X.Admin.MCP.csproj |
| RoslynMCP | src/AI/RoslynMCP/RoslynMCP.csproj |
| WolverineMCP | src/AI/WolverineMCP/WolverineMCP.csproj |
| B2X.Monitoring | src/BoundedContexts/Monitoring/API/B2X.Monitoring.csproj |

---

## Reference Fix Checklist

Projects with broken references (from MSB9008 warnings):

- [ ] B2X.Utils (backend) - references B2X.Shared.Kernel
- [ ] B2X.Shared.Tools (backend) - references B2X.Shared.Kernel
- [ ] B2X.ErpConnector - references B2X.Shared.Erp.Core
- [ ] B2X.Legal.Tests - references B2X.Legal.API
- [ ] B2X.Gateway.Integration.Tests - references B2X.Store
- [ ] B2X.Search.Integration - references B2X.Search
- [ ] B2X.Search.UnitTests - references B2X.Search
- [ ] B2X.PatternAnalysis.Tests - references B2X.PatternAnalysis
- [ ] B2X.SmartDataIntegration.Tests - references B2X.SmartDataIntegration
- [ ] B2X.Validation.Tests - references B2X.Api.Validation
- [ ] B2X.Orders.Tests - references B2X.Orders.API
- [ ] B2X.Compliance.API - references B2X.Shared.Core
- [ ] B2X.Legal.API - references B2X.Shared.Middleware
- [ ] B2X.PatternAnalysis - references B2X.Shared.Kernel
- [ ] B2X.Shared.ErpConnector - references B2X.Shared.Data
- [ ] B2X.Types (shared) - references B2X.Shared.Kernel
- [ ] B2X.Catalog.API (Store) - references B2X.Shared.Core
- [ ] B2X.Orders.API (Store) - references B2X.Shared.Infrastructure
- [ ] B2X.IdsConnectAdapter - references B2X.Customer.API
- [ ] B2X.CLI - references B2X.Shared.Monitoring
