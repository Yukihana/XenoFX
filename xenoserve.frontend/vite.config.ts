import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import type { ViteDevServer } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "XenoServe";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

function getBackendTarget(env: Record<string, string | undefined>): string {
    if (process.env.NODE_ENV === 'development') {
        return 'http://localhost:9600';
    }

    if (env.ASPNETCORE_HTTPS_PORT) {
        return `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`;
    }

    if (env.ASPNETCORE_URLS) {
        return env.ASPNETCORE_URLS.split(';')[0];
    }

    return 'http://localhost:9600'; // fallback
}

function defaultHtmlMiddleware() {
    return {
        name: 'vite-plugin-default-html',
        configureServer(server: ViteDevServer) {
            server.middlewares.use((req, res, next) => {
                void res;
                try {
                    // Skip if URL has extension
                    if (!req.url || path.extname(req.url)) {
                        return next();
                    }

                    const publicDir = server.config.publicDir;
                    const targetDir = path.join(publicDir, req.url);

                    if (fs.existsSync(targetDir) && fs.statSync(targetDir).isDirectory()) {
                        const defaultFilePath = path.join(targetDir, 'default.html');
                        if (fs.existsSync(defaultFilePath)) {
                            req.url = path.posix.join(req.url, 'default.html');
                        }
                    }
                } catch (err) {
                    console.error('Default.html middleware error:', err);
                }
                next();
            });
        }
    };
}

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        process.env.NODE_ENV === 'development' && defaultHtmlMiddleware(),
    ].filter(Boolean),
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/api': {
                target: getBackendTarget(process.env),
                changeOrigin: true,
                secure: false
            }
        },
        port: parseInt(env.DEV_SERVER_PORT || '52661'),

        // Handle https in development
        ...(process.env.NODE_ENV === 'development'
            ? {
                https: {
                    key: fs.readFileSync(keyFilePath),
                    cert: fs.readFileSync(certFilePath),
                }
            }
            : {} // No HTTPS in production
        ),
    },
})