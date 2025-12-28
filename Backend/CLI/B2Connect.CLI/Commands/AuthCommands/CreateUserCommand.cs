using System.CommandLine;
using System.CommandLine.Invocation;
using B2Connect.CLI.Services;
using Spectre.Console;

namespace B2Connect.CLI.Commands.AuthCommands;

public static class CreateUserCommand
{
    public static Command Create()
    {
        var command = new Command("create-user", "Create a new user account");

        var emailArgument = new Argument<string>("email", "User email address");
        var passwordOption = new Option<string>(
            new[] { "-p", "--password" },
            "User password (will prompt if not provided)"
        );
        var firstNameOption = new Option<string?>(
            new[] { "-f", "--first-name" },
            "User first name"
        );
        var lastNameOption = new Option<string?>(
            new[] { "-l", "--last-name" },
            "User last name"
        );
        var tenantIdOption = new Option<string?>(
            new[] { "-t", "--tenant-id" },
            "Tenant ID (defaults to environment variable)"
        );

        command.AddArgument(emailArgument);
        command.AddOption(passwordOption);
        command.AddOption(firstNameOption);
        command.AddOption(lastNameOption);
        command.AddOption(tenantIdOption);

        command.SetHandler(ExecuteAsync, emailArgument, passwordOption, firstNameOption, lastNameOption, tenantIdOption);

        return command;
    }

    private static async Task ExecuteAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? tenantId)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            tenantId ??= config.GetTenantId();
            if (string.IsNullOrEmpty(tenantId))
            {
                console.Error("Tenant ID not provided. Set B2CONNECT_TENANT environment variable or use --tenant-id");
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(password))
            {
                password = console.PromptPassword("Enter password") ?? throw new InvalidOperationException("Password required");
            }

            var identityUrl = config.GetServiceUrl("identity");
            var client = new CliHttpClient(identityUrl);

            var payload = new
            {
                email,
                password,
                firstName = firstName ?? string.Empty,
                lastName = lastName ?? string.Empty,
                tenantId = Guid.Parse(tenantId)
            };

            console.Spinner("Creating user...", async () =>
            {
                var response = await client.PostAsync<CreateUserResponse>("/auth/create-user", payload);

                if (response.Success && response.Data != null)
                {
                    console.Success($"User created successfully");
                    console.Info($"User ID: {response.Data.Id}");
                    console.Info($"Email: {response.Data.Email}");
                }
                else
                {
                    console.Error(response.Error ?? "Failed to create user");
                }
            });
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class CreateUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
