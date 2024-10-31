import React, { useRef, useEffect, useState } from 'react';

interface InfiniteSlideProps {
  images: { id: number; url: string; description: string }[];
}

const InfiniteSlide: React.FC<InfiniteSlideProps> = ({ images }) => {
  const upperRowRef = useRef<HTMLDivElement>(null);
  const lowerRowRef = useRef<HTMLDivElement>(null);
  const [containerHeight, setContainerHeight] = useState<number>(0);

  useEffect(() => {
    const calculateHeight = () => {
      if (upperRowRef.current && lowerRowRef.current) {
        const upperHeight = upperRowRef.current.offsetHeight;
        const lowerHeight = lowerRowRef.current.offsetHeight;
        const spacing = 20; // gap-5의 픽셀값
        // hover 시 scale 효과를 위한 여분 공간 추가
        const hoverSpace = 40; // scale(1.05)를 위한 여유 공간
        setContainerHeight(upperHeight + lowerHeight + spacing + hoverSpace);
      }
    };

    calculateHeight();
    window.addEventListener('resize', calculateHeight);
    return () => window.removeEventListener('resize', calculateHeight);
  }, []);

  const handleMouseEnter = (row: 'upper' | 'lower') => {
    const ref = row === 'upper' ? upperRowRef : lowerRowRef;
    if (ref.current) {
      ref.current.style.animationPlayState = 'paused';
    }
  };

  const handleMouseLeave = (row: 'upper' | 'lower') => {
    const ref = row === 'upper' ? upperRowRef : lowerRowRef;
    if (ref.current) {
      ref.current.style.animationPlayState = 'running';
    }
  };

  const doubledImages = [...images, ...images];

  return (
    <div className="py-8"> {/* 상하 여백 추가 */}
      <div 
        className="relative overflow-visible" // overflow-hidden 제거
        style={{ height: `${containerHeight}px` }}
      >
        <div className="space-y-5">
          <div
            ref={upperRowRef}
            className="flex items-center animate-marquee-right gap-5 px-4" // 좌우 패딩 추가
            onMouseEnter={() => handleMouseEnter('upper')}
            onMouseLeave={() => handleMouseLeave('upper')}
          >
            {doubledImages.map((image, index) => (
              <div
                key={`${image.id}-${index}`}
                className="min-w-[240px] sm:min-w-[280px] md:min-w-[320px] lg:min-w-[360px] 
                           flex-shrink-0 transition-transform duration-500 hover:scale-105 group"
              >
                <div className="p-2"> {/* 카드 내부 여백 */}
                  <img
                    src={image.url}
                    alt={image.description}
                    className="w-full aspect-[4/3] object-cover rounded-lg 
                             border border-zinc-700/50 group-hover:border-zinc-600/80 
                             transition-shadow duration-300 
                             hover:shadow-[0_0_10px_1px_rgba(59,130,246,0.7)]"
                  />
                </div>
              </div>
            ))}
          </div>

          <div
            ref={lowerRowRef}
            className="flex items-center animate-marquee-left gap-5 px-4" // 좌우 패딩 추가
            onMouseEnter={() => handleMouseEnter('lower')}
            onMouseLeave={() => handleMouseLeave('lower')}
          >
            {doubledImages.map((image, index) => (
              <div
                key={`${image.id}-${index}`}
                className="min-w-[240px] sm:min-w-[280px] md:min-w-[320px] lg:min-w-[360px] 
                           flex-shrink-0 transition-transform duration-500 hover:scale-105 group"
              >
                <div className="p-2"> {/* 카드 내부 여백 */}
                  <img
                    src={image.url}
                    alt={image.description}
                    className="w-full aspect-[4/3] object-cover rounded-lg 
                             border border-zinc-700/50 group-hover:border-zinc-600/80 
                             transition-shadow duration-300 
                             hover:shadow-[0_0_10px_1px_rgba(59,130,246,0.7)]"
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default InfiniteSlide;