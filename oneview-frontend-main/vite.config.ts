import path from 'path';
import tailwindcss from '@tailwindcss/vite';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:8080', // your backend
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ''), // optional: remove /api prefix
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  preview: {
    port: 4173, // your container port
    strictPort: true,
    host: true, // listen on all interfaces
    allowedHosts: [
      'localhost',
      '127.0.0.1',
      '::1',
      'oneview-frontend', // Docker internal hostname
      'www.vitanioneview.duckdns.org', // Your public domain
    ],
  },
});
