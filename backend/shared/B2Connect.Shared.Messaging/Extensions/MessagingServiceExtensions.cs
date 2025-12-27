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
    /// Adds Wolverine with RabbitMQ transport (Development)
    /// In development, this uses local queues. RabbitMQ integration is planned for production.
    /// </summary>
    public static IHostBuilder AddWolverineWithRabbitMq(
        this IHostBuilder hostBuilder,
        string rabbitMqUri,
        Action<WolverineOptions>? configure = null)
    {
        hostBuilder.UseWolverine(opts =>
        {
            opts.ServiceName = "B2Connect";

            // Development: Use local durable queue
            opts.LocalQueue("default")
                .UseDurableInbox();

            // TODO: Enable RabbitMQ transport when package is properly configured
            // opts.UseRabbitMq(rabbitMqUri)
            //     .AutoProvision()
            //     .AutoPurgeOnStartup();

            // Publish events to local queue for now
            // opts.PublishAllMessages()
            //     .ToRabbitExchange("b2connect-events");

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

