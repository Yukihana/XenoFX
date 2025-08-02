import type { ReactNode } from 'react';

interface SidebarContainerProps {
    children: ReactNode;
    className?: string; // Optional modifier (e.g., 'left', 'right', 'collapsed')
}

export default function SidebarContainer({ children, className }: SidebarContainerProps) {
    return (
        <aside className={`sidebar-container ${className || ''}`}>
            {children}
        </aside>
    );
}