import { useEffect, useState } from 'react';
import MediaSurface from '../MediaSurface/MediaSurface';
import { getLegacyMediaUrl } from '../../services/media/legacy-media-url-api';

interface Props extends React.HTMLAttributes<HTMLDivElement> {
    id: string;
    className?: string;
}

export default function PageViewer({ id, className, ...rest }: Props) {
    const [src, setSrc] = useState<string | null>(null);

    useEffect(() => {
        /*
        fetch(`/api/media/${id}`)
            .then(res => res.json())
            .then(data => {
                setSrc(data.url); // assume video for now
            });
         */
        // For now: no fetch, just construct URL
        setSrc(getLegacyMediaUrl(id));
    }, [id]);

    if (!src) return <div>Loading...</div>;

    return (
        <div className={className} {...rest}>
            <MediaSurface src={src} />
        </div>
    );
}