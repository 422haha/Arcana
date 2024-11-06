import {
  IngredientRecommendation,
  RecipeRecommendation,
} from '@/shared/api/tasteSuggestTypes';

export default class TasteSuggestApi {
  private client: any;

  constructor(client: any) {
    this.client = client;
  }

  async getRecommendedIngredients(
    ingredients: number[]
  ): Promise<IngredientRecommendation[]> {
    const response = await this.client.getRecommendedIngredients(ingredients);
    return response.data.data;
  }

  async getRecommendedRecipes(
    ingredients: number[]
  ): Promise<RecipeRecommendation[]> {
    const response = await this.client.getRecommendedRecipes(ingredients);
    return response.data.data;
  }
}
