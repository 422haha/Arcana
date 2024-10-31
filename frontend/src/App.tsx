import React, { useState } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';  // 추가
import Footer from "./components/navigation/Footer";
import PressKitModal from './components/modals/PressKitModal';
import AppRoutes from './routes/Routes';
import ScrollToTop from '@/styles/components/ScrollToTop';

const App: React.FC = () => {
  const [isPressKitOpen, setIsPressKitOpen] = useState(false);

  return (
    <Router>
      <ScrollToTop />
      <div className="relative w-full overflow-x-hidden">
        <div className="relative z-0">
          <AppRoutes setPressKitOpen={setIsPressKitOpen} />
        </div>
        <div className="relative z-10">
          <Footer />
        </div>
        <PressKitModal 
          isOpen={isPressKitOpen} 
          onClose={() => setIsPressKitOpen(false)} 
        />
        {/* Toaster 컴포넌트 추가 */}
        <Toaster
          position="top-center"
          reverseOrder={false}
          gutter={8}
          containerStyle={{
            top: 40
          }}
          toastOptions={{
            duration: 4000,
            style: {
              background: '#18181b',
              border: '1px solid #3f3f46',
              padding: '16px',
              color: '#e5e7eb',
            },
            className: 'font-pixel',
            success: {
              iconTheme: {
                primary: '#22c55e',
                secondary: '#18181b',
              },
            },
            error: {
              iconTheme: {
                primary: '#ef4444',
                secondary: '#18181b',
              },
            },
          }}
        />
      </div>
    </Router>
  );
};

export default App;