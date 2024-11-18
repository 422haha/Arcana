import { Link } from 'react-router-dom';
import { useEffect, useRef, useState } from 'react';
import '../../../styles/components/DownloadButtonBackground.css';

const GameDownload = () => {
  const sectionRef = useRef<HTMLDivElement>(null);
  
  const [isVisible, setIsVisible] = useState(false);
  const [firstText, setFirstText] = useState<string[]>([]);
  const [secondText, setSecondText] = useState<string[]>([]);
  const [showCursor, setShowCursor] = useState(true);
  const [showButton, setShowButton] = useState(false);

  const firstLine = "새로운 세계, 아르카나에서".split('');
  const secondLine = "당신의 운명을 개척하세요".split('');

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setIsVisible(true);
          observer.disconnect();
        }
      },
      {
        threshold: 0.3
      }
    );

    if (sectionRef.current) {
      observer.observe(sectionRef.current);
    }

    return () => observer.disconnect();
  }, []);

  useEffect(() => {
    if (!isVisible) return;

    const intervalIds: NodeJS.Timeout[] = [];
    const timeoutIds: NodeJS.Timeout[] = [];

    // 첫 번째 줄 타이핑
    const firstLineInterval = setInterval(() => {
      setFirstText(prev => {
        if (prev.length === firstLine.length) {
          clearInterval(firstLineInterval);
          
          // 첫 번째 줄 완료 후 0.5초 딜레이 후 두 번째 줄 시작
          const timeoutId = setTimeout(() => {
            const secondLineInterval = setInterval(() => {
              setSecondText(prev => {
                if (prev.length === secondLine.length) {
                  clearInterval(secondLineInterval);
                  
                  // 모든 타이핑 완료 후 버튼 표시
                  const buttonTimeoutId = setTimeout(() => setShowButton(true), 500);
                  timeoutIds.push(buttonTimeoutId);
                  return prev;
                }
                return [...prev, secondLine[prev.length]];
              });
            }, 100);
            intervalIds.push(secondLineInterval);
          }, 500);
          timeoutIds.push(timeoutId);
          return prev;
        }
        return [...prev, firstLine[prev.length]];
      });
    }, 100);

    intervalIds.push(firstLineInterval);

    // 커서 깜빡임 효과
    const cursorInterval = setInterval(() => {
      setShowCursor(prev => !prev);
    }, 500);

    intervalIds.push(cursorInterval);

    return () => {
      intervalIds.forEach(id => clearInterval(id));
      timeoutIds.forEach(id => clearTimeout(id));
    };
  }, [isVisible]);

  return (
    <div ref={sectionRef} className="relative min-h-screen">
      {/* Background Container */}
        {/* Background Container */}
        <div className="absolute inset-0 overflow-hidden bg-zinc-900"> {/* 배경색 설정 */}
        {/* Background Image */}
        <div 
            className="absolute w-[150%] h-[150%] top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2
                    bg-cover bg-center opacity-70 saturate-50"   // opacity와 saturate 추가
            style={{ 
            backgroundImage: `url('/assets/down-background.jpg')`
            }}
        />

        {/* Gradient Fade-out */}
        <div className="absolute inset-0 
                        bg-[radial-gradient(circle_at_center,transparent_0%,rgba(24,24,27,0)_30%,rgba(24,24,27,0.7)_60%,rgba(24,24,27,1)_80%)]" />
        
        {/* Edge fade to transparent */}
        <div className="absolute inset-0 
                        bg-gradient-to-t from-zinc-900 via-transparent to-zinc-900" />
        <div className="absolute inset-0 
                        bg-gradient-to-r from-zinc-900 via-transparent to-zinc-900" />
        </div>

      <div className="relative flex flex-col items-center justify-center min-h-screen px-4 py-20">
        <div className="mb-16 text-center flex flex-col items-center">
          {/* First line */}
        <div className="mb-4">
        <h2 className="text-4xl md:text-5xl lg:text-6xl font-pixel inline
                        text-transparent bg-clip-text 
                        bg-gradient-to-br from-slate-100/95 via-blue-50/90 to-slate-200/95
                        drop-shadow-[0_0_1px_rgba(255,255,255,0.1)]">
            {firstText.join('')}
            {firstText.length < firstLine.length && showCursor && (
            <span className="border-r-4 border-slate-300/70 ml-1">&nbsp;</span>
            )}
        </h2>
        </div>

        {/* Second line */}
        <div>
        <h2 className="text-4xl md:text-5xl lg:text-6xl font-pixel inline
                        text-transparent bg-clip-text 
                        bg-gradient-to-br from-slate-100/95 via-blue-50/90 to-slate-200/95
                        drop-shadow-[0_0_2px_rgba(186,230,253,0.1)]">
            {secondText.join('')}
            {firstText.length === firstLine.length && 
            secondText.length < secondLine.length && 
            showCursor && (
            <span className="border-r-4 border-slate-300/70 ml-1">&nbsp;</span>
            )}
        </h2>
        </div>
        </div>

        {/* Download Button */}
        <div className={`transition-opacity duration-500 ${showButton ? 'opacity-100' : 'opacity-0'}`}>
          <Link
            to="/download"
            className="group relative px-14 py-7 overflow-hidden block 
                     transform transition-all duration-300
                     hover:scale-105 active:scale-95
                     animate-float"
          >
            {/* Background and border */}
            <div className="absolute inset-0 bg-gradient-to-br 
                         from-blue-600/80 via-blue-700/80 to-sky-900/90
                         border-2 border-sky-500/30 rounded-2xl
                         transition-all duration-300
                         backdrop-blur-sm
                         group-hover:from-blue-500/80 group-hover:to-sky-800/80
                         group-hover:border-sky-400/50" />
            
            {/* Glow effects는 이전과 동일 */}
            <div className="absolute -inset-1 bg-gradient-to-r 
                         from-blue-500/20 via-sky-500/20 to-blue-500/20 
                         rounded-2xl blur-xl opacity-0 group-hover:opacity-100 
                         transition-all duration-700" />
            
            {/* Content */}
            <div className="relative px-2">
              <span className="font-pixel text-3xl text-white/90
                           tracking-wider font-bold
                           block
                           drop-shadow-[0_0_10px_rgba(186,230,253,0.5)]">
                PLAY NOW
              </span>
            </div>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default GameDownload;