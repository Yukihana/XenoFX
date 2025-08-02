import { Link } from 'react-router-dom';

interface NavMenuItemProps {
    label: string;
    to: string;
}

export default function NavMenuItemComponent(
    { label, to }: NavMenuItemProps
) {
    return (
        <div className="nav-menu-item">
            <Link to={to}>{label}</Link>
        </div>
    );
}