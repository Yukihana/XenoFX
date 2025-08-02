export const ApiRoutes = {
    mediaDetails: (id: string) => `/api/media/${id}`,
    search: (query: string) => `/api/search?q=${encodeURIComponent(query)}`
}