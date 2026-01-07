// <copyright file="IncidentTests.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.PatternAnalysis.Core.Domain;
using Shouldly;
using Xunit;

namespace B2Connect.PatternAnalysis.Tests.Domain;

/// <summary>
/// Unit tests for the Incident domain entity.
/// </summary>
public sealed class IncidentTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesIncident()
    {
        // Arrange
        var title = "Test Incident";
        var description = "Test Description";
        var occurredAt = DateTime.UtcNow;
        var severity = IncidentSeverity.High;
        var impact = "Critical system affected";

        // Act
        var incident = new Incident(title, description, occurredAt, severity, impact);

        // Assert
        incident.Title.ShouldBe(title);
        incident.Description.ShouldBe(description);
        incident.OccurredAt.ShouldBe(occurredAt);
        incident.Severity.ShouldBe(severity);
        incident.Impact.ShouldBe(impact);
        incident.Status.ShouldBe(IncidentStatus.Open);
        incident.Id.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void AddPattern_ValidPattern_AddsToCollection()
    {
        // Arrange
        var incident = CreateTestIncident();
        var patternId = Guid.NewGuid();
        var detectedCode = "public void BadMethod() { }";
        var confidence = 0.85;

        // Act
        incident.AddPattern(patternId, detectedCode, confidence);

        // Assert
        incident.Patterns.Count.ShouldBe(1);
        var pattern = incident.Patterns.First();
        pattern.PatternId.ShouldBe(patternId);
        pattern.DetectedCode.ShouldBe(detectedCode);
        pattern.Confidence.ShouldBe(confidence);
    }

    [Fact]
    public void Resolve_WithResolution_UpdatesStatusAndResolution()
    {
        // Arrange
        var incident = CreateTestIncident();
        var resolution = "Fixed by refactoring the method";

        // Act
        incident.Resolve(resolution);

        // Assert
        incident.Resolution.ShouldBe(resolution);
        incident.Status.ShouldBe(IncidentStatus.Resolved);
    }

    [Fact]
    public void SetLocation_ValidLocation_SetsProperties()
    {
        // Arrange
        var incident = CreateTestIncident();
        var repositoryId = Guid.NewGuid();
        var filePath = "src/Services/MyService.cs";
        var lineNumber = 42;

        // Act
        incident.SetLocation(repositoryId, filePath, lineNumber);

        // Assert
        incident.RepositoryId.ShouldBe(repositoryId);
        incident.FilePath.ShouldBe(filePath);
        incident.LineNumber.ShouldBe(lineNumber);
    }

    [Theory]
    [InlineData(IncidentSeverity.Low)]
    [InlineData(IncidentSeverity.Medium)]
    [InlineData(IncidentSeverity.High)]
    [InlineData(IncidentSeverity.Critical)]
    public void Constructor_AllSeverityLevels_SetsCorrectly(IncidentSeverity severity)
    {
        // Act
        var incident = new Incident("Test", "Description", DateTime.UtcNow, severity, "Impact");

        // Assert
        incident.Severity.ShouldBe(severity);
    }

    private static Incident CreateTestIncident()
    {
        return new Incident(
            "Test Incident",
            "Test Description",
            DateTime.UtcNow,
            IncidentSeverity.Medium,
            "Medium impact");
    }
}
