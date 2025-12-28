# ⚙️ DevOps Engineer Quick Start

**Role Focus:** Infrastructure, CI/CD, Aspire orchestration, Monitoring  
**Time to Productivity:** 3 weeks  
**Critical Components:** P0.3, P0.4, P0.5

---

## ⚡ Week 1: Aspire Orchestration

### Day 1-2: Project Structure
```bash
# Aspire entry point
backend/Orchestration/B2Connect.Orchestration.csproj

# Services to orchestrate
backend/Domain/Identity/src/B2Connect.Identity.csproj
backend/Domain/Catalog/src/B2Connect.Catalog.csproj
backend/Domain/CMS/src/B2Connect.CMS.csproj
# ... and more

# Data services
PostgreSQL (5432) via Docker
Redis (6379) via Docker
Elasticsearch (9200) via Docker
```

### Day 3: Start Aspire
```bash
cd backend/Orchestration
dotnet run

# Dashboard at: http://localhost:15500
# All services auto-discovered ✅

# Or via task:
cd /Users/holger/Documents/Projekte/B2Connect
# Run task: backend-start (in VS Code)
```

### Day 4-5: Troubleshoot Port Issues (macOS specific!)
```bash
# Problem: DCP holds ports after shutdown
# Solution: Kill services before restart

# Run script:
./scripts/kill-all-services.sh

# Or manually:
pkill -9 -f "dcpctrl"
pkill -9 -f "dcpproc"

# Check ports:
./scripts/check-ports.sh

# Verify ports free:
lsof -i :15500
lsof -i :7002   # Identity
lsof -i :7005   # Catalog
```

---

## 🏗️ Week 2: Infrastructure & CI/CD

### Day 1-2: Read Infrastructure Docs
```
1. docs/architecture/ASPIRE_GUIDE.md - 45 min
2. docs/GITHUB_WORKFLOWS.md - 20 min
3. docs/MACOS_CDP_PORTFIX.md - 10 min
4. docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.4, P0.5) - 30 min
```

### Day 3: CI/CD Setup
```yaml
# GitHub Actions workflow structure:
.github/workflows/
├── build.yml          # Build on every push
├── test.yml           # Run tests on PR
├── security.yml       # SAST/dependency check
└── deploy.yml         # Deploy to staging/prod
```

### Day 4-5: Network Security (P0.4)
```yaml
# VPC Structure (AWS/Azure)
Public Subnet:
  - Load Balancer (ALB)
  - DDoS Protection (AWS Shield)
  - WAF Rules

Private Subnet (Services):
  - Identity (7002)
  - Catalog (7005)
  - CMS (7006)
  - mTLS between services

Private Subnet (Databases):
  - PostgreSQL (5432)
  - Redis (6379)
  - Elasticsearch (9200)
  - NO internet access
```

---

## 🔑 Week 3: Key Management & Monitoring

### Day 1-2: Azure KeyVault Setup (P0.5)
```bash
# Create KeyVault
az keyvault create --name b2connect-vault \
  --resource-group b2connect-rg \
  --location westeurope

# Store secrets
az keyvault secret set --vault-name b2connect-vault \
  --name "Encryption--Key" --value "$(openssl rand -base64 32)"

az keyvault secret set --vault-name b2connect-vault \
  --name "Database--ConnectionString" --value "..."

# Access in C#
var credential = new DefaultAzureCredential();
var client = new SecretClient(vaultUri, credential);
var secret = await client.GetSecretAsync("Encryption--Key");
```

### Day 3: Monitoring & Alerting
```csharp
// Key metrics to monitor:
- API Response Time (target: < 100ms P95)
- Error Rate (target: < 1%)
- CPU Utilization (target: < 70%)
- Memory Usage (target: < 80%)
- Database Connection Pool (target: < 80%)
- Disk Space (alert: > 90%)

// Tools:
- Prometheus (metrics collection)
- Grafana (visualization)
- Alert Manager (notifications)
```

### Day 4-5: Incident Response Infrastructure (P0.3)
```bash
# Automated incident detection
1. Brute force detection (5+ failed logins → alert)
2. Data exfiltration (3x normal volume → alert)
3. Service down (> 5 min → alert)
4. Response time degradation (> 2x baseline → alert)

# Notification channels
- Email to security@b2connect.com
- Slack to #incidents channel
- PagerDuty for critical incidents
- SMS for P0 severity

# NIS2 notification procedure
# < 24 hours to competent authority
```

---

## ⚡ Quick Commands

```bash
# Build Aspire
dotnet build backend/Orchestration/B2Connect.Orchestration.csproj

# Start Aspire with dashboard
cd backend/Orchestration && dotnet run

# Kill stuck services (macOS/Linux)
./scripts/kill-all-services.sh

# Check ports
./scripts/check-ports.sh

# View logs
docker logs [container-name]

# Database migrations
dotnet ef migrations add [Name] --project backend/Domain/[Service]/src
dotnet ef database update --project backend/Domain/[Service]/src

# Run specific test
dotnet test backend/Domain/[Service]/tests/ -v minimal
```

---

## 📚 Critical Resources

| Topic | File | Time |
|-------|------|------|
| Aspire Guide | `docs/architecture/ASPIRE_GUIDE.md` | 45 min |
| Port Issues (macOS) | `docs/MACOS_CDP_PORTFIX.md` | 10 min |
| Network Segmentation | EU Roadmap §P0.4 | 30 min |
| Key Management | EU Roadmap §P0.5 | 20 min |
| GitHub Workflows | `docs/GITHUB_WORKFLOWS.md` | 20 min |

---

## 🎯 First Task: Get Aspire Running

**Phase 1: Setup** (1 hour)
1. Navigate to `backend/Orchestration`
2. Run `dotnet run`
3. Open `http://localhost:15500`
4. Verify all services appear in dashboard

**Phase 2: Troubleshoot** (if issues)
1. Kill stuck processes: `./scripts/kill-all-services.sh`
2. Check ports: `./scripts/check-ports.sh`
3. Verify no conflicts (Aspire uses ports 7000-8000 range)
4. Restart: `dotnet run`

**Phase 3: Verify Services**
1. Identity available at `http://localhost:7002`
2. Catalog available at `http://localhost:7005`
3. CMS available at `http://localhost:7006`
4. Dashboard reachable at `http://localhost:15500`

**Time Estimate:** 1-2 hours  
**Success:** All services running, dashboard shows healthy ✅

---

## 🏗️ P0 Components Timeline

```
Week 1:     Aspire setup + troubleshooting
Week 2:     Network Segmentation (P0.4)
Week 3:     Key Management (P0.5) + Incident Response (P0.3)
```

---

## 📞 Getting Help

- **Aspire Issues:** Check `ASPIRE_GUIDE.md` and ASPIRE_DASHBOARD_TROUBLESHOOTING.md
- **Port Conflicts (macOS):** See `MACOS_CDP_PORTFIX.md`
- **Infrastructure Questions:** AWS/Azure documentation
- **Security Questions:** Ask Security Engineer

---

**Key Reminders:**
- Aspire is development-only orchestration
- Production uses Kubernetes or cloud-native deployment
- Always kill services before restarting
- Check ports if deployment fails
- Monitor key metrics continuously
