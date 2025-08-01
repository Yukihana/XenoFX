using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Api.AssetSearch;
using XenoFx.Services.Api.AssetSearch.Contracts;

namespace XenoServe.Features.AssetSearch;

[Route("api/assets")]
[ApiController]
public sealed partial class AssetSearchController : ControllerBase
{
    private readonly IAssetSearchService _assetSearch;

    private readonly ILogger<AssetSearchController> _logger;

    public AssetSearchController(
        IAssetSearchService assetSearch,
        ILogger<AssetSearchController> logger)
    {
        _assetSearch = assetSearch;
        _logger = logger;
    }

    // Search

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] AssetSearchQuery query,
        CancellationToken ctoken = default)
    {
        try
        {
            var partialSeed = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
            var response = await _assetSearch.SearchAsync(query, partialSeed, ctoken);
            var start = response.Page * response.PageSize;
            var end = start + response.Count;

            _logger.LogInformation(
                "Returning {page}:{count} of {total} for the query '{query}' generated in {elapsed}. Results: {@list}",
                response.Page,              // Page
                response.Count,             // Count
                response.Total,             // Total
                query.Keywords,             // Query
                $"[{response.Duration}]",   // Duration
                response.Results);

            // Results
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for assets with query '{query}'", query);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // NextItem
    [HttpGet]
    [Route("nextplore")]
    public async Task<IActionResult> NextploreAsync(
        [FromQuery] AssetNextploreQuery query,
        CancellationToken ctoken = default)
    {
        try
        {
            var partialSeed = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";
            var response = await _assetSearch.NextploreAsync(query, partialSeed, ctoken);

            return response == null
                ? NoContent() // Only on db row count = 0, since results loop.
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for assets with query '{query}'", query);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // PreviousItem

    // Cache (aka Get Have; To be removed when ids are implemented)

    [HttpGet]
    [Route("matches")]
    public async Task<IActionResult> GetHaveAsync([FromQuery] string query, CancellationToken ctoken = default)
    {
        var result = await _assetSearch.GetHaveAsync(query, ctoken);
        _logger.LogInformation("Found {count} results for query '{query}': {list}", result.Length, query, result);
        return Ok(result);
    }
}