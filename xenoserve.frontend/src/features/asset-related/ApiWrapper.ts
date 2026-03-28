import type { RelatedItem, RelatedItemsBucket } from './Models';
import { fetchRelatedAsync } from './Endpoints';
import type { AssetRelatedCardDataDto, AssetRelatedQueryDto, AssetRelatedResultDto } from './Dtos';

// -------------------
// Mappers
// -------------------

const toQueryDto = (
    id: string,
    keywords: string,
    page: number,
    pageSize: number
): AssetRelatedQueryDto => ({
    id,
    keywords,
    page,
    pageSize,
});

const toRelatedItem = (item: AssetRelatedCardDataDto): RelatedItem => ({
    id: item.id,
    mediaType: item.mediaType,
    title: item.title,
    subText: item.subText,
    thumbText: item.thumbText,
    source: item.source,
} satisfies RelatedItem);

const toRelatedItemsBucket = (dto: AssetRelatedResultDto): RelatedItemsBucket => ({
    items: dto.results.map(toRelatedItem),
    /*add page and pagesize here when bucket system is added*/
});

// -------------------
// Public API
// -------------------

export async function getRelatedAsync(
    id: string,
    keywords: string,
    page: number,
    pageSize: number
): Promise<RelatedItemsBucket> {
    const query = toQueryDto(id, keywords, page, pageSize);
    const result = await fetchRelatedAsync(query);
    return toRelatedItemsBucket(result);
}