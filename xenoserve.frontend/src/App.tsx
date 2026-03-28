import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import AppShell from "./layouts/AppShell/AppShell";
import DefaultLayout from './layouts/Default/DefaultLayout';
import SidebarlessLayout from './layouts/Sidebarless/SidebarlessLayout';
import HomePage from './pages/Home/HomePage';
import ViewPage from './pages/View/ViewPage';
import DummyPage from './pages/Dummy/DummyPage';
import RelatedView from './features/asset-related/components/RelatedView';

const router = createBrowserRouter([
    {
        path: "/",               // Root persistent layer
        element: <AppShell />,
        children: [
            {
                element: <DefaultLayout />,  // Main layout
                children: [
                    { index: true, element: <HomePage /> },
                    {
                        path: "view/:id",
                        element: <ViewPage />,
                        handle: { rightSidebar: <RelatedView /> },
                    },
                ],
            },
            {
                element: <SidebarlessLayout />, // Sidebarless layout
                children: [
                    { path: "test", element: <DummyPage /> },
                    { path: "account", element: <DummyPage /> },
                ],
            },
        ],
    },
]);
export default function App() {
    return <RouterProvider router={router} />;
}