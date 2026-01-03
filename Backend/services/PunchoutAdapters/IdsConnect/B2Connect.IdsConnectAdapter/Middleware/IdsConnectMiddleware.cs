using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace B2Connect.IdsConnectAdapter.Middleware;

/// <summary>
/// IDS Connect Middleware
/// Handles IDS Connect specific request processing
/// </summary>
public class IdsConnectMiddleware
{
    private readonly RequestDelegate _next;

    public IdsConnectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Set content type for IDS Connect responses
        if (context.Request.Path.StartsWithSegments("/api/ids"))
        {
            context.Response.ContentType = "application/xml";
        }

        // Add IDS Connect headers
        context.Response.Headers.Add("X-IDS-Version", "2.5");
        context.Response.Headers.Add("X-IDS-Provider", "B2Connect");

        await _next(context);
    }
}