using XenoFx.Services.Api.AssetSearch.Contracts;
using XenoServe.Features.AssetSearch.DTOs;

namespace XenoServe.Features.AssetSearch;

public static class AssetSearchExtensions
{
    public static AssetSearchQuery MapToQueryDto(
        this AssetSearchQueryParams queryParams)
    {
        AssetSearchQuery result = new();

        if (!string.IsNullOrWhiteSpace(queryParams.Keywords))
            result.Keywords = queryParams.Keywords;
        if (queryParams.Page.HasValue)
            result.Page = queryParams.Page.Value;
        if (queryParams.PageSize.HasValue)
            result.PageSize = queryParams.PageSize.Value;
        if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            result.SortBy = queryParams.SortBy;
        if (!string.IsNullOrWhiteSpace(queryParams.SortDirection))
            result.SortDirection = queryParams.SortDirection;

        return result;
    }

    public static AssetNextploreQuery MapToQueryDto(
        this AssetNextploreQueryParams queryParams)
    {
        AssetNextploreQuery result = new();

        if (!string.IsNullOrWhiteSpace(queryParams.Keywords))
            result.Keywords = queryParams.Keywords;
        if (queryParams.PlaybackIndex.HasValue)
            result.PlaybackIndex = queryParams.PlaybackIndex.Value;
        if (!string.IsNullOrWhiteSpace(queryParams.OriginalId))
            result.OriginalId = queryParams.OriginalId;
        if (!string.IsNullOrWhiteSpace(queryParams.CurrentId))
            result.CurrentId = queryParams.CurrentId;

        return result;
    }
}