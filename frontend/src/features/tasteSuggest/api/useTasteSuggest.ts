// React Query와 관련 API, 타입을 가져옵니다.
import { useQuery } from '@tanstack/react-query';
import { tasteSuggestApi } from '../../../entities/tasteSuggest/model/tasteSuggestApi';
import {
  IngredientRecommendation,
  RecipeRecommendation,
} from '../../../shared/api/tasteSuggestTypes';

// 재료 추천을 위한 커스텀 훅
export const useIngredientRecommendations = (ingredients: number[]) => {
  return useQuery<IngredientRecommendation[], Error>({
    queryKey: ['ingredientRecommendations', ingredients],
    queryFn: () => tasteSuggestApi.getRecommendedIngredients(ingredients),
    enabled: ingredients.length > 0,
  });
};

// 레시피 추천을 위한 커스텀 훅
export const useRecipeRecommendations = (ingredients: number[]) => {
  return useQuery<RecipeRecommendation[], Error>({
    queryKey: ['recipeRecommendations', ingredients],
    queryFn: () => tasteSuggestApi.getRecommendedRecipes(ingredients),
    enabled: ingredients.length > 0,
  });
};
