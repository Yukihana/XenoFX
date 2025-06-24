using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using XenoFx.Services.Auth.IpPinAuth;

namespace XenoFx.Middleware.IpPinAuth;

public partial class IpPinAuthMiddleware : IMiddleware
{
    // Infrastructure

    private readonly IIpPinAuthService _authService;
    private readonly ILogger<IpPinAuthMiddleware> _logger;

    // Lifecycle

    public IpPinAuthMiddleware(
        IIpPinAuthService authService,
        ILogger<IpPinAuthMiddleware> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    // Interface : IMiddleware

    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var path = context.Request.Path;

        // Only act on /api/* routes
        if (path.StartsWithSegments("/api"))
        {
            var ip = context.Connection.RemoteIpAddress;

            if (ip is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Bad Request: Remote IP address is not available.");
                return;
            }

            // Optionally handle IPv4-mapped IPv6
            if (ip.IsIPv4MappedToIPv6)
                ip = ip.MapToIPv4();

            var isAuthorized = await _authService.AuthorizeIpAsync(ip, context.RequestAborted);
            if (!isAuthorized)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: IP not authorized.");
                return;
            }
        }

        await next(context);
    }
}