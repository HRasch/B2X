using System.Collections.Generic;
using Spectre.Console;

namespace B2X.CLI.Shared;

/// <summary>
/// Service für formatierte Konsolen-Ausgabe mit Spectre.Console
/// </summary>
public class ConsoleOutputService
{
    public void Success(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓[/] {message}");
    }

    public void Error(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗[/] {message}");
    }

    public void Warning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠[/] {message}");
    }

    public void Info(string message)
    {
        AnsiConsole.MarkupLine($"[blue]ℹ[/] {message}");
    }

    public void Header(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[bold blue]{title}[/]") { Justification = Justify.Left });
        AnsiConsole.WriteLine();
    }

    public void SubHeader(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[bold cyan]{title}[/]") { Justification = Justify.Left });
        AnsiConsole.WriteLine();
    }

    public void Table<T>(IEnumerable<T> items, params (string Header, Func<T, string> Selector)[] columns) where T : class
    {
        var table = new Table();
        table.AddColumn(new TableColumn("[bold]" + columns[0].Header + "[/]").Centered());

        for (int i = 1; i < columns.Length; i++)
        {
            table.AddColumn(new TableColumn("[bold]" + columns[i].Header + "[/]").Centered());
        }

        foreach (var item in items)
        {
            var values = columns.Select(c => c.Selector(item)).ToArray();
            table.AddRow(values);
        }

        AnsiConsole.Write(table);
    }

    public void JsonOutput(object? data)
    {
        if (data == null)
        {
            Info("No data to display");
            return;
        }

        var json = System.Text.Json.JsonSerializer.Serialize(
            data,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        AnsiConsole.Write(new Panel(json) { Border = BoxBorder.Rounded });
    }

    public void WriteJson(object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(
            data,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        // Use plain text output to avoid markup parsing issues
        Console.WriteLine(json);
    }

    public void WriteYaml(object data)
    {
        // Simple YAML-like output for now
        // Could be enhanced with a proper YAML serializer
        var json = System.Text.Json.JsonSerializer.Serialize(
            data,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        // Convert JSON to basic YAML format
        var yaml = json
            .Replace("\"", "")
            .Replace(",", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("  ", "  - ");

        // Use plain text output to avoid markup parsing issues
        Console.WriteLine(yaml);
    }

    public void WriteSuccess(string message)
    {
        Success(message);
    }

    public void WriteError(string message)
    {
        Error(message);
    }

    public void WriteWarning(string message)
    {
        Warning(message);
    }

    public void WriteInfo(string message)
    {
        Info(message);
    }

    public bool Confirm(string question)
    {
        return AnsiConsole.Confirm($"[yellow]{question}[/]");
    }

    public string? Prompt(string prompt, string? defaultValue = null)
    {
        var result = AnsiConsole.Ask($"[yellow]{prompt}[/]", defaultValue);
        return string.IsNullOrEmpty(result) ? defaultValue : result;
    }

    public string? PromptPassword(string prompt)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>($"[yellow]{prompt}[/]")
                .PromptStyle("red")
                .Secret());
    }

    public void Spinner(string title, Func<Task> action)
    {
        AnsiConsole.Status()
            .Spinner(Spectre.Console.Spinner.Known.Default)
            .Start(title, ctx =>
            {
                action().Wait();
            });
    }

    public void LoadingBar(string title, int total, Func<Action<int>, Task> action)
    {
        AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn())
            .Start(ctx =>
            {
                var task = ctx.AddTask($"[green]{title}[/]", autoStart: false, maxValue: total);
                action(x => task.Value = x).Wait();
            });
    }
}
