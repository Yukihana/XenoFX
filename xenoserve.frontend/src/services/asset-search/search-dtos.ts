// Matches server-side C# AssetSearchCardData
export interface AssetSearchCardDataDto {
    id: string;          // unique identifier for the asset
    mediaType: string;   // mime type
    title: string;       // title of the asset
    subText: string;     // subtitle or additional text
    thumbText: string;   // text for the thumbnail
    source: string;      // original path of the asset
}

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