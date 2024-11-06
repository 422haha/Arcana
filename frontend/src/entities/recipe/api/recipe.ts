import FakeRecipeClient from '@/shared/api/fakeRecipeClient';
import RecipeClient from '@/shared/api/recipeClient';
import { PaginationInfo, Recipe, RecipeDetail } from '@/shared/api/recipeTypes';
import { sample, filter } from 'underscore';

export default class RecipeApi {
  private client: RecipeClient | FakeRecipeClient;

  constructor(client: RecipeClient | FakeRecipeClient) {
    this.client = client;
  }

  async getRecipes(
    page: number = 1,
    perPage: number = 10,
    keyword?: string
  ): Promise<{
    recipes: Recipe[];
    pagination: PaginationInfo;
  }> {
    const res = await this.client.getRecipes({ page, perPage, keyword });
    return {
      recipes: res.data.data,
      pagination: res.data.pagination,
    };
  }

  async getSearchSuggestions(keyword: string): Promise<Recipe[]> {
    const res = await this.client.getSearchSuggestions(keyword);
    //TODO: 아래는 Fake 만을 위한 로직으로, 실제 서비스를 위해 조건문이 필요하다.
    const randomSuggestions = sample(
      filter(res.data.data, (recipe) => recipe.name.includes(keyword)),
      5
    );

    return randomSuggestions;
  }

  async getRecipeDetail(recipeId: number): Promise<RecipeDetail> {
    const res = await this.client.getRecipeDetail(recipeId);
    return res.data.data;
  }
}
