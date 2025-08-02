import { useSearchParams } from 'react-router-dom';

export default function ViewPage() {
    const [searchParams] = useSearchParams();
    const id = searchParams.get('id');

    return (
        <section>
            <h2>Viewer</h2>
            <span>Viewing item with ID: {id}</span>
            {/* Players and viewers go here */}
        </section>
    );
}