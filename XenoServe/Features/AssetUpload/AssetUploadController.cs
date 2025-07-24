using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoServe.Data;
using XenoServe.Features.AssetUpload.DTOs;

namespace XenoServe.Features.AssetUpload;

[Route("api/assets/uploader")]
public class AssetUploadController : ControllerBase
{
    // Infrastructure

    private readonly IAssetUploadOrchestrator _orchestrator;
    private readonly ILogger<AssetUploadController> _logger;

    // Lifecycle

    public AssetUploadController(
        IAssetUploadOrchestrator orchestrator,
        ILogger<AssetUploadController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    // Endpoints

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAsync(
        [FromForm] AssetUploadRequest request,
        CancellationToken ctoken = default)
    {
        AssetUploadContext? context = null;

        try
        {
            ctoken.ThrowIfCancellationRequested();

            RequestAuthorization auth = new()
            {
                TimeStamp = DateTimeOffset.UtcNow,
                IPAddress = HttpContext.Connection.RemoteIpAddress,
                // Add auth parameters when implemented
            };

            // Handle the upload operation
            AssetUploadResult result = await _orchestrator.CacheAndRegisterAsync(
                context: new(auth, request),
                ctoken: ctoken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for context: {@context}", context);
            return StatusCode(500, new { message = "Failed to upload file." });
        }
    }
}