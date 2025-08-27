using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Processing.AssetStaticThumbnail;
using IOFile = System.IO.File;

namespace XenoServe.Features.AssetThumbnail;

[Route("api/assets/thumbs")]
[ApiController]
public class AssetThumbnailController : ControllerBase
{
    private readonly IAssetStaticThumbnailService _staticThumb;
    private readonly ILogger<AssetThumbnailController> _logger;

    public AssetThumbnailController(
        IAssetStaticThumbnailService staticThumb,
        ILogger<AssetThumbnailController> logger)
    {
        _staticThumb = staticThumb;
        _logger = logger;
    }

    [HttpGet("static")]
    public async Task<IActionResult> StaticThumbnailAsync(
        [FromQuery] string id,
        CancellationToken ctoken = default)
    {
        try
        {
            ctoken.ThrowIfCancellationRequested();

            string thumbPath = await _staticThumb.GetFilePathAsync(id, ctoken);

            if (!IOFile.Exists(thumbPath))
                return NotFound($"Thumbnail not found for id: {id}");

            // Determine MIME type (simple way using extension)
            string contentType = Path.GetExtension(thumbPath)?.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            // Return the physical file
            return PhysicalFile(thumbPath, contentType, enableRangeProcessing: true);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to deliver static thumbnail for request with id: {id}", id);
            return BadRequest(ex.Message);
        }
    }
}