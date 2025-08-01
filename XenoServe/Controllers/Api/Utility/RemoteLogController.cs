using CSX.DotNet.Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoServe.Controllers.Api.Utility;

[Route("api/utility/[controller]")]
[ApiController]
public class RemoteLogController : ControllerBase
{
    // Infrastructure
    private readonly ILogger<RemoteLogController> _logger;

    // Data
    public const string LogTemplate = "[{Source}] {Message}\n{Data}";

    // Lifecycle
    public RemoteLogController(
        ILogger<RemoteLogController> logger)
    {
        _logger = logger;
    }

    // Endpoints

    [HttpPost]
    [Route("debug")]
    public async Task<IActionResult> DebugAsync([FromBody] LogPackage package, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        await StoreLogDataAsync(LogLevel.Debug, package, ctoken);

        _logger.LogDebug(LogTemplate,
            package.Source,
            package.Message,
            package.Data);

        return Ok();
    }

    [HttpPost]
    [Route("info")]
    public async Task<IActionResult> InfoAsync([FromBody] LogPackage package, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        await StoreLogDataAsync(LogLevel.Information, package, ctoken);

        _logger.LogInformation(LogTemplate,
            package.Source,
            package.Message,
            package.Data);

        return Ok();
    }

    [HttpPost]
    [Route("warn")]
    public async Task<IActionResult> WarnAsync([FromBody] LogPackage package, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        await StoreLogDataAsync(LogLevel.Warning, package, ctoken);

        _logger.LogWarning(LogTemplate,
            package.Source,
            package.Message,
            package.Data);

        return Ok();
    }

    [HttpPost]
    [Route("error")]
    public async Task<IActionResult> ErrorAsync([FromBody] LogPackage package, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();
        await StoreLogDataAsync(LogLevel.Error, package, ctoken);

        _logger.LogError("[{Source}] {Message}\n{Data}",
            package.Source,
            package.Message,
            package.Data);

        return Ok();
    }

    // Common

    private async Task StoreLogDataAsync(LogLevel level, LogPackage package, CancellationToken ctoken = default)
    {
        // Skipping log storage since it's not implemented yet.
        _ = DateTime.UtcNow;
        _ = level;
        _ = package;
        _ = ctoken;
        _ = _logger;
        await Task.Yield();
    }
}