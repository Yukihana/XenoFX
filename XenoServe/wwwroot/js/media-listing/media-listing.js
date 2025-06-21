/**
 * Definitions for intellisense support:
 * - Api data models need to be matched with their backend counterparts.
 *
 * @typedef {Object} AssetSearchResult
 * @property {string} keywords
 * @property {number} page
 * @property {number} pageSize
 * @property {string} sortBy
 * @property {string} sortDirection
 * @property {AssetSearchCardData[]} matches
 * @property {number} total
 * @property {string} duration   // TimeSpan usually comes as ISO string (e.g. "00:00:01.234")
 * @property {string} timestamp  // DateTime usually as ISO string (e.g. "2024-01-01T12:34:56Z")
 * @property {number} count      // Derived from matches.length
 *
 * @typedef {Object} AssetSearchCardData
 * @property {string} id         // Unique identifier for the asset
 * @property {string} mediaType  // MIME type
 * @property {string} title      // Title of the asset
 * @property {string} source     // Original path of the asset
 */

// Required for url state
import { updateQueryParam, getQueryParams } from "../shared/url-state.js";

export class XenoServeMediaListing {
    #view = "";
    #type = "";
    #keywords = "";
    #page = 0;
    #pageSize = 30;
    #totalItems = 0;

    constructor() {
        this.onResultClicked = this.onResultClicked.bind(this);
    }

    // Properties : Search

    get keywords() {
        return this.#keywords;
    }
    set keywords(value) {
        this.#keywords = value;
        updateQueryParam("keywords", value);
    }

    get page() {
        return this.#page;
    }
    set page(value) {
        this.#page = value;
        updateQueryParam("page", value);
    }

    get pageSize() {
        return this.#pageSize;
    }
    set pageSize(value) {
        this.#pageSize = value;
        updateQueryParam("pageSize", value);
    }

    get totalItems() {
        return this.#totalItems;
    }
    set totalItems(value) {
        this.#totalItems = value;
        console.log(this.#totalItems);
    }

    // Properties : Media

    get view() {
        return this.#view;
    }
    set view(value) {
        this.#view = value;
        updateQueryParam("view", value);
    }

    get type() {
        return this.#type;
    }
    set type(value) {
        this.#type = value;
        updateQueryParam("type", value);
    }

    // Form submit

    onFormSubmit = event => {
        console.log(this.#page);
        event.preventDefault(); // Prevent page reload
        window.xenoserveMediaListing.doSearchScaffold(0);
    }

    // Search

    doSearchScaffold = async (page) => {
        // this class handles parameter and state operations
        try {
            // Update this method to restore pagination data from class scope members before running the actual api call.
            // TODO pull these from their respective input boxes
            const keywords = this.searchInput.value.trim();
            let pageSize = this.pageSize;

            // Ensure pagesize is a valid number
            if (!pageSize || pageSize < 1) {
                pageSize = 30;
            }

            const success = await this.doSearch(keywords, page, pageSize);

            if (success) {
                this.keywords = keywords;
                this.page = page;
                this.pageSize = pageSize;
            }
        }
        catch (error) {
            console.debug(error.message);
        }
    }

    doSearch = async (keywords, page, pageSize) => {
        try {
            const response = await this.fetchSearchResults(keywords, page, pageSize);

            // bail if input has changed
            if (this.searchInput.value.trim() !== keywords)
                return false;

            this.populateResults(response.results);
            this.updatePagination(response.page, response.pageSize, response.total);
            return true;
        } catch (error) {
            console.error("Error during search:", error);
        }
    }

    fetchSearchResults = async (keywords, page, pageSize) => {
        try {
            let keywordsComponent = "";
            if (keywords)
                keywordsComponent = "keywords=" + encodeURIComponent(keywords) + "&";

            var url = "/api/assets/search?"
                + keywordsComponent
                + "page=" + encodeURIComponent(page) + "&"
                + "pagesize=" + encodeURIComponent(pageSize);

            /** @type {AssetSearchResult} */
            var response = await fetch(url);

            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }

            return await response.json();
        } catch (error) {
            console.debug(error.message);
        }
    }

    populateResults = results => {
        this.searchResultsList.innerHTML = '';

        // Map and append results
        const resultCards = results.map(this.createResultCard);
        this.searchResultsList.append(...resultCards);
    }

    // Results

    createResultCard = result => {
        const card = document.createElement('div');
        card.className = "media-card";

        const thumb = document.createElement('div');
        thumb.className = "media-card-thumb";
        card.appendChild(thumb);

        const detail = document.createElement('div');
        detail.className = "media-card-details";
        card.appendChild(detail);

        const text = document.createElement('div');
        text.className = "media-title";
        text.innerHTML = result.title;

        detail.appendChild(text);

        card.title = result.title;
        card.setAttribute('data-media-id', result.source);  // change this to id when using the id system instead
        card.setAttribute('data-media-type', result.mediaType);

        return card;
    }

    onResultClicked = event => {
        try {
            const target = event.target.closest('.media-card');
            if (target) {
                const id = target.getAttribute('data-media-id');
                const type = target.getAttribute('data-media-type');

                this.setMedia(id, type, false);
                this.view = id;
                this.type = type;
            }
        } catch (error) {
            console.debug(error.message);
        }
    }

    setMedia = (id, type) => {
        // Temporary: change source in video player
        // Future: change viewer based on content type
        const viewer = this.mediaContainer.querySelector('video#media-viewer');
        viewer.style.display = 'block';
        viewer.setAttribute('src', `/api/assets/file?id=${id}`);
        viewer.load();
    }

    // pagination

    updatePagination = (currentPage, pageSize, totalItems) => {
        this.#totalItems = totalItems;

        // disable enable pagination buttons
        const lastPage = Math.max(0, Math.ceil(totalItems / pageSize) - 1);
        const isFirst = currentPage <= 0;
        const isLast = currentPage >= lastPage;

        this.paginationFirst.disabled = isFirst;
        this.paginationPrevious.disabled = isFirst;
        this.paginationNext.disabled = isLast;
        this.paginationLast.disabled = isLast;
    }

    onFirst = event => {
        let page = this.page;
        if (page > 0) {
            this.doSearchScaffold(0);
        }
    }
    onPrevious = event => {
        let page = this.page;
        if (page > 0) {
            this.doSearchScaffold(page - 1);
        }
    }
    onNext = event => {
        let page = this.page;
        let pageSize = this.pageSize;
        if (pageSize < 1)
            pageSize = 30;
        let lastPage = Math.max(0, Math.ceil(this.totalItems / this.pageSize) - 1);
        if (page < lastPage) {
            this.doSearchScaffold(page + 1);
        }
    }
    onLast = event => {
        let page = this.page;
        let pageSize = this.pageSize;
        if (pageSize < 1)
            pageSize = 30;
        let lastPage = Math.max(0, Math.ceil(this.totalItems / this.pageSize) - 1);
        if (page < lastPage)
            this.doSearchScaffold(lastPage);
    }

    // Internal

    isValid = data => {
        return data !== null && data !== undefined;
    }

    // Setup

    setupElements = document => {
        // Register search event
        this.searchForm = document.querySelector('#search-form');
        this.searchForm.addEventListener('submit', window.xenoserveMediaListing.onFormSubmit);

        this.searchInput = document.querySelector('#search-input');
        this.searchSubmit = document.querySelector('#search-submit');

        // pagination
        this.paginationContainer = document.querySelector('#pagination-container');

        this.paginationFirst = document.querySelector('#pagination-first');
        this.paginationFirst.addEventListener('click', event => this.onFirst(event));
        this.paginationPrevious = document.querySelector('#pagination-previous');
        this.paginationPrevious.addEventListener('click', event => this.onPrevious(event));
        this.paginationNext = document.querySelector('#pagination-next');
        this.paginationNext.addEventListener('click', event => this.onNext(event));
        this.paginationLast = document.querySelector('#pagination-last');
        this.paginationLast.addEventListener('click', event => this.onLast(event));

        this.mediaContainer = document.querySelector('#media-container');
        this.mediaViewer = document.querySelector('#media-viewer');

        // Results
        this.searchResultsList = document.querySelector('#results-list');
        this.searchResultsList.addEventListener("click", event => window.xenoserveMediaListing.onResultClicked(event));
    }

    reloadState = () => {
        const restoredState = getQueryParams();

        // set parameter backing values
        const keywords = restoredState.get("keywords");
        if (this.isValid(keywords)) {
            this.#keywords = keywords;
            this.searchInput.value = this.keywords;
        }

        const page = parseInt(restoredState.get("page"));
        if (!isNaN(page))
            this.#page = page;

        let pageSize = parseInt(restoredState.get("pageSize"));
        if (!isNaN(pageSize)) {
            this.#pageSize = pageSize;
        }

        this.doSearchScaffold(page);

        const view = restoredState.get("view");
        if (this.isValid(view))
            this.#view = view;

        const type = restoredState.get("type");
        if (this.isValid(type))
            this.#type = type;

        // update interface

        if (this.view && this.type)
            this.setMedia(this.view, this.type);
    }

    // Bootstrap

    initialize = () => {
        this.setupElements(document);
        this.reloadState();
    }
}