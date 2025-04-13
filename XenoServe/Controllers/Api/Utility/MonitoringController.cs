using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XenoFx.Services.Api.StateMonitor;

namespace XenoServe.Controllers.Api.Utility;

[Route("api/[controller]")]
[ApiController]
public class MonitoringController : ControllerBase
{
    private readonly IStateMonitorService _stateMonitor;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(
        IStateMonitorService stateMonitor,
        ILogger<MonitoringController> logger)
    {
        _stateMonitor = stateMonitor;
        _logger = logger;
    }

    [HttpGet]
    [Route("assets")]
    public IActionResult Assets([FromQuery] string type)
    {
        switch (type.ToLowerInvariant())
        {
            case "stateindex":
                return Ok(_stateMonitor.GetAssetRepositoryStateIndex());

            case "lastmodified":
                return Ok(_stateMonitor.GetAssetRepositoryLastModified());

            default:
                _logger.LogInformation("Missing type parameter in query.");
                return BadRequest("The 'type' parameter is required.");
        }
    }
}