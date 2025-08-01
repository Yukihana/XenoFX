using CSX.DotNet.Common.Data.Exceptions;
using CSX.DotNet.Common.IO;
using CSX.DotNet.Modules.FileUploader.Services.UploadApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;

namespace XenoServe.Controllers.Api.Assets;

[Route(ControllerRoute)]
[ApiController]
public class AssetContentController : ControllerBase
{
    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly IUploadApiService _assetUpload;
    private readonly ILogger<AssetContentController> _logger;

    // Data

    public const string ControllerRoute = "api/assets";
    public const string FileRoute = "file";

    public static string FileApiPath => $"{ControllerRoute}/{FileRoute}";

    public static string FileApiTemplate => $"{FileApiPath}?id={{0}}&type={{1}}";

    // Lifecycle

    public AssetContentController(
        IAssetAbstractionService assetAbstraction,
        IUploadApiService assetUpload,
        ILogger<AssetContentController> logger)
    {
        _assetAbstraction = assetAbstraction;
        _assetUpload = assetUpload;
        _logger = logger;
    }

    // Endpoints

    [HttpGet]
    [Route(FileRoute)]
    public async Task<IActionResult> FileAsync(
        [FromQuery] string id,
        [FromQuery] string? type,
        CancellationToken ctoken = default)
    {
        try
        {
            // make this more OO instead of calling one off methods
            // ie get the AssetInfo, then use abstraction as a function facilitator

            // Placeholder for [ID lookup -> AssetInfo]
            // currently using [searchKey(as id) -> relativePath]
            string path = await _assetAbstraction.GetFirstMatchingAssetPathAsync(id, ctoken);

            // Placeholder for cross-checking asset info with presences for the file's current location;
            // returns usable full path;
            // currently using [relativePath -> fullPath] and notifies if the file is missing
            string fullPath = await _assetAbstraction.GetAssetFilePathAsync(path, ctoken);

            // Analyse content type
            string extension = Path.GetExtension(fullPath).TrimStart('.').ToLowerInvariant(); // Normalize extension to lowercase without leading dot
            string contentType = MimeTyping.GetMimeType(extension);

            // if expected content type is provided, verify the extension matches (this is a temporary measure)
            if (!string.IsNullOrEmpty(type) &&
                !extension.Equals(type.ToLowerInvariant()))
            {
                _logger.LogWarning("The content's specified type:{type} for id:{id} didn't match the file:{fullPath}", type, id, fullPath);
                return BadRequest("Content type mismatch. Please refresh.");
            }

            // Log it
            var range = Request.Headers.Range;
            if (range.Count > 0)
                _logger.LogInformation("Delivering: {fullPath}; Ranges: {ranges}", fullPath, range);
            else
                _logger.LogInformation("Delivering: {fullPath}", fullPath);

            // Attempt to deliver the file
            return new PhysicalFileResult(fullPath, contentType)    // Do not use File() wrapper as it ends up assigning the wrong type.
            {
                EnableRangeProcessing = true,
            };
        }
        catch (ResourceNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for id: {id}", id);
            return NotFound(new { message = "Resource not found." }); // TODO: add logging reference id system.
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in {controller}/{route} for id: {id}",
                ControllerRoute, FileRoute, id);
            return Problem(
                detail: "An internal server error has occured.",
                statusCode: 500);
        }
    }
}