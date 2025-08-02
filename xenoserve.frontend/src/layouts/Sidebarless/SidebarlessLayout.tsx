import Header from '../../components/Header/Header';
import Overlays from '../../components/Overlays/OverlaysPlaceholder';
import type { ReactNode } from 'react';
import { Outlet } from 'react-router-dom';

interface SidebarlessLayoutProps {
    footer?: ReactNode;
}

export default function DefaultLayout({
    footer,
}: SidebarlessLayoutProps) {
    return (
        <div className="layout-container">
            <Header />

            <div className="layout-body">
                <main className="layout-main">
                    <Outlet />
                </main>
            </div>

            {footer && <footer className="layout-footer">{footer}</footer>}

            <div id="overlays-container">
                {/*players and such; probably will get a syntactic upgrade later*/}
                <Overlays />
            </div>
        </div>
    );
}