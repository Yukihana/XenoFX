import { Link } from 'react-router-dom';
import { getThumbUrl } from '../../services/asset-thumb/thumb-api';
import { useResponsive, type DeviceType, type Orientation } from '../../contexts/ResponsiveContext';
import styles from './MediaCardComponent.module.css';

interface MediaCardProps {
    id: string,
    mediaType: string,
    title: string,
    subtext: string,
    thumbText: string,
    source: string,
    styleOverride?: string, // Optional prop for display tile
}

const getStyle = (
    styleOverride?: string,
    deviceType?: DeviceType,
    orientation?: Orientation
): string => {
    if (styleOverride)
        return styles[`${styleOverride}Card`] || styles.stackCard;

    return deviceType !== 'mobile' || orientation === 'landscape'
        ? styles.gridCard : styles.stackCard;
}

export default function MediaCardComponent(
    props: MediaCardProps
) {
    const linkTo = `/view?id=${props.id}`; // TODO Build using a centralized api service
    const thumbUrl: string = getThumbUrl(props.id);

    const { deviceType, orientation } = useResponsive();

    const cardStyle = getStyle(
        props.styleOverride,
        deviceType,
        orientation
    );

    return (
        <Link to={linkTo} className={cardStyle} data-media-type={props.mediaType}>
            {/*square-minimum, expand horizontally to fill container grid*/}
            <div className="thumbnail-container">{/*Top 70%*/}
                <img className="thumb-image" src={thumbUrl} />
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