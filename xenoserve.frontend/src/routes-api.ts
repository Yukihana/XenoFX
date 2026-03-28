const isDev = import.meta.env.DEV;
const hostname = window.location.hostname;
const devPort = 9600;
const baseUrl = `http://${hostname}:${devPort}`;

/*
const rawBase = import.meta.env.VITE_API_BASE_URL || '';
const apiBase = isDev
    ? (rawBase.endsWith('/') ? rawBase.slice(0, -1) : rawBase)
    : ''; // empty string for production
    */

// Public API

export const ApiPaths = {
    assets: {
        search: '/api/assets/search',
        related: '/api/assets/related',
        thumb: '/api/assets/thumbs/static',
        details: '/api/assets/details',
    },
}

export function normalizePath(
    path: string,
    queryParams?: URLSearchParams,
) {
    const base = isDev ? baseUrl : '';
    const queryString = queryParams?.toString();
    return `${base}${path}${queryString ? `?${queryString}` : ''}`;
}

// for legacy compatibility
export const ApiRoutes = {
    search: (query: string) => `${ApiPaths.assets.search}?q=${encodeURIComponent(query)}`,
    mediaDetails: (id: string) => `${ApiPaths.assets.details}/${id}`,
}