interface MyIngredientsProps {
  ingredients: Array<{ id: number; name: string }>;
  onRemove: (id: number) => void;
  onClear: () => void;
}

export const MyIngredients = ({
  ingredients,
  onRemove,
  onClear,
}: MyIngredientsProps) => {
  return (
    <div className="mb-6">
      <h2 className="text-xl font-bold mb-3 uppercase">나의 식재료</h2>
      <ul className="space-y-2">
        {ingredients.map((ingredient) => (
          <li
            key={ingredient.id}
            className="flex justify-between items-center bg-gray-100 p-2 rounded"
          >
            <span className="text-gray-800">{ingredient.name}</span>
            <button
              onClick={() => onRemove(ingredient.id)}
              className="text-red-500 hover:text-red-700 transition-colors duration-300"
            >
              &times;
            </button>
          </li>
        ))}
      </ul>
      {ingredients.length > 0 && (
        <button
          onClick={onClear}
          className="mt-3 text-sm text-gray-500 hover:text-gray-700 transition-colors duration-300"
        >
          전체 삭제
        </button>
      )}
    </div>
  );
};
