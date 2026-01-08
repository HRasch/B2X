using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Infrastructure.Validation;

/// <summary>
/// Architecture Decision Record (ADR) Compliance Scanner and Architecture Rules Engine.
/// Validates codebase against documented ADRs and architectural patterns.
/// Provides automated detection of architecture violations and drift.
/// </summary>
public class ArchitectureValidator
{
    private readonly ILogger<ArchitectureValidator> _logger;
    private readonly List<IArchitectureRule> _rules = new();

    public ArchitectureValidator(ILogger<ArchitectureValidator> logger)
    {
        _logger = logger;
        InitializeRules();
    }

    /// <summary>
    /// Validates all loaded assemblies against architecture rules.
    /// </summary>
    /// <param name="assemblies">Assemblies to validate. If null, scans current AppDomain.</param>
    /// <returns>Validation results with any violations found.</returns>
    public ArchitectureValidationResult ValidateAssemblies(IEnumerable<Assembly>? assemblies = null)
    {
        var targetAssemblies = (assemblies ?? AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("B2X") == true)).ToList();

        var results = new ArchitectureValidationResult();

        foreach (var assembly in targetAssemblies)
        {
            _logger.LogInformation("Validating assembly: {Assembly}", assembly.FullName);

            foreach (var rule in _rules)
            {
                var violations = rule.Validate(assembly);
                results.AddViolations(violations);
            }
        }

        return results;
    }

    /// <summary>
    /// Scans for service boundary violations in the current application.
    /// </summary>
    public IEnumerable<ArchitectureViolation> ScanServiceBoundaries()
    {
        var violations = new List<ArchitectureViolation>();

        // Check for cross-service database access patterns
        violations.AddRange(ScanForCrossServiceDatabaseAccess());

        // Check for synchronous service call chains
        violations.AddRange(ScanForSynchronousCallChains());

        // Check for shared domain models
        violations.AddRange(ScanForSharedDomainModels());

        return violations;
    }

    /// <summary>
    /// Validates CQRS patterns and event-driven architecture compliance.
    /// </summary>
    public IEnumerable<ArchitectureViolation> ValidateCqrsPatterns()
    {
        var violations = new List<ArchitectureViolation>();

        // Check command naming conventions
        violations.AddRange(ValidateCommandNaming());

        // Check event naming conventions
        violations.AddRange(ValidateEventNaming());

        // Check for missing event publishers on state changes
        violations.AddRange(ValidateEventPublishing());

        return violations;
    }

    /// <summary>
    /// Generates a compliance report for PR reviews.
    /// </summary>
    public ArchitectureComplianceReport GenerateComplianceReport()
    {
        var result = ValidateAssemblies();
        var boundaryViolations = ScanServiceBoundaries();
        var cqrsViolations = ValidateCqrsPatterns();

        var boundaryList = boundaryViolations.ToList();
        var cqrsList = cqrsViolations.ToList();

        return new ArchitectureComplianceReport
        {
            Timestamp = DateTime.UtcNow,
            OverallCompliance = CalculateCompliance(result, boundaryList, cqrsList),
            AssemblyViolations = result.Violations,
            BoundaryViolations = boundaryList,
            CqrsViolations = cqrsList,
            Recommendations = GenerateRecommendations(result, boundaryList, cqrsList)
        };
    }

    private void InitializeRules()
    {
        // ADR-001: Event-Driven Architecture Rules
        _rules.Add(new EventDrivenArchitectureRule());

        // ADR_SERVICE_BOUNDARIES: Service Boundary Rules
        _rules.Add(new ServiceBoundaryRule());

        // ADR-025: Communication Pattern Rules
        _rules.Add(new CommunicationPatternRule());

        // Resilience Rules
        _rules.Add(new ResiliencePatternRule());
    }

    private List<ArchitectureViolation> ScanForCrossServiceDatabaseAccess()
    {
        var violations = new List<ArchitectureViolation>();

        // Look for direct database access patterns that cross service boundaries
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("B2X") == true).ToList();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            // Check for repository patterns accessing other services' data
            var repositories = types.Where(t => t.Name.Contains("Repository") &&
                                              !(t.Namespace?.Contains(assembly.GetName().Name?.Split('.')?[^1] ?? "") ?? false));

            foreach (var repo in repositories)
            {
                violations.Add(new ArchitectureViolation
                {
                    Severity = ViolationSeverity.High,
                    Rule = "ServiceBoundary-CrossServiceDataAccess",
                    Message = $"Repository {repo.FullName} appears to access data from another service",
                    Location = repo.FullName ?? repo.Name,
                    AdrReference = "ADR_SERVICE_BOUNDARIES",
                    Recommendation = "Use service APIs instead of direct database access"
                });
            }
        }

        return violations;
    }

    private IEnumerable<ArchitectureViolation> ScanForSynchronousCallChains()
    {
        // Implementation would scan for HttpClient usage patterns that indicate
        // synchronous service-to-service calls in deep chains
        return Enumerable.Empty<ArchitectureViolation>();
    }

    private List<ArchitectureViolation> ScanForSharedDomainModels()
    {
        var violations = new List<ArchitectureViolation>();

        // Look for domain models referenced across multiple service assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("B2X") == true &&
                       a.FullName.Contains("Domain")).ToList();

        var domainModels = new Dictionary<string, List<string>>();

        foreach (var assembly in assemblies)
        {
            var models = assembly.GetTypes()
                .Where(t => t.Namespace?.Contains("Domain") == true &&
                           (t.Name.EndsWith("Entity", StringComparison.OrdinalIgnoreCase) ||
                            t.Name.EndsWith("Model", StringComparison.OrdinalIgnoreCase) ||
                            t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)));

            foreach (var model in models)
            {
                var fullName = model.FullName ?? model.Name;
                if (!domainModels.ContainsKey(fullName))
                    domainModels[fullName] = new List<string>();

                domainModels[fullName].Add(assembly.GetName().Name ?? "UnknownAssembly");
            }
        }

        // Check for models used in multiple services
        foreach (var kvp in domainModels.Where(d => d.Value.Count > 1))
        {
            violations.Add(new ArchitectureViolation
            {
                Severity = ViolationSeverity.Medium,
                Rule = "ServiceBoundary-SharedDomainModel",
                Message = $"Domain model {kvp.Key} is shared across services: {string.Join(", ", kvp.Value)}",
                Location = kvp.Key,
                AdrReference = "ADR_SERVICE_BOUNDARIES",
                Recommendation = "Create separate DTOs for each service or use shared kernel carefully"
            });
        }

        return violations;
    }

    private List<ArchitectureViolation> ValidateCommandNaming()
    {
        var violations = new List<ArchitectureViolation>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("B2X") == true).ToList();

        foreach (var assembly in assemblies)
        {
            var commandTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase) &&
                           !t.Name.EndsWith("Result", StringComparison.OrdinalIgnoreCase));

            foreach (var command in commandTypes)
            {
                // Commands should follow CQRS naming: VerbNounCommand
                if (!Regex.IsMatch(command.Name, @"^[A-Z][a-z]+[A-Z][a-zA-Z]*Command$"))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Severity = ViolationSeverity.Low,
                        Rule = "CQRS-CommandNaming",
                        Message = $"Command {command.Name} doesn't follow CQRS naming convention (VerbNounCommand)",
                        Location = command.FullName ?? command.Name,
                        AdrReference = "ADR-001",
                        Recommendation = "Rename to follow VerbNounCommand pattern"
                    });
                }
            }
        }

        return violations;
    }

    private List<ArchitectureViolation> ValidateEventNaming()
    {
        var violations = new List<ArchitectureViolation>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.Contains("B2X") == true).ToList();

        foreach (var assembly in assemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Event", StringComparison.OrdinalIgnoreCase));

            foreach (var eventType in eventTypes)
            {
                // Events should follow: NounVerbEvent or NounStateChangedEvent
                if (!Regex.IsMatch(eventType.Name, @"^[A-Z][a-zA-Z]*Event$"))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Severity = ViolationSeverity.Low,
                        Rule = "CQRS-EventNaming",
                        Message = $"Event {eventType.Name} doesn't follow CQRS naming convention",
                        Location = eventType.FullName ?? eventType.Name,
                        AdrReference = "ADR-001",
                        Recommendation = "Rename to follow NounVerbEvent or NounStateChangedEvent pattern"
                    });
                }
            }
        }

        return violations;
    }

    private IEnumerable<ArchitectureViolation> ValidateEventPublishing()
    {
        // This would require more sophisticated analysis of method bodies
        // For now, return empty - could be enhanced with Roslyn analysis
        return Enumerable.Empty<ArchitectureViolation>();
    }

    private double CalculateCompliance(ArchitectureValidationResult result,
                                     List<ArchitectureViolation> boundaryViolations,
                                     List<ArchitectureViolation> cqrsViolations)
    {
        var totalViolations = result.Violations.Count +
                             boundaryViolations.Count +
                             cqrsViolations.Count;

        // Base compliance score - adjust based on violation severity
        var baseScore = 100.0;

        foreach (var violation in result.Violations.Concat(boundaryViolations).Concat(cqrsViolations))
        {
            var penalty = violation.Severity switch
            {
                ViolationSeverity.Critical => 20,
                ViolationSeverity.High => 10,
                ViolationSeverity.Medium => 5,
                ViolationSeverity.Low => 1,
                _ => 1
            };
            baseScore -= penalty;
        }

        return Math.Max(0, baseScore);
    }

    private List<string> GenerateRecommendations(ArchitectureValidationResult result,
                                              List<ArchitectureViolation> boundaryViolations,
                                              List<ArchitectureViolation> cqrsViolations)
    {
        var recommendations = new List<string>();
        var allViolations = result.Violations
            .Concat(boundaryViolations)
            .Concat(cqrsViolations)
            .ToList();

        if (allViolations.Any(v => v.Rule.StartsWith("ServiceBoundary", StringComparison.Ordinal)))
        {
            recommendations.Add("Review service boundaries and ensure each service owns its domain entities");
            recommendations.Add("Replace direct database access with service API calls");
        }

        if (allViolations.Any(v => v.Rule.StartsWith("CQRS", StringComparison.Ordinal)))
        {
            recommendations.Add("Ensure commands and events follow CQRS naming conventions");
            recommendations.Add("Verify all state changes publish appropriate events");
        }

        if (allViolations.Any(v => v.Severity >= ViolationSeverity.High))
        {
            recommendations.Add("Address high-severity violations before merging");
            recommendations.Add("Consider architectural review for critical violations");
        }

        return recommendations;
    }
}

/// <summary>
/// Represents a violation of architectural rules.
/// </summary>
public class ArchitectureViolation
{
    public ViolationSeverity Severity { get; set; }
    public string Rule { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AdrReference { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}

/// <summary>
/// Severity levels for architecture violations.
/// </summary>
public enum ViolationSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Result of architecture validation.
/// </summary>
public class ArchitectureValidationResult
{
    public List<ArchitectureViolation> Violations { get; } = new();

    public void AddViolations(IEnumerable<ArchitectureViolation> violations)
    {
        Violations.AddRange(violations);
    }

    public bool IsValid => !Violations.Any();
}

/// <summary>
/// Compliance report for PR reviews.
/// </summary>
public class ArchitectureComplianceReport
{
    public DateTime Timestamp { get; set; }
    public double OverallCompliance { get; set; }
    public List<ArchitectureViolation> AssemblyViolations { get; set; } = new();
    public List<ArchitectureViolation> BoundaryViolations { get; set; } = new();
    public List<ArchitectureViolation> CqrsViolations { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Interface for architecture validation rules.
/// </summary>
public interface IArchitectureRule
{
    IEnumerable<ArchitectureViolation> Validate(Assembly assembly);
}

/// <summary>
/// Validates event-driven architecture patterns (ADR-001).
/// </summary>
public class EventDrivenArchitectureRule : IArchitectureRule
{
    public IEnumerable<ArchitectureViolation> Validate(Assembly assembly)
    {
        var violations = new List<ArchitectureViolation>();

        // Check for Wolverine usage in services that should be event-driven
        var types = assembly.GetTypes();
        var hasCommands = types.Any(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase));
        var hasEvents = types.Any(t => t.Name.EndsWith("Event", StringComparison.OrdinalIgnoreCase));

        if (hasCommands && !hasEvents)
        {
            violations.Add(new ArchitectureViolation
            {
                Severity = ViolationSeverity.Medium,
                Rule = "EventDriven-MissingEvents",
                Message = $"Assembly {assembly.GetName().Name} has commands but no events - ensure state changes publish events",
                Location = assembly.FullName ?? assembly.GetName().Name ?? "UnknownAssembly",
                AdrReference = "ADR-001",
                Recommendation = "Add event publishing for all state-changing commands"
            });
        }

        return violations;
    }
}

/// <summary>
/// Validates service boundary rules (ADR_SERVICE_BOUNDARIES).
/// </summary>
public class ServiceBoundaryRule : IArchitectureRule
{
    public IEnumerable<ArchitectureViolation> Validate(Assembly assembly)
    {
        var violations = new List<ArchitectureViolation>();

        // Check for database context usage that might indicate cross-service access
        var types = assembly.GetTypes();
        var dbContexts = types.Where(t => t.Name.Contains("Context") &&
                                        t.GetInterfaces().Any(i => i.Name.Contains("DbContext")));

        foreach (var context in dbContexts)
        {
            // Check if context is accessing entities from other services
            var properties = context.GetProperties()
                .Where(p => p.PropertyType.Name.Contains("Set") ||
                           p.PropertyType.Name.Contains("DbSet"));

            foreach (var prop in properties)
            {
                var entityType = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                if (entityType != null && !IsEntityOwnedByService(entityType, assembly))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Severity = ViolationSeverity.High,
                        Rule = "ServiceBoundary-CrossServiceEntity",
                        Message = $"DbContext {context.Name} references entity {entityType.Name} not owned by this service",
                        Location = $"{context.FullName}.{prop.Name}",
                        AdrReference = "ADR_SERVICE_BOUNDARIES",
                        Recommendation = "Remove cross-service entity references, use service APIs instead"
                    });
                }
            }
        }

        return violations;
    }

    private bool IsEntityOwnedByService(Type entityType, Assembly assembly)
    {
        // Simple heuristic: check if entity namespace matches assembly service name
        var serviceName = assembly.GetName().Name?.Split('.').Last() ?? "";
        var entityNamespace = entityType.Namespace ?? "";

        return entityNamespace.Contains(serviceName, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Validates communication patterns (ADR-025).
/// </summary>
public class CommunicationPatternRule : IArchitectureRule
{
    public IEnumerable<ArchitectureViolation> Validate(Assembly assembly)
    {
        var violations = new List<ArchitectureViolation>();

        // Check for hardcoded URLs (should use service discovery)
        var types = assembly.GetTypes();
        var hardcodedUrls = new[] { "localhost", "http://", "https://" };

        foreach (var type in types)
        {
            var constants = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .Select(f => f.GetValue(null) as string)
                .Where(v => v != null && hardcodedUrls.Any(url => v.Contains(url)));

            foreach (var constant in constants)
            {
                violations.Add(new ArchitectureViolation
                {
                    Severity = ViolationSeverity.Medium,
                    Rule = "Communication-HardcodedUrl",
                    Message = $"Hardcoded URL found in {type.FullName}: {constant}",
                    Location = type.FullName ?? type.Name,
                    AdrReference = "ADR-025",
                    Recommendation = "Use Aspire service discovery instead of hardcoded URLs"
                });
            }
        }

        return violations;
    }
}

/// <summary>
/// Validates resilience patterns.
/// </summary>
public class ResiliencePatternRule : IArchitectureRule
{
    public IEnumerable<ArchitectureViolation> Validate(Assembly assembly)
    {
        var violations = new List<ArchitectureViolation>();

        // Check for HttpClient usage without Polly policies
        var types = assembly.GetTypes();
        var hasHttpClient = types.Any(t => t.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(f => f.FieldType == typeof(System.Net.Http.HttpClient)));

        var hasPolly = assembly.GetReferencedAssemblies()
            .Any(a => a.Name?.Contains("Polly") == true);

        if (hasHttpClient && !hasPolly)
        {
            violations.Add(new ArchitectureViolation
            {
                Severity = ViolationSeverity.Medium,
                Rule = "Resilience-MissingCircuitBreaker",
                Message = $"Assembly {assembly.GetName().Name} uses HttpClient but no Polly resilience policies detected",
                Location = assembly.FullName ?? assembly.GetName().Name ?? "UnknownAssembly",
                AdrReference = "ADR-001",
                Recommendation = "Add Polly policies for circuit breaker, retry, and timeout handling"
            });
        }

        return violations;
    }
}
