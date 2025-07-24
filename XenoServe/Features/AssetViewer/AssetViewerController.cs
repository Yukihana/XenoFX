using CSX.Common.Data.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Abstraction.AssetAbstraction;
using XenoFx.Services.Abstraction.AssetAbstraction.DTOs;
using XenoServe.Controllers.Api.Assets;
using XenoServe.Data;
using XenoServe.Shared.Extensions;

namespace XenoServe.Features.AssetViewer;

[Route("web/assets/viewer")]
public class AssetViewerController : Controller
{
    // Infrastructure

    private readonly IAssetAbstractionService _assetAbstraction;
    private readonly ILogger<AssetViewerController> _logger;

    // Data

    public const string ViewName = "View";

    // Lifecycle

    public AssetViewerController(
        IAssetAbstractionService assetAbstraction,
        ILogger<AssetViewerController> logger)
    {
        _assetAbstraction = assetAbstraction;
        _logger = logger;
    }

    // Core Endpoints

    [HttpGet]
    public async Task<IActionResult> RenderAsync(
        [FromQuery] string id,
        [FromQuery] ViewRenderType renderType = ViewRenderType.Full,
        CancellationToken ctoken = default)
    {
        try
        {
            _logger.LogInformation("Viewer requested for id: {id}", id);

            // Validate: Terminate on invalid referers.
            if (renderType == ViewRenderType.Partial)
                Request.EnsureSameOriginForPartial();

            // Prepare the model
            AssetViewerInfo info = await _assetAbstraction.GetAssetViewerInfoAsync(id, ctoken);
            string sourceUrl = string.Format(AssetContentController.FileApiTemplate, id, info.Extension);
            AssetViewerViewModel model = new()
            {
                Title = info.Title,
                SourceUrl = Url.Content($"~/{sourceUrl}"),
                MimeType = info.MimeType,
                Extension = info.Extension,
                RenderType = renderType,
            };

            // Render the view.
            return View(ViewName, model); // Don't use PartialView(). No need to duplicate code. Handle it in the View.
        }
        catch (Exception ex) when (ex is MissingRefererOrOriginException or CORSViolationException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Access denied.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to render asset viewer for id: {id}", id);
            return BadRequest("Failed to render asset viewer.");
        }
    }

    // Addon Endpoints

    [HttpGet]
    [Route("partial")]
    public async Task<IActionResult> PartialAsync([FromQuery] string id, CancellationToken ctoken = default)
        => await RenderAsync(id: id, renderType: ViewRenderType.Partial, ctoken: ctoken);

    [HttpGet]
    [Route("embed")]
    public async Task<IActionResult> EmbedAsync([FromQuery] string id, CancellationToken ctoken = default)
        => await RenderAsync(id: id, renderType: ViewRenderType.Embed, ctoken: ctoken);

    [HttpGet]
    [Route("iframe")]
    public async Task<IActionResult> IFrameAsync([FromQuery] string id, CancellationToken ctoken = default)
        => await RenderAsync(id: id, renderType: ViewRenderType.IFrame, ctoken: ctoken);

    // Shared Internal
}