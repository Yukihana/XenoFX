import styles from './MediaSurface.module.css'

interface Props {
    src: string;
}

export default function MediaSurface({ src }: Props) {
    return (
        <div className={styles.mediaSurface}>
            <video
                src={src}
                controls
                style={{ width: '100%' }}
            />
        </div>
    );
}