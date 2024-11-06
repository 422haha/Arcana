import {
  keepPreviousData,
  useQuery,
  UseQueryResult,
} from '@tanstack/react-query';
import { recipeApi } from '@/entities/recipe/model/recipeApi';
import { Recipe } from '@/shared/api/recipeTypes';
import { isArray } from 'underscore';
import { queryClient } from '@/shared/query/query-client';

export const useSearchSuggestion = (
  keyword: string
): UseQueryResult<Recipe[] | undefined, Error> => {
  // empty data(empty arr) will be deleted from cache immediately
  const selectValidData = (data: Recipe[], keyword: string) => {
    if (isArray(data) && data.length > 0) return data;
    else {
      queryClient.removeQueries({
        queryKey: ['recipe-suggestions', keyword],
      });
      return undefined;
    }
  };

  return useQuery<Recipe[], Error, Recipe[] | undefined, string[]>({
    queryKey: ['recipe-suggestions', keyword],
    queryFn: () => recipeApi.getSearchSuggestions(keyword),
    enabled: keyword.length > 0,
    select: (data) => selectValidData(data, keyword),
    placeholderData: keepPreviousData,
  });
};
