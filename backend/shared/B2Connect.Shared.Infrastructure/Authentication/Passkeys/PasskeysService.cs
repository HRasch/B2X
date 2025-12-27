using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Shared.Infrastructure.Authentication.Passkeys;

/// <summary>
/// Passkeys (FIDO2/WebAuthn) Service für sichere authentifizierungslos Anmeldung
/// </summary>
public interface IPasskeysService
{
    Task<PasskeyRegistrationOptions> StartRegistrationAsync(string userId, string userName, string userEmail);
    Task<PasskeyRegistrationResult> CompleteRegistrationAsync(string userId, PasskeyRegistrationResponse response);
    Task<PasskeyAuthenticationOptions> StartAuthenticationAsync(string? username = null);
    Task<PasskeyAuthenticationResult> CompleteAuthenticationAsync(PasskeyAuthenticationResponse response);
    Task<bool> DeletePasskeyAsync(string userId, string credentialId);
    Task<IEnumerable<PasskeyInfo>> GetUserPasskeysAsync(string userId);
}

/// <summary>
/// Implementation of Passkeys Service
/// </summary>
public class PasskeysService : IPasskeysService
{
    private readonly ILogger<PasskeysService> _logger;
    private readonly IConfiguration _configuration;

    public PasskeysService(ILogger<PasskeysService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<PasskeyRegistrationOptions> StartRegistrationAsync(string userId, string userName, string userEmail)
    {
        try
        {
            var challenge = GenerateChallenge();

            return new PasskeyRegistrationOptions
            {
                Challenge = Convert.ToBase64String(challenge),
                RP = new RelyingPartyInfo
                {
                    Name = _configuration["Auth:Passkeys:RP:Name"] ?? "B2Connect",
                    Id = GetRelyingPartyId(),
                },
                User = new UserInfo
                {
                    Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId)),
                    Name = userName,
                    DisplayName = userEmail,
                },
                PubKeyCredParams = new[]
                {
                    new PublicKeyCredentialParameters { Alg = -7, Type = "public-key" },  // ES256
                    new PublicKeyCredentialParameters { Alg = -257, Type = "public-key" }, // RS256
                },
                Attestation = _configuration["Auth:Passkeys:AttestationConveyance"] ?? "none",
                AuthenticatorSelection = new AuthenticatorSelectionInfo
                {
                    AuthenticatorAttachment = "platform",
                    ResidentKey = _configuration["Auth:Passkeys:AuthenticatorSelection:Resident"] ?? "true",
                    UserVerification = _configuration["Auth:Passkeys:UserVerification"] ?? "preferred",
                },
                Timeout = int.Parse(_configuration["Auth:Passkeys:Challenge:Timeout"] ?? "60000"),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting passkey registration");
            throw;
        }
    }

    public async Task<PasskeyRegistrationResult> CompleteRegistrationAsync(string userId, PasskeyRegistrationResponse response)
    {
        try
        {
            // Validate response
            if (string.IsNullOrEmpty(response.Id) || string.IsNullOrEmpty(response.RawId))
            {
                return new PasskeyRegistrationResult { Success = false, Error = "Invalid credential response" };
            }

            // In production: Verify attestation using WebAuthn libraries
            // For now: Basic validation
            var credentialId = Convert.FromBase64String(response.RawId);

            return new PasskeyRegistrationResult
            {
                Success = true,
                CredentialId = response.RawId,
                PublicKey = response.AttestationObject ?? "",
                SignCount = 0,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing passkey registration for user {UserId}", userId);
            return new PasskeyRegistrationResult { Success = false, Error = ex.Message };
        }
    }

    public async Task<PasskeyAuthenticationOptions> StartAuthenticationAsync(string? username = null)
    {
        try
        {
            var challenge = GenerateChallenge();

            return new PasskeyAuthenticationOptions
            {
                Challenge = Convert.ToBase64String(challenge),
                Timeout = int.Parse(_configuration["Auth:Passkeys:Challenge:Timeout"] ?? "60000"),
                UserVerification = _configuration["Auth:Passkeys:UserVerification"] ?? "preferred",
                AllowCredentials = username != null ? new[] { new CredentialDescriptor { Type = "public-key" } } : Array.Empty<CredentialDescriptor>(),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting passkey authentication");
            throw;
        }
    }

    public async Task<PasskeyAuthenticationResult> CompleteAuthenticationAsync(PasskeyAuthenticationResponse response)
    {
        try
        {
            // Validate response
            if (string.IsNullOrEmpty(response.Id) || string.IsNullOrEmpty(response.ClientDataJSON))
            {
                return new PasskeyAuthenticationResult { Success = false, Error = "Invalid assertion response" };
            }

            // In production: Verify signature and counter using WebAuthn libraries
            // For now: Basic validation

            return new PasskeyAuthenticationResult
            {
                Success = true,
                CredentialId = response.Id,
                UserId = "user-from-credential-mapping", // Should be fetched from DB
                SignCount = response.SignatureCounter,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing passkey authentication");
            return new PasskeyAuthenticationResult { Success = false, Error = ex.Message };
        }
    }

    public async Task<bool> DeletePasskeyAsync(string userId, string credentialId)
    {
        try
        {
            _logger.LogInformation("Deleting passkey {CredentialId} for user {UserId}", credentialId, userId);
            // TODO: Implement database deletion
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting passkey");
            return false;
        }
    }

    public async Task<IEnumerable<PasskeyInfo>> GetUserPasskeysAsync(string userId)
    {
        try
        {
            // TODO: Fetch from database
            return Enumerable.Empty<PasskeyInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user passkeys");
            throw;
        }
    }

    private byte[] GenerateChallenge()
    {
        using var rng = RandomNumberGenerator.Create();
        var challenge = new byte[32];
        rng.GetBytes(challenge);
        return challenge;
    }

    private string GetRelyingPartyId()
    {
        var origin = _configuration["Auth:Passkeys:RP:Origin"] ?? "https://localhost:5174";
        var uri = new Uri(origin);
        return uri.Host;
    }
}

// ===== DTO Classes =====

public class PasskeyRegistrationOptions
{
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = "";

    [JsonPropertyName("rp")]
    public RelyingPartyInfo RP { get; set; } = new();

    [JsonPropertyName("user")]
    public UserInfo User { get; set; } = new();

    [JsonPropertyName("pubKeyCredParams")]
    public PublicKeyCredentialParameters[] PubKeyCredParams { get; set; } = Array.Empty<PublicKeyCredentialParameters>();

    [JsonPropertyName("attestation")]
    public string Attestation { get; set; } = "none";

    [JsonPropertyName("authenticatorSelection")]
    public AuthenticatorSelectionInfo AuthenticatorSelection { get; set; } = new();

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60000;
}

public class RelyingPartyInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
}

public class UserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";
}

public class PublicKeyCredentialParameters
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "public-key";

    [JsonPropertyName("alg")]
    public int Alg { get; set; }
}

public class AuthenticatorSelectionInfo
{
    [JsonPropertyName("authenticatorAttachment")]
    public string AuthenticatorAttachment { get; set; } = "platform";

    [JsonPropertyName("residentKey")]
    public string ResidentKey { get; set; } = "true";

    [JsonPropertyName("userVerification")]
    public string UserVerification { get; set; } = "preferred";
}

public class PasskeyRegistrationResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("rawId")]
    public string? RawId { get; set; }

    [JsonPropertyName("response")]
    public AuthenticatorAttestationResponse? Response { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "public-key";

    public string? AttestationObject => Response?.AttestationObject;
    public string? ClientDataJSON => Response?.ClientDataJSON;
}

public class AuthenticatorAttestationResponse
{
    [JsonPropertyName("attestationObject")]
    public string? AttestationObject { get; set; }

    [JsonPropertyName("clientDataJSON")]
    public string? ClientDataJSON { get; set; }

    [JsonPropertyName("transports")]
    public string[]? Transports { get; set; }
}

public class PasskeyRegistrationResult
{
    public bool Success { get; set; }
    public string? CredentialId { get; set; }
    public string? PublicKey { get; set; }
    public int SignCount { get; set; }
    public string? Error { get; set; }
}

public class PasskeyAuthenticationOptions
{
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = "";

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60000;

    [JsonPropertyName("userVerification")]
    public string UserVerification { get; set; } = "preferred";

    [JsonPropertyName("allowCredentials")]
    public CredentialDescriptor[] AllowCredentials { get; set; } = Array.Empty<CredentialDescriptor>();
}

public class CredentialDescriptor
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "public-key";

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("transports")]
    public string[]? Transports { get; set; }
}

public class PasskeyAuthenticationResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("rawId")]
    public string? RawId { get; set; }

    [JsonPropertyName("response")]
    public AuthenticatorAssertionResponse? Response { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "public-key";

    public string? ClientDataJSON => Response?.ClientDataJSON;
    public int SignatureCounter => Response?.SignatureCounter ?? 0;
}

public class AuthenticatorAssertionResponse
{
    [JsonPropertyName("authenticatorData")]
    public string? AuthenticatorData { get; set; }

    [JsonPropertyName("clientDataJSON")]
    public string? ClientDataJSON { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("userHandle")]
    public string? UserHandle { get; set; }

    [JsonPropertyName("signatureCounter")]
    public int SignatureCounter { get; set; }
}

public class PasskeyAuthenticationResult
{
    public bool Success { get; set; }
    public string? CredentialId { get; set; }
    public string? UserId { get; set; }
    public int SignCount { get; set; }
    public string? Error { get; set; }
}

public class PasskeyInfo
{
    public string CredentialId { get; set; } = "";
    public string PublicKey { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime LastUsedAt { get; set; }
    public int SignCount { get; set; }
}
