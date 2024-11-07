import { useRef, useEffect, useState } from 'react';

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
        const spacing = 40;
        const hoverSpace = 40;
        setContainerHeight(upperHeight + middleHeight + lowerHeight + (spacing * 2) + hoverSpace);
      }
    };

    calculateHeight();
    window.addEventListener('resize', calculateHeight);
    return () => window.removeEventListener('resize', calculateHeight);
  }, []);

  // 이미지를 무작위로 섞는 함수
  const shuffleArray = <T,>(array: T[]): T[] => {
    const shuffled = [...array];
    for (let i = shuffled.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
  };

  // 컴포넌트 마운트 시 한 번만 이미지 섞기
  const [shuffledRows] = useState(() => {
    const shuffled = shuffleArray([...images]);
    const chunkSize = Math.ceil(shuffled.length / 3);
    return {
      upper: shuffled.slice(0, chunkSize),
      middle: shuffled.slice(chunkSize, chunkSize * 2),
      lower: shuffled.slice(chunkSize * 2)
    };
  });

  // 각 줄마다 4번 복제하여 더 부드러운 무한 스크롤 효과 생성
  const infiniteUpperImages = [...shuffledRows.upper, ...shuffledRows.upper, ...shuffledRows.upper, ...shuffledRows.upper];
  const infiniteMiddleImages = [...shuffledRows.middle, ...shuffledRows.middle, ...shuffledRows.middle, ...shuffledRows.middle];
  const infiniteLowerImages = [...shuffledRows.lower, ...shuffledRows.lower, ...shuffledRows.lower, ...shuffledRows.lower];

  const handleMouseEnter = (ref: React.RefObject<HTMLDivElement>) => {
    if (ref.current) {
      ref.current.style.animationPlayState = 'paused';
    }
  };

  const handleMouseLeave = (ref: React.RefObject<HTMLDivElement>) => {
    if (ref.current) {
      ref.current.style.animationPlayState = 'running';
    }
  };

  return (
    <div className="py-8">
      <style>
        {`
          @keyframes marquee-right {
            0% { transform: translateX(0); }
            100% { transform: translateX(-200%); }
          }
          
          @keyframes marquee-left {
            0% { transform: translateX(-200%); }
            100% { transform: translateX(0); }
          }
          
          .animate-marquee-right {
            animation: marquee-right 60s linear infinite;
            will-change: transform;
          }
          
          .animate-marquee-left {
            animation: marquee-left 60s linear infinite;
            will-change: transform;
          }
        `}
      </style>
      <div 
        className="relative overflow-hidden"
        style={{ height: `${containerHeight}px` }}
      >
        <div className="space-y-5">
          {/* 첫 번째 줄 - 오른쪽으로 이동 */}
          <div
            ref={upperRowRef}
            className="flex items-center animate-marquee-right gap-3 px-4"
            onMouseEnter={() => handleMouseEnter(upperRowRef)}
            onMouseLeave={() => handleMouseLeave(upperRowRef)}
          >
            {infiniteUpperImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-upper`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group"
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
                      loading="lazy"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* 두 번째 줄 - 왼쪽으로 이동 */}
          <div
            ref={middleRowRef}
            className="flex items-center animate-marquee-left gap-3 px-4"
            onMouseEnter={() => handleMouseEnter(middleRowRef)}
            onMouseLeave={() => handleMouseLeave(middleRowRef)}
          >
            {infiniteMiddleImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-middle`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group"
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
                      loading="lazy"
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* 세 번째 줄 - 오른쪽으로 이동 */}
          <div
            ref={lowerRowRef}
            className="flex items-center animate-marquee-right gap-3 px-4"
            onMouseEnter={() => handleMouseEnter(lowerRowRef)}
            onMouseLeave={() => handleMouseLeave(lowerRowRef)}
          >
            {infiniteLowerImages.map((image, index) => (
              <div
                key={`${image.id}-${index}-lower`}
                className="min-w-[140px] sm:min-w-[160px] md:min-w-[180px] lg:min-w-[200px] 
                         flex-shrink-0 transition-transform duration-500 hover:scale-105 group"
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
                      loading="lazy"
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
