import { useState } from 'react';
import InfiniteSlide from './InfiniteSlide';

interface Screenshot {
  id: number;
  url: string;
  description: string;
}

const GameGallery = () => {
  const [selectedImage, setSelectedImage] = useState<Screenshot | null>(null);

  const screenshots = [
    // 아티팩트 이미지
    { id: 1, url: "/assets/gallery/Arcana_Artifact_Image/1. 정비키트.jpg", description: "정비키트" },
    { id: 2, url: "/assets/gallery/Arcana_Artifact_Image/2. 가면.jpg", description: "가면" },
    { id: 3, url: "/assets/gallery/Arcana_Artifact_Image/3. 게걸스런 쓰레기통.jpg", description: "게걸스런 쓰레기통" },
    { id: 4, url: "/assets/gallery/Arcana_Artifact_Image/4. 재활용 상자.jpg", description: "재활용 상자" },
    { id: 5, url: "/assets/gallery/Arcana_Artifact_Image/5. 임프의 단검.jpg", description: "임프의 단검" },
    { id: 6, url: "/assets/gallery/Arcana_Artifact_Image/6. 드워프 장갑.jpg", description: "드워프 장갑" },
    { id: 7, url: "/assets/gallery/Arcana_Artifact_Image/7. 엘프의 목걸이.jpg", description: "엘프의 목걸이" },
    { id: 8, url: "/assets/gallery/Arcana_Artifact_Image/8. 봉인된 자물쇠.jpg", description: "봉인된 자물쇠" },
    { id: 9, url: "/assets/gallery/Arcana_Artifact_Image/9. 마법의 정수.jpg", description: "마법의 정수" },
    { id: 10, url: "/assets/gallery/Arcana_Artifact_Image/10. 자가 치유 섬유.jpg", description: "자가 치유 섬유" },

    // 카드 이미지 (6~60)
    { id: 11, url: "/assets/gallery/Arcana_Card_Image_6~60/6. 솜뭉치 실키.jpg", description: "솜뭉치 실키" },
    { id: 12, url: "/assets/gallery/Arcana_Card_Image_6~60/7. 봉고를 든 실키.jpg", description: "봉고를 든 실키" },
    { id: 13, url: "/assets/gallery/Arcana_Card_Image_6~60/8. 파수꾼 실키.jpg", description: "파수꾼 실키" },
    // ... (나머지 6~60 카드 이미지들)
    { id: 65, url: "/assets/gallery/Arcana_Card_Image_6~60/60. 위대한 뿔 유니콘.jpg", description: "위대한 뿔 유니콘" },

    // 카드 이미지 (61~113)
    { id: 66, url: "/assets/gallery/Arcana_Card_Image_61~113/61. 견습 켄타우로스.jpg", description: "견습 켄타우로스" },
    { id: 67, url: "/assets/gallery/Arcana_Card_Image_61~113/62. 나무칼 수집가 켄타우로스.jpg", description: "나무칼 수집가 켄타우로스" },
    // ... (나머지 61~113 카드 이미지들)
    { id: 118, url: "/assets/gallery/Arcana_Card_Image_61~113/113. 레어 키퍼.jpg", description: "레어 키퍼" },

    // 포션 이미지
    { id: 119, url: "/assets/gallery/Arcana_Potion_Image/1. 피 포션.jpg", description: "피 포션" },
    { id: 120, url: "/assets/gallery/Arcana_Potion_Image/2. 회복 포션.jpg", description: "회복 포션" },
    { id: 121, url: "/assets/gallery/Arcana_Potion_Image/3. 엘프의 눈물.jpg", description: "엘프의 눈물" },
    { id: 122, url: "/assets/gallery/Arcana_Potion_Image/4. 재생 포션.jpg", description: "재생 포션" },
    { id: 123, url: "/assets/gallery/Arcana_Potion_Image/5. 달빛 정수.jpg", description: "달빛 정수" },
    { id: 124, url: "/assets/gallery/Arcana_Potion_Image/6. 진정 포션.jpg", description: "진정 포션" },
    { id: 125, url: "/assets/gallery/Arcana_Potion_Image/7. 따끔 포션.jpg", description: "따끔 포션" },
    { id: 126, url: "/assets/gallery/Arcana_Potion_Image/8. 공격 포션.jpg", description: "공격 포션" },
    { id: 127, url: "/assets/gallery/Arcana_Potion_Image/9. 폭발 포션.jpg", description: "폭발 포션" },
    { id: 128, url: "/assets/gallery/Arcana_Potion_Image/10. 마나 포션.jpg", description: "마나 포션" },
  ];

  const handleImageClick = (image: Screenshot) => {
    setSelectedImage(image);
  };

  return (
    <section className="py-20 sm:py-24 md:py-28 lg:py-32 relative overflow-hidden">
      <div className="container mx-auto px-4 md:px-6 lg:px-8 relative z-10">
        {/* 제목 섹션 */}
        <div className="mb-12 sm:mb-16 md:mb-20">
          <h2 className="text-4xl md:text-5xl lg:text-6xl font-pixel text-center text-transparent 
                         bg-clip-text bg-gradient-to-r from-blue-400 to-teal-400">
            135종의 다양한 카드
          </h2>
        </div>

        {/* 갤러리 섹션 */}
        <div className="relative py-10 sm:py-12 md:py-14 lg:py-16">
          <InfiniteSlide images={screenshots} onImageClick={handleImageClick} />
        </div>
      </div>

      {/* 심플 모달 - 버튼들 제거 */}
      {selectedImage && (
        <div 
          className="fixed inset-0 bg-black/90 z-50 flex items-center justify-center cursor-pointer"
          onClick={() => setSelectedImage(null)}
        >
          <div className="relative max-w-5xl w-full mx-4">
            <div className="relative">
              <img
                src={selectedImage.url}
                alt={selectedImage.description}
                className="w-full h-auto object-contain max-h-[80vh]"
              />
            </div>
            <div className="text-white text-center mt-4">
              {selectedImage.description}
            </div>
          </div>
        </div>
      )}

      {/* 코너 장식 */}
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