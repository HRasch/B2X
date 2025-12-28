using Xunit;
using FluentAssertions;
using B2Connect.Catalog.Application.DTOs;
using B2Connect.Catalog.Application.Validators;
using FluentValidation;

namespace B2Connect.Catalog.Tests.Application.Validators;

/// <summary>
/// Unit tests for Return Management Validators
/// 
/// Tests validation rules for CreateReturnRequestDto and CreateRefundRequestDto
/// Story 8: Widerrufsmanagement (Return/Withdrawal Management)
/// </summary>
public class ReturnValidatorTests
{
    private readonly IValidator<CreateReturnRequestDto> _returnValidator;

    public ReturnValidatorTests()
    {
        _returnValidator = new CreateReturnRequestValidator();
    }

    // ========================================
    // CreateReturnRequestValidator Tests
    // ========================================

    [Fact]
    public async Task ValidateReturnRequest_AllFieldsValid_PassesValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Changed my mind",
            Description = "Product not as expected",
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateReturnRequest_MissingOrderId_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.Empty, // Invalid - required
            Reason = "Changed mind",
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "OrderId");
    }

    [Fact]
    public async Task ValidateReturnRequest_EmptyReason_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "", // Empty - invalid
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Reason");
    }

    [Fact]
    public async Task ValidateReturnRequest_ReasonTooShort_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "x", // Too short (< 3 chars)
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Reason");
    }

    [Fact]
    public async Task ValidateReturnRequest_ReasonTooLong_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "A".PadRight(501), // Exceeds 500 chars
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Reason");
    }

    [Fact]
    public async Task ValidateReturnRequest_DescriptionTooLong_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Valid reason",
            Description = "A".PadRight(1001), // Exceeds 1000 chars
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Description");
    }

    [Fact]
    public async Task ValidateReturnRequest_EmptyItemsList_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Changed mind",
            ReturnedItemIds = new List<Guid>() // Empty - invalid
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName.Contains("Items"));
    }

    [Fact]
    public async Task ValidateReturnRequest_MultipleValidationErrors_ReturnsAll()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.Empty, // Invalid
            Reason = "", // Invalid
            ReturnedItemIds = new List<Guid>() // Invalid
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1); // Multiple errors
    }

    [Fact]
    public async Task ValidateReturnRequest_LegalReasonValues_PassValidation()
    {
        // Arrange
        var validReasons = new[]
        {
            "Changed my mind",
            "Product defective",
            "Product damaged",
            "Wrong item sent",
            "Item not as described",
            "Quality issue",
            "Other reason"
        };

        // Act & Assert
        foreach (var reason in validReasons)
        {
            var request = new CreateReturnRequestDto
            {
                OrderId = Guid.NewGuid(),
                Reason = reason,
                ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
            };

            var result = await _returnValidator.ValidateAsync(request);
            result.IsValid.Should().BeTrue($"Reason '{reason}' should be valid");
        }
    }

    [Fact]
    public async Task ValidateReturnRequest_WithValidDescription_PassesValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Product defective",
            Description = "The color faded after first wash, not matching product photos",
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateReturnRequest_WithoutOptionalDescription_PassesValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Changed my mind",
            Description = null, // Optional
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateReturnRequest_SingleItem_PassesValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Item broken",
            ReturnedItemIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateReturnRequest_MultipleItems_PassesValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Multiple items defective",
            ReturnedItemIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            }
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateReturnRequest_MaxItemsAllowed_PassesValidation()
    {
        // Arrange - Test with 100 items (should be allowed)
        var itemIds = Enumerable.Range(0, 100)
            .Select(_ => Guid.NewGuid())
            .ToList();

        var request = new CreateReturnRequestDto
        {
            OrderId = Guid.NewGuid(),
            Reason = "Bulk return",
            ReturnedItemIds = itemIds
        };

        // Act
        var result = await _returnValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

/// <summary>
/// Unit tests for CreateRefundRequestValidator
/// </summary>
public class RefundValidatorTests
{
    private readonly IValidator<CreateRefundRequestDto> _refundValidator;

    public RefundValidatorTests()
    {
        _refundValidator = new CreateRefundRequestValidator();
    }

    [Fact]
    public async Task ValidateRefundRequest_AllFieldsValid_PassesValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "Original Payment",
            ProcessingNotes = "Customer approved"
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateRefundRequest_MissingReturnId_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.Empty, // Invalid
            RefundMethod = "Original Payment",
            ProcessingNotes = "Test"
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "ReturnId");
    }

    [Fact]
    public async Task ValidateRefundRequest_EmptyRefundMethod_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "", // Empty - invalid
            ProcessingNotes = "Test"
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RefundMethod");
    }

    [Fact]
    public async Task ValidateRefundRequest_ValidRefundMethods_PassesValidation()
    {
        // Arrange
        var validMethods = new[]
        {
            "Original Payment",
            "Credit Note",
            "Bank Transfer",
            "Check",
            "PayPal",
            "Gift Card"
        };

        // Act & Assert
        foreach (var method in validMethods)
        {
            var request = new CreateRefundRequestDto
            {
                ReturnId = Guid.NewGuid(),
                RefundMethod = method,
                ProcessingNotes = "Valid method test"
            };

            var result = await _refundValidator.ValidateAsync(request);
            result.IsValid.Should().BeTrue($"Method '{method}' should be valid");
        }
    }

    [Fact]
    public async Task ValidateRefundRequest_InvalidRefundMethod_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "InvalidMethod", // Not in allowed list
            ProcessingNotes = "Test"
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "RefundMethod");
    }

    [Fact]
    public async Task ValidateRefundRequest_ProcessingNotesTooLong_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "Original Payment",
            ProcessingNotes = "A".PadRight(501) // Exceeds 500 chars
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "ProcessingNotes");
    }

    [Fact]
    public async Task ValidateRefundRequest_WithoutOptionalNotes_PassesValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "Original Payment",
            ProcessingNotes = null // Optional
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateRefundRequest_WithDetailedNotes_PassesValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "Bank Transfer",
            ProcessingNotes = "Customer account: DE89370400440532013000. Processing within 3-5 business days."
        };

        // Act
        var result = await _refundValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
