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
        className={`w-[85%] mx-auto mt-6 
          bg-zinc-900/80 backdrop-blur-sm transition-all duration-300 z-50
          border border-zinc-800/50 overflow-hidden rounded-2xl
          ${isFixed ? 'fixed top-4 left-1/2 transform -translate-x-1/2' : 'relative'}
        `}
      >
        {/* Top border gradient - 새로 추가 */}
        <div className="absolute -top-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
        
        <div className="flex justify-between items-center p-4">
          {/* 로고와 타이틀 - 기존 내용 유지 */}
          <Link to="/" className="flex items-center gap-4 group">
            <div className="relative">
              <div className="absolute inset-0 bg-blue-500/10 blur-lg group-hover:bg-blue-500/20 
                          transition-all duration-300" />
              <div className="relative bg-zinc-800/80 p-2 rounded-lg 
                          border border-zinc-700/50 group-hover:border-zinc-600
                          transition-all duration-300">
                <img src="/logo.png" alt="Logo" className="h-8 w-8 relative z-10
                                                        group-hover:scale-105 transition-transform
                                                        duration-300" />
                <div className="absolute inset-0 bg-gradient-to-t from-blue-500/10 to-transparent 
                            rounded-lg pointer-events-none" />
              </div>
            </div>
            <div className="relative">
              <span className="font-pixel text-2xl text-gray-300 tracking-wider 
                             group-hover:text-gray-100 transition-colors duration-300
                             flex items-center gap-2">
                ARCANA
                <span className="text-xs text-blue-400 font-mono tracking-tight
                               opacity-60 group-hover:opacity-100 transition-opacity">v1.0.2</span>
              </span>
              <div className="absolute h-px w-0 bg-gradient-to-r from-blue-500/50 to-teal-500/50
                            bottom-0 left-0 group-hover:w-full transition-all duration-500" />
            </div>
          </Link>

          {/* 메뉴 항목 - 버튼 스타일만 수정 */}
          <div className="flex gap-6"> {/* gap 증가 */}
    {[
      { text: 'Press Kit', onClick: () => setPressKitOpen(true) },
      { text: 'Download', to: '/download' },
      { text: 'Support', to: '/support' }
    ].map((item, index) => {
      const ButtonComponent = item.to ? Link : 'button';
      return (
        <ButtonComponent
          key={index}
          {...(item.to ? { to: item.to } : { onClick: item.onClick })}
          className="relative group"
        >
          <div className="relative px-2 py-2"> {/* 패딩 조정 */}
            <span className="font-pixel text-gray-400 group-hover:text-gray-200
                           relative z-10 transition-colors duration-300">
              {item.text}
            </span>
            {/* 밑줄 효과 */}
            <div className="absolute -bottom-1 left-0 w-0 h-px 
                          bg-gradient-to-r from-blue-500 to-teal-500
                          group-hover:w-full transition-all duration-300" />
            {/* 호버 시 글로우 효과 */}
            <div className="absolute -inset-1 opacity-0 group-hover:opacity-100
                          bg-gradient-to-r from-blue-500/10 to-teal-500/10
                          blur-md transition-opacity duration-300
                          rounded-lg -z-10" />
          </div>
          {/* 클릭 효과용 가상 요소 */}
          <div className="absolute inset-0 rounded-lg opacity-0 
                        active:opacity-20 active:bg-blue-500
                        transition-all duration-150" />
        </ButtonComponent>
      );
    })}
  </div>
        </div>

        {/* Side gradient lines - 새로 추가 */}
        <div className="absolute -left-px top-1/2 -translate-y-1/2 w-px h-8
                     bg-gradient-to-b from-transparent via-blue-500/30 to-transparent" />
        <div className="absolute -right-px top-1/2 -translate-y-1/2 w-px h-8
                     bg-gradient-to-b from-transparent via-blue-500/30 to-transparent" />
        
        {/* Bottom border gradient - 새로 추가 */}
        <div className="absolute -bottom-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      </nav>
    </>
  );
};

export default Navbar;