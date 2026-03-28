import type { ReactNode } from 'react';
import { useMatches, Outlet } from 'react-router-dom';
import clsx from 'clsx';

import Header from '../../components/Header/Header';
import Overlays from '../../components/Overlays/OverlaysPlaceholder';
import styles from './DefaultLayout.module.css';

interface DefaultLayoutProps {
    leftSidebar?: ReactNode;
    rightSidebar?: ReactNode;
    footer?: ReactNode;
}

type LayoutHandle = {
    leftSidebar?: ReactNode;
    rightSidebar?: ReactNode;
};

export default function DefaultLayout({
    leftSidebar,
    rightSidebar,
    footer,
}: DefaultLayoutProps) {
    const matches = useMatches() as Array<{
        handle?: LayoutHandle;
    }>;

    // Left
    const routeLeftSidebar = matches
        .map(m => m.handle?.leftSidebar)
        .filter(Boolean)
        .at(-1);
    const finalLeftSidebar = leftSidebar ?? routeLeftSidebar;
    const isLeftCollapsed = !finalLeftSidebar;
    const leftSidebarClass = clsx(
        styles.layoutLeftSidebarContainer,
        isLeftCollapsed && styles.collapsed
    );

    // Right
    const routeRightSidebar = matches
        .map(m => m.handle?.rightSidebar)
        .filter(Boolean)
        .at(-1);
    const finalRightSidebar = rightSidebar ?? routeRightSidebar;
    const isRightCollapsed = !finalRightSidebar;
    const rightSidebarClass = clsx(
        styles.layoutRightSidebarContainer,
        isRightCollapsed && styles.collapsed
    );

    // JSX
    return (
        <div className={styles.layoutContainer}>
            <div className={styles.layoutHeader}>
                <Header />
            </div>

            <div className={styles.layoutBody}>
                <aside className={leftSidebarClass}>
                    {finalLeftSidebar}
                </aside>

                <main className={styles.layoutContent}>
                    <Outlet /> {/* <-- This is where the routed Page content renders */}
                </main>

                <aside className={rightSidebarClass}>
                    {finalRightSidebar}
                </aside>
            </div>

            {footer && <footer className={styles.layoutFooter}>{footer}</footer>}

            <div id="overlays-container">
                <Overlays />
            </div>
        </div>
    );
}