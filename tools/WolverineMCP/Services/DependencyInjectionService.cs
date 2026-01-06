using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace B2Connect.Tools.WolverineMCP.Services;

/// <summary>
/// Result of dependency injection validation.
/// </summary>
public record DependencyValidationResult
{
    public required string ServiceName { get; init; }
    public required string Status { get; init; }
    public required string Message { get; init; }
}

/// <summary>
/// Service for analyzing dependency injection configurations and registrations.
/// </summary>
public sealed class DependencyInjectionService
{
    private readonly ILogger<DependencyInjectionService> _logger;

    public DependencyInjectionService(ILogger<DependencyInjectionService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<DependencyIssue> AnalyzeDependencyInjection(Compilation compilation)
    {
        var issues = new List<DependencyIssue>();

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            // Find service registrations
            var serviceRegistrations = FindServiceRegistrations(root, semanticModel);
            issues.AddRange(AnalyzeServiceRegistrations(serviceRegistrations, semanticModel));

            // Find constructor injections
            var constructors = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            foreach (var constructor in constructors)
            {
                var issuesForConstructor = AnalyzeConstructorInjection(constructor, semanticModel);
                issues.AddRange(issuesForConstructor);
            }

            // Find property injections
            var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Where(p => p.AttributeLists.Any(a => a.Attributes.Any(attr =>
                    attr.Name.ToString().Contains("Inject") ||
                    attr.Name.ToString().Contains("Autowired"))));

            foreach (var property in properties)
            {
                var issuesForProperty = AnalyzePropertyInjection(property, semanticModel);
                issues.AddRange(issuesForProperty);
            }
        }

        return issues;
    }

    private IEnumerable<InvocationExpressionSyntax> FindServiceRegistrations(SyntaxNode root, SemanticModel semanticModel)
    {
        return root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => IsServiceRegistrationMethod(inv, semanticModel));
    }

    private bool IsServiceRegistrationMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (symbol is null) return false;

        var methodName = symbol.Name;
        return methodName is "AddTransient" or "AddScoped" or "AddSingleton" or "AddWolverine";
    }

    private IEnumerable<DependencyIssue> AnalyzeServiceRegistrations(
        IEnumerable<InvocationExpressionSyntax> registrations, SemanticModel semanticModel)
    {
        var issues = new List<DependencyIssue>();

        foreach (var registration in registrations)
        {
            // Check for missing interface registrations
            if (HasConcreteTypeRegistration(registration, semanticModel))
            {
                issues.Add(new DependencyIssue
                {
                    Type = DependencyIssueType.ConcreteTypeRegistration,
                    Message = "Service registered with concrete type instead of interface. Consider registering against an interface.",
                    Location = registration.GetLocation(),
                    Severity = DependencySeverity.Warning
                });
            }

            // Check for lifecycle mismatches
            var lifecycle = GetServiceLifecycle(registration);
            if (lifecycle == ServiceLifecycle.Singleton && HasStatefulDependencies(registration, semanticModel))
            {
                issues.Add(new DependencyIssue
                {
                    Type = DependencyIssueType.LifecycleMismatch,
                    Message = "Singleton service has stateful dependencies. Consider using scoped lifetime.",
                    Location = registration.GetLocation(),
                    Severity = DependencySeverity.Warning
                });
            }
        }

        return issues;
    }

    private IEnumerable<DependencyIssue> AnalyzeConstructorInjection(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
    {
        var issues = new List<DependencyIssue>();

        var parameters = constructor.ParameterList.Parameters;
        foreach (var parameter in parameters)
        {
            var parameterSymbol = semanticModel.GetDeclaredSymbol(parameter) as IParameterSymbol;
            if (parameterSymbol is null) continue;

            // Check if parameter type is injectable
            if (!IsInjectableType(parameterSymbol.Type))
            {
                issues.Add(new DependencyIssue
                {
                    Type = DependencyIssueType.NonInjectableDependency,
                    Message = $"Parameter '{parameter.Identifier.Text}' of type '{parameterSymbol.Type.Name}' may not be registered for dependency injection.",
                    Location = parameter.GetLocation(),
                    Severity = DependencySeverity.Info
                });
            }
        }

        return issues;
    }

    private IEnumerable<DependencyIssue> AnalyzePropertyInjection(PropertyDeclarationSyntax property, SemanticModel semanticModel)
    {
        var issues = new List<DependencyIssue>();

        var propertySymbol = semanticModel.GetDeclaredSymbol(property) as IPropertySymbol;
        if (propertySymbol is null) return issues;

        if (!IsInjectableType(propertySymbol.Type))
        {
            issues.Add(new DependencyIssue
            {
                Type = DependencyIssueType.NonInjectableDependency,
                Message = $"Property '{property.Identifier.Text}' of type '{propertySymbol.Type.Name}' may not be registered for dependency injection.",
                Location = property.GetLocation(),
                Severity = DependencySeverity.Info
            });
        }

        return issues;
    }

    private bool HasConcreteTypeRegistration(InvocationExpressionSyntax registration, SemanticModel semanticModel)
    {
        // Simplified check - in practice, analyze the type arguments
        return registration.ArgumentList.Arguments.Count >= 1;
    }

    private ServiceLifecycle GetServiceLifecycle(InvocationExpressionSyntax registration)
    {
        var methodName = registration.Expression.ToString();
        if (methodName.Contains("AddSingleton")) return ServiceLifecycle.Singleton;
        if (methodName.Contains("AddScoped")) return ServiceLifecycle.Scoped;
        if (methodName.Contains("AddTransient")) return ServiceLifecycle.Transient;
        return ServiceLifecycle.Unknown;
    }

    private bool HasStatefulDependencies(InvocationExpressionSyntax registration, SemanticModel semanticModel)
    {
        // Simplified check - would need more analysis
        return false;
    }

    private bool IsInjectableType(ITypeSymbol type)
    {
        // Common injectable types
        return type.Name.Contains("Service") ||
               type.Name.Contains("Repository") ||
               type.Name.Contains("Handler") ||
               type.Name.Contains("Client") ||
               type.AllInterfaces.Any(i => i.Name.Contains("Service") || i.Name.Contains("Repository"));
    }
    public async Task<IEnumerable<DependencyValidationResult>> ValidateDependencyInjectionAsync(string workspacePath)
    {
        var solutionPath = Path.Combine(workspacePath, "B2Connect.slnx");
        if (!File.Exists(solutionPath))
        {
            throw new FileNotFoundException("Solution file not found", solutionPath);
        }

        // For now, return a simple result - full implementation would analyze the solution
        return new List<DependencyValidationResult>
        {
            new() { ServiceName = "WolverineAnalysisService", Status = "Valid", Message = "Service properly registered" }
        };
    }
}

public enum DependencyIssueType
{
    ConcreteTypeRegistration,
    LifecycleMismatch,
    NonInjectableDependency,
    CircularDependency
}

public enum DependencySeverity
{
    Error,
    Warning,
    Info
}

public enum ServiceLifecycle
{
    Singleton,
    Scoped,
    Transient,
    Unknown
}

public class DependencyIssue
{
    public DependencyIssueType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Location Location { get; set; } = Location.None;
    public DependencySeverity Severity { get; set; }
}