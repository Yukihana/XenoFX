using CSX.Common.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetUpload;
using XenoFx.Services.Api.AssetUpload.DTOs;

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
            string fileName = Path.GetFileName(fullPath);
            string contentType = MimeTyping.GetMimeType(Path.GetExtension(fullPath));
            _logger.LogInformation("Attempting to deliver resource located at: {fullPath}", fullPath);
            return new PhysicalFileResult(fullPath, contentType);
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

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> UploadAsync(
        IFormFile data,
        [FromForm] string title = "",
        [FromForm] string pageUrl = "",
        [FromForm] string dataUrl = "",
        CancellationToken ctoken = default)
    {
        try
        {
            if (data is null)
                return BadRequest("Invalid request: Upload data missing.");

            _logger.LogInformation("Found data file with name: {name}", data.FileName);

            AssetUploadRequest request = new(data.OpenReadStream())
            {
                Title = title,
                Filename = data.FileName,
                ContentMimeType = data.ContentType,
                PageUrl = pageUrl,
                DataUrl = dataUrl,
            };

            var uploadResult = await _assetUpload.RegisterUploadAsync(request, ctoken);

            return Ok(uploadResult.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed.");
            return StatusCode(500, new { message = "Upload failed." }); // TODO add logging reference id system.
        }
    }
}