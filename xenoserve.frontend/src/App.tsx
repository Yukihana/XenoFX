import { BrowserRouter, Routes, Route } from 'react-router-dom';
import DefaultLayout from './layouts/Default/DefaultLayout';
import SidebarlessLayout from './layouts/Sidebarless/SidebarlessLayout';
import HomePage from './pages/Home/HomePage';
import ViewPage from './pages/View/ViewPage';
import DummyPage from './pages/Dummy/DummyPage';

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<DefaultLayout />}>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/view" element={<ViewPage />} />
                </Route>

                <Route element={<SidebarlessLayout />}>
                    <Route path="/test" element={<DummyPage />} />
                    <Route path="/account" element={<DummyPage />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}