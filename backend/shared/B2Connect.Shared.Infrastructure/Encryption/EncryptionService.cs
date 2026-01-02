using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2Connect.Infrastructure.Encryption;

/// <summary>
/// Provides AES-256 encryption/decryption for sensitive data at rest
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    private readonly ILogger<EncryptionService> _logger;

    public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
    {
        _logger = logger;

        var keyBase64 = configuration["Encryption:Key"];
        var ivBase64 = configuration["Encryption:IV"];

        if (string.IsNullOrEmpty(keyBase64) || string.IsNullOrEmpty(ivBase64))
        {
            if (!configuration.GetValue<bool>("Encryption:AutoGenerateKeys"))
            {
                throw new InvalidOperationException(
                    "❌ Encryption keys not configured. " +
                    "Set 'Encryption:Key' and 'Encryption:IV' in configuration, " +
                    "or enable 'Encryption:AutoGenerateKeys' for development only.");
            }

            _logger.LogWarning(
                "⚠️ Generating encryption keys for development. " +
                "In production, use Azure Key Vault, AWS Secrets Manager, or similar.");

            // Auto-generate for development only!
            _key = new byte[32];
            _iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_key);
                rng.GetBytes(_iv);
            }
        }
        else
        {
            try
            {
                _key = Convert.FromBase64String(keyBase64);
                _iv = Convert.FromBase64String(ivBase64);

                if (_key.Length != 32)
                {
                    throw new InvalidOperationException("Encryption key must be 32 bytes (256 bits)");
                }

                if (_iv.Length != 16)
                {
                    throw new InvalidOperationException("IV must be 16 bytes");
                }

                _logger.LogInformation("✅ Encryption service initialized with configured keys");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to load encryption keys from configuration. " +
                    "Ensure 'Encryption:Key' and 'Encryption:IV' are valid Base64 strings.", ex);
            }
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Encryption failed for data");
            throw new InvalidOperationException("Encryption failed", ex);
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }

        try
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Decryption failed");
            throw new InvalidOperationException("Decryption failed", ex);
        }
    }

    public string? EncryptNullable(string? plainText)
    {
        if (plainText == null)
        {
            return null;
        }

        return Encrypt(plainText);
    }

    public string? DecryptNullable(string? cipherText)
    {
        if (cipherText == null)
        {
            return null;
        }

        return Decrypt(cipherText);
    }

    /// <summary>
    /// Generate encryption keys for initial setup (Base64-encoded)
    /// </summary>
    public static (string KeyBase64, string IvBase64) GenerateKeys()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            aes.GenerateIV();
            return (
                Convert.ToBase64String(aes.Key),
                Convert.ToBase64String(aes.IV)
            );
        }
    }
}
