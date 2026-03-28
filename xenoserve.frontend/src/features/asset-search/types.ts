import type { MediaItem } from '../../shared/types/MediaItem';

// Matches server-side C# AssetSearchCardData
export type AssetSearchCardDataDto = MediaItem;

// Matches server-side C# AssetSearchQuery
export interface AssetSearchQueryDto {
    keywords: string;
    page: number;
    pageSize: number;
    sortBy: string;
    sortDirection: string;
}

// Matches server-side C# AssetSearchResult
export interface AssetSearchResultDto extends AssetSearchQueryDto {
    results: AssetSearchCardDataDto[];
    total: number;
    duration: string; // C# TimeSpan will serialize as ISO8601 duration or string
    timestamp: string; // DateTime -> ISO string

    count: number;
}