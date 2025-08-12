import { normalizePath, ApiPaths } from '../../routes-api';

export const getThumbUrl = (id: string): string => {
    const params = new URLSearchParams({
        id: id,
    });

    return normalizePath(`${ApiPaths.assets.thumb}?${params.toString()}`);
};