using System.CommandLine;
using System.Text;
using Spectre.Console;
using B2X.CLI.Services;
using B2X.CLI.Shared;

namespace B2X.CLI.Commands;

public static class TemplateCommand
{
    public static Command BuildCommand()
    {
        var command = new Command("template", "Generate code templates with resilience patterns");

        var typeOption = new Option<string>(
            "--type",
            description: "Template type (handler, context, component, repository, actor, pipeline)")
        {
            IsRequired = true
        };

        var nameOption = new Option<string>(
            "--name",
            description: "Name of the generated class/component")
        {
            IsRequired = true
        };

        var outputOption = new Option<string>(
            "--output",
            description: "Output directory path")
        {
            IsRequired = true
        };

        var tenantOption = new Option<bool>(
            "--tenant-aware",
            description: "Include tenant-aware patterns");

        command.AddOption(typeOption);
        command.AddOption(nameOption);
        command.AddOption(outputOption);
        command.AddOption(tenantOption);

        command.SetHandler(async (type, name, output, tenantAware) =>
        {
            try
            {
                var generator = new CodeTemplateGenerator();
                var result = await generator.GenerateTemplateAsync(type, name, output, tenantAware);

                AnsiConsole.MarkupLine($"[green]✅ Template generated successfully:[/]");
                AnsiConsole.MarkupLine($"[blue]📁 {result.FilePath}[/]");

                if (result.Warnings.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]⚠️  Warnings:[/]");
                    foreach (var warning in result.Warnings)
                    {
                        AnsiConsole.MarkupLine($"[yellow]   - {warning}[/]");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Error generating template: {ex.Message}[/]");
                return;
            }
        }, typeOption, nameOption, outputOption, tenantOption);

        return command;
    }
}