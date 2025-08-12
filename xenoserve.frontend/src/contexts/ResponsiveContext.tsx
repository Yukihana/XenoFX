// ResponsiveContext.tsx
import React, { createContext, useContext, useEffect, useState, type ReactNode } from 'react';

export type DeviceType = 'mobile' | 'tablet' | 'desktop';
export type Orientation = 'portrait' | 'landscape';

interface ResponsiveState {
    deviceType: DeviceType;
    orientation: Orientation;
    width: number;
    height: number;
    isMobile: boolean;
    isTablet: boolean;
    isDesktop: boolean;
    isPortrait: boolean;
    isLandscape: boolean;
}

const MOBILE_MAX_WIDTH = 767;
const TABLET_MAX_WIDTH = 1024;

const ResponsiveContext = createContext<ResponsiveState | undefined>(undefined);

const getDeviceType = (width: number): DeviceType => {
    if (width <= MOBILE_MAX_WIDTH) return 'mobile';
    if (width <= TABLET_MAX_WIDTH) return 'tablet';
    return 'desktop';
};

const getOrientation = (width: number, height: number): Orientation => {
    return height >= width ? 'portrait' : 'landscape';
};

export const ResponsiveProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [state, setState] = useState<ResponsiveState>(() => {
        const width = typeof window !== 'undefined' ? window.innerWidth : 1920;
        const height = typeof window !== 'undefined' ? window.innerHeight : 1080;
        const deviceType = getDeviceType(width);
        const orientation = getOrientation(width, height);

        return {
            deviceType,
            orientation,
            width,
            height,
            isMobile: deviceType === 'mobile',
            isTablet: deviceType === 'tablet',
            isDesktop: deviceType === 'desktop',
            isPortrait: orientation === 'portrait',
            isLandscape: orientation === 'landscape',
        };
    });

    useEffect(() => {
        const handleResize = () => {
            const width = window.innerWidth;
            const height = window.innerHeight;
            const deviceType = getDeviceType(width);
            const orientation = getOrientation(width, height);

            setState({
                deviceType,
                orientation,
                width,
                height,
                isMobile: deviceType === 'mobile',
                isTablet: deviceType === 'tablet',
                isDesktop: deviceType === 'desktop',
                isPortrait: orientation === 'portrait',
                isLandscape: orientation === 'landscape',
            });
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    return (
        <ResponsiveContext.Provider value={state}>
            {children}
        </ResponsiveContext.Provider>
    );
};

export const useResponsive = (): ResponsiveState => {
    const context = useContext(ResponsiveContext);
    if (!context) {
        throw new Error('useResponsive must be used within a ResponsiveProvider');
    }
    return context;
};