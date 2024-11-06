import { sample } from 'underscore';
import axios, { AxiosInstance } from 'axios';
import {
  RecipeDetailResponse,
  RecipeListResponse,
  RecipeSearchResponse,
} from './recipeTypes';

export default class FakeRecipeClient {
  private baseUrl: string;
  private axiosInstance: AxiosInstance;

  constructor() {
    this.baseUrl = import.meta.env.VITE_APP_DEV_URL;
    this.axiosInstance = axios.create({
      baseURL: this.baseUrl,
    });
  }

  async getRecipes(params: {
    page: number;
    perPage: number;
    keyword?: string;
  }) {
    return this.axiosInstance.get<RecipeListResponse>(
      'public/data/recipe-search.json',
      {
        params,
      }
    );
  }

  async getSearchSuggestions() {
    return this.axiosInstance.get<RecipeSearchResponse>(
      'public/data/recipe-search.json'
    );
  }

  async getRecipeDetail(recipeId: number) {
    const newRecipeId = sample([1, 2, 3, 4, 5, recipeId]);
    return this.axiosInstance.get<RecipeDetailResponse>(
      `public/data/recipe-detail-${newRecipeId}.json`
    );
  }
}
