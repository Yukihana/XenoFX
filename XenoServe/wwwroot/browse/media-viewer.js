/**
 * @typedef {Object} AssetSearchCardData
 * @property {string} id         // Unique identifier for the asset
 * @property {string} mediaType  // MIME type
 * @property {string} title      // Title of the asset
 * @property {string} source     // Original path of the asset
 */

// Required for url state
import { updateQueryParam, getQueryParams } from "../js/shared/url-state.js";

export class XenoServeMediaViewer {
    // elements
    #viewer = null;
    #viewerContainer = null;
    #nextMediaTitle = undefined;
    #nextPreview = undefined;
    #nextButton = undefined;
    #autoPlay = undefined;
    // parameters
    #playbackHistory = [];
    #id = null;
    #nextId = null;
    #keywords = '';

    constructor() {

    }

    // properties

    get id() {
        return this.#id;
    }
    set id(value) {
        this.#id = value;
        updateQueryParam("id", value);
    }

    // events

    onPlayNextAsync = async (event) => {
        try {
            await this.playNextAsync();
        } catch (error) {
            console.error(error.message);
        }

    };

    onPlaybackEndedAsync = async (event) => {
        try {
            await this.playNextAsync();
        } catch (error) {
            console.error(error.message);
        }
    };

    onMediaLinkClickAsync = async (event) => {
        try {
            const target = event.target.closest('.media-card');
            if (!target)
                return;

            const id = target.getAttribute('data-media-id');
            if (typeof id !== 'string')
                return;
            if (id.Trim() === '')
                return;

            await this.playMediaAsync(id);
        } catch (error) {
            console.error(error.message);
        }
    }

    // api

    playMediaAsync = async (id, keywords = '') => {
        if (typeof id !== 'string' || id.trim() === '')
            throw new error();

        // Reset history
        if (typeof this.#playbackHistory !== 'array')
            this.#playbackHistory = [];
        this.#playbackHistory.length = 0;

        // Set state (viewer, next, related, etc)
        this.#keywords = keywords;
        await this.setPageState(id);
    };

    playNextAsync = async () => {
        // If next is missing, acquire it
        let nextId = this.#nextId;
        if (typeof nextId !== 'string' || nextId.trim() === '') {
            const nextItem = await this.fetchNextAsync(0, this.id, '', '');
            nextId = nextItem.source;
        }
        if (typeof nextId !== 'string' || nextId.trim() === '')
            throw new error();
        this.#nextId = nextId;

        // Push current id to history
        if (typeof this.#playbackHistory !== 'array')
            this.#playbackHistory = [];
        this.#playbackHistory.push(this.id);

        // Set next id as the current state
        await this.setPageState(nextId);
    };

    setPageState = async (id) => {
        this.id = id;
        const playbackIndex = this.#playbackHistory.length;
        const keywords = this.#keywords;
        const originalId = playbackIndex > 0 ? playbackIndex[0] : '';

        // Set next id

        /**@type {Promise<AssetSearchCardData>}*/
        const nextItem = await this.fetchNextAsync(playbackIndex, id, originalId, keywords);
        this.#nextId = nextItem.source; // set correct parameters here
        this.#nextMediaTitle.innerHTML = nextItem.title;
        this.setNextInQueue(nextItem);

        // Set viewer
        await this.setViewerAsync(id);

        // Set related
        await this.setRelatedItems(id);
    }

    // Shared

    /** @returns {Promise<AssetSearchCardData>} */
    fetchNextAsync = async (playbackIndex = 0, currentId, originalId, keywords) => {
        const params = new URLSearchParams();

        params.set('playbackIndex', playbackIndex);

        if (currentId) params.set('currentId', currentId);
        if (originalId) params.set('originalId', originalId);
        if (keywords) params.set('keywords', keywords);

        const url = `/api/assets/nextplore?${params.toString()}`;
        var response = await fetch(url);

        if (!response.ok) {
            throw new error(response.message);
        }

        return await response.json();
    }

    // Set media in page

    setViewerAsync = async (id) => {
        // Temporary: change source in video player
        // Future: change viewer based on content type
        const viewer = this.#viewer;

        // fetch info, get type
        // set viewer by type
        // for now:
        await this.setVideoViewer(id);
    };

    setNextInQueue = nextItem => {
    }

    setRelatedItems = async (id) => {
    }

    // Video

    setVideoViewer = async (id) => {
        // set the player in position here, when applicable
        /** @type {HTMLVideoElement} */
        const viewer = this.ensureVideoElement();

        // set the video
        viewer.src = `/api/assets/file?id=${id}`;
        viewer.load();
        try {
            await viewer.play();
        } catch (error) {
            if (error.name === 'NotAllowedError')
                console.log('Autoplay blocked.');
            else
                throw error;
        }

    };
    /** @returns {HTMLVideoElement} */
    ensureVideoElement = () => {
        const isVideo = this.#viewer instanceof HTMLVideoElement;
        if (!isVideo) {
            const newVideoElement = document.createElement('video');
            if (this.#viewer instanceof HTMLElement) {
                this.#viewer.replaceWith(newVideoElement);
            } else if (this.#viewerContainer instanceof HTMLDivElement) {
                this.#viewerContainer.innerHTML = '';
                this.#viewerContainer.appendChild(newVideoElement);
            } else {
                throw new Error('Cannot find media container.');
            }
            this.#viewer = newVideoElement;
        }

        // Apply details
        this.#viewer.style.display = 'block';

        return this.#viewer;
    };

    // Set media types

    setImageViewer = () => {
        // create a control wrapper
        // event trigger when timer finishes
        this.setOtherViewer();
    };
    setAudioViewer = () => {
        this.setOtherViewer();
    };
    setTextViewer = () => {
        this.setOtherViewer();
    };
    setOtherViewer = () => {
        console.error('viewer not implemented');
    };

    // Startup / Restore

    setElements = () => {
        this.#viewerContainer = document.querySelector('#viewer-container');
        /** @type {HTMLVideoElement} */
        this.#viewer = document.querySelector('#media-viewer');
        this.#nextMediaTitle = document.querySelector('#next-media-title');
        this.#viewer.addEventListener('ended', event => this.onPlaybackEndedAsync(event));
        document.querySelector('#next-media-btn').addEventListener('click', event => this.onPlayNextAsync(event));
    };
    restoreState = () => {
        const restoredState = getQueryParams();

        // id
        let id = restoredState.get("id");
        if (typeof id === 'string') {
            id = id.trim();
            if (id !== '') {
                // this.#id = id; // Next line already does that
                this.setPageState(id);
            }
        }
    };

    // Bootstrap

    initialize = () => {
        this.setElements();
        this.restoreState();
    };
}