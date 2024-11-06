import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import IngredientCard from '@/widgets/IngredientCard/IngredientCard';
import { setLandingPagePreference, setLandingPageViewed } from '@/shared/utils/landingPageUtils';
import { FaTag, FaUtensils, FaCarrot, FaPiggyBank, FaSearch, FaListUl, FaThumbsUp } from 'react-icons/fa';
import { motion, useAnimation } from 'framer-motion';
import { useIngredientSise } from '@/features/ingredient/api/useIngredients';

const LandingPage = () => {
  const [showAgain, setShowAgain] = useState(true);
  const [currentSection, setCurrentSection] = useState(0);
  const [direction, setDirection] = useState(0);

  const { data: ingredientSiseData } = useIngredientSise({
    period: 'week',
    change: 'positive',
    count: 4
  });

  const controls = useAnimation();
  const cardRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = 'auto';
    };
  }, []);

  useEffect(() => {
    const handleWheel = (e: WheelEvent) => {
      e.preventDefault();
      const newDirection = e.deltaY > 0 ? 1 : -1;
      const nextSection = Math.max(0, Math.min(3, currentSection + newDirection));
      if (nextSection !== currentSection) {
        setDirection(newDirection);
        setCurrentSection(nextSection);
      }
    };

    window.addEventListener('wheel', handleWheel, { passive: false });
    return () => window.removeEventListener('wheel', handleWheel);
  }, [currentSection]);

  useEffect(() => {
    const handleScroll = () => {
      if (cardRef.current) {
        const { top, bottom } = cardRef.current.getBoundingClientRect();
        const windowHeight = window.innerHeight;
        const progress = 1 - (bottom - windowHeight) / (bottom - top);
        controls.start({ progress: Math.max(0, Math.min(1, progress)) } as any);
      }
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [controls]);

  const handleShowAgainChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setShowAgain(!event.target.checked);
  };

  const handleContinue = () => {
    if (!showAgain) {
      setLandingPagePreference(false);
    }
    setLandingPageViewed();
  };

  const cardVariants = {
    hidden: (i: number) => {
      const angle = (i / 4) * 2 * Math.PI;
      return {
        x: Math.cos(angle) * window.innerWidth * 1.5,
        y: Math.sin(angle) * window.innerHeight * 1.5,
        opacity: 0,
        scale: 0.5,
      };
    },
    visible: {
      x: 0,
      y: 0,
      opacity: 1,
      scale: 1,
      transition: {
        type: 'spring',
        stiffness: 50,
        damping: 20,
        mass: 1.5,
        duration: 1.5,
      },
    },
  };

  const pageVariants = {
    enter: (direction: number) => ({
      y: direction > 0 ? '100%' : '-100%',
      opacity: 0,
    }),
    center: {
      y: 0,
      opacity: 1,
    },
    exit: (direction: number) => ({
      y: direction < 0 ? '100%' : '-100%',
      opacity: 0,
    }),
  };

  const pageTransition = {
    type: 'spring',
    stiffness: 50,
    damping: 20,
    mass: 2,
    duration: 1,
  };

  const renderSection = (index: number) => {
    switch (index) {
      case 0:
        return (
          <div className="h-screen flex flex-col justify-center items-center text-center px-4">
            <h1 className="text-6xl font-bold mb-6">
              미정<span className="text-3xl">味定</span>
            </h1>
            <p className="text-2xl mb-8">경제적인 자취생활의 시작</p>
            <p className="text-xl mb-8">실시간 식재료 가격으로 현명한 요리 생활을</p>
          </div>
        );
      case 1:
        return (
          <div className="h-screen flex flex-col justify-center items-center px-4">
            <h2 className="text-3xl font-bold text-center mb-12">미정(味定)은 이런 서비스예요</h2>
            <div className="max-w-4xl mx-auto grid grid-cols-2 gap-8">
              <div className="text-center">
                <FaTag className="w-16 h-16 mx-auto mb-4 text-blue-500" />
                <h3 className="text-xl font-semibold mb-2">실시간 식재료 가격</h3>
                <p>최신 시세를 확인하고 경제적인 식재료를 선택하세요</p>
              </div>
              <div className="text-center">
                <FaUtensils className="w-16 h-16 mx-auto mb-4 text-blue-500" />
                <h3 className="text-xl font-semibold mb-2">맞춤 레시피 추천</h3>
                <p>가지고 있는 재료로 만들 수 있는 레시피를 추천해드려요</p>
              </div>
              <div className="text-center">
                <FaCarrot className="w-16 h-16 mx-auto mb-4 text-blue-500" />
                <h3 className="text-xl font-semibold mb-2">식재료 조합 추천</h3>
                <p>선택한 재료와 잘 어울리는 다른 재료를 추천해드려요</p>
              </div>
              <div className="text-center">
                <FaPiggyBank className="w-16 h-16 mx-auto mb-4 text-blue-500" />
                <h3 className="text-xl font-semibold mb-2">경제적인 자취 생활</h3>
                <p>현명한 식재료 선택으로 생활비를 절약하세요</p>
              </div>
            </div>
          </div>
        );
      case 2:
        return (
          <div className="h-screen flex flex-col justify-center items-center px-4 py-8" ref={cardRef}>
            <h2 className="text-3xl font-bold text-center mb-8">실시간 가격, 현명한 선택</h2>
            <div className="w-full max-w-screen-lg flex-grow flex items-center">
              <div className="grid grid-cols-2 md:grid-cols-4 gap-3 md:gap-4">
                {ingredientSiseData?.slice(0, 4).map((ingredient, index) => (
                  <motion.div
                    key={ingredient.ingredientId}
                    className="w-full aspect-[1/1]"
                    variants={cardVariants}
                    custom={index}
                    initial="hidden"
                    animate="visible"
                  >
                    {/* 
                      랜딩 페이지에서는 IngredientCard의 클릭 기능을 비활성화합니다.
                      이는 사용자가 랜딩 페이지에서 실수로 다른 페이지로 이동하는 것을 방지합니다.
                    */}
                    <IngredientCard 
                      ingredient={ingredient} 
                      disableNavigation={true}
                    />
                  </motion.div>
                ))}
              </div>
            </div>
            <div className="mt-8 text-center max-w-2xl">
              <p className="text-xl font-semibold mb-4">
                실시간 가격으로 똑똑한 장보기
              </p>
              <p className="text-lg text-gray-600 mb-4">
                최신 시세를 확인하고 현명하게 선택하세요
              </p>
              <p className="text-lg text-gray-600">
                자취 생활의 경제적인 파트너, 미정
              </p>
            </div>
          </div>
        );
      case 3:
        return (
          <div className="h-screen flex flex-col justify-center items-center px-4">
            <h2 className="text-3xl font-bold text-center mb-12">이렇게 사용해보세요</h2>
            <div className="max-w-4xl mx-auto space-y-8">
              <div className="flex items-center">
                <div className="w-1/2">
                  <h3 className="text-2xl font-semibold mb-4">1. 실시간 가격 확인</h3>
                  <p>메인 페이지에서 실시간으로 업데이트되는 식재료 가격을 확인하세요.</p>
                </div>
                <div className="w-1/2 flex justify-center">
                  <FaSearch className="w-24 h-24 text-blue-500" />
                </div>
              </div>
              <div className="flex items-center flex-row-reverse">
                <div className="w-1/2">
                  <h3 className="text-2xl font-semibold mb-4">2. 식재료 선택</h3>
                  <p>가지고 있거나 구매할 예정인 식재료를 선택하세요.</p>
                </div>
                <div className="w-1/2 flex justify-center">
                  <FaListUl className="w-24 h-24 text-blue-500" />
                </div>
              </div>
              <div className="flex items-center">
                <div className="w-1/2">
                  <h3 className="text-2xl font-semibold mb-4">3. 추천 받기</h3>
                  <p>선택한 식재료를 기반으로 어울리는 재료와 레시피를 추천받으세요.</p>
                </div>
                <div className="w-1/2 flex justify-center">
                  <FaThumbsUp className="w-24 h-24 text-blue-500" />
                </div>
              </div>
            </div>
            <div className="mt-12 text-center">
              <div className="mb-4">
                <label className="flex items-center justify-center">
                  <input
                    type="checkbox"
                    checked={!showAgain}
                    onChange={handleShowAgainChange}
                    className="mr-2"
                  />
                  오늘 다시 보지 않기
                </label>
                {/* 오늘 다시 보지 않기 체크 시 랜딩페이지를 보여주지 않습니다. */}
              </div>
              <Link
                to="/"
                onClick={handleContinue}
                className="bg-blue-500 text-white px-8 py-4 rounded-full font-semibold hover:bg-blue-600 transition duration-300 text-lg"
              >
                시작하기
              </Link>
              {/* 시작하기 버튼을 누르면 세션 단위에서 랜딩페이지를 보여주지 않습니다. 
              새로운 탭이나 창에서는 랜딩페이지를 보여줍니다. */}
            </div>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="h-screen overflow-hidden">
      <nav className="fixed top-0 right-0 p-4 z-50">
        {[0, 1, 2, 3].map((index) => (
          <button
            key={index}
            className={`w-3 h-3 rounded-full mx-1 ${
              currentSection === index ? 'bg-blue-500' : 'bg-gray-300'
            }`}
            onClick={() => {
              setDirection(index > currentSection ? 1 : -1);
              setCurrentSection(index);
            }}
          />
        ))}
      </nav>

      <motion.div
        key={currentSection}
        custom={direction}
        variants={pageVariants}
        initial="enter"
        animate="center"
        exit="exit"
        transition={pageTransition}
        className="absolute w-full h-full"
      >
        {renderSection(currentSection)}
      </motion.div>
    </div>
  );
};

export default LandingPage;