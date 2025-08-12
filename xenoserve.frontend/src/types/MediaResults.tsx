// MediaResults.tsx
import type { MediaItem } from './MediaItem';

export interface MediaResults {
    keywords: string;
    page: number;
    pageSize: number;
    sortBy: string;
    sortDirection: string;

    items: MediaItem[];
    total: number;
    duration: string; // C# TimeSpan will serialize as ISO8601 duration or string
    timestamp: string; // DateTime -> ISO string

    count: number;
};