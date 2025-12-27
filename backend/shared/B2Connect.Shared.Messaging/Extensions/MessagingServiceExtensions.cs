namespace B2Connect.Shared.Messaging.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Wolverine;

/// <summary>
/// Messaging Extension Methods
/// </summary>
public static class MessagingServiceExtensions
{
    /// <summary>
    /// Adds Wolverine messaging with default configuration
    /// </summary>
    public static IHostBuilder AddWolverineMessaging(
        this IHostBuilder hostBuilder,
        Action<WolverineOptions>? configure = null)
    {
        hostBuilder.UseWolverine(opts =>
        {
            // Default configuration for all services
            opts.ServiceName = "B2Connect";

            // Local queue for development
            opts.LocalQueue("default")
                .UseDurableInbox();

            // Apply custom configuration if provided
            configure?.Invoke(opts);
        });

        return hostBuilder;
    }

    /// <summary>
    /// Adds Wolverine with RabbitMQ transport
    /// </summary>
    public static IHostBuilder AddWolverineWithRabbitMq(
        this IHostBuilder hostBuilder,
        string rabbitMqUri,
        Action<WolverineOptions>? configure = null)
    {
        hostBuilder.UseWolverine(opts =>
        {
            opts.ServiceName = "B2Connect";

            // Configure RabbitMQ
            opts.UseRabbitMq(rabbitMqUri)
                .AutoProvision()
                .AutoPurgeOnStartup();

            // Publish events to RabbitMQ exchange
            opts.PublishAllMessages()
                .ToRabbitExchange("b2connect-events");

            // Apply custom configuration if provided
            configure?.Invoke(opts);
        });

        return hostBuilder;
    }

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

