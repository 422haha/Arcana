import { useEffect, useRef, useState } from 'react';
import '@/styles/components/CRT.css';

const HeroSection = () => {
  const [scale, setScale] = useState(2);
  const [isZooming, setIsZooming] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);
  const iframeRef = useRef<HTMLIFrameElement>(null);
  const [dimensions, setDimensions] = useState({ width: 0, height: 0 });
  const MIN_SCALE = 1.4;        // 축소할 수 있는 최소 크기 (1 = 원본 크기)
  const MAX_SCALE = 3.8;         // 확대할 수 있는 최대 크기 (3.5 = 원본의 350%)
  const SCALE_STEP = 0.3;        // 한 번 휠할 때 확대/축소되는 비율 (높을수록 급격하게 확대/축소)
  const BASE_SCROLL_STEP = 4;    // 기본 스크롤 이동 픽셀 단위 (낮을수록 부드럽게 스크롤)
  const MAX_SCROLL_STEP = 100;   // 최대 스크롤 이동 픽셀 단위 (연속 스크롤시 최대 속도 제한)
  const ZOOM_COOLDOWN = 0.1;       // 줌 동작 사이의 최소 간격(ms) (낮을수록 연속 줌 가능)
  const ACCELERATION_FACTOR = 30;  // 연속 스크롤시 가속 비율 (1 = 가속 없음)
  const DECELERATION_FACTOR = 30; // 스크롤 중단시 감속 비율 (1 = 감속 없음)
  const SENSITIVITY = 0.5;       // 전체적인 마우스 휠 감도 (높을수록 민감하게 반응)

  const ORIGINAL_GIF_WIDTH = 781;
  const ORIGINAL_GIF_HEIGHT = 457;
  
  const IFRAME_RELATIVE_X = 0.4237;
  const IFRAME_RELATIVE_Y = 0.426;

  const IFRAME_WIDTH_RATIO = (990 * 0.138) / 781;
  const IFRAME_HEIGHT_RATIO = (615 * 0.138) / 457;

  useEffect(() => {
    const updateDimensions = () => {
      if (containerRef.current) {
        const { clientWidth, clientHeight } = containerRef.current;
        setDimensions({
          width: clientWidth,
          height: clientHeight
        });
      }
    };

    updateDimensions();

    window.addEventListener('resize', updateDimensions);
    return () => window.removeEventListener('resize', updateDimensions);
  }, []);

  const calculateGifDimensions = () => {
    const containerAspectRatio = dimensions.width / dimensions.height;
    const gifAspectRatio = ORIGINAL_GIF_WIDTH / ORIGINAL_GIF_HEIGHT;
    
    let gifWidth, gifHeight;
    
    if (containerAspectRatio > gifAspectRatio) {
      gifHeight = dimensions.height;
      gifWidth = gifHeight * gifAspectRatio;
    } else {
      gifWidth = dimensions.width;
      gifHeight = gifWidth / gifAspectRatio;
    }
    
    return { gifWidth, gifHeight };
  };

  const { gifWidth, gifHeight } = calculateGifDimensions();

  // iframe 크기 계산
  const iframeWidth = gifWidth * IFRAME_WIDTH_RATIO;
  // const iframeHeight = gifHeight * IFRAME_HEIGHT_RATIO;
  
  // iframe 내부 content 스케일 계산
  const contentScale = iframeWidth / 990;

  const isAtTop = () => window.scrollY === 0;
  const isFullyZoomedIn = () => scale >= MAX_SCALE;
  const isFullyZoomedOut = () => scale <= MIN_SCALE;

  // ... (이전의 wheel 관련 로직들은 동일하게 유지)
  const lastWheelTime = useRef(Date.now());
  const lastScrollSpeed = useRef(0);

  const getScrollStep = (deltaY: number) => {
    const now = Date.now();
    const timeDiff = now - lastWheelTime.current;
    const normalizedDelta = Math.abs(deltaY) * SENSITIVITY;
    
    let currentSpeed;
    if (timeDiff < 50) {
      currentSpeed = Math.min(
        lastScrollSpeed.current * ACCELERATION_FACTOR,
        MAX_SCROLL_STEP
      );
    } else if (timeDiff < 150) {
      currentSpeed = lastScrollSpeed.current;
    } else {
      currentSpeed = lastScrollSpeed.current * DECELERATION_FACTOR;
    }

    currentSpeed = Math.max(
      BASE_SCROLL_STEP,
      Math.min(normalizedDelta * 3, currentSpeed)
    );
    
    lastScrollSpeed.current = currentSpeed;
    lastWheelTime.current = now;

    return currentSpeed;
  };

  const handleZoom = (zoomIn: boolean, deltaY: number) => {
    if (isZooming) return;

    setIsZooming(true);
    setScale((prev) => {
      const intensity = Math.min(Math.abs(deltaY) / 100, 1) * 1.5;
      const adjustedStep = SCALE_STEP * intensity;
      const newScale = zoomIn
        ? Math.min(MAX_SCALE, prev + adjustedStep)
        : Math.max(MIN_SCALE, prev - adjustedStep);
      return newScale;
    });

    setTimeout(() => setIsZooming(false), ZOOM_COOLDOWN);
  };

  const handleScroll = (scrollUp: boolean, deltaY: number) => {
    const scrollStep = getScrollStep(deltaY);
    window.scrollBy({
      top: scrollUp ? -scrollStep : scrollStep,
      behavior: 'auto',
    });
  };

  const handleWheel = (deltaY: number) => {
    const scrollUp = deltaY < 0;
    const normalizedDelta = deltaY * SENSITIVITY;

    if (isAtTop()) {
      if (scrollUp) {
        if (!isFullyZoomedIn()) {
          handleZoom(true, normalizedDelta);
        }
      } else {
        if (!isFullyZoomedOut()) {
          handleZoom(false, normalizedDelta);
        } else {
          handleScroll(false, normalizedDelta);
        }
      }
    } else {
      handleScroll(scrollUp, normalizedDelta);
    }
  };

  useEffect(() => {
    const onWheel = (event: WheelEvent) => {
      event.preventDefault();
      const adjustedDeltaY = event.deltaMode === 1 
        ? event.deltaY * 32
        : event.deltaY;
      
      handleWheel(adjustedDeltaY);
    };

    window.addEventListener('wheel', onWheel, { passive: false });
    return () => window.removeEventListener('wheel', onWheel);
  }, [scale, isZooming]);

  useEffect(() => {
    const handleMessage = (event: MessageEvent) => {
      if (event.data.type === 'FORWARD_WHEEL') {
        handleWheel(event.data.deltaY * SENSITIVITY);
      }
    };

    window.addEventListener('message', handleMessage);
    return () => window.removeEventListener('message', handleMessage);
  }, [scale, isZooming]);

  return (
    <div 
      ref={containerRef} 
      className="relative min-h-screen bg-black overflow-hidden"
    >
      {/* Hero Section */}
      <div
        className="absolute transition-all duration-500 ease-out"
        style={{
          left: '46.45%',
          top: '45%',
          width: `${gifWidth}px`,
          height: `${gifHeight}px`,
          transform: `translate(-50%, -50%) scale(${scale})`,
          backgroundImage: `url('/assets/hero_section_slow.gif')`,
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          willChange: 'transform',
        }}
      >
        {/* Iframe Wrapper */}
        <div
          className="absolute iframe-wrapper"
          style={{
            left: `${IFRAME_RELATIVE_X * 100}%`,
            top: `${IFRAME_RELATIVE_Y * 100}%`,
            width: `${IFRAME_WIDTH_RATIO * 100}%`,
            height: `${IFRAME_HEIGHT_RATIO * 100}%`,
            overflow: 'hidden',
            borderRadius: '7px',
            boxSizing: 'border-box',
            boxShadow: `
              inset 0 0 20px 10px rgba(0, 0, 0, 0.7),
              0 0 15px 5px rgba(0, 0, 0, 0.5)
            `,
            transform: 'scale(0.98)',
            transformOrigin: 'center',
            pointerEvents: 'auto', // iframe 클릭 허용
          }}
        >
          <div
            style={{
              width: '100%',
              height: '100%',
              overflow: 'hidden',
              pointerEvents: 'auto', // 이벤트가 아래로 전달되도록 설정
            }}
          >
            {/* Iframe 설정 */}
            <iframe
              ref={iframeRef}
              src={`${import.meta.env.BASE_URL}98/index.html`}
              style={{
                width: '990px',
                height: '615px',
                transform: `scale(${contentScale})`,
                transformOrigin: 'top left',
                border: 'none',
                imageRendering: 'pixelated',
                filter: 'brightness(0.99) contrast(1) saturate(1.0)',
              }}
              allow="pointer-lock"
              title="Windows 98"
            />
          </div>
        </div>
      </div>
  
      {/* CRT 효과 */}
      <div className="refresh"></div>
      <div className="cligno"></div>
    </div>
  );
  
};

export default HeroSection;