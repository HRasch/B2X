using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using B2X.ErpConnector.Models;

namespace B2X.ErpConnector.Services
{
    /// <summary>
    /// Manages tenant API keys with secure hash storage.
    /// Keys are stored in a JSON file with SHA256 hashes (never plaintext).
    /// </summary>
    public class ApiKeyManager
    {
        private readonly string _keyFilePath;
        private readonly JavaScriptSerializer _serializer;
        private readonly object _lock = new object();
        private ApiKeyStore _store;
        private DateTime _lastLoadTime;
        private readonly TimeSpan _reloadInterval = TimeSpan.FromMinutes(1);

        public ApiKeyManager(string keyFilePath = null)
        {
            _keyFilePath = keyFilePath ?? GetDefaultKeyFilePath();
            _serializer = new JavaScriptSerializer();
            _store = new ApiKeyStore();
            LoadKeys();
        }

        private static string GetDefaultKeyFilePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configDir = Path.Combine(baseDir, "config");
            return Path.Combine(configDir, "api-keys.json");
        }

        /// <summary>
        /// Generates a new API key for a tenant.
        /// Returns the plaintext key (only shown once) and saves the hash.
        /// </summary>
        public string GenerateKey(string tenantId, string name, List<string> allowedBusinessUnits = null, DateTime? expiresAt = null)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID is required", nameof(tenantId));

            // Generate secure random key
            var randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            var randomPart = Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            // Key format: {tenantId}.{randomPart}
            var plainKey = $"{tenantId}.{randomPart}";
            var keyHash = ComputeHash(plainKey);
            var keyPrefix = $"{tenantId}.{randomPart.Substring(0, 4)}";

            var apiKey = new TenantApiKey
            {
                TenantId = tenantId,
                KeyHash = keyHash,
                KeyPrefix = keyPrefix,
                Name = name ?? "Default",
                AllowedBusinessUnits = allowedBusinessUnits ?? new List<string>(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsActive = true
            };

            lock (_lock)
            {
                _store.Keys.Add(apiKey);
                SaveKeys();
            }

            LogAudit("KEY_GENERATED", tenantId, $"New key created: {keyPrefix}");

            return plainKey;
        }

        /// <summary>
        /// Validates an API key and returns the associated tenant info.
        /// </summary>
        public ApiKeyValidationResult ValidateKey(string apiKey, string requestedBusinessUnit = null, string clientIp = null)
        {
            if (string.IsNullOrEmpty(apiKey))
                return ApiKeyValidationResult.Failure("API key is required");

            // Reload keys if stale
            ReloadIfNeeded();

            var keyHash = ComputeHash(apiKey);

            // Check admin key first
            if (!string.IsNullOrEmpty(_store.AdminKeyHash) && keyHash == _store.AdminKeyHash)
            {
                LogAudit("ADMIN_ACCESS", "admin", $"Admin key used from {clientIp}");
                return ApiKeyValidationResult.AdminSuccess();
            }

            // Find matching tenant key
            TenantApiKey matchedKey;
            lock (_lock)
            {
                matchedKey = _store.Keys.FirstOrDefault(k => k.KeyHash == keyHash);
            }

            if (matchedKey == null)
            {
                LogAudit("AUTH_FAILED", "unknown", $"Invalid key attempt from {clientIp}");
                return ApiKeyValidationResult.Failure("Invalid API key");
            }

            // Check if key is valid (active and not expired)
            if (!matchedKey.IsValid())
            {
                var reason = !matchedKey.IsActive ? "Key is deactivated" : "Key has expired";
                LogAudit("AUTH_FAILED", matchedKey.TenantId, $"{reason}: {matchedKey.KeyPrefix}");
                return ApiKeyValidationResult.Failure(reason);
            }

            // Check IP restriction
            if (matchedKey.AllowedIpAddresses != null && matchedKey.AllowedIpAddresses.Count > 0)
            {
                if (!string.IsNullOrEmpty(clientIp) && !matchedKey.AllowedIpAddresses.Contains(clientIp))
                {
                    LogAudit("AUTH_FAILED", matchedKey.TenantId, $"IP not allowed: {clientIp}");
                    return ApiKeyValidationResult.Failure("IP address not authorized");
                }
            }

            // Check business unit restriction
            if (!string.IsNullOrEmpty(requestedBusinessUnit) && !matchedKey.IsBusinessUnitAllowed(requestedBusinessUnit))
            {
                LogAudit("AUTH_FAILED", matchedKey.TenantId, $"BU not allowed: {requestedBusinessUnit}");
                return ApiKeyValidationResult.Failure($"Business unit '{requestedBusinessUnit}' not authorized for this key");
            }

            // Update last used timestamp
            UpdateLastUsed(matchedKey);

            LogAudit("AUTH_SUCCESS", matchedKey.TenantId, $"Key {matchedKey.KeyPrefix} authenticated from {clientIp}");
            return ApiKeyValidationResult.Success(matchedKey, requestedBusinessUnit);
        }

        /// <summary>
        /// Sets the admin key (for admin operations like key management).
        /// </summary>
        public string SetAdminKey()
        {
            var randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            var adminKey = "admin." + Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            lock (_lock)
            {
                _store.AdminKeyHash = ComputeHash(adminKey);
                SaveKeys();
            }

            LogAudit("ADMIN_KEY_SET", "system", "New admin key generated");
            return adminKey;
        }

        /// <summary>
        /// Deactivates a key by prefix.
        /// </summary>
        public bool DeactivateKey(string keyPrefix)
        {
            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix);
                if (key == null)
                    return false;

                key.IsActive = false;
                SaveKeys();
                LogAudit("KEY_DEACTIVATED", key.TenantId, $"Key deactivated: {keyPrefix}");
                return true;
            }
        }

        /// <summary>
        /// Rotates a key - deactivates old and generates new.
        /// </summary>
        public string RotateKey(string keyPrefix)
        {
            TenantApiKey oldKey;
            lock (_lock)
            {
                oldKey = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix && k.IsActive);
            }

            if (oldKey == null)
                throw new InvalidOperationException($"Active key not found: {keyPrefix}");

            // Generate new key with same settings
            var newKey = GenerateKey(
                oldKey.TenantId,
                oldKey.Name + " (rotated)",
                oldKey.AllowedBusinessUnits,
                oldKey.ExpiresAt
            );

            // Deactivate old key
            DeactivateKey(keyPrefix);

            LogAudit("KEY_ROTATED", oldKey.TenantId, $"Key rotated: {keyPrefix} -> new key");
            return newKey;
        }

        /// <summary>
        /// Sets ERP credentials for a tenant key.
        /// Credentials are encrypted using DPAPI - can only be decrypted on this machine.
        /// </summary>
        public bool SetErpCredentials(string keyPrefix, string erpUsername, string erpPassword, string defaultBusinessUnit = null)
        {
            if (string.IsNullOrWhiteSpace(keyPrefix))
                throw new ArgumentException("Key prefix is required", nameof(keyPrefix));
            if (string.IsNullOrWhiteSpace(erpUsername))
                throw new ArgumentException("ERP username is required", nameof(erpUsername));
            if (string.IsNullOrWhiteSpace(erpPassword))
                throw new ArgumentException("ERP password is required", nameof(erpPassword));

            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix && k.IsActive);
                if (key == null)
                    return false;

                // Encrypt credentials using DPAPI (machine-specific)
                key.ErpUsernameEncrypted = CredentialProtection.Encrypt(erpUsername);
                key.ErpPasswordEncrypted = CredentialProtection.Encrypt(erpPassword);
                key.ErpDefaultBusinessUnit = defaultBusinessUnit;

                SaveKeys();
                LogAudit("ERP_CREDS_SET", key.TenantId, $"ERP credentials configured for key: {keyPrefix}");
                return true;
            }
        }

        /// <summary>
        /// Removes ERP credentials from a tenant key.
        /// </summary>
        public bool RemoveErpCredentials(string keyPrefix)
        {
            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix);
                if (key == null)
                    return false;

                key.ErpUsernameEncrypted = null;
                key.ErpPasswordEncrypted = null;
                key.ErpDefaultBusinessUnit = null;

                SaveKeys();
                LogAudit("ERP_CREDS_REMOVED", key.TenantId, $"ERP credentials removed from key: {keyPrefix}");
                return true;
            }
        }

        /// <summary>
        /// Checks if a key has ERP credentials configured.
        /// </summary>
        public bool HasErpCredentials(string keyPrefix)
        {
            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix);
                return key?.HasErpCredentials ?? false;
            }
        }

        /// <summary>
        /// Checks if a key has ERP credentials configured.
        /// </summary>
        public bool HasErpCredentials(string keyPrefix)
        {
            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix);
                return key?.HasErpCredentials ?? false;
            }
        }

        /// <summary>
        /// Creates an ERP service account for bidirectional API access.
        /// ERP service accounts have limited permissions and are used by enventa Trade ERP
        /// to securely call B2X APIs for customer/product updates and usage statistics.
        /// </summary>
        public string CreateErpServiceAccount(string tenantId, string name, List<ErpServicePermission> permissions, DateTime? expiresAt = null)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID is required", nameof(tenantId));
            if (permissions == null || permissions.Count == 0)
                throw new ArgumentException("At least one ERP permission is required", nameof(permissions));

            // Generate secure random key
            var randomBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            var randomPart = Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            // ERP service account key format: {tenantId}.erp.{randomPart}
            var plainKey = $"{tenantId}.erp.{randomPart}";
            var keyHash = ComputeHash(plainKey);
            var keyPrefix = $"{tenantId}.erp.{randomPart.Substring(0, 4)}";

            var apiKey = new TenantApiKey
            {
                TenantId = tenantId,
                KeyHash = keyHash,
                KeyPrefix = keyPrefix,
                Name = name ?? "ERP Service Account",
                AllowedBusinessUnits = new List<string>(), // ERP service accounts are not restricted by business units
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsActive = true,
                IsErpServiceAccount = true,
                ErpServicePermissions = permissions
            };

            lock (_lock)
            {
                _store.Keys.Add(apiKey);
                SaveKeys();
            }

            LogAudit("ERP_SERVICE_ACCOUNT_CREATED", tenantId, $"ERP service account created: {keyPrefix} with permissions: {string.Join(", ", permissions)}");

            return plainKey;
        }

        /// <summary>
        /// Updates permissions for an ERP service account.
        /// </summary>
        public bool UpdateErpServiceAccountPermissions(string keyPrefix, List<ErpServicePermission> permissions)
        {
            if (string.IsNullOrWhiteSpace(keyPrefix))
                throw new ArgumentException("Key prefix is required", nameof(keyPrefix));
            if (permissions == null || permissions.Count == 0)
                throw new ArgumentException("At least one ERP permission is required", nameof(permissions));

            lock (_lock)
            {
                var key = _store.Keys.FirstOrDefault(k => k.KeyPrefix == keyPrefix && k.IsErpServiceAccount);
                if (key == null)
                    return false;

                key.ErpServicePermissions = permissions;
                SaveKeys();

                LogAudit("ERP_SERVICE_ACCOUNT_UPDATED", key.TenantId, $"ERP service account permissions updated: {keyPrefix}");
                return true;
            }
        }

        /// <summary>
        /// Lists all ERP service accounts for a tenant.
        /// </summary>
        public List<TenantApiKey> ListErpServiceAccounts(string tenantId = null)
        {
            ReloadIfNeeded();
            lock (_lock)
            {
                var query = _store.Keys.Where(k => k.IsErpServiceAccount && k.IsActive);
                if (!string.IsNullOrEmpty(tenantId))
                    query = query.Where(k => k.TenantId == tenantId);

                // Return copies without sensitive data
                return query.Select(k => new TenantApiKey
                {
                    TenantId = k.TenantId,
                    KeyPrefix = k.KeyPrefix,
                    Name = k.Name,
                    CreatedAt = k.CreatedAt,
                    ExpiresAt = k.ExpiresAt,
                    IsActive = k.IsActive,
                    IsErpServiceAccount = k.IsErpServiceAccount,
                    ErpServicePermissions = k.ErpServicePermissions,
                    LastUsedAt = k.LastUsedAt
                }).ToList();
            }
        }

        /// <summary>
        /// Validates if an ERP service account has the required permission.
        /// </summary>
        public bool ValidateErpServiceAccountPermission(string apiKey, ErpServicePermission requiredPermission)
        {
            var validation = ValidateKey(apiKey);
            if (!validation.IsValid || !validation.IsErpServiceAccount)
                return false;

            return validation.ErpPermissions.Contains(requiredPermission);
        }

        /// <summary>
        /// Lists all keys for a tenant (without exposing hashes).
        /// </summary>
        public List<TenantApiKey> ListKeys(string tenantId = null)
        {
            ReloadIfNeeded();
            lock (_lock)
            {
                var query = _store.Keys.AsEnumerable();
                if (!string.IsNullOrEmpty(tenantId))
                    query = query.Where(k => k.TenantId == tenantId);

                // Return copies without the hash
                return query.Select(k => new TenantApiKey
                {
                    TenantId = k.TenantId,
                    KeyPrefix = k.KeyPrefix,
                    Name = k.Name,
                    AllowedBusinessUnits = k.AllowedBusinessUnits,
                    CreatedAt = k.CreatedAt,
                    ExpiresAt = k.ExpiresAt,
                    IsActive = k.IsActive,
                    LastUsedAt = k.LastUsedAt,
                    AllowedIpAddresses = k.AllowedIpAddresses,
                    ErpDefaultBusinessUnit = k.ErpDefaultBusinessUnit,
                    // Show whether credentials are configured (not the actual values)
                    ErpUsernameEncrypted = k.HasErpCredentials ? "[CONFIGURED]" : null
                    // KeyHash and ErpPasswordEncrypted intentionally omitted
                }).ToList();
            }
        }

        /// <summary>
        /// Check if any keys exist (for first-run detection).
        /// </summary>
        public bool HasKeys()
        {
            lock (_lock)
            {
                return !string.IsNullOrEmpty(_store.AdminKeyHash) || _store.Keys.Count > 0;
            }
        }

        private void LoadKeys()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(_keyFilePath))
                    {
                        var json = File.ReadAllText(_keyFilePath);
                        _store = _serializer.Deserialize<ApiKeyStore>(json) ?? new ApiKeyStore();
                    }
                    else
                    {
                        _store = new ApiKeyStore();
                        // Ensure directory exists
                        var dir = Path.GetDirectoryName(_keyFilePath);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                    }
                    _lastLoadTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SECURITY] Error loading API keys: {ex.Message}");
                    _store = new ApiKeyStore();
                }
            }
        }

        private void SaveKeys()
        {
            try
            {
                var dir = Path.GetDirectoryName(_keyFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = _serializer.Serialize(_store);
                File.WriteAllText(_keyFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SECURITY] Error saving API keys: {ex.Message}");
            }
        }

        private void ReloadIfNeeded()
        {
            if (DateTime.UtcNow - _lastLoadTime > _reloadInterval)
            {
                LoadKeys();
            }
        }

        private void UpdateLastUsed(TenantApiKey key)
        {
            lock (_lock)
            {
                var storedKey = _store.Keys.FirstOrDefault(k => k.KeyHash == key.KeyHash);
                if (storedKey != null)
                {
                    storedKey.LastUsedAt = DateTime.UtcNow;
                    // Save periodically, not on every request
                    if (DateTime.UtcNow - _lastLoadTime > TimeSpan.FromMinutes(5))
                    {
                        SaveKeys();
                        _lastLoadTime = DateTime.UtcNow;
                    }
                }
            }
        }

        private static string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private static void LogAudit(string action, string tenantId, string message)
        {
            var timestamp = DateTime.UtcNow.ToString("o");
            Console.WriteLine($"[AUDIT] {timestamp} | {action} | tenant={tenantId} | {message}");
        }
    }
}
