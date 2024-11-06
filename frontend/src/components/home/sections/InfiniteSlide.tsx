import React, { useRef, useEffect, useState } from 'react';

export interface InfiniteSlideProps {
  images: { id: number; url: string; description: string; }[];
  onImageClick?: (image: { id: number; url: string; description: string; }) => void;
}

const InfiniteSlide: React.FC<InfiniteSlideProps> = ({ images, onImageClick }) => {
  const upperRowRef = useRef<HTMLDivElement>(null);
  const middleRowRef = useRef<HTMLDivElement>(null);
  const lowerRowRef = useRef<HTMLDivElement>(null);
  const [containerHeight, setContainerHeight] = useState<number>(0);

  useEffect(() => {
    const calculateHeight = () => {
      if (upperRowRef.current && middleRowRef.current && lowerRowRef.current) {
        const upperHeight = upperRowRef.current.offsetHeight;
        const middleHeight = middleRowRef.current.offsetHeight;
        const lowerHeight = lowerRowRef.current.offsetHeight;
        const spacing = 40; // 줄 간격을 20에서 40으로 증가
        const hoverSpace = 40;
        setContainerHeight(upperHeight + middleHeight + lowerHeight + (spacing * 2) + hoverSpace);
      }
    };

    calculateHeight();
    window.addEventListener('resize', calculateHeight);
    return () => window.removeEventListener('resize', calculateHeight);
  }, []);

  const handleMouseEnter = (row: 'upper' | 'middle' | 'lower') => {
    const ref = row === 'upper' ? upperRowRef : row === 'middle' ? middleRowRef : lowerRowRef;
    if (ref.current) {
      ref.current.style.animationPlayState = 'paused';
    }
  };

  const handleMouseLeave = (row: 'upper' | 'middle' | 'lower') => {
    const ref = row === 'upper' ? upperRowRef : row === 'middle' ? middleRowRef : lowerRowRef;
    if (ref.current) {
      ref.current.style.animationPlayState = 'running';
    }
  };

  // 이미지를 세 부분으로 나누기
  const chunkSize = Math.ceil(images.length / 3);
  const upperImages = images.slice(0, chunkSize);
  const middleImages = images.slice(chunkSize, chunkSize * 2);
  const lowerImages = images.slice(chunkSize * 2);
  
  // 각 라인의 이미지를 네 번 복제
  const doubledUpperImages = [...upperImages, ...upperImages, ...upperImages, ...upperImages];
  const doubledMiddleImages = [...middleImages, ...middleImages, ...middleImages, ...middleImages];
  const doubledLowerImages = [...lowerImages, ...lowerImages, ...lowerImages, ...lowerImages];

  return (
    <div className="py-8">
      <div 
        className="relative overflow-visible"
        style={{ height: `${containerHeight}px` }}
      >
        <div className="space-y-5">
          {/* 첫 번째 줄 - 오른쪽으로 */}
          <div
            ref={upperRowRef}
            className="flex items-center animate-marquee-right gap-3 px-4"
            onMouseEnter={() => handleMouseEnter('upper')}
            onMouseLeave={() => handleMouseLeave('upper')}
          >
            {doubledUpperImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-upper`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group 
                         cursor-pointer"
                onClick={() => onImageClick?.(image)}
              >
                <div className="p-1.5">
                  <div className="relative aspect-square w-full">
                    <img
                      src={image.url}
                      alt={image.description}
                      className="absolute inset-0 w-full h-full object-cover rounded-lg 
                               border border-zinc-700/50 group-hover:border-zinc-600/80 
                               transition-shadow duration-300 
                               hover:shadow-[0_0_10px_1px_rgba(59,130,246,0.7)]"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* 두 번째 줄 - 왼쪽으로 */}
          <div
            ref={middleRowRef}
            className="flex items-center animate-marquee-left gap-3 px-4"
            onMouseEnter={() => handleMouseEnter('middle')}
            onMouseLeave={() => handleMouseLeave('middle')}
          >
            {doubledMiddleImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-middle`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group
                         cursor-pointer"
                onClick={() => onImageClick?.(image)}
              >
                <div className="p-1.5">
                  <div className="relative aspect-square w-full">
                    <img
                      src={image.url}
                      alt={image.description}
                      className="absolute inset-0 w-full h-full object-cover rounded-lg 
                               border border-zinc-700/50 group-hover:border-zinc-600/80 
                               transition-shadow duration-300 
                               hover:shadow-[0_0_10px_1px_rgba(59,130,246,0.7)]"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* 세 번째 줄 - 오른쪽으로 */}
          <div
            ref={lowerRowRef}
            className="flex items-center animate-marquee-right gap-3 px-4"
            onMouseEnter={() => handleMouseEnter('lower')}
            onMouseLeave={() => handleMouseLeave('lower')}
          >
            {doubledLowerImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-lower`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group
                         cursor-pointer"
                onClick={() => onImageClick?.(image)}
              >
                <div className="p-1.5">
                  <div className="relative aspect-square w-full">
                    <img
                      src={image.url}
                      alt={image.description}
                      className="absolute inset-0 w-full h-full object-cover rounded-lg 
                               border border-zinc-700/50 group-hover:border-zinc-600/80 
                               transition-shadow duration-300 
                               hover:shadow-[0_0_10px_1px_rgba(59,130,246,0.7)]"
                    />
                  </div>
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