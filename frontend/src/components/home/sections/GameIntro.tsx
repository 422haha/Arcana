
const GameIntro = () => {
  return (
    <section className="py-20 sm:py-24 md:py-28 lg:py-32 relative overflow-hidden">
      {/* Background effects */}
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] 
                    from-blue-500/5 via-zinc-900 to-zinc-900 pointer-events-none" />

      <div className="container mx-auto px-4 md:px-6 lg:px-8 relative z-10">
        {/* Refined Section Title */}
        <div className="relative mb-12 sm:mb-16 md:mb-20">
          <div className="relative mx-auto w-fit group">
            {/* Subtle outer glow */}
            <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500/10 to-teal-500/10 
                         rounded-xl blur-md opacity-0 group-hover:opacity-100 
                         transition-all duration-700" />
            
            <div className="relative px-10 py-5 bg-zinc-900/90 backdrop-blur-sm 
                         border border-zinc-800/50 rounded-xl
                         group-hover:border-zinc-700/50 transition-all duration-500">
              {/* Minimal border highlights */}
              <div className="absolute -top-px left-0 w-full h-px 
                           bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
              <div className="absolute -bottom-px left-0 w-full h-px 
                           bg-gradient-to-r from-transparent via-teal-500/30 to-transparent" />
              
              {/* Refined tech corners - only on the left and right */}
              <div className="absolute -left-px top-0 h-full w-px 
                           bg-gradient-to-b from-transparent via-blue-500/30 to-transparent" />
              <div className="absolute -right-px top-0 h-full w-px 
                           bg-gradient-to-b from-transparent via-teal-500/30 to-transparent" />
              
              {/* Title text with subtle effects */}
              <div className="relative">
                <h2 className="text-4xl md:text-5xl lg:text-6xl font-pixel text-center text-transparent 
                            bg-clip-text bg-gradient-to-r from-blue-400 to-teal-400
                            group-hover:from-blue-300 group-hover:to-teal-300
                            transition-all duration-500">
                  게임 소개
                </h2>
              </div>

              {/* Minimal scanline effect */}
              <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                           opacity-[0.03] mix-blend-overlay pointer-events-none rounded-xl" />
            </div>
          </div>
        </div>

        {/* Content Sections */}
        <div className="space-y-12 md:space-y-24">
          {/* 아르카나의 세계 섹션 */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 md:gap-12 lg:gap-16 items-center">
            {/* 왼쪽: 텍스트 */}
            <div className="relative group">
              <div className="relative p-6 bg-zinc-900/80 backdrop-blur-sm rounded-xl
                           border border-zinc-800/50 hover:border-zinc-700/50
                           transition-all duration-300">
                <div className="absolute -top-px left-0 w-full h-px 
                             bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
                <div className="absolute -bottom-px left-0 w-full h-px 
                             bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
                
                <h3 className="text-2xl md:text-3xl font-pixel mb-6 text-transparent bg-clip-text 
                            bg-gradient-to-r from-blue-400 to-teal-400">
                  아르카나의 세계
                </h3>

                <div className="relative p-4 bg-zinc-800/50 rounded-lg
                             border border-zinc-700/50 group-hover:border-zinc-600/80">
                  <p className="text-lg md:text-xl lg:text-2xl font-content text-gray-300 
                    leading-8 md:leading-9 lg:leading-10 tracking-wide 
                    px-2 md:px-4 
                    max-w-prose 
                    break-keep 
                    [word-spacing:2px] md:[word-spacing:3px]
                    [text-wrap:pretty]
                    relative
                    after:content-['|'] 
                    after:ml-[2px]
                    after:animate-[cursor_1s_steps(2)_infinite]
                    after:text-blue-400">
                    행성 지구는 결국 지구온난화로 인한 환경오염을 막지 못했고...
                  </p>
                  <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                               bg-gradient-to-r from-blue-500/5 to-teal-500/5
                               transition-opacity duration-300 rounded-lg" />
                </div>

                <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                             opacity-[0.05] pointer-events-none rounded-xl" />
              </div>

              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500/20 to-teal-500/20 
                           rounded-xl blur-lg opacity-0 group-hover:opacity-50 
                           transition-opacity duration-500 -z-10" />
            </div>

            {/* 오른쪽: 이미지 */}
            <div className="relative group">
              <div className="relative bg-zinc-900/80 backdrop-blur-sm p-2 rounded-xl
                           border border-zinc-800/50 hover:border-zinc-700/50
                           transition-all duration-300">
                <img 
                  src="/assets/world-map.jpg" 
                  alt="아르카나 세계" 
                  className="w-full aspect-video object-cover rounded-lg 
                           border border-zinc-700/50 group-hover:border-zinc-600
                           transition-all duration-300"
                />
                <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                             opacity-[0.05] pointer-events-none rounded-xl" />
              </div>

              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500/20 to-teal-500/20 
                           rounded-xl blur-lg opacity-0 group-hover:opacity-50 
                           transition-opacity duration-500 -z-10" />
            </div>
          </div>

          {/* 게임플레이 섹션 */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 md:gap-12 lg:gap-16 items-center">
            {/* 왼쪽: 이미지 */}
            <div className="relative group order-2 lg:order-1">
              <div className="relative bg-zinc-900/80 backdrop-blur-sm p-2 rounded-xl
                           border border-zinc-800/50 hover:border-zinc-700/50
                           transition-all duration-300">
                <img 
                  src="/assets/gameplay.jpg" 
                  alt="게임플레이" 
                  className="w-full aspect-video object-cover rounded-lg 
                           border border-zinc-700/50 group-hover:border-zinc-600
                           transition-all duration-300"
                />
                <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                             opacity-[0.05] pointer-events-none rounded-xl" />
              </div>

              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500/20 to-teal-500/20 
                           rounded-xl blur-lg opacity-0 group-hover:opacity-50 
                           transition-opacity duration-500 -z-10" />
            </div>

            {/* 오른쪽: 텍스트 */}
            <div className="relative group order-1 lg:order-2">
              <div className="relative p-6 bg-zinc-900/80 backdrop-blur-sm rounded-xl
                           border border-zinc-800/50 hover:border-zinc-700/50
                           transition-all duration-300">
                <div className="absolute -top-px left-0 w-full h-px 
                             bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
                <div className="absolute -bottom-px left-0 w-full h-px 
                             bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
                
                <h3 className="text-2xl md:text-3xl font-pixel mb-6 text-transparent bg-clip-text 
                            bg-gradient-to-r from-blue-400 to-teal-400">
                  게임 플레이
                </h3>

                <div className="relative p-4 bg-zinc-800/50 rounded-lg
                             border border-zinc-700/50 group-hover:border-zinc-600/80">
                  <p className="text-lg md:text-xl lg:text-2xl font-content text-gray-300 
                    leading-8 md:leading-9 lg:leading-10 tracking-wide 
                    px-2 md:px-4 
                    max-w-prose 
                    break-keep 
                    [word-spacing:2px] md:[word-spacing:3px]
                    [text-wrap:pretty]
                    relative
                    after:content-['|'] 
                    after:ml-[2px]
                    after:animate-[cursor_1s_steps(2)_infinite]
                    after:text-blue-400">
                    덱빌딩 로그라이크 게임으로, 전략적 사고와 카드 수집의 재미를 동시에 즐길 수 있습니다. 
                    플레이어는 각각 고유한 특성을 가진 카드들을 수집하고 조합하여 자신만의 덱을 구축하게 됩니다. 
                    매 런마다 새로운 카드와 전략을 발견하며, 도전적인 전투를 통해 게임의 진정한 재미를 경험하세요...
                  </p>
                  <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                               bg-gradient-to-r from-blue-500/5 to-teal-500/5
                               transition-opacity duration-300 rounded-lg" />
                </div>

                <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                             opacity-[0.05] pointer-events-none rounded-xl" />
              </div>

              <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500/20 to-teal-500/20 
                           rounded-xl blur-lg opacity-0 group-hover:opacity-50 
                           transition-opacity duration-500 -z-10" />
            </div>
          </div>
        </div>
      </div>

      {/* Ambient corner decorations */}
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

export default GameIntro;