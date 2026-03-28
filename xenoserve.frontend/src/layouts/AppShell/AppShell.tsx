import { Outlet } from "react-router-dom";

export default function AppShell() {
    return (
        <>
            {/* Persistent layer (never unmounts) */}
            <div>Media Player (persistent)</div>
            <div>Chat System (persistent)</div>

            {/* Routed content */}
            <Outlet />
        </>
    );
}