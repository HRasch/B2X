using System;
using System.Collections.Generic;

namespace B2X.Customer.Utilities;

/// <summary>
/// Consolidated validation helper to eliminate duplicate validation logic across services.
/// Issue #53: Code Quality Refactoring & Dependency Updates - Phase 3
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates that a deadline date has not already passed.
    /// Throws InvalidOperationException if deadline is in the past.
    /// </summary>
    /// <param name="deadline">The deadline date to validate</param>
    /// <param name="fieldName">The name of the field for error messages (e.g., "Withdrawal Deadline")</param>
    /// <exception cref="InvalidOperationException">Thrown if deadline has passed</exception>
    public static void ValidateDeadlineNotPassed(DateTime deadline, string fieldName = "Deadline")
    {
        if (DateTime.UtcNow <= deadline)
        {
            return;
        }

        throw new InvalidOperationException($"{fieldName} has already passed. Deadline was {deadline:O}");
    }

    /// <summary>
    /// Validates that an amount is positive and non-zero.
    /// Throws InvalidOperationException if amount is negative or zero.
    /// </summary>
    /// <param name="amount">The amount to validate</param>
    /// <param name="fieldName">The name of the field for error messages (e.g., "Refund Amount")</param>
    /// <exception cref="InvalidOperationException">Thrown if amount is not positive</exception>
    public static void ValidatePositiveAmount(decimal amount, string fieldName = "Amount")
    {
        if (amount > 0m)
        {
            return;
        }

        throw new InvalidOperationException($"{fieldName} must be positive. Received: {amount}");
    }

    /// <summary>
    /// Validates that an enum value is defined in the enum type.
    /// Throws InvalidOperationException if value is not a valid enum member.
    /// </summary>
    /// <typeparam name="T">The enum type to validate against</typeparam>
    /// <param name="value">The enum value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="InvalidOperationException">Thrown if value is not defined in enum</exception>
    public static void ValidateEnumDefined<T>(T value, string fieldName = "EnumValue") where T : Enum
    {
        if (Enum.IsDefined(typeof(T), value))
        {
            return;
        }

        throw new InvalidOperationException($"{fieldName} has an invalid value: {value}");
    }

    /// <summary>
    /// Validates that a string is not null or empty.
    /// Throws ArgumentException if string is null or whitespace.
    /// </summary>
    /// <param name="value">The string value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="ArgumentException">Thrown if string is null or empty</exception>
    public static void ValidateStringNotEmpty(string value, string fieldName = "String")
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        throw new ArgumentException($"{fieldName} cannot be null or empty", nameof(value));
    }

    /// <summary>
    /// Validates that a GUID is not empty (Guid.Empty).
    /// Throws ArgumentException if guid is Guid.Empty.
    /// </summary>
    /// <param name="value">The GUID value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="ArgumentException">Thrown if GUID is empty</exception>
    public static void ValidateGuidNotEmpty(Guid value, string fieldName = "Id")
    {
        if (value != Guid.Empty)
        {
            return;
        }

        throw new ArgumentException($"{fieldName} cannot be empty", nameof(value));
    }

    /// <summary>
    /// Validates that a collection is not null and not empty.
    /// Throws ArgumentException if collection is null or has no items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The collection to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="ArgumentException">Thrown if collection is null or empty</exception>
    public static void ValidateCollectionNotEmpty<T>(IEnumerable<T> collection, string fieldName = "Collection")
    {
        if (collection is null)
        {
            throw new ArgumentException($"{fieldName} cannot be null", nameof(collection));
        }

        if (collection.GetEnumerator().MoveNext())
        {
            return;
        }

        throw new ArgumentException($"{fieldName} cannot be empty", nameof(collection));
    }

    /// <summary>
    /// Validates that a date falls within an expected range.
    /// Throws InvalidOperationException if date is outside the range.
    /// </summary>
    /// <param name="value">The date to validate</param>
    /// <param name="minDate">The minimum allowed date (inclusive)</param>
    /// <param name="maxDate">The maximum allowed date (inclusive)</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="InvalidOperationException">Thrown if date is outside range</exception>
    public static void ValidateDateInRange(DateTime value, DateTime minDate, DateTime maxDate, string fieldName = "Date")
    {
        if (value >= minDate && value <= maxDate)
        {
            return;
        }

        throw new InvalidOperationException($"{fieldName} must be between {minDate:O} and {maxDate:O}. Received: {value:O}");
    }

    /// <summary>
    /// Validates that a string matches a expected length.
    /// Throws ArgumentException if string length doesn't match.
    /// </summary>
    /// <param name="value">The string value to validate</param>
    /// <param name="expectedLength">The expected length of the string</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="ArgumentException">Thrown if string length doesn't match</exception>
    public static void ValidateStringLength(string value, int expectedLength, string fieldName = "String")
    {
        if (value?.Length == expectedLength)
        {
            return;
        }

        throw new ArgumentException($"{fieldName} must be exactly {expectedLength} characters. Received: {value?.Length ?? 0}", nameof(value));
    }

    /// <summary>
    /// Validates that a string matches a expected maximum length.
    /// Throws ArgumentException if string exceeds maximum length.
    /// </summary>
    /// <param name="value">The string value to validate</param>
    /// <param name="maxLength">The maximum allowed length of the string</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    /// <exception cref="ArgumentException">Thrown if string exceeds maximum length</exception>
    public static void ValidateStringMaxLength(string value, int maxLength, string fieldName = "String")
    {
        if (value == null || value.Length <= maxLength)
        {
            return;
        }

        throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters. Received: {value.Length}", nameof(value));
    }
}
