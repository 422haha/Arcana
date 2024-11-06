import { Link } from 'react-router-dom';
import { RecipeRecommendation } from '@/shared/api/tasteSuggestTypes';

interface RecommendedRecipesProps {
  recipes: RecipeRecommendation[] | undefined;
}

export const RecommendedRecipes = ({ recipes }: RecommendedRecipesProps) => {
  if (!recipes || recipes.length === 0) {
    return <div>추천 레시피가 없습니다.</div>;
  }

  return (
    <div className="mt-6">
      <h2 className="text-xl font-bold mb-3 uppercase">추천 레시피</h2>
      <ul className="space-y-4">
        {recipes.map((recipe) => (
          <li
            key={recipe.recipeId}
            className="bg-gray-100 rounded overflow-hidden"
          >
            <Link
              to={`/recipes/${recipe.recipeId}`}
              className="flex items-center p-2 hover:bg-gray-200 transition-colors duration-300"
            >
              <img
                src={recipe.image || 'default-recipe-image.jpg'}
                alt={recipe.name}
                className="w-16 h-16 object-cover mr-4 rounded"
              />
              <div>
                <div className="font-bold text-gray-800">{recipe.name}</div>
                <div className="text-sm text-coral">{recipe.kind}</div>
              </div>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
};
