import IngredientCard from '../../../widgets/IngredientCard/IngredientCard';
import { IngredientSise } from '../../../shared/api/ingredientTypes';

interface IngredientSiseListProps {
  ingredients: IngredientSise[];
  title: string;
}

export const IngredientSiseList = ({
  ingredients,
  title,
}: IngredientSiseListProps) => {
  console.log(`${title} 데이터:`, ingredients);

  if (ingredients.length === 0) {
    console.log(`${title}: 데이터 없음`);
    return (
      <p className="text-center text-gray-500">
        현재 표시할 데이터가 없습니다.
      </p>
    );
  }

  return (
    <section className="mb-8">
      <h2 className="text-2xl font-semibold mb-4">{title}</h2>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {ingredients.map((ingredient) => (
          <IngredientCard
            key={ingredient.ingredientId}
            ingredient={ingredient}
          />
        ))}
      </div>
    </section>
  );
};
