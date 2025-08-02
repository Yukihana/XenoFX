import { useEffect, useState } from "react";
import PaginationComponent from "../../components/Pagination/PaginationComponent";
import GridMediaCardComponent from "../../components/GridMediaCard/GridMediaCardComponent";
import type { MediaItem } from "../../data/MediaItem";
import { searchItemsAsync } from "../../services/search-api";
import type { SearchQuery } from "../../services/search-api";

export default function HomePage() {
    const [mediaItems, setMediaItems] = useState<MediaItem[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        document.title = "Home";

        const fetchData = async () => {
            try {
                const query: SearchQuery = {
                    query: '',
                    page: 1,
                    pageSize: 20
                };

                const items = await searchItemsAsync<MediaItem[]>(query);
                setMediaItems(items);
            } catch (error) {
                console.error("Failed to fetch items", error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <main>
            <section className="page-content-header">
                <h2>Suggested for you:</h2>
            </section>

            {/*Top pagination*/}
            <section className="main-content-paginator-section">
                <PaginationComponent />
            </section>

            {/*thumb suggestions grid*/}
            <section className="suggestions-section">
                {mediaItems.map((item) => (
                    <GridMediaCardComponent
                        key={item.id}
                        id={item.id}
                        title={item.title}
                        subtext={item.subtext}
                        thumbUrl={item.thumbUrl}
                        thumbText={item.thumbText} />
                ))}
            </section>

            {/*Bottom pagination*/}
            <section className="main-content-paginator-section">
                <PaginationComponent />
            </section>
        </main>
    );
}