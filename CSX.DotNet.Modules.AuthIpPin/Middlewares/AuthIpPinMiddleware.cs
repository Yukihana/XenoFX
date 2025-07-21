using CSX.DotNet.Modules.AuthIpPin.Services.AuthApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Middlewares;

public partial class AuthIpPinMiddleware : IMiddleware
{
    // Infrastructure

    private readonly IAuthApiService _authService;
    private readonly ILogger<AuthIpPinMiddleware> _logger;

    // Lifecycle

    public AuthIpPinMiddleware(
        IAuthApiService authService,
        ILogger<AuthIpPinMiddleware> logger)
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

        // Only act on /api/* routes (TODO, make it a part of configuration)
        if (path.StartsWithSegments("/api"))
        {
            var ip = context.Connection.RemoteIpAddress;

            if (ip is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Bad Request: Remote IP address is not available.");
                return;
            }

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