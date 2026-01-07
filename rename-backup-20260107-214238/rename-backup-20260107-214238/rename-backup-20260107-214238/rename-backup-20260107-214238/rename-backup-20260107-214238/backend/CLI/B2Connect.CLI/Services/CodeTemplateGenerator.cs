using System.Text;
using System.Text.Json;
using Spectre.Console;

namespace B2Connect.CLI.Services;

public class CodeTemplateGenerator
{
    private readonly Dictionary<string, ITemplateProvider> _templateProviders;

    public CodeTemplateGenerator()
    {
        _templateProviders = new Dictionary<string, ITemplateProvider>
        {
            ["handler"] = new WolverineHandlerTemplate(),
            ["context"] = new EfCoreContextTemplate(),
            ["component"] = new VueComponentTemplate(),
            ["repository"] = new RepositoryTemplate(),
            ["actor"] = new ActorTemplate(),
            ["pipeline"] = new ResiliencePipelineTemplate()
        };
    }

    public async Task<TemplateResult> GenerateTemplateAsync(string type, string name, string outputPath, bool tenantAware = false)
    {
        if (!_templateProviders.TryGetValue(type, out var provider))
        {
            throw new ArgumentException($"Unknown template type: {type}. Available types: {string.Join(", ", _templateProviders.Keys)}");
        }

        var template = provider.GenerateTemplate(name, tenantAware);
        var filePath = Path.Combine(outputPath, template.FileName);

        // Ensure output directory exists
        Directory.CreateDirectory(outputPath);

        // Write file with UTF-8 encoding
        await File.WriteAllTextAsync(filePath, template.Content, Encoding.UTF8);

        return new TemplateResult
        {
            FilePath = filePath,
            Warnings = template.Warnings
        };
    }

    public IEnumerable<string> GetAvailableTypes() => _templateProviders.Keys;
}

public class TemplateResult
{
    public string FilePath { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
}

public interface ITemplateProvider
{
    Template GenerateTemplate(string name, bool tenantAware = false);
}

public class Template
{
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
}