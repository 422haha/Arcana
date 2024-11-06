import React from 'react';
import { Swiper, SwiperSlide } from 'swiper/react';
import { EffectCards } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/effect-cards';
import '@/styles/components/swiper.css';

const races = [
  // 첫번째 그룹 (체력 10)
  { 
    name: '실키 (Silky)', 
    description: '집안일을 돕는 온순한 요정으로, 물리적 힘은 매우 약하며 청결과 안락함을 책임집니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '브라우니 (Brownie)',
    description: '가정의 일을 돕는 작은 요정입니다. 밤에 몰래 나타나 집안일을 돕지만 모습을 드러내지 않습니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '그렘린 (Gremlin)',
    description: '기계와 기술에 장난을 치며, 망가뜨리는 능력이 있어 골칫거리가 될 수 있습니다.',
    image: '/assets/races/dragon.jpg',
  },
  
  // 두번째 그룹 (체력 15)
  {
    name: '임프 (Imp)',
    description: '작은 악마로 장난기 많고 마법 능력을 지니며 사람들을 유혹하거나 혼란스럽게 만듭니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '코볼트 (Kobold)',
    description: '지하에서 생활하며 마법과 함정을 능숙하게 다룹니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '고블린 (Goblin)',
    description: '민첩하고 교활하며, 단체로 행동하며 장난과 약탈을 즐깁니다.',
    image: '/assets/races/dragon.jpg',
  },

  // 세번째 그룹 (체력 20)
  {
    name: '드워프 (Dwarf)',
    description: '광산과 대장간에서 일하며 강한 체력과 뛰어난 기술력을 가졌습니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '유니콘 (Unicorn)',
    description: '순수함과 치유의 상징이며, 마법적 치유 능력을 지니고 있습니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '켄타우로스 (Centaur)',
    description: '인간과 말의 모습을 결합한 존재로, 힘과 지혜를 겸비한 전사입니다.',
    image: '/assets/races/dragon.jpg',
  },

  // 네번째 그룹 (체력 25)
  {
    name: '엘프 (Elf)',
    description: '마법 능력과 지혜를 가진 불멸의 종족으로 자연과 예술을 사랑합니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '오크 (Orc)',
    description: '강한 체력을 지닌 전사 종족으로 조직적인 군대를 이룹니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '트롤 (Troll)',
    description: '강한 재생 능력을 가진 거대한 생물로 햇빛 아래에서 돌로 변하는 약점이 있습니다.',
    image: '/assets/races/dragon.jpg',
  },
  {
    name: '자이언트 (Giant)',
    description: '거대한 체구와 막강한 힘을 지닌 종족으로 산을 움직일 수 있습니다.',
    image: '/assets/races/dragon.jpg',
  },

  // 마지막 (드래곤)
  {
    name: '드래곤 (Dragon)',
    description: '전설적인 힘과 마법 능력을 지닌 가장 강력한 생물로 불을 뿜고 하늘을 날 수 있습니다.',
    image: '/assets/races/dragon.jpg',
  },
];

const RaceFeatures: React.FC = () => {
  return (
    <div className="relative w-full min-h-screen flex flex-col lg:flex-row items-start 
                justify-center gap-12 p-6 lg:p-12 px-5 lg:px-20 overflow-hidden">
      {/* Background effects */}
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] 
                    from-blue-500/5 via-transparent to-transparent" />
      
      {/* Left section: Swiper cards */}
      <div className="relative w-full lg:w-1/2 flex flex-col items-center 
              justify-start lg:justify-center min-h-[600px] lg:min-h-[800px]">
        {/* Decorative elements */}
        <div className="absolute -left-4 top-1/2 -translate-y-1/2 w-1 h-32
                     bg-gradient-to-b from-transparent via-blue-500/30 to-transparent" />
        <div className="absolute -right-4 top-1/2 -translate-y-1/2 w-1 h-32
                     bg-gradient-to-b from-transparent via-blue-500/30 to-transparent" />

        <div className="relative w-full max-w-xs md:max-w-md lg:max-w-lg h-[450px]"> {/* 높이 추가 */}
          <Swiper
            effect="cards"
            grabCursor
            modules={[EffectCards]}
            className="mySwiper"
          >
            {races.map((race) => (
              <SwiperSlide key={race.name}>
                <div className="swiper-slide-inner relative p-4 bg-zinc-800/90 rounded-lg
                              border border-zinc-700 group h-full flex flex-col"> {/* flex-col과 h-full 추가 */}
                  {/* Card glow effect */}
                  <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                                bg-gradient-to-b from-blue-500/10 to-transparent
                                transition-opacity duration-500" />
                  
                  {/* Tech frame corners */}
                  <div className="absolute top-0 left-0 w-6 h-6 border-t-2 border-l-2 
                                border-blue-500/30" />
                  <div className="absolute top-0 right-0 w-6 h-6 border-t-2 border-r-2 
                                border-blue-500/30" />
                  <div className="absolute bottom-0 left-0 w-6 h-6 border-b-2 border-l-2 
                                border-blue-500/30" />
                  <div className="absolute bottom-0 right-0 w-6 h-6 border-b-2 border-r-2 
                                border-blue-500/30" />

                  {/* Image container */}
                  <div className="image-container relative w-full h-[200px] mb-4"> {/* 높이 고정 */}
                    <img
                      src={race.image}
                      alt={race.name}
                      className="w-full h-full object-cover rounded-lg
                               border border-zinc-600 group-hover:border-zinc-500
                               transition-colors duration-300"
                    />
                  </div>

                  {/* Content container */}
                  <div className="content flex flex-col flex-grow"> {/* flex-grow 추가 */}
                    <h3 className="font-pixel text-xl md:text-2xl text-center mb-2 
                                  text-transparent bg-clip-text 
                                  bg-gradient-to-r from-blue-400 to-teal-400">
                      {race.name}
                    </h3>
                    <p className="text-sm md:text-base text-gray-400 text-center
                                leading-relaxed">
                      {race.description}
                    </p>
                  </div>

                  {/* Scan line effect */}
                  <div className="absolute inset-0 bg-[url('/assets/scanlines.png')] 
                                opacity-[0.05] pointer-events-none" />
                </div>
              </SwiperSlide>
            ))}
          </Swiper>
        </div>
      </div>

      {/* Right section: Description */}
      <div className="relative w-full lg:w-1/2 h-full flex flex-col 
              justify-start lg:justify-center min-h-[400px] lg:min-h-[800px]
              px-4 lg:px-8">
        {/* Section title with tech frame */}
        <div className="relative mb-8 p-4">
          <div className="absolute top-0 left-0 w-8 h-8 border-t-2 border-l-2 
                       border-blue-500/30" />
          <div className="absolute top-0 right-0 w-8 h-8 border-t-2 border-r-2 
                       border-blue-500/30" />
          <h2 className="font-pixel text-3xl lg:text-4xl text-center
                      text-transparent bg-clip-text 
                      bg-gradient-to-r from-blue-400 to-teal-400">
            다양한 종족과 그들의 특성
          </h2>
        </div>

        {/* Description paragraphs */}
        <div className="relative p-6 bg-zinc-800/50 rounded-lg border border-zinc-700/50 group">
          <p className="text-base md:text-lg lg:text-xl font-pretendard text-gray-300 
                      leading-7 md:leading-8 lg:leading-9
                      tracking-normal
                      max-w-prose 
                      break-keep 
                      [text-wrap:pretty]
                      space-y-6">
            이 게임에서는 다양한 종족이 등장하며, 각 종족은 고유한 능력과 특성을 지니고 있습니다. 실키(Silky)와 브라우니(Brownie) 같은 요정들은 주로 집안일과 안락함을 책임지는 반면, 드래곤(Dragon)과 자이언트(Giant) 같은 종족은 압도적인 힘과 전투 능력을 자랑합니다.
            
            플레이어는 이들 종족의 고유한 특성을 이해하고 전략적으로 활용하여 게임을 진행해야 합니다. 어떤 종족은 뛰어난 체력을, 다른 종족은 마법적 능력을 보유하고 있어, 다양한 전략이 필요합니다.
            
            이처럼 다양한 종족 간의 상호작용과 전략적 선택이 이 게임의 핵심입니다. 각 종족의 특성을 최대한 활용해 최적의 팀을 구성하세요!
          </p>
          <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                        bg-gradient-to-r from-blue-500/5 to-teal-500/5
                        transition-opacity duration-300 rounded-lg" />
        </div>
      </div>

      {/* Additional ambient effects */}
      <div className="absolute bottom-0 left-0 w-full h-px
                   bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      <div className="absolute top-0 left-0 w-full h-px
                   bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
    </div>
  );
};

export default RaceFeatures;
