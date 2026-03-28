import { searchAssetsAsync } from "../../features/asset-search/api";
import type { AssetSearchQueryDto, AssetSearchResultDto, AssetSearchCardDataDto } from "../../features/asset-search/types";
import type { MediaResults } from "../../shared/types/MediaResults";
import type { MediaItem } from "../../shared/types/MediaItem";

// -------------------
// Mappers
// -------------------

const toQueryDto = (
    keywords: string,
    page: number,
    pageSize: number,
    sortBy: string,
    sortDirection: string
): AssetSearchQueryDto => ({
    keywords,
    page,
    pageSize,
    sortBy,
    sortDirection
});

const toMediaItem = (item: AssetSearchCardDataDto): MediaItem => ({
    id: item.id,
    mediaType: item.mediaType,
    title: item.title,
    subText: item.subText,
    thumbText: item.thumbText,
    source: item.source,
} satisfies MediaItem);

const toMediaResults = (dto: AssetSearchResultDto): MediaResults => ({
    // Copy query passthrough parameters
    keywords: dto.keywords,
    page: dto.page,
    pageSize: dto.pageSize,
    sortBy: dto.sortBy,
    sortDirection: dto.sortDirection,

    // Map the results
    items: dto.results.map(toMediaItem),
    total: dto.total,
    count: dto.count,
    duration: dto.duration,
    timestamp: dto.timestamp
});

// -------------------
// Public API
// -------------------

export async function searchItemsAsync(
    keywords: string,
    page = 0,
    pageSize = 30,
    sortBy = "relevance",
    sortDirection = "desc"
): Promise<MediaResults> {
    const queryDto = toQueryDto(keywords, page, pageSize, sortBy, sortDirection);
    const resultDto = await searchAssetsAsync(queryDto);
    return toMediaResults(resultDto);
}

export const getSuggestionsAsync = (page: number, pageSize: number) =>
    searchItemsAsync("", page, pageSize, "relevance", "desc");