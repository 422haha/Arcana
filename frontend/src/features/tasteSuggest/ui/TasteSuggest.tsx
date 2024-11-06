import { useState } from 'react';
import { MyIngredients } from './MyIngredients';
import { RecommendedIngredients } from './RecommendedIngredients';
import { RecommendedRecipes } from './RecommendedRecipes';
import {
  useIngredientRecommendations,
  useRecipeRecommendations,
} from '../api/useTasteSuggest';
import { useIsMobile } from '@/shared/hooks/useIsMobile';
import { useMyIngredientsStore } from '@/shared/stores/myIngredientsStore';
import Searchbar from '@/widgets/Searchbar/Searchbar';

interface TasteSuggestProps {
  isOpen: boolean;
  onClose: () => void;
}

export const TasteSuggest = ({ isOpen, onClose }: TasteSuggestProps) => {
  const [activeTab, setActiveTab] = useState('ingredients');
  const isMobile = useIsMobile();

  const { ingredients, addIngredient, removeIngredient, clearIngredients } =
    useMyIngredientsStore();

  const { data: recommendedIngredients } = useIngredientRecommendations(
    ingredients.map((i) => i.id)
  );
  const { data: recommendedRecipes } = useRecipeRecommendations(
    ingredients.map((i) => i.id)
  );

  const handleSuggestItemClick = (item: { id: number; name: string }) => {
    addIngredient(item.id, item.name);
  };

  const content = (
    <div className="bg-white rounded-2xl overflow-hidden transition-shadow duration-500 h-full">
      <div className="p-6 flex flex-col h-full">
        <h2 className="text-2xl font-bold mb-4">나만의 요리 도우미</h2>
        <p className="text-gray-600 mb-4">가지고 있는 재료나 사고 싶은 재료를 추가해보세요</p>
        <Searchbar
          type="ingredients"
          onSearch={() => {}}
          isSuggestSearch={true}
          onSuggestItemClick={handleSuggestItemClick}
        />
        <MyIngredients
          ingredients={ingredients}
          onRemove={removeIngredient}
          onClear={clearIngredients}
        />
        <div className="flex mt-4">
          <button
            className={`flex-1 py-2 rounded-full ${
              activeTab === 'ingredients'
                ? 'bg-blue-500 text-white'
                : 'bg-gray-200 text-gray-600'
            } transition-colors duration-300`}
            onClick={() => setActiveTab('ingredients')}
          >
            어울리는 재료
          </button>
          <button
            className={`flex-1 py-2 rounded-full ml-2 ${
              activeTab === 'recipes'
                ? 'bg-blue-500 text-white'
                : 'bg-gray-200 text-gray-600'
            } transition-colors duration-300`}
            onClick={() => setActiveTab('recipes')}
          >
            추천 레시피
          </button>
        </div>
        {activeTab === 'ingredients' && recommendedIngredients && (
          <RecommendedIngredients
            ingredients={recommendedIngredients}
            onAdd={addIngredient}
          />
        )}
        {activeTab === 'recipes' && recommendedRecipes && (
          <RecommendedRecipes recipes={recommendedRecipes} />
        )}
      </div>
    </div>
  );

  if (isMobile) {
    return (
      <>
        {isOpen && (
          <div className="fixed inset-0 bg-white z-40 overflow-auto pt-16 pb-20">
            <button
              onClick={onClose}
              className="absolute top-4 left-4 text-3xl text-gray-600 hover:text-gray-800"
            >
              &times;
            </button>
            {content}
          </div>
        )}
      </>
    );
  }

  return (
    <div className="fixed top-16 right-0 w-full lg:w-1/5 bg-white shadow-lg overflow-y-auto h-[calc(100vh-4rem)]">
      {content}
    </div>
  );
};