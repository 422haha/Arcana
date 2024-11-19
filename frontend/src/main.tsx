import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import "./styles/index.css";
import App from './App.tsx'

if (import.meta.hot) {
  import.meta.hot.accept(() => {
    console.log('HMR update');
  });
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
