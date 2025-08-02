import NavMenuItemComponent from '../NavMenuItem/NavMenuItemComponent';

interface NavMenuComponentProps {
    items: { label: string; to: string }[];
}
export default function NavMenuComponent(
    { items }: NavMenuComponentProps) {
    return (
        <div className="nav-menu">
            {items.map(item => (
                <NavMenuItemComponent
                    key={item.to}
                    label={item.label}
                    to={item.to} />
            ))}
        </div>
    );
}