import { useNavigate } from 'react-router-dom';
import { Ingredient, IngredientSise } from '../../shared/api/ingredientTypes';
import { useMyIngredientsStore } from '@/shared/stores/myIngredientsStore';

interface IngredientCardProps {
  ingredient: Ingredient | IngredientSise;
  disableNavigation?: boolean; // 네비게이션 비활성화 여부를 결정하는 prop
}

const IngredientCard = ({ ingredient, disableNavigation = false }: IngredientCardProps): JSX.Element => {
  const navigate = useNavigate();
  const { ingredients, addIngredient, removeIngredient } = useMyIngredientsStore();

  const isInMyIngredients = ingredients.some((i) => i.id === ingredient.ingredientId);

  const handleCardClick = () => {
    // disableNavigation이 false일 때만 상세 페이지로 이동
    if (!disableNavigation) {
      navigate(`/ingredients/${ingredient.ingredientId}`);
    }
  };

  const handleAddOrRemove = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (isInMyIngredients) {
      removeIngredient(ingredient.ingredientId);
    } else {
      addIngredient(ingredient.ingredientId, ingredient.name);
    }
  };

  const formatPrice = (price: string | number | undefined) => {
    if (price === undefined) return '가격 정보 없음';
    return `${Number(price).toLocaleString()}원`;
  };

  return (
    <div
      className={`bg-white rounded-2xl shadow-md overflow-hidden cursor-pointer transition-all duration-300 hover:shadow-lg flex flex-col h-[300px] group ${!disableNavigation ? 'cursor-pointer' : ''}`}
      onClick={handleCardClick}
    >
      <div className="relative h-1/2 overflow-hidden">
        {ingredient?.image ? (
          <img
            src={ingredient.image}
            alt={ingredient.name ?? '재료 이미지'}
            className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
          />
        ) : (
          <div className="w-full h-full bg-gray-100 flex items-center justify-center text-gray-400">
            이미지 없음
          </div>
        )}
      </div>
      <div className="p-4 flex-grow flex flex-col justify-between">
        <div>
          <h3 className="text-lg font-semibold text-gray-800 truncate">
            {ingredient?.name || '식재료 정보 없음'}
          </h3>
          {ingredient?.retailUnit && ingredient?.retailUnitsize && (
            <p className="text-sm text-gray-500">
              {ingredient.retailUnitsize}{ingredient.retailUnit}
            </p>
          )}
          {ingredient?.price ? (
            <p className="text-xl font-bold text-gray-800 mt-2">
              {formatPrice(ingredient.price)}
            </p>
          ) : (
            <p className="text-lg text-gray-400 mt-2">가격정보 없음</p>
          )}
          {ingredient?.changeRate !== undefined && ingredient?.changePrice !== undefined && (
            <p className={`text-sm ${ingredient.changeRate >= 0 ? 'text-red-500' : 'text-blue-500'} mt-1`}>
              {ingredient.changeRate >= 0 ? '▲' : '▼'} {Math.abs(ingredient.changeRate).toFixed(1)}%
              ({formatPrice(Math.abs(ingredient.changePrice))})
            </p>
          )}
        </div>
        <button
          className={`mt-2 ${
            isInMyIngredients
              ? 'bg-gray-200 text-gray-800 hover:bg-gray-300'
              : 'bg-blue-500 text-white hover:bg-blue-600'
          } px-3 py-1 rounded-full text-sm transition-colors duration-300`}
          onClick={handleAddOrRemove}
        >
          {isInMyIngredients ? '장바구니에서 제거' : '장바구니에 추가'}
        </button>
      </div>
    </div>
  );
};

export default IngredientCard;