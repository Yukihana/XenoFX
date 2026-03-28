export type PlayerKind = 'video' | 'audio';

export interface PlayerState {
    currentTime: number;
    duration: number;
    paused: boolean;
}

/* Owns the player/media element */
export class Player {
    private el: HTMLVideoElement | HTMLAudioElement;

    constructor(kind: PlayerKind) {
        this.el = document.createElement(kind);
        this.el.controls = false;
    }

    get element() {
        return this.el;
    }

    load(src: string) {
        this.el.src = src;
    }

    play() {
        this.el.play();
    }

    pause() {
        this.el.pause();
    }

    seek(time: number) {
        this.el.currentTime = time;
    }

    getState(): PlayerState {
        return {
            currentTime: this.el.currentTime,
            duration: this.el.duration || 0,
            paused: this.el.paused,
        };
    }
}