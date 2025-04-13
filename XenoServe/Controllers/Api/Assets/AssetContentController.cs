using CSX.Common.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Api.AssetUpload;
using XenoFx.Services.Api.AssetUpload.DTOs;

namespace XenoServe.Controllers.Api.Assets;

[Route(ControllerRoute)]
[ApiController]
public class AssetContentController : ControllerBase
{
    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IAssetUploadService _assetUpload;
    private readonly ILogger<AssetContentController> _logger;

    // Data

    public const string ControllerRoute = "api/assets";
    public const string DownloadRoute = "download";
    public const string FileRoute = "file";
    public const string UploadRoute = "upload";

    public static string DownloadPath => $"{ControllerRoute}/{DownloadRoute}";
    public static string DownloadApiTemplate => $"{DownloadPath}?id={{0}}";

    public static string FilePath => $"{ControllerRoute}/{FileRoute}";
    public static string FileApiTemplate => $"{FilePath}?path={{0}}";

    public static string UploadPath => $"{ControllerRoute}/{UploadRoute}";

    // Lifecycle

    public AssetContentController(
        IAssetAbstractionService assetAbstraction,
        IAssetUploadService assetUpload,
        ILogger<AssetContentController> logger)
    {
        _assetAbstraction = assetAbstraction;
        _assetUpload = assetUpload;
        _logger = logger;
    }

    // Endpoints

    [HttpGet]
    [Route(DownloadRoute)]
    public async Task<IActionResult> DownloadAsync([FromQuery] string id, CancellationToken ctoken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route(FileRoute)]
    public async Task<IActionResult> FileAsync([FromQuery] string path, CancellationToken ctoken = default)
    {
        // Once asset database is up, store the mime type in the db, to prevent redundant analysis overhead.
        try
        {
            string fullPath = await _assetAbstraction.GetContentPathAsync(path, ctoken);
            string fileName = Path.GetFileName(fullPath);
            string contentType = MimeTyping.GetMimeType(Path.GetExtension(fullPath));

            _logger.LogInformation("Attempting to deliver resource located at: {fullPath}", fullPath);
            return new PhysicalFileResult(fullPath, contentType)    // Do not use File() wrapper as it ends up assigning the wrong type.
            {
                EnableRangeProcessing = true,
            };
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
    [Route(UploadRoute)]
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