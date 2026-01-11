using B2X.Notifications.Adapters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// register no-op adapters until real implementations are added
builder.Services.AddSingleton<IEmailAdapter, NoOpEmailAdapter>();
builder.Services.AddSingleton<IPushAdapter, NoOpPushAdapter>();
builder.Services.AddSingleton<ISmsAdapter, NoOpSmsAdapter>();
builder.Services.AddSingleton<IWhatsAppAdapter, NoOpWhatsAppAdapter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/health", () => Results.Ok("Notifications OK"));

app.MapPost("/api/notifications/send", async (
    NotificationRequest req,
    IEmailAdapter email,
    IPushAdapter push,
    ISmsAdapter sms,
    IWhatsAppAdapter wa,
    ILogger<Program> log) =>
{
    log.LogInformation("Received notification request: {Type} for tenant {Tenant}", req.Type, req.TenantId);

    // simple routing: honor req.Type, otherwise broadcast to all adapters
    try
    {
        if (string.Equals(req.Type, "email", System.StringComparison.OrdinalIgnoreCase))
        {
            await email.SendAsync(req.TenantId, req.Payload);
        }
        else if (string.Equals(req.Type, "push", System.StringComparison.OrdinalIgnoreCase))
        {
            await push.SendAsync(req.TenantId, req.Payload);
        }
        else if (string.Equals(req.Type, "sms", System.StringComparison.OrdinalIgnoreCase))
        {
            await sms.SendAsync(req.TenantId, req.Payload);
        }
        else if (string.Equals(req.Type, "whatsapp", System.StringComparison.OrdinalIgnoreCase))
        {
            await wa.SendAsync(req.TenantId, req.Payload);
        }
        else
        {
            await Task.WhenAll(
                email.SendAsync(req.TenantId, req.Payload),
                push.SendAsync(req.TenantId, req.Payload),
                sms.SendAsync(req.TenantId, req.Payload),
                wa.SendAsync(req.TenantId, req.Payload)
            );
        }

        return Results.Accepted();
    }
    catch (System.Exception ex)
    {
        log.LogError(ex, "Error sending notification");
        return Results.StatusCode(500);
    }
});

app.MapControllers();

app.Run();

public record NotificationRequest(string Type, string TenantId, object? Payload);
