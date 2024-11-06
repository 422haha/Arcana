import axios from 'axios';
import FakeIngredientClient from '../../../shared/api/fakeIngredientClient';
import IngredientClient from '../../../shared/api/ingredientClient';
import {
  PaginationInfo,
  Ingredient,
} from '../../../shared/api/ingredientTypes';
import {
  IngredientSiseRequest,
  IngredientSise,
} from '../../../shared/api/ingredientTypes';
import { IngredientInfo } from '../../../shared/api/ingredientTypes';

// 재료 API를 추상화한 클래스
export default class IngredientApi {
  private client: IngredientClient | FakeIngredientClient;

  constructor(client: IngredientClient | FakeIngredientClient) {
    this.client = client;
  }

  // 재료 목록을 가져오는 메서드
  async getIngredients(
    page: number = 1,
    perPage: number = 10,
    category: string = 'all'
  ): Promise<{
    ingredients: Ingredient[];
    pagination: PaginationInfo;
  }> {
    const res = await this.client.getIngredients({ page, perPage, category });
    return {
      ingredients: res.data.data,
      pagination: res.data.pagination,
    };
  }

  // 메인페이지 재료 가격 추이 데이터 가져오는 메서드
  async getIngredientSise(
    params: IngredientSiseRequest
  ): Promise<IngredientSise[]> {
    try {
      console.log('API 호출 파라미터:', params);
      const response = await this.client.getIngredientSise(params);
      console.log('API 응답:', response);
      if (response instanceof Response && response.status === 204) {
        console.log('데이터 없음 (204 상태 코드)');
        return [];
      }
      return response.data.data;
    } catch (error) {
      console.error('API 호출 오류:', error);
      if (axios.isAxiosError(error)) {
        if (error.response?.status === 400) {
          throw new Error('잘못된 요청입니다.');
        } else if (error.response?.status === 500) {
          throw new Error('서버 오류가 발생했습니다.');
        }
      }
      throw error;
    }
  }

  async getIngredientInfo(ingredientId: number): Promise<IngredientInfo> {
    try {
      const response = await this.client.getIngredientInfo(ingredientId);
      return response.data.data;
    } catch (error) {
      console.error('식재료 정보 조회 API 오류:', error);
      throw error;
    }
  }

  async getIngredientAutoComplete(search: string) {
    try {
      const response = await this.client.getIngredientAutoComplete(search);
      return response.data.data;
    } catch (error) {
      console.error('식재료 자동완성 API 오류:', error);
      return [];
    }
  }

  async searchIngredients(params: {
    category: string;
    page: number;
    perPage: number;
    keyword: string | null;
  }) {
    try {
      const response = await this.client.searchIngredients(params);
      return {
        ingredients: response.data.data,
        pagination: response.data.pagination,
      };
    } catch (error) {
      console.error('식재료 검색 API 오류:', error);
      throw error;
    }
  }
}
