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
    { id: 1, url: "/assets/gallery/Arcana_Artifact/1. 정비키트.jpg", description: "정비키트" },
    { id: 2, url: "/assets/gallery/Arcana_Artifact/2. 가면.jpg", description: "가면" },
    { id: 3, url: "/assets/gallery/Arcana_Artifact/3. 게걸스런 쓰레기통.jpg", description: "게걸스런 쓰레기통" },
    { id: 4, url: "/assets/gallery/Arcana_Artifact/4. 재활용 상자.jpg", description: "재활용 상자" },
    { id: 5, url: "/assets/gallery/Arcana_Artifact/5. 임프의 단검.jpg", description: "임프의 단검" },
    { id: 6, url: "/assets/gallery/Arcana_Artifact/6. 드워프 장갑.jpg", description: "드워프 장갑" },
    { id: 7, url: "/assets/gallery/Arcana_Artifact/7. 엘프의 목걸이.jpg", description: "엘프의 목걸이" },
    { id: 8, url: "/assets/gallery/Arcana_Artifact/8. 봉인된 자물쇠.jpg", description: "봉인된 자물쇠" },
    { id: 9, url: "/assets/gallery/Arcana_Artifact/9. 마법의 정수.jpg", description: "마법의 정수" },
    { id: 10, url: "/assets/gallery/Arcana_Artifact/10. 자가 치유 섬유.jpg", description: "자가 치유 섬유" },

    // 카드 이미지 (1~68)
    { id: 11, url: "/assets/gallery/Arcana_Card_1~68/1. 가드맨.jpg", description: "가드맨" },
    { id: 12, url: "/assets/gallery/Arcana_Card_1~68/2. 자원 밀수꾼.jpg", description: "자원 밀수꾼" },
    { id: 13, url: "/assets/gallery/Arcana_Card_1~68/3. 지역 보안관.jpg", description: "지역 보안관" },
    { id: 14, url: "/assets/gallery/Arcana_Card_1~68/4. SSAFY 개발자.jpg", description: "SSAFY 개발자" },
    { id: 15, url: "/assets/gallery/Arcana_Card_1~68/9. 햇빛 수집가.jpg", description: "햇빛 수집가" },
    { id: 16, url: "/assets/gallery/Arcana_Card_1~68/10. 쿠키 굽는 실키.jpg", description: "쿠키 굽는 실키" },
    { id: 17, url: "/assets/gallery/Arcana_Card_1~68/11. 수호자 실키.jpg", description: "수호자 실키" },
    { id: 18, url: "/assets/gallery/Arcana_Card_1~68/12. 뒹굴거리는 브라우니.jpg", description: "뒹굴거리는 브라우니" },
    { id: 19, url: "/assets/gallery/Arcana_Card_1~68/17. 간식상자 브라우니.jpg", description: "간식상자 브라우니" },
    { id: 20, url: "/assets/gallery/Arcana_Card_1~68/18. 시계장치 그렘린.jpg", description: "시계장치 그렘린" },
    { id: 21, url: "/assets/gallery/Arcana_Card_1~68/19. 공구상자 절도범.jpg", description: "공구상자 절도범" },
    { id: 22, url: "/assets/gallery/Arcana_Card_1~68/20. 정비사 그렘린.jpg", description: "정비사 그렘린" },
    { id: 23, url: "/assets/gallery/Arcana_Card_1~68/25. 변장한 임프.jpg", description: "변장한 임프" },
    { id: 24, url: "/assets/gallery/Arcana_Card_1~68/26. 파티 임프.jpg", description: "파티 임프" },
    { id: 25, url: "/assets/gallery/Arcana_Card_1~68/27. 로켓을 탄 임프.jpg", description: "로켓을 탄 임프" },
    { id: 26, url: "/assets/gallery/Arcana_Card_1~68/28. 녹색 임프.jpg", description: "녹색 임프" },
    { id: 27, url: "/assets/gallery/Arcana_Card_1~68/33. 응급병 코볼트.jpg", description: "응급병 코볼트" },
    { id: 28, url: "/assets/gallery/Arcana_Card_1~68/34. 코볼트 마법사.jpg", description: "코볼트 마법사" },
    { id: 29, url: "/assets/gallery/Arcana_Card_1~68/35. 휴식 중인 코볼트.jpg", description: "휴식 중인 코볼트" },
    { id: 30, url: "/assets/gallery/Arcana_Card_1~68/36. 코뿔소 기수 코볼트.jpg", description: "코뿔소 기수 코볼트" },
    { id: 31, url: "/assets/gallery/Arcana_Card_1~68/41. 소란스런 고블린.jpg", description: "소란스런 고블린" },
    { id: 32, url: "/assets/gallery/Arcana_Card_1~68/42. 깃발 고블린.jpg", description: "깃발 고블린" },
    { id: 33, url: "/assets/gallery/Arcana_Card_1~68/43. 정예 고블린.jpg", description: "정예 고블린" },
    { id: 34, url: "/assets/gallery/Arcana_Card_1~68/44. 락스타 고블린.jpg", description: "락스타 고블린" },
    { id: 35, url: "/assets/gallery/Arcana_Card_1~68/49. 양손도끼 드워프.jpg", description: "양손도끼 드워프" },
    { id: 36, url: "/assets/gallery/Arcana_Card_1~68/50. 버려낸 도끼 장인 드워프.jpg", description: "버려낸 도끼 장인 드워프" },
    { id: 37, url: "/assets/gallery/Arcana_Card_1~68/51. 황금투구 드워프.jpg", description: "황금투구 드워프" },
    { id: 38, url: "/assets/gallery/Arcana_Card_1~68/52. 드워프 기사단장.jpg", description: "드워프 기사단장" },
    { id: 39, url: "/assets/gallery/Arcana_Card_1~68/57. 무지개 수호병 유니콘.jpg", description: "무지개 수호병 유니콘" },
    { id: 40, url: "/assets/gallery/Arcana_Card_1~68/58. 물을 긷는 유니콘.jpg", description: "물을 긷는 유니콘" },
    { id: 41, url: "/assets/gallery/Arcana_Card_1~68/59. 푸른 갈기 유니콘.jpg", description: "푸른 갈기 유니콘" },
    { id: 42, url: "/assets/gallery/Arcana_Card_1~68/60. 위대한 뿔 유니콘.jpg", description: "위대한 뿔 유니콘" },
    { id: 43, url: "/assets/gallery/Arcana_Card_1~68/65. 돌격대장 켄타우로스.jpg", description: "돌격대장 켄타우로스" },
    { id: 44, url: "/assets/gallery/Arcana_Card_1~68/66. 치어리더 켄타우로스.jpg", description: "치어리더 켄타우로스" },
    { id: 45, url: "/assets/gallery/Arcana_Card_1~68/67. 장학사 켄타우로스.jpg", description: "장학사 켄타우로스" },
    { id: 46, url: "/assets/gallery/Arcana_Card_1~68/68. 마스터 켄타우로스.jpg", description: "마스터 켄타우로스" },

    // 카드 이미지 (69~135)
    { id: 47, url: "/assets/gallery/Arcana_Card_69~135/69. 물을 긷는 엘프.jpg", description: "물을 긷는 엘프" },
    { id: 48, url: "/assets/gallery/Arcana_Card_69~135/70. 하프를 든 엘프.jpg", description: "하프를 든 엘프" },
    { id: 49, url: "/assets/gallery/Arcana_Card_69~135/71. 엘프 정찰대.jpg", description: "엘프 정찰대" },
    { id: 50, url: "/assets/gallery/Arcana_Card_69~135/100. 정령의 친구.jpg", description: "정령의 친구" },
    { id: 51, url: "/assets/gallery/Arcana_Card_69~135/101. 암초 투척병.jpg", description: "암초 투척병" },
    { id: 52, url: "/assets/gallery/Arcana_Card_69~135/102. 설산 관리인.jpg", description: "설산 관리인" },
    { id: 53, url: "/assets/gallery/Arcana_Card_69~135/103. 화산 파괴자 자이언트.jpg", description: "화산 파괴자 자이언트" },
    { id: 54, url: "/assets/gallery/Arcana_Card_69~135/104. 타이탄.jpg", description: "타이탄" },
    { id: 55, url: "/assets/gallery/Arcana_Card_69~135/105. 영롱한 구체.jpg", description: "영롱한 구체" },
    { id: 56, url: "/assets/gallery/Arcana_Card_69~135/106. 베이비 드래곤.jpg", description: "베이비 드래곤" },
    { id: 57, url: "/assets/gallery/Arcana_Card_69~135/107. 와이번.jpg", description: "와이번" },
    { id: 58, url: "/assets/gallery/Arcana_Card_69~135/108. 가난한 적룡.jpg", description: "가난한 적룡" },
    { id: 59, url: "/assets/gallery/Arcana_Card_69~135/109. 얹혀 사는 공룡.jpg", description: "얹혀 사는 공룡" },
    { id: 60, url: "/assets/gallery/Arcana_Card_69~135/110. 주정뱅이 용.jpg", description: "주정뱅이 용" },
    { id: 61, url: "/assets/gallery/Arcana_Card_69~135/111. 쌍두용.jpg", description: "쌍두용" },
    { id: 62, url: "/assets/gallery/Arcana_Card_69~135/112. 원소술사 드래곤.jpg", description: "원소술사 드래곤" },
    { id: 63, url: "/assets/gallery/Arcana_Card_69~135/113. 레어 키퍼.jpg", description: "레어 키퍼" },
    { id: 64, url: "/assets/gallery/Arcana_Card_69~135/114. 신속 사격.jpg", description: "신속 사격" },
    { id: 65, url: "/assets/gallery/Arcana_Card_69~135/115. 정조준.jpg", description: "정조준" },
    { id: 66, url: "/assets/gallery/Arcana_Card_69~135/116. 저격.jpg", description: "저격" },
    { id: 67, url: "/assets/gallery/Arcana_Card_69~135/117. 자책.jpg", description: "자책" },
    { id: 68, url: "/assets/gallery/Arcana_Card_69~135/118. 암기 투척.jpg", description: "암기 투척" },
    { id: 69, url: "/assets/gallery/Arcana_Card_69~135/119. 강한 타격.jpg", description: "강한 타격" },
    { id: 70, url: "/assets/gallery/Arcana_Card_69~135/120. 전방위 폭격.jpg", description: "전방위 폭격" },
    { id: 71, url: "/assets/gallery/Arcana_Card_69~135/121. 정밀 폭격.jpg", description: "정밀 폭격" },
    { id: 72, url: "/assets/gallery/Arcana_Card_69~135/122. 빛 쪼이기.jpg", description: "빛 쪼이기" },
    { id: 73, url: "/assets/gallery/Arcana_Card_69~135/123. 격전.jpg", description: "격전" },
    { id: 74, url: "/assets/gallery/Arcana_Card_69~135/124. 초토화.jpg", description: "초토화" },
    { id: 75, url: "/assets/gallery/Arcana_Card_69~135/125. 폴리모프.jpg", description: "폴리모프" },
    { id: 76, url: "/assets/gallery/Arcana_Card_69~135/126. 응급처치.jpg", description: "응급처치" },
    { id: 77, url: "/assets/gallery/Arcana_Card_69~135/127. 치유의 손길.jpg", description: "치유의 손길" },
    { id: 78, url: "/assets/gallery/Arcana_Card_69~135/128. 보급품 투하.jpg", description: "보급품 투하" },
    { id: 79, url: "/assets/gallery/Arcana_Card_69~135/129. 순간 치유.jpg", description: "순간 치유" },
    { id: 80, url: "/assets/gallery/Arcana_Card_69~135/130. 치유의 빛.jpg", description: "치유의 빛" },
    { id: 81, url: "/assets/gallery/Arcana_Card_69~135/131. 이상한 스크롤.jpg", description: "이상한 스크롤" },
    { id: 82, url: "/assets/gallery/Arcana_Card_69~135/132. 정비부품.jpg", description: "정비부품" },
    { id: 83, url: "/assets/gallery/Arcana_Card_69~135/133. 엘프의 비밀 간식.jpg", description: "엘프의 비밀 간식" },
    { id: 84, url: "/assets/gallery/Arcana_Card_69~135/134. 가치 투자.jpg", description: "가치 투자" },
    { id: 85, url: "/assets/gallery/Arcana_Card_69~135/135. 방패 부여.jpg", description: "방패 부여" },

    // 포션 이미지
    { id: 136, url: "/assets/gallery/Arcana_Potion/1. 피 포션.jpg", description: "피 포션" },
    { id: 137, url: "/assets/gallery/Arcana_Potion/2. 회복 포션.jpg", description: "회복 포션" },
    { id: 138, url: "/assets/gallery/Arcana_Potion/3. 엘프의 눈물.jpg", description: "엘프의 눈물" },
    { id: 139, url: "/assets/gallery/Arcana_Potion/4. 재생 포션.jpg", description: "재생 포션" },
    { id: 140, url: "/assets/gallery/Arcana_Potion/5. 달빛 정수.jpg", description: "달빛 정수" },
    { id: 141, url: "/assets/gallery/Arcana_Potion/6. 진정 포션.jpg", description: "진정 포션" },
    { id: 142, url: "/assets/gallery/Arcana_Potion/7. 따끔 포션.jpg", description: "따끔 포션" },
    { id: 143, url: "/assets/gallery/Arcana_Potion/8. 공격 포션.jpg", description: "공격 포션" },
    { id: 144, url: "/assets/gallery/Arcana_Potion/9. 폭발 포션.jpg", description: "폭발 포션" },
    { id: 145, url: "/assets/gallery/Arcana_Potion/10. 마나 포션.jpg", description: "마나 포션" },
    { id: 146, url: "/assets/gallery/Arcana_Card_1~68/13. 밤의 청소부.jpg", description: "밤의 청소부" },
    { id: 147, url: "/assets/gallery/Arcana_Card_1~68/14. 약초 수집가.jpg", description: "약초 수집가" },
    { id: 148, url: "/assets/gallery/Arcana_Card_1~68/15. 브라우니 치료사.jpg", description: "브라우니 치료사" },
    { id: 149, url: "/assets/gallery/Arcana_Card_1~68/16. 먹보 브라우니.jpg", description: "먹보 브라우니" },
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
    className="fixed inset-0 bg-black/80 z-50 flex items-center justify-center cursor-pointer backdrop-blur-sm"
    onClick={() => setSelectedImage(null)}
  >
    <div className="relative max-w-4xl w-full mx-4 transform hover:scale-[1.02] transition-transform duration-300">
      <div className="relative bg-zinc-900/90 rounded-lg p-4 shadow-[0_0_15px_rgba(59,130,246,0.5)]">
        {/* 코너 데코레이션 */}
        <div className="absolute top-0 left-0 w-8 h-8 border-l-2 border-t-2 border-blue-400/50 -translate-x-1 -translate-y-1" />
        <div className="absolute top-0 right-0 w-8 h-8 border-r-2 border-t-2 border-blue-400/50 translate-x-1 -translate-y-1" />
        <div className="absolute bottom-0 left-0 w-8 h-8 border-l-2 border-b-2 border-blue-400/50 -translate-x-1 translate-y-1" />
        <div className="absolute bottom-0 right-0 w-8 h-8 border-r-2 border-b-2 border-blue-400/50 translate-x-1 translate-y-1" />
        
        {/* 이미지 컨테이너 */}
        <div className="flex justify-center items-center">
          <img
            src={selectedImage.url}
            alt={selectedImage.description}
            className="rounded-2xl max-w-full max-h-[65vh] object-cover"
            style={{ borderRadius: '1rem' }}
          />
        </div>

        {/* 설명 텍스트 */}
        <div className="mt-4 text-center">
          <h3 className="text-2xl md:text-3xl font-pixel bg-gradient-to-r from-blue-400 via-cyan-400 to-teal-400 text-transparent bg-clip-text">
            {selectedImage.description}
          </h3>
          <p className="text-zinc-400 text-sm mt-2 font-medium">
            Click anywhere to close
          </p>
        </div>
      </div>
    </div>

    {/* 닫기 버튼 */}
    <button 
      className="absolute top-4 right-4 text-gray-400 hover:text-white transition-colors duration-200"
      onClick={(e) => {
        e.stopPropagation();
        setSelectedImage(null);
      }}
    >
      <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>
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