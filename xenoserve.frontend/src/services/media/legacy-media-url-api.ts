import { normalizePath } from '../../routes-api';

// Placeholder for actual media-info api call.
// This file will probably be deleted down the line.
export function getLegacyMediaUrl(id: string): string {
    const params = new URLSearchParams({
        id: id
    });
    return normalizePath('/api/assets/delivery/file', params);
}