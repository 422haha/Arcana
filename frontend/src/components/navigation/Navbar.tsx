import React, { useState, useEffect, useRef } from 'react';
import { Link, useLocation } from 'react-router-dom';

interface NavbarProps {
  setPressKitOpen: (isOpen: boolean) => void;
}

const Navbar: React.FC<NavbarProps> = ({ setPressKitOpen }) => {
  const [isFixed, setIsFixed] = useState(false);
  const navRef = useRef<HTMLDivElement>(null);
  const location = useLocation();
  const isMainPage = location.pathname === '/';

  useEffect(() => {
    const handleScroll = () => {
      const scrollY = window.scrollY;
      if (isMainPage) {
        const heroHeight = window.innerHeight;
        setIsFixed(scrollY > heroHeight * 0.8);
      } else {
        setIsFixed(scrollY > 50);
      }
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [isMainPage]);

  return (
    <>
      {isFixed && <div style={{ height: navRef.current?.offsetHeight || 0 }} />}

      <nav
        ref={navRef}
        className={`w-[85%] mx-auto mt-8 
          bg-gradient-to-b from-sky-900/10 to-zinc-800/80
          backdrop-blur-md transition-all duration-300 z-50
          border-2 border-zinc-700/40 overflow-hidden rounded-2xl
          ${isFixed ? 'fixed top-4 left-1/2 transform -translate-x-1/2' : 'relative'}
        `}
      >
        {/* Ambient glow effect */}
        <div className="absolute inset-0 bg-gradient-to-r from-sky-400/5 via-blue-400/5 to-sky-400/5" />
        
        <div className="absolute -top-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-sky-400/50 to-transparent" />
        
        <div className="flex justify-between items-center p-4 relative">
          <Link to="/" className="flex items-center gap-5 group">
            <img src="/logo.png" alt="Logo" className="h-14 w-14 relative z-10
                                                     group-hover:scale-105 transition-transform
                                                     duration-300" />

            <div className="relative">
              <span className="font-pixel text-3xl text-gray-200 tracking-wider 
                             group-hover:text-gray-100 transition-colors duration-300
                             flex items-center gap-2">
                ARCANA
                <span className="text-sm text-sky-400 font-mono tracking-tight
                               opacity-70 group-hover:opacity-100 transition-opacity">v1.0.2</span>
              </span>
              <div className="absolute h-px w-0 bg-gradient-to-r from-sky-400/50 via-blue-400/50 to-sky-400/50
                            bottom-0 left-0 group-hover:w-full transition-all duration-500" />
            </div>
          </Link>

          <div className="flex gap-8">
            {[ 
              { text: 'Press Kit', onClick: () => setPressKitOpen(true) },
              { text: 'Download', to: '/download' },
              { text: 'Support', to: '/support' }
            ].map((item, index) => (
              item.to ? (
                <Link
                  key={index}
                  to={item.to}
                  className="relative group"
                >
                  <div className="relative px-4 py-3">
                    <span className="font-pixel text-lg text-gray-300 group-hover:text-gray-100
                                    relative z-10 transition-colors duration-300">
                      {item.text}
                    </span>
                    <div className="absolute -bottom-1 left-0 w-0 h-px 
                                  bg-gradient-to-r from-sky-400 via-blue-400 to-sky-400
                                  group-hover:w-full transition-all duration-300" />
                    <div className="absolute -inset-1 opacity-0 group-hover:opacity-100
                                  bg-gradient-to-r from-sky-400/10 via-blue-400/10 to-sky-400/10
                                  blur-md transition-opacity duration-300
                                  rounded-lg -z-10" />
                  </div>
                  <div className="absolute inset-0 rounded-lg opacity-0 
                                active:opacity-20 active:bg-sky-400
                                transition-all duration-150" />
                </Link>
              ) : (
                <button
                  key={index}
                  onClick={item.onClick}
                  className="relative group"
                >
                  <div className="relative px-4 py-3">
                    <span className="font-pixel text-lg text-gray-300 group-hover:text-gray-100
                                    relative z-10 transition-colors duration-300">
                      {item.text}
                    </span>
                    <div className="absolute -bottom-1 left-0 w-0 h-px 
                                  bg-gradient-to-r from-sky-400 via-blue-400 to-sky-400
                                  group-hover:w-full transition-all duration-300" />
                    <div className="absolute -inset-1 opacity-0 group-hover:opacity-100
                                  bg-gradient-to-r from-sky-400/10 via-blue-400/10 to-sky-400/10
                                  blur-md transition-opacity duration-300
                                  rounded-lg -z-10" />
                  </div>
                  <div className="absolute inset-0 rounded-lg opacity-0 
                                active:opacity-20 active:bg-sky-400
                                transition-all duration-150" />
                </button>
              )
            ))}
          </div>
        </div>

        <div className="absolute -left-px top-1/2 -translate-y-1/2 w-px h-12
                     bg-gradient-to-b from-transparent via-sky-400/40 to-transparent" />
        <div className="absolute -right-px top-1/2 -translate-y-1/2 w-px h-12
                     bg-gradient-to-b from-transparent via-sky-400/40 to-transparent" />
        
        <div className="absolute -bottom-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-sky-400/50 to-transparent" />

        {/* Scanline effect */}
        <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                     opacity-[0.03] mix-blend-overlay pointer-events-none" />
      </nav>
    </>
  );
};

export default Navbar;