import Branding from '../Branding/Branding';

export default function Header() {
    return (
        <header style={{ padding: '1rem', background: '#333', color: 'white' }}>
            <Branding />
        </header>
    );
}