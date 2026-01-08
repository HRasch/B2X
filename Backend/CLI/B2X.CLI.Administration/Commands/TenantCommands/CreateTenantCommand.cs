using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;

namespace B2X.CLI.Administration.Commands.TenantCommands;

public static class CreateTenantCommand
{
    public static Command Create()
    {
        var command = new Command("create", "Create a new tenant");

        var nameArgument = new Argument<string>("name", "Tenant name");
        var adminEmailOption = new Option<string>(
            ["-a", "--admin-email"], "Admin email address");
        var adminPasswordOption = new Option<string>(
            ["-p", "--admin-password"], "Admin password");

        command.AddArgument(nameArgument);
        command.AddOption(adminEmailOption);
        command.AddOption(adminPasswordOption);

        command.SetHandler(ExecuteAsync, nameArgument, adminEmailOption, adminPasswordOption);

        return command;
    }

    private static async Task ExecuteAsync(string name, string adminEmail, string adminPassword)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            var tenancyUrl = config.GetServiceUrl("tenancy");
            var client = new CliHttpClient(tenancyUrl);

            var token = config.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.SetAuthorizationToken(token);
            }

            var payload = new
            {
                name,
                adminEmail,
                adminPassword
            };

            console.Spinner("Creating tenant...", async () =>
            {
                var response = await client.PostAsync<CreateTenantResponse>("/tenants/create", payload);

                if (response.Success && response.Data != null)
                {
                    console.Success("Tenant created successfully");
                    console.Info($"Tenant ID: {response.Data.Id}");
                    console.Info($"Name: {response.Data.Name}");
                }
                else
                {
                    console.Error(response.Error ?? "Failed to create tenant");
                }
            });
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class CreateTenantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}