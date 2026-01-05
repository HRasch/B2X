using System.CommandLine;
using System.CommandLine.Invocation;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;

namespace B2Connect.CLI.Administration.Commands.AuthCommands;

public static class LoginCommand
{
    public static Command Create()
    {
        var command = new Command("login", "Authenticate and get JWT token");

        var emailArgument = new Argument<string>("email", "User email address");
        var passwordOption = new Option<string>(
            ["-p", "--password"], "User password (will prompt if not provided)");
        var saveOption = new Option<bool>(
            ["-s", "--save"],
            getDefaultValue: () => false,
            description: "Save token to B2CONNECT_TOKEN environment variable");

        command.AddArgument(emailArgument);
        command.AddOption(passwordOption);
        command.AddOption(saveOption);

        command.SetHandler(ExecuteAsync, emailArgument, passwordOption, saveOption);

        return command;
    }

    private static async Task ExecuteAsync(string email, string password, bool save)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            if (string.IsNullOrEmpty(password))
            {
                password = console.PromptPassword("Enter password") ?? throw new InvalidOperationException("Password required");
            }

            var identityUrl = config.GetServiceUrl("identity");
            var client = new CliHttpClient(identityUrl);

            var payload = new { email, password };

            LoginResponse? response = null;

            console.Spinner("Authenticating...", async () =>
            {
                var result = await client.PostAsync<LoginResponse>("/auth/login", payload);
                response = result.Data;

                if (!result.Success || response == null)
                {
                    console.Error(result.Error ?? "Authentication failed");
                    Environment.Exit(1);
                }
            });

            if (response != null)
            {
                console.Success("Authentication successful");
                console.Info($"Token: {response.Token[..50]}...");
                console.Info($"Expires in: {response.ExpiresIn} seconds");

                if (save)
                {
                    Environment.SetEnvironmentVariable("B2CONNECT_TOKEN", response.Token, EnvironmentVariableTarget.User);
                    console.Success("Token saved to B2CONNECT_TOKEN");
                }
            }
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public Guid UserId { get; set; }
    }
}