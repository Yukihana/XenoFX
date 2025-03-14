using Microsoft.AspNetCore.Mvc;
using XenoFx.Services.Abstraction.AssetAbstraction;

namespace XenoServe.Controllers.Assets;

[Route("api/assets")]
[ApiController]
public class AssetContentController : ControllerBase
{
    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly ILogger<AssetContentController> _logger;

    public AssetContentController(
        IAssetAbstractionService assetAbstraction,
        ILogger<AssetContentController> logger)
    {
        _assetAbstraction = assetAbstraction;
        _logger = logger;
    }

    [HttpGet]
    [Route("content")]
    public async Task<IActionResult> GetAsync([FromQuery] string path, CancellationToken ctoken = default)
    {
        try
        {
            string fullPath = await _assetAbstraction.GetContentPathAsync(path, ctoken);
            return new PhysicalFileResult(fullPath, "application/octet-stream");
        }
        catch (InvalidDataException ex) when (ex.Message is AssetAbstractionService.ResourceNotFoundMessage)
        {
            _logger.LogWarning(ex, "Attempt to access unlisted resource: {path}", path);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bad request: {path}", path);
            return BadRequest();
        }
    }
}