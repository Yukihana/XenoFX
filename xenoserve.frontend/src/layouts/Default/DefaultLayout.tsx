import Header from '../../components/Header/Header';
import Overlays from '../../components/Overlays/OverlaysPlaceholder';
import type { ReactNode } from 'react';
import { Outlet } from 'react-router-dom';
import './DefaultLayout.css';

interface DefaultLayoutProps {
    leftSidebar?: ReactNode;
    rightSidebar?: ReactNode;
    footer?: ReactNode;
}

export default function DefaultLayout({
    leftSidebar,
    rightSidebar,
    footer,
}: DefaultLayoutProps) {
    return (
        <div className="layout-container">
            <div className="layout-header">
                <Header />
            </div>

            <div className="layout-body">

                {leftSidebar && <aside className="layout-left-sidebar">{leftSidebar}</aside>}

                <main className="layout-content">
                    <Outlet /> {/* <-- This is where the routed Page content renders */}
                </main>

                {rightSidebar && <aside className="layout-right-sidebar">{rightSidebar}</aside>}

            </div>

            {footer && <footer className="layout-footer">{footer}</footer>}

            <div id="overlays-container">
                <Overlays />
            </div>
        </div>
    );
}