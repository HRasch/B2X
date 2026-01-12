using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace B2X.IdsConnectAdapter.Middleware;

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
        context.Response.Headers["X-IDS-Version"] = "2.5";
        context.Response.Headers["X-IDS-Provider"] = "B2X";

        await _next(context);
    }
}
