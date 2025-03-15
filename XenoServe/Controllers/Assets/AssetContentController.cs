using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetUpload;
using XenoFx.Services.Api.AssetUpload.Models;

namespace XenoServe.Controllers.Assets;

[Route("api/assets")]
[ApiController]
public class AssetContentController : ControllerBase
{
    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IAssetUploadService _assetUpload;
    private readonly ILogger<AssetContentController> _logger;

    public AssetContentController(
        IAssetAbstractionService assetAbstraction,
        IAssetUploadService assetUpload,
        ILogger<AssetContentController> logger)
    {
        _assetAbstraction = assetAbstraction;
        _assetUpload = assetUpload;
        _logger = logger;
    }

    [HttpGet]
    [Route("download")]
    public async Task<IActionResult> DownloadAsync([FromQuery] string path, CancellationToken ctoken = default)
    {
        try
        {
            string fullPath = await _assetAbstraction.GetContentPathAsync(path, ctoken);
            return new PhysicalFileResult(fullPath, "application/octet-stream");
        }
        catch (InvalidDataException ex) when (ex.Message is AssetAbstractionService.ResourceNotFoundMessage)
        {
            _logger.LogWarning(ex, "Attempt to access unlisted resource at: {path}", path);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bad request: {path}", path);
            return BadRequest();
        }
    }

    [HttpPost("upload")]
    [Route("upload")]
    public async Task<IActionResult> UploadAsync(CancellationToken ctoken = default)
    {
        try
        {
            var form = await Request.ReadFormAsync(ctoken);
            IFormFile? data = form.Files["data"];         // the file
            string title = form["title"].ToString();      // the name of the file inferred from the title or lowest path segment of the page depending on the site.
            string pageUrl = form["pageurl"].ToString();  // the original page this file was cached from
            string dataUrl = form["dataUrl"].ToString();  // the source url of the data file.

            if (data is null)
                return BadRequest("Invalid request: Upload data missing.");

            AssetUploadRequest request = new(data.OpenReadStream())
            {
                Title = title,
                PageUrl = pageUrl,
                DataUrl = dataUrl,
            };

            var uploadResult = await _assetUpload.RegisterUploadAsync(request, ctoken);

            return Ok(uploadResult.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed.");
            return StatusCode(500, new { message = "Internal error." }); // TODO add logging reference id system.
        }
    }
}