using Quartz;
using B2Connect.CatalogService.Jobs;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Extensions;

/// <summary>
/// Extension methods for PIM Sync service with Quartz Scheduler
/// </summary>
public static class PimSyncQuartzExtensions
{
    /// <summary>
    /// Add PIM Sync Service with Quartz Scheduler
    /// Configures scheduled synchronization via Quartz
    /// Enables progress tracking through ISyncProgressService
    /// </summary>
    public static IServiceCollection AddPimSyncWithQuartz(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register sync progress service (in-memory)
        // For production, consider Redis or database
        services.AddSingleton<ISyncProgressService, SyncProgressService>();

        // Register the sync service itself
        services.AddScoped<IPimSyncService, PimSyncService>();

        // Register Quartz scheduler
        services.AddQuartz(q =>
        {
            q.SchedulerId = "B2Connect-PimSync-Scheduler";

            // Configure job (PimSyncJob)
            var jobKey = new JobKey("PimSyncJob");
            q.AddJob<PimSyncJob>(opts =>
            {
                opts.WithIdentity(jobKey)
                    .WithDescription("PIM data synchronization job")
                    .StoreDurably();
            });

            // Configure trigger from appsettings
            var enabled = configuration.GetValue<bool>("PimSync:Enabled", false);
            if (enabled)
            {
                // Check for Cron expression first
                var cronExpression = configuration.GetValue<string>("PimSync:CronExpression");

                if (!string.IsNullOrEmpty(cronExpression))
                {
                    try
                    {
                        var cronTriggerKey = new TriggerKey("PimSyncCronTrigger");
                        var cronTrigger = TriggerBuilder.Create()
                            .WithIdentity(cronTriggerKey)
                            .WithDescription("Cron trigger for PIM sync job")
                            .ForJob(jobKey)
                            .WithCronSchedule(cronExpression)
                            .Build();

                        q.AddTrigger(cronTrigger);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail - invalid cron expression
                        var logger = services.BuildServiceProvider().GetService<ILogger<PimSyncQuartzExtensions>>();
                        logger?.LogError(ex, "Invalid Cron expression for PimSync: {CronExpression}", cronExpression);
                    }
                }
                else
                {
                    // Fall back to interval-based trigger
                    var intervalSeconds = configuration.GetValue<int>("PimSync:IntervalSeconds", 3600);

                    var triggerKey = new TriggerKey("PimSyncIntervalTrigger");
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .WithDescription("Interval trigger for PIM sync job")
                        .ForJob(jobKey)
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(intervalSeconds)
                            .RepeatForever())
                        .Build();

                    q.AddTrigger(trigger);
                }
            }
        });

        // Add Quartz hosted service
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
