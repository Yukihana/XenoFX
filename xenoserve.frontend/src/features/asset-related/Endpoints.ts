import { normalizePath, ApiPaths } from '../../routes-api';
import type { AssetRelatedQueryDto, AssetRelatedResultDto } from './Dtos';

const buildUrl = (query: AssetRelatedQueryDto) => {
    const params = new URLSearchParams({
        id: query.id,
        keywords: query.keywords,
        page: query.page.toString(),
        pageSize: query.pageSize.toString(),
    });

    return normalizePath(`${ApiPaths.assets.related}?${params.toString()}`);
}

// -------------------
// Public API
// -------------------

export async function fetchRelatedAsync(
    query: AssetRelatedQueryDto
): Promise<AssetRelatedResultDto> {
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

    const payload: AssetRelatedResultDto = await response.json();
    if (!payload) {
        throw new Error("Payload is missing from OK response");
    }

    return payload;
}