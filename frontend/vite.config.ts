import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  base: process.env.NODE_ENV === 'production' ? '/98/' : '/',
  resolve: {
    alias: [
      { find: '@', replacement: path.resolve(__dirname, 'src') }
    ]
  },
  server: {
    proxy: {
      '/api': {
        target: process.env.BASE_URL || 'http://localhost:8080',
        changeOrigin: true,
        secure: false,
        ws: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
      }
    }
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom', 'react-router-dom']
        }
      }
    }
  }
});
