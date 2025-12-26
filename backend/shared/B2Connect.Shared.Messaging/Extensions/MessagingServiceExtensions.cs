namespace B2Connect.Shared.Messaging.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Quartz;

/// <summary>
/// Messaging Extension Methods
/// Wolverine-Setup gehört direkt in service-spezifische Program.cs
/// </summary>
public static class MessagingServiceExtensions
{
    /// <summary>
    /// Registriert Quartz Job Scheduling
    /// </summary>
    public static IServiceCollection AddQuartzScheduling(
        this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            // Standard Quartz Configuration
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
