import InfiniteSlide from './InfiniteSlide';

const GameGallery = () => {
  const screenshots = [
    { id: 1, url: "/assets/screenshots/1.jpg", description: "전투 장면" },
    { id: 2, url: "/assets/screenshots/2.jpg", description: "보스 전투" },
    { id: 3, url: "/assets/screenshots/3.jpg", description: "맵 탐험" },
    { id: 4, url: "/assets/screenshots/1.jpg", description: "전투 장면" },
    { id: 5, url: "/assets/screenshots/2.jpg", description: "보스 전투" },
    { id: 6, url: "/assets/screenshots/3.jpg", description: "맵 탐험" },
    { id: 7, url: "/assets/screenshots/1.jpg", description: "전투 장면" },
    { id: 8, url: "/assets/screenshots/2.jpg", description: "보스 전투" },
    { id: 9, url: "/assets/screenshots/3.jpg", description: "맵 탐험" },
  ];

  return (
    <section className="py-20 sm:py-24 md:py-28 lg:py-32 relative overflow-hidden">
      <div className="container mx-auto px-4 md:px-6 lg:px-8 relative z-10">
        {/* 제목 섹션 */}
        <div className="mb-12 sm:mb-16 md:mb-20">
          <h2 className="text-4xl md:text-5xl lg:text-6xl font-pixel text-center text-transparent 
                         bg-clip-text bg-gradient-to-r from-blue-400 to-teal-400">
            미디어 갤러리
          </h2>
        </div>

        {/* 갤러리 섹션 - 상하 여백 추가 */}
        <div className="relative py-10 sm:py-12 md:py-14 lg:py-16">
          <InfiniteSlide images={screenshots} />
        </div>
      </div>

      {/* 코너 장식 - 반응형으로 크기 조정 */}
      <div className="absolute top-0 left-0 w-12 sm:w-14 md:w-16 h-12 sm:h-14 md:h-16 
                      border-l-2 border-t-2 border-blue-500/20 pointer-events-none" />
      <div className="absolute top-0 right-0 w-12 sm:w-14 md:w-16 h-12 sm:h-14 md:h-16 
                      border-r-2 border-t-2 border-blue-500/20 pointer-events-none" />
      <div className="absolute bottom-0 left-0 w-12 sm:w-14 md:w-16 h-12 sm:h-14 md:h-16 
                      border-l-2 border-b-2 border-blue-500/20 pointer-events-none" />
      <div className="absolute bottom-0 right-0 w-12 sm:w-14 md:w-16 h-12 sm:h-14 md:h-16 
                      border-r-2 border-b-2 border-blue-500/20 pointer-events-none" />
    </section>
  );
};

export default GameGallery;