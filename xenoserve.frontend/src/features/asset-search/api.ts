import { normalizePath, ApiPaths } from '../../routes-api';
import type { AssetSearchQueryDto, AssetSearchResultDto } from './types';

const buildUrl = (query: AssetSearchQueryDto) => {
    const params = new URLSearchParams({
        keywords: query.keywords,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
        sortBy: query.sortBy,
        sortDirection: query.sortDirection,
    });

    return normalizePath(`${ApiPaths.assets.search}?${params.toString()}`);
}

// -------------------
// Public API
// -------------------

export async function searchAssetsAsync(
    query: AssetSearchQueryDto
): Promise<AssetSearchResultDto> {
    const token = localStorage.getItem('auth_token');
    const url = buildUrl(query);

    const response = await fetch(url, {
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
    });

    if (!response.ok) {
        throw new Error(`HTTP error ${response.status}`);
    }

    const contentType = response.headers.get("content-type");
    if (!contentType?.includes("application/json")) {
        throw new Error(`Expected JSON response but got: ${contentType}`);
    }

    const payload: AssetSearchResultDto = await response.json();
    if (!payload) {
        throw new Error("Payload is missing from OK response");
    }

    return payload;
}