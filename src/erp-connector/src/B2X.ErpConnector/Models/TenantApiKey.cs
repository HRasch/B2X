using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace B2X.ErpConnector.Models
{
    /// <summary>
    /// Helper for encrypting/decrypting ERP credentials using Windows DPAPI.
    /// DPAPI ensures credentials can only be decrypted on this specific machine.
    /// </summary>
    public static class CredentialProtection
    {
        /// <summary>
        /// Encrypt a string using DPAPI (machine-specific).
        /// </summary>
        public static string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return string.Empty;

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] encryptedBytes = ProtectedData.Protect(
                plaintextBytes,
                null, // Optional entropy
                DataProtectionScope.LocalMachine // Can only decrypt on this machine
            );
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypt a DPAPI-protected string.
        /// </summary>
        public static string Decrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return string.Empty;

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encrypted);
                byte[] decryptedBytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    null,
                    DataProtectionScope.LocalMachine
                );
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                // Decryption failed (wrong machine, corrupted data, etc.)
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Decrypted ERP credentials (never stored, only in memory).
    /// </summary>
    public class ErpCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BusinessUnit { get; set; } = string.Empty;

        public bool IsValid => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
    }

    /// <summary>
    /// Permissions for ERP service accounts (bidirectional API access).
    /// These define what operations enventa Trade ERP can perform on B2X APIs.
    /// </summary>
    public enum ErpServicePermission
    {
        /// <summary>
        /// Read customer information (GET /api/erp/customers)
        /// </summary>
        ReadCustomers,

        /// <summary>
        /// Update customer information (PUT /api/erp/customers/{id})
        /// </summary>
        UpdateCustomers,

        /// <summary>
        /// Read product catalog information (GET /api/erp/products)
        /// </summary>
        ReadProducts,

        /// <summary>
        /// Update product information (PUT /api/erp/products/{id})
        /// </summary>
        UpdateProducts,

        /// <summary>
        /// Read usage statistics (GET /api/erp/usage)
        /// </summary>
        ReadUsageStats,

        /// <summary>
        /// Manage access permissions (POST/PUT/DELETE /api/erp/access)
        /// </summary>
        ManageAccess,

        /// <summary>
        /// Receive webhook notifications for B2X events
        /// </summary>
        ReceiveWebhooks
    }

    /// <summary>
    /// Represents a tenant-coupled API key for secure, multi-tenant access.
    /// Keys are stored hashed - the plaintext key is only shown once during generation.
    /// </summary>
    public class TenantApiKey
    {
        /// <summary>
        /// Unique tenant identifier this key belongs to.
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// SHA256 hash of the API key (never store plaintext).
        /// </summary>
        public string KeyHash { get; set; } = string.Empty;

        /// <summary>
        /// First 8 characters of the key for identification/debugging.
        /// Format: "{tenantId}.{first4random}"
        /// </summary>
        public string KeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Business units this key is authorized to access.
        /// Empty list = unrestricted (all business units allowed).
        /// </summary>
        public List<string> AllowedBusinessUnits { get; set; } = new List<string>();

        /// <summary>
        /// Friendly name for the key (e.g., "Production", "Integration Test").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// When this key was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this key expires. Null = never expires.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Whether this key is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Last time this key was used for authentication.
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        /// <summary>
        /// IP addresses allowed to use this key. Empty = all IPs allowed.
        /// </summary>
        public List<string> AllowedIpAddresses { get; set; } = new List<string>();

        // ========== ERP CREDENTIALS (stored encrypted with DPAPI) ==========
        // These credentials NEVER leave the on-premise ERP Connector.
        // Even if B2X cloud is compromised, attackers cannot access ERP.

        /// <summary>
        /// ERP username (encrypted with DPAPI).
        /// </summary>
        public string? ErpUsernameEncrypted { get; set; }

        /// <summary>
        /// ERP password (encrypted with DPAPI).
        /// </summary>
        public string? ErpPasswordEncrypted { get; set; }

        /// <summary>
        /// Default business unit for ERP operations.
        /// </summary>
        public string? ErpDefaultBusinessUnit { get; set; }

        /// <summary>
        /// Whether this is an ERP service account (for bidirectional API access).
        /// ERP service accounts have limited permissions and are used by enventa Trade ERP
        /// to securely call B2X APIs for customer/product updates and usage statistics.
        /// </summary>
        public bool IsErpServiceAccount { get; set; } = false;

        /// <summary>
        /// ERP service account permissions (only applies when IsErpServiceAccount = true).
        /// Defines what operations the ERP system can perform on B2X APIs.
        /// </summary>
        public List<ErpServicePermission> ErpServicePermissions { get; set; } = new List<ErpServicePermission>();

        /// <summary>
        /// Whether ERP credentials have been configured for this key.
        /// </summary>
        public bool HasErpCredentials => !string.IsNullOrEmpty(ErpUsernameEncrypted);

        /// <summary>
        /// Check if this key is valid (active and not expired).
        /// </summary>
        public bool IsValid()
        {
            if (!IsActive)
                return false;

            if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
                return false;

            return true;
        }

        /// <summary>
        /// Check if a business unit is allowed for this key.
        /// </summary>
        public bool IsBusinessUnitAllowed(string businessUnit)
        {
            if (AllowedBusinessUnits == null || AllowedBusinessUnits.Count == 0)
                return true;

            return AllowedBusinessUnits.Contains(businessUnit);
        }

        /// <summary>
        /// Check if this ERP service account has a specific permission.
        /// Only applies when IsErpServiceAccount = true.
        /// </summary>
        public bool HasErpPermission(ErpServicePermission permission)
        {
            if (!IsErpServiceAccount)
                return false;

            return ErpServicePermissions.Contains(permission);
        }

        /// <summary>
        /// Check if this ERP service account has any of the specified permissions.
        /// Only applies when IsErpServiceAccount = true.
        /// </summary>
        public bool HasAnyErpPermission(params ErpServicePermission[] permissions)
        {
            if (!IsErpServiceAccount)
                return false;

            return permissions.Any(p => ErpServicePermissions.Contains(p));
        }

        /// <summary>
        /// Get all permissions for this ERP service account.
        /// Only applies when IsErpServiceAccount = true.
        /// </summary>
        public IEnumerable<ErpServicePermission> GetErpPermissions()
        {
            return IsErpServiceAccount ? ErpServicePermissions : Enumerable.Empty<ErpServicePermission>();
        }
    }

    /// <summary>
    /// Container for API keys JSON file.
    /// </summary>
    public class ApiKeyStore
    {
        /// <summary>
        /// Master admin key hash for admin operations.
        /// </summary>
        public string AdminKeyHash { get; set; } = string.Empty;

        /// <summary>
        /// List of tenant API keys.
        /// </summary>
        public List<TenantApiKey> Keys { get; set; } = new List<TenantApiKey>();
    }

    /// <summary>
    /// Result of API key validation.
    /// </summary>
    public class ApiKeyValidationResult
    {
        public bool IsValid { get; set; }
        public string? TenantId { get; set; }
        public TenantApiKey? Key { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsAdmin { get; set; }

        /// <summary>
        /// ERP credentials (decrypted) if configured for this key.
        /// </summary>
        public ErpCredentials? ErpCredentials { get; set; }

        /// <summary>
        /// Whether this is an ERP service account validation.
        /// </summary>
        public bool IsErpServiceAccount { get; set; }

        /// <summary>
        /// ERP service permissions (only populated for ERP service accounts).
        /// </summary>
        public List<ErpServicePermission> ErpPermissions { get; set; } = new List<ErpServicePermission>();

        public static ApiKeyValidationResult Success(TenantApiKey key, string? requestedBusinessUnit = null)
        {
            ErpCredentials? creds = null;

            // Decrypt credentials if configured
            if (key.HasErpCredentials)
            {
                creds = new ErpCredentials
                {
                    Username = CredentialProtection.Decrypt(key.ErpUsernameEncrypted ?? ""),
                    Password = CredentialProtection.Decrypt(key.ErpPasswordEncrypted ?? ""),
                    BusinessUnit = requestedBusinessUnit ?? key.ErpDefaultBusinessUnit ?? ""
                };
            }

            return new ApiKeyValidationResult
            {
                IsValid = true,
                TenantId = key.TenantId,
                Key = key,
                ErpCredentials = creds,
                IsErpServiceAccount = key.IsErpServiceAccount,
                ErpPermissions = key.IsErpServiceAccount ? key.ErpServicePermissions : new List<ErpServicePermission>()
            };
        }

        public static ApiKeyValidationResult AdminSuccess()
        {
            return new ApiKeyValidationResult
            {
                IsValid = true,
                IsAdmin = true
            };
        }

        public static ApiKeyValidationResult Failure(string message)
        {
            return new ApiKeyValidationResult
            {
                IsValid = false,
                ErrorMessage = message
            };
        }
    }
}
