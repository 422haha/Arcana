import axios from 'axios';
import {
  IngredientResponse,
  IngredientSiseRequest,
  IngredientInfo,
} from './ingredientTypes';

// 개발 환경에서 사용할 가짜 API 클라이언트 클래스
export default class FakeIngredientClient {
  private baseUrl: string;

  constructor() {
    this.baseUrl = import.meta.env.VITE_APP_DEV_URL;
  }

  // 가짜 재료 목록을 가져오는 메서드
  async getIngredients(params: { page?: number; perPage?: number }) {
    return axios.get<IngredientResponse>(
      `${this.baseUrl}public/data/ingredient-search.json`,
      { params }
    );
  }

  async getIngredientSise(params: IngredientSiseRequest) {
    const response = await axios.get<any>(
      `${this.baseUrl}public/data/ingredient-sise.json`
    );
    const { period, change, count } = params;
    const filteredData = response.data[period][change].slice(0, count);
    return { data: { data: filteredData } };
  }

  async getIngredientInfo(ingredientId: number) {
    const response = await axios.get<IngredientResponse>(
      `${this.baseUrl}public/data/ingredient-search.json`
    );

    const ingredient = response.data.data.find(
      (item) => item.ingredientId === ingredientId
    );

    if (!ingredient) {
      throw new Error('Ingredient not found');
    }

    // IngredientInfo 형식에 맞게 데이터 변환
    const ingredientInfo: IngredientInfo = {
      ingredientId: ingredient.ingredientId,
      name: ingredient.name,
      retailUnit: ingredient.retailUnit,
      retailUnitsize: ingredient.retailUnitsize,
      image: ingredient.image,
      price: ingredient.price,
      changeRate: ingredient.changeRate,
      changePrice: ingredient.changePrice,
    };

    return { data: { data: ingredientInfo } };
  }

  async getIngredientAutoComplete(search: string) {
    const response = await axios.get(
      `${this.baseUrl}public/data/ingredient-search.json`
    );
    return {
      data: {
        data: response.data.data
          .filter((item: any) =>
            item.name.toLowerCase().includes(search.toLowerCase())
          )
          .slice(0, 5),
      },
    };
  }

  async searchIngredients(params: {
    category: string;
    page: number;
    perPage: number;
    keyword: string | null;
  }) {
    const response = await axios.get(
      `${this.baseUrl}public/data/ingredient-search.json`
    );
    let filteredData = response.data.data;

    if (params.category !== 'all') {
      filteredData = filteredData.filter(
        (item: any) => item.category === params.category
      );
    }

    if (params.keyword) {
      const keyword = params.keyword.toLowerCase();
      filteredData = filteredData.filter((item: any) =>
        item.name.toLowerCase().includes(keyword)
      );
    }

    const startIndex = (params.page - 1) * params.perPage;
    const endIndex = startIndex + params.perPage;
    const paginatedData = filteredData.slice(startIndex, endIndex);

    return {
      data: {
        data: paginatedData,
        pagination: {
          total: filteredData.length,
          page: params.page,
          perPage: params.perPage,
        },
      },
    };
  }
}
