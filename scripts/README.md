# B2X Scripts Directory

## 📋 Overview

This directory contains utility scripts for B2X development, testing, and operations. Scripts are organized into categories for better discoverability and maintenance.

## 📁 Directory Structure

### `ai/` - AI and Machine Learning Scripts
Scripts related to AI model management, cost monitoring, and AI-assisted development tools.

**Scripts:**
- `ai-batch-processor.py` - Batch processing for AI operations
- `ai-cache.py` - AI cache management
- `ai-cost-monitor.py` / `ai-cost-monitor.sh` - Monitor AI API costs
- `ai-integration-tests.py` - Test AI integrations
- `ai-model-selector.py` - Select appropriate AI models
- `ai-optimization-center.py` - AI performance optimization
- `ai-quality-scorer.py` - Score AI output quality
- `README-ai-monitoring.md` - AI monitoring documentation

### `deployment/` - Deployment and Service Management
Scripts for starting, stopping, and managing B2X services and deployments.

**Scripts:**
- `aspire-run.sh` - Run Aspire orchestration
- `aspire-start.sh` - Start Aspire dashboard
- `aspire-stop.sh` - Stop Aspire services
- `kill-all-services.sh` - Stop all B2X services
- `rollback-mcp.sh` - Rollback MCP deployments
- `rollback-refactor.sh` - Rollback refactoring changes
- `start-all-services.sh` - Start all services
- `start-all.sh` - Start complete stack
- `start-aspire-with-frontends.sh` - Start Aspire with frontends
- `start-aspire.sh` - Start Aspire orchestration
- `start-frontend.sh` - Start frontend services
- `start-full-stack.sh` - Start complete application stack
- `start-services-local.sh` - Start services locally
- `start-vscode.sh` - Start VS Code development environment
- `stop-services-local.sh` - Stop local services

### `docs/` - Documentation Management
Scripts for documentation maintenance, auditing, and quality control.

**Scripts:**
- `archive-old-docs.sh` - Archive outdated documentation
- `docs-quality-monitor.sh` - Monitor documentation quality
- `docs-validation.sh` - Validate documentation
- `documentation-audit.ps1` - Audit documentation completeness

### `monitoring/` - System Monitoring and Health Checks
Scripts for monitoring system health, performance, and service status.

**Scripts:**
- `B2X-heartbeat.service` - Systemd service for heartbeat monitoring
- `B2X-heartbeat.timer` - Timer for heartbeat checks
- `health-check.sh` - General health checks
- `monitor-canary.sh` - Canary deployment monitoring
- `monitor-collaboration.sh` - Collaboration monitoring
- `monitor-deployment.sh` - Deployment monitoring
- `monitor-port-8080.sh` - Port 8080 monitoring
- `service-health.sh` - Service health monitoring

### `utilities/` - General Utility Scripts
Miscellaneous utility scripts for development, maintenance, and operations.

**Scripts:**
- `audit-hardcoded-paths.sh` - Audit hardcoded paths
- `audit-tokens.sh` - Audit token usage
- `auto-glitch-research.sh` - Research automation glitches
- `auto-remediate.sh` - Automatic remediation
- `clean-duplicate-nuget-obj.sh` - Clean duplicate NuGet objects
- `cleanup-cdp.sh` - CDP cleanup
- `cleanup-continuation.sh` - Continuation cleanup
- `commit-pr-quality-gate.sh` - PR quality gate commits
- `copilot-guardian.sh` - Copilot monitoring
- `copilot-size-audit.sh` - Copilot size auditing
- `create-pr.sh` - Create pull requests
- `create-refactor-structure.sh` - Create refactoring structure
- `daily-mcp-review.sh` - Daily MCP reviews
- `deployment-status.sh` - Deployment status
- `detect-duplicates.sh` - Detect duplicates
- `dev-node.py` - Development node utilities
- `diagnose.sh` - Diagnostic utilities
- `discover-b2x-references.sh` - Discover B2X references
- `enable-pr-quality-gate.sh` - Enable PR quality gates
- `find-archived-references.sh` - Find archived references
- `fix-project-references.sh` - Fix project references
- `generate-coverage.ps1` - Generate coverage reports
- `generate-coverage.sh` - Generate coverage (bash)
- `generate-features-index.js` - Generate feature index
- `GITHUB_ISSUE_UPDATES_QUICK_REFERENCE.md` - GitHub issue reference
- `identify-pilot-files-new.js` - Identify pilot files (new)
- `identify-pilot-files.js` - Identify pilot files
- `install-hooks.sh` - Install git hooks
- `kubernetes-setup.sh` - Kubernetes setup
- `legacy-code-cleanup.js` - Legacy code cleanup
- `lessons-maintenance.ps1` - Maintain lessons learned
- `lessons-workflow-integration.ps1` - Lessons workflow integration
- `load-test-translations.k6.js` - Load test translations
- `MANIFEST.sh` - Manifest utilities
- `markdown-fragment-reader.sh` - Markdown fragment reader
- `mcp-ab-testing.js` - MCP A/B testing
- `mcp-audit-trail.js` - MCP audit trail
- `mcp-cache-manager.js` - MCP cache management
- `mcp-console-logger.js` - MCP console logging
- `mcp-daily-report.sh` - MCP daily reports
- `mcp-metrics-dashboard.js` - MCP metrics dashboard
- `mcp-rate-limiter.js` - MCP rate limiting
- `mcp-validation-checklist.sh` - MCP validation checklist
- `migrate-lessons.ps1` - Migrate lessons
- `migrate-lessons.sh` - Migrate lessons (bash)
- `namespace-renamer.ps1` - Namespace renaming
- `parse-logs.sh` - Log parsing
- `path-mapping.sh` - Path mapping
- `performance-benchmark.sh` - Performance benchmarking
- `pre-commit` - Pre-commit hook
- `pre-commit-setup.sh` - Setup pre-commit hooks
- `prompt-compression-engine-simple.sh` - Simple prompt compression
- `prompt-compression-engine.sh` - Prompt compression engine
- `rate-limit-monitor.sh` - Rate limit monitoring
- `README-Migration.md` - Migration documentation
- `README-update-vision-issues.md` - Vision issues documentation
- `refactor-project.sh` - Project refactoring
- `refactor-start.sh` - Start refactoring
- `roslyn-batch-analysis-phase4.ps1` - Roslyn batch analysis
- `roslyn-batch-analysis.ps1` - Roslyn batch analysis
- `run-gap-analysis.sh` - Run gap analysis
- `run-local-checks.sh` - Run local checks
- `run-login-e2e-tests.sh` - Run login E2E tests
- `runtime-health-check.sh` - Runtime health checks
- `seo-validation.sh` - SEO validation
- `setup-docs-quality-schedule.ps1` - Setup docs quality schedule
- `setup-git-hooks.sh` - Setup git hooks
- `setup-production-monitoring.sh` - Setup production monitoring
- `smoke-test.sh` - Smoke testing
- `temp-file-manager.sh` - Temporary file management
- `token-monitor.sh` - Token monitoring
- `token-optimization-benchmark.sh` - Token optimization benchmarking
- `track-large-file-editing-metrics.sh` - Track large file editing metrics
- `translate-keys.js` - Translation key utilities
- `typescript-batch-analysis.ps1` - TypeScript batch analysis
- `update-dependencies.sh` - Update dependencies
- `update-kb-sources.sh` - Update knowledge base sources
- `update-project-namespaces.ps1` - Update project namespaces
- `update-references.sh` - Update references
- `update-sprint-issues.sh` - Update sprint issues
- `update-vision-issues.js` - Update vision issues
- `verify-installation.sh` - Verify installation
- `verify-localization.sh` - Verify localization
- `watch-collaboration.sh` - Watch collaboration

**Archived Scripts:** `Migrate-Project.ps1`, `ProjectMigration.psm1` (deprecated migration scripts moved to `scripts/archive/`)

### `validation/` - Validation and Testing Scripts
Scripts for validation, testing, and quality assurance.

**Scripts:**
- `check-ai-duplicates.sh` - Check AI duplicates
- `check-build-configs.sh` - Check build configurations
- `check-external-deps.sh` - Check external dependencies
- `check-kb-links.sh` - Check knowledge base links
- `check-monitoring-configs.sh` - Check monitoring configurations
- `check-platform-configs.sh` - Check platform configurations
- `check-ports.sh` - Check port availability
- `check-security-configs.sh` - Check security configurations
- `ci-validate-dependencies.sh` - CI dependency validation
- `pr-preflight-check.sh` - PR preflight checks
- `pre-commit-cleanup-check.sh` - Pre-commit cleanup checks
- `test-and-fix-all.sh` - Test and fix all
- `test-aspire-startup.sh` - Test Aspire startup
- `test-builds.sh` - Test builds
- `test-double-start.sh` - Test double start prevention
- `test-port-lifecycle.sh` - Test port lifecycle
- `validate-canary.sh` - Validate canary deployments
- `validate-deployment.sh` - Validate deployments
- `validate-large-file-edit.sh` - Validate large file edits
- `validate-metadata.sh` - Validate metadata
- `validate-moves.sh` - Validate file moves
- `validate-no-secrets.sh` - Validate no secrets
- `validate-rollback.sh` - Validate rollbacks
- `validate-segregation.sh` - Validate segregation

## 🚀 Usage Guidelines

### Finding Scripts
1. **Check the appropriate subdirectory** based on the task category
2. **Use the README** in each subdirectory for detailed script descriptions
3. **Run with `--help`** flag for usage information when available

### Best Practices
- **Test scripts locally** before using in CI/CD pipelines
- **Review script permissions** and ensure proper execution context
- **Update documentation** when modifying scripts
- **Use absolute paths** when calling scripts from different directories

### Maintenance
- **Regular review**: Audit scripts quarterly for relevance and security
- **Update paths**: Keep hardcoded paths current with project structure changes
- **Remove deprecated**: Archive unused scripts to avoid confusion
- **Document changes**: Update this README when adding/removing scripts

## 📞 Support

For script-related issues or questions:
- Check individual script headers for usage examples
- Review related documentation in `docs/`
- Contact the development team for assistance

---

**Last Updated**: January 2026
**Maintained by**: Development Team

1. **Aspire verwenden** - Für normale Entwicklung
   ```bash
   dotnet run --project backend/AppHost/B2X.AppHost.csproj
   ```

2. **Manueller Cleanup** - Nur wenn nötig
   ```bash
   ./scripts/kill-all-services.sh
   ```

3. **Port-Status checken** - Vor dem Starten
   ```bash
   ./scripts/check-ports.sh
   ```

## ⚙️ Automatisches Cleanup

Mit Aspire DCP wird Cleanup automatisch gehandhabt:
- ✅ Services werden beim Herunterfahren sauber beendet
- ✅ Ports werden freigegeben
- ✅ Keine manuellen Interventionen normalerweise nötig

Nur bei Edge-Cases (Crashes, Force-Stops) das manuelle Script verwenden.

## 📝 Fehlerbehebung

| Problem | Lösung |
|---------|--------|
| "Address already in use" | `./scripts/check-ports.sh` dann `./scripts/kill-all-services.sh` |
| Aspire Dashboard nicht erreichbar | Kill-Script ausführen und erneut starten |
| Service-Prozess hängt | `./scripts/kill-all-services.sh` |
| DCP-Controller blockiert | Force-kill über Neubau oder Rechner-Neustart |

## ❤️ Heartbeat-System (Produktion)

Das Heartbeat-System überwacht kontinuierlich die Gesundheit der Backend-Services und führt automatische Neustarts bei Fehlern durch.

### Setup

1. **Slack-Webhook konfigurieren:**
   - Erstelle einen Slack-Webhook in deinem Workspace
   - Setze die URL als Environment-Variable: `export SLACK_WEBHOOK_URL="https://hooks.slack.com/services/YOUR/WEBHOOK"`

2. **Systemd Service einrichten (empfohlen für Linux-Produktion):**
   ```bash
   # Service-Datei kopieren
   sudo cp scripts/B2X-heartbeat.service /etc/systemd/system/
   sudo cp scripts/B2X-heartbeat.timer /etc/systemd/system/

   # Pfade anpassen in der .service Datei:
   # - WorkingDirectory=/path/to/B2X
   # - ExecStart=/path/to/B2X/scripts/runtime-health-check.sh ...
   # - User=B2X (oder entsprechender User)

   # Service aktivieren und starten
   sudo systemctl daemon-reload
   sudo systemctl enable B2X-heartbeat.timer
   sudo systemctl start B2X-heartbeat.timer
   ```

3. **Cron-Job Alternative (falls systemd nicht verfügbar):**
   ```bash
   # Cron-Job hinzufügen (zwei Einträge für 30s Intervall)
   crontab -e
   # Füge hinzu:
   * * * * * /path/to/B2X/scripts/runtime-health-check.sh --heartbeat --slack-webhook https://hooks.slack.com/services/YOUR/WEBHOOK
   * * * * * sleep 30; /path/to/B2X/scripts/runtime-health-check.sh --heartbeat --slack-webhook https://hooks.slack.com/services/YOUR/WEBHOOK
   ```

### Testen in Staging

1. **Staging-Umgebung starten:**
   ```bash
   # Services in Staging starten
   ./scripts/start-aspire.sh
   ```

2. **Heartbeat testen:**
   ```bash
   # Einzel-Check
   ./scripts/runtime-health-check.sh

   # Heartbeat-Modus (für Test)
   timeout 120 ./scripts/runtime-health-check.sh --heartbeat --slack-webhook YOUR_TEST_WEBHOOK
   ```

3. **Logs prüfen:**
   ```bash
   journalctl -u B2X-heartbeat.service -f  # Für systemd
   # Oder Script-Output direkt
   ```

### Monitoring

- **Systemd:** `systemctl status B2X-heartbeat.timer`
- **Logs:** `journalctl -u B2X-heartbeat.service`
- **Slack-Alerts:** Bei Fehlern werden automatische Benachrichtigungen gesendet
