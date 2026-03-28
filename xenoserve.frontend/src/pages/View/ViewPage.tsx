import { useParams } from 'react-router-dom';
import styles from './ViewPage.module.css';
import PageViewer from '../../components/Viewer/PageViewer';

export default function ViewPage() {
    const { id } = useParams();
    /*
    const [meta, setMeta] = useState<any>(null);

    function handleMediaChange(media: any) {
        fetch(`/api/media/${media.id}/details`)
            .then(res => res.json())
            .then(setMeta);
    }
    return (
        <div className={styles.viewBody}>
            <PageViewer id={id!} onMediaChange={handleMediaChange} />

            <div>
                <h2>{meta?.title}</h2>
                <p>{meta?.description}</p>
            </div>
        </div>
    );
    */

    if (!id) return <div>Invalid ID</div>;

    return (
        <main className={styles.viewPageBody}>
            <section id='viewer'>
                <PageViewer id={id} className={styles.pageViewer} />
            </section>
            <section>
                <h2>Media Details Placeholder</h2>
                <span></span>
            </section>
        </main>
    );
}