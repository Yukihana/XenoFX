import { useEffect, useState } from 'react';
import { Player, type PlayerState } from './Player';

/*React hook: for state sync with the player object*/
export function usePlayer(player: Player) {
    const [state, setState] = useState<PlayerState>(player.getState());

    useEffect(() => {
        const interval = setInterval(() => {
            setState(player.getState());
        }, 300);

        return () => clearInterval(interval);
    }, [player]);

    return {
        state,
        play: () => player.play(),
        pause: () => player.pause(),
        seek: (t: number) => player.seek(t),
    };
}