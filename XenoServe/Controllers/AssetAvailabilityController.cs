using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XenoServe.OfflineProfile;

namespace XenoServe.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed partial class AssetAvailabilityController : ControllerBase
{
    private readonly NitefoxCore _core;

    public AssetAvailabilityController(NitefoxCore core)
    {
        _core = core;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        var result = await _core.GetHaveAsset(id);
        if (result.Any())
            return Ok(result);
        else
            return NotFound();
    }
}