import IngredientCard from '../../../widgets/IngredientCard/IngredientCard';
import {
  Ingredient,
  PaginationInfo,
} from '../../../shared/api/ingredientTypes';

interface IngredientListProps {
  ingredients: Ingredient[];
  pagination?: PaginationInfo;
  onPageChange: (page: number) => void;
}

export const IngredientList = ({
  ingredients,
  pagination,
  onPageChange,
}: IngredientListProps) => {
  if (ingredients.length === 0) return <div>데이터가 없습니다.</div>;

  return (
    <>
      <ul className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {ingredients.map((ingredient) => (
          <li key={ingredient.ingredientId}>
            <IngredientCard ingredient={ingredient} />
          </li>
        ))}
      </ul>
      {pagination && (
        <div className="mt-4 flex justify-between items-center">
          <span>
            총 {pagination.total} 중 {pagination.page} 페이지
          </span>
          <div>
            <button
              onClick={() => onPageChange(pagination.page - 1)}
              disabled={pagination.page === 1}
              className="px-4 py-2 mr-2 bg-blue-500 text-white rounded disabled:bg-gray-300"
            >
              이전
            </button>
            <button
              onClick={() => onPageChange(pagination.page + 1)}
              disabled={
                pagination.page * pagination.perPage >= pagination.total
              }
              className="px-4 py-2 bg-blue-500 text-white rounded disabled:bg-gray-300"
            >
              다음
            </button>
          </div>
        </div>
      )}
    </>
  );
};
