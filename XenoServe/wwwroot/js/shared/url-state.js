// url-state.js

export function updateQueryParam(key, value) {
    const params = new URLSearchParams(window.location.search);

    if (value != null && value !== "") {
        params.set(key, value);
    } else {
        params.delete(key);
    }

    const newUrl = `${window.location.pathname}?${params.toString()}`;
    window.history.replaceState({}, "", newUrl);
}

export function getQueryParams() {
    return new URLSearchParams(window.location.search);
}