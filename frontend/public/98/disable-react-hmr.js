// Vite HMR 비활성화
window.__vite_is_modern_browser = true;
window.__vite_plugin_react_preamble_installed__ = true;
window.process = { env: { NODE_ENV: 'production' } };

if (import.meta.hot) {
  import.meta.hot.decline();
  import.meta.hot.invalidate();
}