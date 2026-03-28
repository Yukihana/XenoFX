import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getRelatedAsync } from '../ApiWrapper';
import type { RelatedItem, RelatedItemsBucket } from '../Models';
import MediaCardComponent from '../../../components/MediaCard/MediaCardComponent';
import styles from './RelatedView.module.css'

export default function RelatedView() {
    const { id } = useParams<{ id: string }>();
    const [relatedItems, setRelatedItems] = useState<RelatedItem[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!id) return;
        setLoading(true);
        document.title = "Home";

        const fetchData = async () => {
            try {
                const response: RelatedItemsBucket = await getRelatedAsync(id, '', 1, 15);
                setRelatedItems(response.items);
            } catch (err) {
                console.error("Failed to fetch items", err);

                if (err instanceof Error) {
                    setError(err.message);
                } else {
                    setError(String(err));
                }
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [id]);

    if (!id) return <div>No ID provided</div>;
    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;

    // The initial bucket (for now)
    // TODO add more as user keeps scrolling
    return (
        <div>
            <div className={styles.relatedViewBody}>
                {relatedItems.map((item) => (
                    <MediaCardComponent
                        key={item.source}
                        id={item.id}
                        mediaType={item.mediaType}
                        title={item.title}
                        subtext={item.subText}
                        thumbText={item.thumbText}
                        source={item.source} />
                ))}
            </div>
        </div>
    );
}