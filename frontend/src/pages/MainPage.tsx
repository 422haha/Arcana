import React from 'react';
import HeroSection from '../components/home/HeroSection';
import MainContent from '../components/home/MainContent';
import Navbar from '../components/navigation/Navbar';

interface MainPageProps {
  setPressKitOpen: (isOpen: boolean) => void;
}

const MainPage: React.FC<MainPageProps> = ({ setPressKitOpen }) => {
  return (
    <div className="relative w-full min-h-screen bg-zinc-900 text-gray-200">
      {/* Ambient background effects */}
      <div className="fixed inset-0 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] 
                    from-blue-500/5 via-zinc-900 to-zinc-900 pointer-events-none" />
      <div className="fixed inset-0 bg-[url('/assets/noise.png')] opacity-[0.015] 
                    pointer-events-none" />
      
      {/* Content wrapper */}
      <div className="relative z-10">
        {/* Hero Section with scan line effect */}
        <div className="relative">
          <HeroSection />
          <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                        opacity-[0.02] pointer-events-none
                        animate-scan" />
        </div>

        {/* Navigation trigger and navbar */}
        <div id="nav-trigger" className="relative">
          <div className="absolute -top-px left-0 w-full h-px 
                       bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
          <Navbar setPressKitOpen={setPressKitOpen} />
        </div>

        {/* Main content section */}
        <div className="relative px-4 md:px-8 lg:px-12 mt-12">
          <div className="absolute top-0 left-0 w-full h-px 
                       bg-gradient-to-r from-transparent via-zinc-700/50 to-transparent" />
          <MainContent />
        </div>

        {/* Footer with top border effect */}
        {/* <div className="relative mt-20">
          <div className="absolute -top-12 left-0 w-full h-12 
                       bg-gradient-to-b from-transparent to-zinc-900/50" />
          <Footer />
        </div> */}
      </div>

      {/* Decorative corner elements */}
      <div className="fixed top-0 left-0 w-16 h-16 border-l-2 border-t-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed top-0 right-0 w-16 h-16 border-r-2 border-t-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed bottom-0 left-0 w-16 h-16 border-l-2 border-b-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed bottom-0 right-0 w-16 h-16 border-r-2 border-b-2 
                   border-blue-500/20 pointer-events-none" />
    </div>
  );
};

export default MainPage;

{/* Add these animations to your global CSS or tailwind.config.js */}
/* 
@keyframes scan {
  from {
    transform: translateY(0);
  }
  to {
    transform: translateY(100%);
  }
}

.animate-scan {
  animation: scan 8s linear infinite;
}
*/