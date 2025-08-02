import { Link } from 'react-router-dom';
// import { buildMediaLink } from '@/services/URLService';

interface GridMediaCardProps {
    title: string,
    subtext: string,
    thumbUrl: string,
    thumbText: string,
    id: string,
}

export default function GridMediaCardComponent(
    props: GridMediaCardProps
) {
    const linkTo = `/view?id=${props.id}`; // TODO Build using a centralized api service

    return (
        <Link to={linkTo} className="grid-media-card">
            {/*square-minimum, expand horizontally to fill container grid*/}
            <div className="thumbnail-container">{/*Top 70%*/}
                <img className="thumb-image" src={props.thumbUrl} />
                <div className="overlay">
                    <div className="label"> {/*overlay, bottom right, grey background 50% opacity*/}
                        <span className="text">{props.thumbText}</span>
                    </div>
                </div>
            </div>
            <div className="details">{/*Bottom 30%*/}
                <div className="title">{props.title}</div>
                <div className="subtext">{props.subtext}</div>
            </div>
        </Link >
    );
}