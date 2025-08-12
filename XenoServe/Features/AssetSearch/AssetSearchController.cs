using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoServe.Features.AssetSearch.DTOs;

namespace XenoServe.Features.AssetSearch;

[Route("api/assets")]
[ApiController]
public sealed partial class AssetSearchController : ControllerBase
{
    private readonly IAssetSearchOrchestrator _orchestrator;
    private readonly ILogger<AssetSearchController> _logger;

    public AssetSearchController(
        IAssetSearchOrchestrator orchestrator,
        ILogger<AssetSearchController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    // Search

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] AssetSearchQueryParams queryParams,
        CancellationToken ctoken = default)
    {
        try
        {
            var response = await _orchestrator.SearchAsync(
                queryParams: queryParams,
                ipAddress: HttpContext.Connection.RemoteIpAddress,
                ctoken: ctoken);

            _logger.LogInformation(
                "Returning {page}:{count} of {total} for the query '{query}' generated in {elapsed}. Results: {@list}",
                response.Page,              // Page
                response.Count,             // Count
                response.Total,             // Total
                response.Keywords,          // Query
                $"[{response.Duration}]",   // Duration
                response.Results);

            // Results
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for assets with query '{query}'", queryParams);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // NextItem
    [HttpGet]
    [Route("nextplore")]
    public async Task<IActionResult> NextploreAsync(
        [FromQuery] AssetNextploreQueryParams queryParams,
        CancellationToken ctoken = default)
    {
        try
        {
            var response = await _orchestrator.NextploreAsync(
                queryParams: queryParams,
                ipAddress: HttpContext.Connection.RemoteIpAddress,
                ctoken: ctoken);

            return response == null
                ? NoContent() // Only on db row count = 0, since results loop.
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for assets with query '{query}'", queryParams);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // Cache (aka Get Have; To be removed when ids are implemented)

    [HttpGet]
    [Route("matches")]
    public async Task<IActionResult> GetHaveAsync(
        [FromQuery] string query,
        CancellationToken ctoken = default)
    {
        try
        {
            var result = await _orchestrator.GetHaveAsync(
                query: query,
                ctoken: ctoken);

            _logger.LogInformation(
                "Found {count} results for query '{query}': {list}",
                result.Length,
                query, result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking assets catalogue with query '{query}'", query);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}