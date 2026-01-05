# CLI Auto-Update Functionality - Brainstorm

**Date:** January 5, 2026  
**Context:** ADR-031 CLI Architecture Split  
**Authors:** @Architect, @DevOps, @Security  
**Status:** Brainstorm - Initial Ideas

---

## Problem Statement

CLI tools need to stay current with:
- Security patches and vulnerability fixes
- New features and bug fixes
- API compatibility updates
- Performance improvements

Current challenges:
- Manual update process (`dotnet tool update`) requires user awareness
- Version drift across different users/teams
- Delayed security patch application
- Inconsistent tool versions in CI/CD pipelines

---

## Solution Concepts

### 1. Native .NET Tool Updates
**Leverage existing `dotnet tool` ecosystem**

#### Implementation
```bash
# Automatic update check on startup
dotnet tool update B2Connect.CLI.Administration --global

# Or via package management
dotnet tool update --all --global
```

#### Pros
- ✅ Uses established .NET tooling
- ✅ No custom implementation needed
- ✅ Handles dependencies automatically
- ✅ Works offline (when packages cached)

#### Cons
- ❌ Requires .NET SDK installed
- ❌ No selective update control
- ❌ No rollback capability
- ❌ No progress indication

### 2. Custom Update Service
**Build update checker into CLI itself**

#### Architecture
```
CLI Startup Flow:
1. Check for updates (configurable)
2. Download new version if available
3. Replace executable
4. Restart with new version
```

#### Implementation Options

**Option A: GitHub Releases Integration**
```csharp
public class UpdateService
{
    private readonly HttpClient _client;
    private readonly string _currentVersion;
    private readonly string _repoUrl;

    public async Task<UpdateInfo> CheckForUpdates()
    {
        var releases = await _client.GetFromJsonAsync<GitHubRelease[]>(
            $"{_repoUrl}/releases"
        );

        var latest = releases.FirstOrDefault(r => !r.Prerelease);
        return latest?.TagName != _currentVersion
            ? new UpdateInfo { Available = true, Version = latest.TagName }
            : new UpdateInfo { Available = false };
    }
}
```

**Option B: Internal Update API**
```csharp
// For Operations CLI (internal)
var updateUrl = "https://updates.b2connect.internal/api/cli-updates";

// For Administration CLI (public)
var updateUrl = "https://api.b2connect.com/cli-updates";
```

**Option C: Self-Contained Updater**
```bash
# CLI downloads updater executable
curl -o updater.exe https://updates.b2connect.com/updater.exe
./updater.exe --update-cli --version latest
```

#### Pros
- ✅ Full control over update process
- ✅ Can implement rollback
- ✅ Progress indication possible
- ✅ Version-specific updates
- ✅ Can work without .NET SDK

#### Cons
- ❌ Complex implementation
- ❌ Security risks (malicious updates)
- ❌ Platform-specific binaries needed
- ❌ Network dependency

### 3. Hybrid Approach
**Combine native tooling with custom enhancements**

#### Implementation
```csharp
public class SmartUpdateService
{
    public async Task UpdateIfNeeded(UpdateMode mode)
    {
        switch (mode)
        {
            case UpdateMode.Automatic:
                await PerformSilentUpdate();
                break;
            case UpdateMode.Notify:
                var update = await CheckForUpdates();
                if (update.Available)
                    await PromptUser(update);
                break;
            case UpdateMode.Manual:
                // User-initiated only
                break;
        }
    }
}
```

---

## Security Considerations

### Update Verification
- **Code Signing:** All releases must be signed
- **Checksum Verification:** SHA256 hashes for integrity
- **Certificate Pinning:** Prevent MITM attacks
- **Trust Chain:** Validate update source authenticity

### Attack Vectors
- **Malicious Updates:** Compromised update server
- **Downgrade Attacks:** Force older vulnerable versions
- **Update Spam:** Frequent unnecessary updates
- **Network Interception:** Man-in-the-middle during download

### Mitigation Strategies
```csharp
public class SecureUpdateService
{
    private readonly ICertificateValidator _certValidator;
    private readonly IChecksumVerifier _checksumVerifier;

    public async Task<bool> VerifyUpdate(UpdatePackage package)
    {
        // Verify certificate chain
        if (!await _certValidator.Validate(package.Certificate))
            throw new SecurityException("Invalid certificate");

        // Verify checksum
        if (!await _checksumVerifier.Verify(package.Checksum))
            throw new SecurityException("Checksum mismatch");

        return true;
    }
}
```

---

## User Experience Design

### Update Modes
1. **Silent (Automatic)**
   - Updates happen in background
   - No user interaction required
   - Best for CI/CD and automated environments

2. **Notify (User Choice)**
   - CLI notifies of available update
   - User decides when to update
   - Good balance of automation and control

3. **Manual (User Initiated)**
   - User explicitly runs update command
   - Full control over timing
   - Minimal automation

### Configuration
```json
{
  "updates": {
    "mode": "notify",
    "checkInterval": "24h",
    "autoRestart": true,
    "rollbackEnabled": true,
    "maxDownloadSize": "50MB"
  }
}
```

### User Interface
```
🔄 Update available: v2.1.0 (current: v2.0.0)
   New features: Enhanced catalog import, security fixes
   Release notes: https://github.com/b2connect/cli/releases/v2.1.0

   [U] Update now  [L] Later  [N] Never ask again  [?] More info
```

---

## Platform-Specific Considerations

### Windows
- Use Windows Update APIs for integration
- Support for enterprise update policies
- Handle UAC prompts for system-level updates

### macOS
- Integration with Sparkle framework
- App Store sandbox considerations
- Gatekeeper compatibility

### Linux
- Package manager integration (apt, yum, etc.)
- Systemd service updates
- Container environment handling

### CI/CD Environments
- Respect CI/CD environment variables
- No interactive prompts
- Prefer silent updates
- Integration with deployment pipelines

---

## Implementation Complexity

### Effort Estimate
- **Basic Version Check:** 2-3 days
- **Full Auto-Update:** 1-2 weeks
- **Cross-Platform:** Additional 1 week
- **Security Hardening:** 3-5 days
- **Testing:** 1 week

### Dependencies
- **NuGet Packages:**
  - `System.Net.Http.Json` (for API calls)
  - `Microsoft.Extensions.Http` (for HTTP client)
  - `Octokit` (GitHub API integration)

- **External Services:**
  - Update metadata API
  - CDN for binary distribution
  - Certificate authority for signing

### Testing Requirements
- Unit tests for update logic
- Integration tests for update process
- Network failure simulation
- Rollback testing
- Cross-platform compatibility
- Security testing (penetration testing)

---

## Business Impact

### Benefits
- **Security:** Faster patch deployment
- **User Experience:** Always current features
- **Support:** Reduced "please update" tickets
- **Compliance:** Easier to enforce version standards

### Risks
- **Downtime:** Failed updates could break CLI
- **Support:** More complex troubleshooting
- **Cost:** Development and maintenance overhead
- **Compatibility:** Update conflicts with custom environments

### Success Metrics
- **Adoption Rate:** % of users with auto-update enabled
- **Update Success Rate:** % of updates that complete successfully
- **Time to Update:** Average time between release and user update
- **Support Ticket Reduction:** % decrease in version-related issues

---

## Recommended Approach

### Phase 1: Basic Update Notification (Low Risk)
```bash
# Add to CLI startup
b2connect-admin check-updates
b2connect-admin update
```

**Implementation:**
- Simple version check against GitHub releases
- Manual update command
- No automatic downloads
- Easy rollback via `dotnet tool`

### Phase 2: Smart Update Service (Medium Risk)
- Background update checking
- User notification system
- Safe update process with rollback
- Configuration management

### Phase 3: Full Automation (High Risk)
- Silent updates for CI/CD
- Advanced rollback capabilities
- Enterprise integration
- Advanced security features

---

## Integration with ADR-031

### Operations CLI (Internal)
- **Update Mode:** Automatic (silent updates)
- **Update Source:** Internal artifact repository
- **Security:** Enterprise certificate validation
- **Monitoring:** Update success/failure telemetry

### Administration CLI (Customer-Facing)
- **Update Mode:** Notify (user choice)
- **Update Source:** Public NuGet or GitHub releases
- **Security:** Standard code signing
- **Monitoring:** Anonymous usage telemetry

### Shared Library Integration
```csharp
// Add to B2Connect.CLI.Shared
public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdates();
    Task UpdateToVersion(string version);
    Task RollbackVersion();
}
```

---

## Next Steps

1. **Prototype:** Build basic update checker
2. **Security Review:** Validate approach with @Security
3. **User Research:** Survey potential users on preferences
4. **Technical Spike:** Implement proof-of-concept
5. **Decision:** Create ADR for auto-update feature

---

## Open Questions

- Should updates be enabled by default?
- How to handle enterprise proxy environments?
- What's the update check frequency?
- How to handle beta/pre-release versions?
- Should we support offline updates?
- Integration with existing deployment pipelines?

---

**Status:** Brainstorm Complete - Ready for prototyping  
**Next Action:** Create technical spike for basic update notification</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/ADR-032-cli-auto-update-brainstorm.md