import GameIntro from './sections/GameIntro';
import RaceFeatures from './sections/RaceFeatures';
import GameGallery from './sections/GameGallery';
import GameDownload from './sections/GameDownload';

const MainContent = () => {
  return (
    <main className="relative w-full text-gray-200">
      {/* Game Intro Section */}
      <section className="relative py-24">
        {/* Top separator */}
        <div className="absolute top-0 left-0 w-full overflow-hidden h-12">
          <div className="absolute top-0 left-0 w-full h-px 
                         bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
          <div className="w-32 h-32 absolute -top-16 -left-16 rotate-45
                         border-r border-blue-500/20" />
          <div className="w-32 h-32 absolute -top-16 -right-16 rotate-45
                         border-l border-blue-500/20" />
        </div>
        
        <div className="w-[90%] md:w-[85%] lg:w-[80%] mx-auto">
          <GameIntro />
        </div>
      </section>

      {/* Race Features Section */}
      <section className="relative py-24">
        <div className="absolute inset-0 bg-gradient-to-b 
                       from-zinc-900 via-zinc-900/95 to-zinc-900
                       pointer-events-none" />
        
        {/* Section separators */}
        <div className="absolute top-0 left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
        <div className="absolute bottom-0 left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
        
        {/* Ambient tech patterns */}
        <div className="absolute inset-0 opacity-[0.03]
                     bg-[url('/assets/circuit-pattern.png')] bg-repeat
                     pointer-events-none" />
        
        <div className="w-[90%] md:w-[85%] lg:w-[80%] mx-auto relative">
          <RaceFeatures />
        </div>

        {/* Side decorative elements */}
        <div className="absolute top-1/2 left-0 w-16 h-32 -translate-y-1/2
                     border-t border-b border-r border-blue-500/20" />
        <div className="absolute top-1/2 right-0 w-16 h-32 -translate-y-1/2
                     border-t border-b border-l border-blue-500/20" />
      </section>

      {/* Game Gallery Section */}
      <section className="relative py-24">
        {/* Bottom separator */}
        <div className="absolute bottom-0 left-0 w-full overflow-hidden h-12">
          <div className="absolute bottom-0 left-0 w-full h-px 
                         bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
          <div className="w-32 h-32 absolute -bottom-16 -left-16 rotate-45
                         border-r border-blue-500/20" />
          <div className="w-32 h-32 absolute -bottom-16 -right-16 rotate-45
                         border-l border-blue-500/20" />
        </div>

        <div className="w-[90%] md:w-[85%] lg:w-[80%] mx-auto relative">
          <GameGallery />
        </div>
      </section>

      <section className="relative py-24">
        {/* Top separator */}
        <div className="absolute top-0 left-0 w-full overflow-hidden h-12">
          <div className="absolute top-0 left-0 w-full h-px 
                         bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
          <div className="w-32 h-32 absolute -top-16 -left-16 rotate-45
                         border-r border-blue-500/20" />
          <div className="w-32 h-32 absolute -top-16 -right-16 rotate-45
                         border-l border-blue-500/20" />
        </div>

        <div className="absolute inset-0 bg-gradient-to-b 
                       from-zinc-900 via-zinc-900/95 to-zinc-900
                       pointer-events-none" />

        <div className="w-[90%] md:w-[85%] lg:w-[80%] mx-auto relative">
          <GameDownload />
        </div>
      </section>
      {/* Floating tech elements */}
      <div className="fixed top-1/4 left-0 w-1 h-12 
                   bg-gradient-to-b from-transparent via-blue-500/20 to-transparent" />
      <div className="fixed top-1/3 right-0 w-1 h-12
                   bg-gradient-to-b from-transparent via-blue-500/20 to-transparent" />
      <div className="fixed bottom-1/4 left-0 w-1 h-12
                   bg-gradient-to-b from-transparent via-blue-500/20 to-transparent" />
      <div className="fixed bottom-1/3 right-0 w-1 h-12
                   bg-gradient-to-b from-transparent via-blue-500/20 to-transparent" />

      {/* Scan line effect */}
      <div className="fixed inset-0 pointer-events-none 
                   bg-[url('/assets/scanlines.png')] opacity-[0.02]" />
    </main>
  );
};

export default MainContent;