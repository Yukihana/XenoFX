using CSX.DotNet.Common.Data.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetMedia.DTOs;
using XenoServe.Features.AssetDelivery.DTOs;

namespace XenoServe.Features.AssetDelivery;

[Route(ControllerRoute)]
[ApiController]
public class AssetDeliveryController : ControllerBase
{
    // Infrastructure

    private readonly IAssetDeliveryOrchestrator _orchestrator;
    private readonly ILogger<AssetDeliveryController> _logger;

    // Data

    public const string ControllerRoute = "api/assets/delivery";
    public const string FileRoute = "file";

    public static string FileApiPath => $"{ControllerRoute}/{FileRoute}";

    public static string FileApiTemplate => $"{FileApiPath}?id={{0}}&type={{1}}";

    // Lifecycle

    public AssetDeliveryController(
        IAssetDeliveryOrchestrator orchestrator,
        ILogger<AssetDeliveryController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    // Endpoints

    [HttpGet]
    [Route(FileRoute)]
    public async Task<IActionResult> FileAsync(
        [FromQuery] AssetDeliveryRequest request,
        CancellationToken ctoken = default)
    {
        try
        {
            AssetMediaResult result = await _orchestrator.GetFilePathAsync(
                request.Id, request.TranscodeType, ctoken);

            // Note, Temporary:
            // if expected content type is provided,
            // verify the extension matches.
            // Content type will be fixed once ids are implemented,
            // and can simply be set from the model.
            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                string expected = request.Type;
                string actual = Path.GetExtension(result.FullPath).TrimStart('.');

                if (!request.Type.Equals(actual, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"Content type mismatch for request id:{request.Id}, expected:{request.Type}, actual:{actual}");
                }
            }

            // Log it
            var range = Request.Headers.Range;
            if (range.Count > 0)
                _logger.LogInformation("Delivering: {fullPath}; Ranges: {ranges}", result.FullPath, range);
            else
                _logger.LogInformation("Delivering: {fullPath}", result.FullPath);

            // Attempt to deliver the file
            return new PhysicalFileResult(result.FullPath, result.ContentType)    // Do not use File() wrapper as it ends up assigning the wrong type.
            {
                EnableRangeProcessing = true,
            };
        }
        catch (ResourceNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for id: {id}", request.Id);
            return NotFound(new { message = "Resource not found." }); // TODO: add logging reference id system.
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Type mismatch for id:{id}", request.Id);
            return BadRequest("Content type mismatch. Please refresh.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in {controller}/{route} for id: {id}",
                ControllerRoute, FileRoute, request.Id);
            return Problem(
                detail: "An internal server error has occured.",
                statusCode: 500);
        }
    }
}