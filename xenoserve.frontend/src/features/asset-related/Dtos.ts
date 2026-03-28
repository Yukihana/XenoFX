import type { MediaItem } from '../../shared/types/MediaItem';

// Matches server-side C# AssetRelatedCardData
export type AssetRelatedCardDataDto = MediaItem;

// Matches server-side C# AssetRelatedQuery
export interface AssetRelatedQueryDto {
    id: string;
    keywords: string;
    page: number;
    pageSize: number;
}

// Matches server-side C# AssetRelatedResult
export interface AssetRelatedResultDto extends AssetRelatedQueryDto {
    results: AssetRelatedCardDataDto[];
    timestamp: string; // DateTime -> ISO string

    count: number;
}