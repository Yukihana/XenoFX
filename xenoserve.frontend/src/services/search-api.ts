import { ApiRoutes } from '../routes-api';

export interface SearchQuery {
    query: string,
    page: number,
    pageSize: number,
}

function buildUrl(query: SearchQuery) {
    const params = new URLSearchParams({
        query: query.query,
        page: query.page.toString(),
        pageSize: query.pageSize.toString()
    });

    const path = ApiRoutes.search;
    const apiBase = import.meta.env.VITE_API_BASE_URL;
    return `${apiBase}${path}?${params.toString()}`;
}

export async function searchItemsAsync<T>(query: SearchQuery): Promise<T> {
    const token = localStorage.getItem('auth_token');
    const url = buildUrl(query);

    const response = await fetch(url, {
        headers: {
            Authorization: token ? `Bearer ${token}` : ''
        }
    });

    if (!response.ok)
        throw new Error(`API Error: ${response.status}`);

    const responseContent: T = await response.json();
    return responseContent;
}