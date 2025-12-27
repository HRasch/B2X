namespace B2Connect.Infrastructure.Encryption;

/// <summary>
/// Provides AES encryption/decryption for sensitive data at rest
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a plain text string using AES encryption
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <returns>Base64-encoded encrypted string</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts a Base64-encoded encrypted string
    /// </summary>
    /// <param name="cipherText">The encrypted text (Base64-encoded)</param>
    /// <returns>Decrypted plain text</returns>
    string Decrypt(string cipherText);

    /// <summary>
    /// Encrypts a string, handling null values
    /// </summary>
    string? EncryptNullable(string? plainText);

    /// <summary>
    /// Decrypts a string, handling null values
    /// </summary>
    string? DecryptNullable(string? cipherText);
}
