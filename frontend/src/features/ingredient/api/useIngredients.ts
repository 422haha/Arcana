// 데이터 fetching을 위한 React Query 훅
import { useQuery } from '@tanstack/react-query';
import { ingredientApi } from '../../../entities/ingredient/model/ingredientApi';
import { IngredientSiseRequest } from '../../../shared/api/ingredientTypes';

export const useIngredients = (
  page: number = 1,
  perPage: number = 10,
  category: string = 'all',
  keyword: string | null = null
) => {
  return useQuery({
    queryKey: ['ingredients', page, perPage, category, keyword],
    queryFn: () =>
      ingredientApi.searchIngredients({ category, page, perPage, keyword }),
    enabled: !!category, // 카테고리가 존재할 때만 쿼리 실행
  });
};

// 메인페이지 재료 가격 추이 데이터 가져오는 메서드
export const useIngredientSise = (params: IngredientSiseRequest) => {
  return useQuery({
    queryKey: ['ingredientSise', params],
    queryFn: () => ingredientApi.getIngredientSise(params),
  });
};

export const useIngredientInfo = (ingredientId: number) => {
  return useQuery({
    queryKey: ['ingredientInfo', ingredientId],
    queryFn: () => ingredientApi.getIngredientInfo(ingredientId),
  });
};

export const useIngredientAutoComplete = (search: string) => {
  return useQuery({
    queryKey: ['ingredientAutoComplete', search],
    queryFn: () => ingredientApi.getIngredientAutoComplete(search),
    enabled: search.length > 0,
  });
};
