import { useNavigate } from 'react-router-dom';

// PriceGraphCard 컴포넌트의 props 타입을 정의합니다.
// 이 인터페이스는 컴포넌트가 받을 수 있는 속성들을 명시합니다.
interface PriceGraphCardProps {
  graphId: number; // 그래프의 고유 ID
  title: string; // 카드에 표시될 제목
  width?: number; // 카드의 너비 (선택적)
  height?: number; // 카드의 높이 (선택적)
}

// PriceGraphCard 컴포넌트를 정의합니다.
// 이 컴포넌트는 가격 그래프를 카드 형태로 표시합니다.
const PriceGraphCard = ({
  graphId,
  title,
  width = 300, // 기본 너비 값
  height = 200, // 기본 높이 값
}: PriceGraphCardProps) => {
  // useNavigate 훅을 사용하여 페이지 이동 함수를 가져옵니다.
  const navigate = useNavigate();

  // 카드 클릭 시 실행될 함수입니다.
  // 클릭하면 해당 가격 그래프의 상세 페이지로 이동합니다.
  const handleCardClick = () => {
    navigate(`/price-graphs/${graphId}`);
  };

  // 컴포넌트의 UI를 렌더링합니다.
  return (
    <div
      className="bg-white rounded-lg shadow-md overflow-hidden cursor-pointer transition-transform hover:scale-105 flex flex-col"
      style={{ width: `${width}px`, height: `${height}px` }}
      onClick={handleCardClick}
    >
      <div className="p-4 flex-grow flex flex-col justify-between">
        <h3 className="text-lg font-semibold mb-2">{title}</h3>
        <div className="flex-grow flex items-center justify-center text-gray-500">
          가격 그래프 카드입니다.
        </div>
      </div>
    </div>
  );
};

export default PriceGraphCard;
