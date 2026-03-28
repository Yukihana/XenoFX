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
    const linkTo = `/view/${props.source}`; // TODO Build using a centralized api service
    const thumbUrl: string = getThumbUrl(props.source);

    const { deviceType, orientation } = useResponsive();

    const cardStyle = getStyle(
        props.styleOverride,
        deviceType,
        orientation
    );

    return (
        <Link to={linkTo} className={cardStyle} data-media-type={props.mediaType}>
            {/*square-minimum, expand horizontally to fill container grid*/}
            <div className={styles.thumbnailContainer}>{/*Top 70%*/}
                <img className={styles.image} src={thumbUrl} />
                <div className={styles.overlay}>
                    <div className={styles.label}> {/*overlay, bottom right, grey background 50% opacity*/}
                        <span className={styles.text}>{props.thumbText}</span>
                    </div>
                </div>
            </div>
            <div className={styles.detailContainer}>{/*Bottom 30%*/}
                <div className={styles.title}>{props.title}</div>
                <div className={styles.subtext}>{props.subtext}</div>
            </div>
        </Link >
    );
}