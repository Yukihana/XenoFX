import { useEffect } from 'react';

export default function DummyPage() {
    useEffect(() => {
        document.title = "Dummy - XenoServe";
    }, []);

    return (
        <section>
            <h2>Dummy Page</h2>
            <span>This is just a placeholder page with no actual functionality.</span>
        </section>
    );
}