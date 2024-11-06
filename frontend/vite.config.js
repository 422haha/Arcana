import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  
  return {
    plugins: [react()],
    base: '/',
    resolve: {
      alias: [
        { find: '@', replacement: path.resolve(__dirname, 'src') }
      ]
    },
    server: {
      proxy: {
        '/api': {
          target: mode === 'production' 
            ? 'https://k11d103.p.ssafy.io'
            : 'http://localhost:8080',
          changeOrigin: true,
          secure: mode === 'production',
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
  };
});