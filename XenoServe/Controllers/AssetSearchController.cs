using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XenoFx.Services.Api.AssetSearch;
using XenoServe.OfflineProfile;

namespace XenoServe.Controllers;

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

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Get([FromQuery] string id, CancellationToken ctoken = default)
    {
        // turn this into api-v1 defaulting to whole string search
        // v2 will do both full-match and word-by-word relevance match
        // - and only revert to v1 if match-whole-string equivalent parameter is used

        var result = await _assetSearch.GetHave(id, ctoken);
        return Ok(result);
    }
}