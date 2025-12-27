using B2Connect.Infrastructure.Encryption;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Shared.Infrastructure.Tests.Encryption;

public class EncryptionServiceTests
{
    private readonly IEncryptionService _encryptionService;
    private readonly IConfiguration _configuration;

    public EncryptionServiceTests()
    {
        // Set up configuration for development (auto-generate keys)
        var configDict = new Dictionary<string, string?>
        {
            { "Encryption:AutoGenerateKeys", "true" },
            { "Encryption:Key", null },
            { "Encryption:IV", null }
        };
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict);
        _configuration = configBuilder.Build();

        var mockLogger = new Mock<ILogger<EncryptionService>>();
        _encryptionService = new EncryptionService(_configuration, mockLogger.Object);
    }

    [Fact]
    public void Encrypt_WithPlainText_ReturnsEncryptedString()
    {
        // Arrange
        var plainText = "test@example.com";

        // Act
        var encrypted = _encryptionService.Encrypt(plainText);

        // Assert
        encrypted.Should().NotBeNullOrEmpty();
        encrypted.Should().NotBe(plainText); // Must be encrypted
        encrypted.Should().HaveLength(System.Convert.ToBase64String(System.Convert.FromBase64String(encrypted)).Length);
    }

    [Fact]
    public void Decrypt_WithEncryptedText_ReturnsOriginalPlainText()
    {
        // Arrange
        var plainText = "sensitive-data-12345";
        var encrypted = _encryptionService.Encrypt(plainText);

        // Act
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_And_Decrypt_RoundTrip_PreservesData()
    {
        // Arrange
        var testCases = new[]
        {
            "simple",
            "test@example.com",
            "123-456-7890",
            "Special!@#$%^&*()",
            "Unicode: 你好世界 🌍",
            "VeryLongStringThatExceedsNormalLengths" + new string('x', 1000)
        };

        // Act & Assert
        foreach (var testCase in testCases)
        {
            var encrypted = _encryptionService.Encrypt(testCase);
            var decrypted = _encryptionService.Decrypt(encrypted);
            decrypted.Should().Be(testCase, because: $"Failed for: {testCase}");
        }
    }

    [Fact]
    public void Encrypt_WithEmptyString_ReturnsEmptyString()
    {
        // Act
        var result = _encryptionService.Encrypt("");

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void Encrypt_WithNullString_ReturnsNull()
    {
        // Act
        var result = _encryptionService.Encrypt(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void EncryptNullable_WithNullString_ReturnsNull()
    {
        // Act
        var result = _encryptionService.EncryptNullable(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void EncryptNullable_WithPlainText_ReturnsEncrypted()
    {
        // Act
        var result = _encryptionService.EncryptNullable("test");

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe("test");
    }

    [Fact]
    public void DecryptNullable_WithNullString_ReturnsNull()
    {
        // Act
        var result = _encryptionService.DecryptNullable(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Decrypt_WithInvalidBase64_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidBase64 = "not-valid-base64!!!";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _encryptionService.Decrypt(invalidBase64));
        exception.Message.Should().Contain("Decryption failed");
    }

    [Fact]
    public void GenerateKeys_ReturnsValidBase64Strings()
    {
        // Act
        var (keyBase64, ivBase64) = EncryptionService.GenerateKeys();

        // Assert
        keyBase64.Should().NotBeNullOrEmpty();
        ivBase64.Should().NotBeNullOrEmpty();

        // Verify they're valid Base64
        var keyBytes = System.Convert.FromBase64String(keyBase64);
        var ivBytes = System.Convert.FromBase64String(ivBase64);

        keyBytes.Length.Should().Be(32); // 256 bits
        ivBytes.Length.Should().Be(16);  // 128 bits
    }

    [Fact]
    public void EncryptionService_WithConfiguredKeys_LoadsSuccessfully()
    {
        // Arrange
        var (keyBase64, ivBase64) = EncryptionService.GenerateKeys();
        var configDict = new Dictionary<string, string?>
        {
            { "Encryption:AutoGenerateKeys", "false" },
            { "Encryption:Key", keyBase64 },
            { "Encryption:IV", ivBase64 }
        };
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict);
        var config = configBuilder.Build();
        var mockLogger = new Mock<ILogger<EncryptionService>>();

        // Act
        var service = new EncryptionService(config, mockLogger.Object);
        var plainText = "test-with-configured-keys";
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void EncryptionService_WithoutKeys_AndAutoGenerateDisabled_ThrowsException()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            { "Encryption:AutoGenerateKeys", "false" },
            { "Encryption:Key", null },
            { "Encryption:IV", null }
        };
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict);
        var config = configBuilder.Build();
        var mockLogger = new Mock<ILogger<EncryptionService>>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => new EncryptionService(config, mockLogger.Object));
        exception.Message.Should().Contain("not configured");
    }

    [Fact]
    public void Encrypt_SameInput_ProducesDifferentOutputs_WithDifferentInstances()
    {
        // Arrange
        var plainText = "test";
        var (keyBase64, ivBase64) = EncryptionService.GenerateKeys();
        var configDict1 = new Dictionary<string, string?>
        {
            { "Encryption:AutoGenerateKeys", "false" },
            { "Encryption:Key", keyBase64 },
            { "Encryption:IV", ivBase64 }
        };
        var config1 = new ConfigurationBuilder().AddInMemoryCollection(configDict1).Build();
        var mockLogger = new Mock<ILogger<EncryptionService>>();
        var service1 = new EncryptionService(config1, mockLogger.Object);

        var (key2Base64, iv2Base64) = EncryptionService.GenerateKeys();
        var configDict2 = new Dictionary<string, string?>
        {
            { "Encryption:AutoGenerateKeys", "false" },
            { "Encryption:Key", key2Base64 },
            { "Encryption:IV", iv2Base64 }
        };
        var config2 = new ConfigurationBuilder().AddInMemoryCollection(configDict2).Build();
        var service2 = new EncryptionService(config2, mockLogger.Object);

        // Act
        var encrypted1 = service1.Encrypt(plainText);
        var encrypted2 = service2.Encrypt(plainText);

        // Assert - Same input with different keys should produce different ciphertexts
        encrypted1.Should().NotBe(encrypted2);
    }
}
