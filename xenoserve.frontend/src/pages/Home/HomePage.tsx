import { useEffect, useState } from "react";
import { useResponsive } from '../../contexts/ResponsiveContext';
import PaginationComponent from "../../components/Pagination/PaginationComponent";
import MediaCardComponent from "../../components/MediaCard/MediaCardComponent";
import type { MediaItem } from "../../types/MediaItem";
import type { MediaResults } from "../../types/MediaResults";
import { getSuggestionsAsync } from "./search-service";
import styles from "./HomePage.module.css";

export default function HomePage() {
    const [mediaItems, setMediaItems] = useState<MediaItem[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        document.title = "Home";

        const fetchData = async () => {
            try {
                const response: MediaResults = await getSuggestionsAsync(1, 30); // Fetch first page with 30 items

                setMediaItems(response.items);
            } catch (error) {
                console.error("Failed to fetch items", error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    // Compute responsive styles
    const { deviceType, orientation } = useResponsive();
    const paginationAlignClass =
        deviceType !== 'mobile' || orientation === "landscape"
            ? styles.right : styles.center;
    const resultsLayoutClass =
        deviceType !== 'mobile' || orientation === "landscape"
            ? styles.resultsGrid : styles.resultsStack;

    // View
    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <main>
            <section id="suggestions">
                <h2 className={styles.header}>Suggested for you:</h2>

                <nav aria-label="Top pagination"
                    className={`${styles.pagination} ${paginationAlignClass}`}>
                    <PaginationComponent />
                </nav>

                {/*Temporary Legacy Port-In*/}
                <iframe src="../../legacy/browse/"
                    className={styles.legacy}
                    title="Legacy Browse Section" />

                {/*thumb suggestions grid*/}
                <div className={resultsLayoutClass}>
                    {mediaItems.map((item) => (
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

                <nav aria-label="Bottom pagination"
                    className={`${styles.pagination} ${paginationAlignClass}`}>
                    <PaginationComponent />
                </nav>
            </section>
        </main >
    );
}