import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ResponsiveProvider } from './contexts/ResponsiveContext.tsx'
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ResponsiveProvider>
            <App />
        </ResponsiveProvider>
    </StrictMode>,
)