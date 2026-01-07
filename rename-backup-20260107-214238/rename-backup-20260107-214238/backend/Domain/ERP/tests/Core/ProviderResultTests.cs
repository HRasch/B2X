// <copyright file="ProviderResultTests.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Core;
using Shouldly;
using Xunit;

namespace B2X.ERP.Tests.Core;

/// <summary>
/// Tests for the ProviderResult class.
/// </summary>
public sealed class ProviderResultTests
{
    [Fact]
    public void Success_WithData_CreatesSuccessResult()
    {
        // Act
        var result = ProviderResult<string>.Success("test-data");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Data.ShouldBe("test-data");
        result.ErrorCode.ShouldBeNull();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithCodeAndMessage_CreatesFailureResult()
    {
        // Act
        var result = ProviderResult<string>.Failure("ERROR_CODE", "Error message");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Data.ShouldBeNull();
        result.ErrorCode.ShouldBe("ERROR_CODE");
        result.ErrorMessage.ShouldBe("Error message");
    }

    [Fact]
    public void FromException_CreatesFailureWithExceptionDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        var result = ProviderResult<string>.FromException(exception);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorCode.ShouldBe("InvalidOperationException");
        result.ErrorMessage.ShouldBe("Test exception");
    }

    [Fact]
    public void Match_Success_ExecutesSuccessAction()
    {
        // Arrange
        var result = ProviderResult<int>.Success(42);
        string? output = null;

        // Act
        result.Match(
            onSuccess: data => output = $"Success: {data}",
            onFailure: (code, msg) => output = $"Failure: {code}");

        // Assert
        output.ShouldBe("Success: 42");
    }

    [Fact]
    public void Match_Failure_ExecutesFailureAction()
    {
        // Arrange
        var result = ProviderResult<int>.Failure("NOT_FOUND", "Not found");
        string? output = null;

        // Act
        result.Match(
            onSuccess: data => output = $"Success: {data}",
            onFailure: (code, msg) => output = $"Failure: {code} - {msg}");

        // Assert
        output.ShouldBe("Failure: NOT_FOUND - Not found");
    }

    [Fact]
    public void Map_Success_TransformsData()
    {
        // Arrange
        var result = ProviderResult<int>.Success(42);

        // Act
        var mapped = result.Map(x => x.ToString());

        // Assert
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Data.ShouldBe("42");
    }

    [Fact]
    public void Map_Failure_PreservesError()
    {
        // Arrange
        var result = ProviderResult<int>.Failure("ERROR", "Error message");

        // Act
        var mapped = result.Map(x => x.ToString());

        // Assert
        mapped.IsFailure.ShouldBeTrue();
        mapped.ErrorCode.ShouldBe("ERROR");
        mapped.ErrorMessage.ShouldBe("Error message");
    }
}
