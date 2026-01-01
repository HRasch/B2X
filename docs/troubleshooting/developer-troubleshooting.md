# Troubleshooting Guide: B2Connect Development Environment

**Last Updated:** January 1, 2026  
**Applies to:** B2Connect v1.0+  
**Audience:** Developers

## Quick Diagnosis
Use this flowchart to quickly identify development issues:

```
Build failing?
├── Yes → Check [dependencies and compilation errors]
├── No → App not starting?
    ├── Yes → Check [configuration and services]
    ├── No → Runtime errors?
        ├── Yes → Check [logs and debugging]
        └── No → Contact team lead
```

## Common Issues and Solutions

### Issue 1: Build Failures
**Symptoms:**
- Compilation errors in Visual Studio/VS Code
- dotnet build command fails
- Package restore errors

**Possible Causes:**
1. Missing NuGet packages
2. .NET version mismatch
3. Missing environment variables

**Solutions:**

#### Solution A: Dependency Issues
1. Run `dotnet restore` in the solution root
2. Clear NuGet cache: `dotnet nuget locals all --clear`
3. Delete bin/ and obj/ folders
4. Rebuild solution

**Expected Result:** All packages restored, build succeeds

#### Solution B: .NET Version Mismatch
1. Check global.json for correct .NET version
2. Install required SDK: `dotnet --list-sdks`
3. Update Visual Studio if needed

**Prevention:** Keep .NET SDK updated and check global.json

### Issue 2: Database Connection Issues
**Symptoms:**
- "Cannot connect to database" errors
- Migration failures
- Entity Framework timeouts

**Possible Causes:**
1. PostgreSQL not running
2. Incorrect connection string
3. Database not initialized

**Solutions:**

#### Solution A: Local Database
1. Ensure PostgreSQL is running: `brew services list`
2. Check connection string in appsettings.json
3. Run migrations: `dotnet ef database update`

**Expected Result:** Database connection successful

#### Solution B: Aspire Environment
1. Verify Aspire dashboard is running (localhost:15500)
2. Check service dependencies in AppHost
3. Review container logs

**Prevention:** Use Aspire for local development orchestration

### Issue 3: Frontend Build Issues
**Symptoms:**
- npm install fails
- Vite build errors
- TypeScript compilation failures

**Possible Causes:**
1. Node.js version mismatch
2. Missing dependencies
3. TypeScript configuration issues

**Solutions:**

#### Solution A: Dependency Issues
1. Clear node_modules: `rm -rf node_modules package-lock.json`
2. Reinstall: `npm install`
3. Check Node version: `node --version` (should be 18+)

**Expected Result:** Clean install succeeds

#### Solution B: TypeScript Errors
1. Run type check: `npm run type-check`
2. Fix type errors in code
3. Update tsconfig.json if needed

**Prevention:** Run type-check before commits

### Issue 4: Authentication Problems
**Symptoms:**
- Login fails
- JWT token errors
- Authorization denied

**Possible Causes:**
1. Identity service not running
2. Incorrect JWT configuration
3. User account issues

**Solutions:**

#### Solution A: Service Dependencies
1. Check Identity service in Aspire dashboard
2. Verify JWT settings in appsettings.json
3. Test with known good credentials

**Expected Result:** Authentication works

#### Solution B: Token Issues
1. Check token expiration
2. Verify signing keys
3. Clear browser storage

**Prevention:** Use consistent test accounts

## Diagnostic Tools

### Tool 1: Aspire Dashboard
**Purpose:** Monitor service health and dependencies

**How to Use:**
1. Start AppHost: `dotnet run --project AppHost`
2. Open http://localhost:15500
3. Check service status and logs

**Interpreting Results:**
- Green: Service healthy
- Yellow: Warnings present
- Red: Service failed

### Tool 2: Application Logs
**Purpose:** Debug runtime issues

**How to Use:**
```bash
# View logs in terminal
dotnet run --project [ServiceName]

# Or check Aspire dashboard logs
```

**Interpreting Results:**
- INFO: Normal operations
- WARN: Potential issues
- ERROR: Failures requiring attention

### Tool 3: Database Tools
**Purpose:** Check database state and queries

**How to Use:**
```bash
# Connect to PostgreSQL
psql -U b2connect -d b2connect_dev

# Check tables
\dt

# View recent queries
SELECT * FROM information_schema.processlist;
```

## Logs and Debugging

### Where to Find Logs
- **Backend**: Console output or Serilog files in logs/
- **Frontend**: Browser dev tools console
- **Database**: PostgreSQL logs in /usr/local/var/log/postgresql/
- **Aspire**: Dashboard at localhost:15500

### Enabling Debug Mode
1. Set environment: `ASPNETCORE_ENVIRONMENT=Development`
2. Enable detailed logging in appsettings.Development.json
3. Use debugger in VS Code/Visual Studio

### Common Log Messages
| Message | Meaning | Action |
|---------|---------|--------|
| Connection timeout | Database unreachable | Check PostgreSQL service |
| Authentication failed | Invalid credentials | Verify JWT config |
| Build failed | Compilation error | Check code for syntax errors |

## Advanced Troubleshooting

### For Backend Developers
- Use Entity Framework debugging: `context.Database.Log`
- Enable SQL query logging in EF Core
- Use distributed tracing with OpenTelemetry

### For Frontend Developers
- Use Vue DevTools for component debugging
- Enable Vite HMR for hot reload
- Use browser network tab for API calls

### Escalation
If these steps don't resolve the issue:
1. Gather logs and error messages
2. Document steps attempted
3. Create issue in project repository
4. Tag relevant team members

## Prevention and Maintenance

### Regular Maintenance Tasks
- **Weekly**: Update dependencies and run full test suite
- **Daily**: Check Aspire dashboard for service health
- **Before commits**: Run linting and type checks

### Monitoring
- Service response times
- Error rates in logs
- Database connection pool usage
- Build success rates

## Related Resources
- [Quick Start Guide](QUICK_START_GUIDE.md)
- [Architecture Documentation](docs/architecture/)
- [API Documentation](api/)
- [Contributing Guide](CONTRIBUTING.md)

## Changelog
- 2026-01-01: Initial version with common development issues

---
*Contribute to this guide: Create PR with improvements*