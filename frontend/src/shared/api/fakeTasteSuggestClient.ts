import axios from 'axios';

const baseURL = import.meta.env.VITE_APP_DEV_URL;

export default class FakeTasteSuggestClient {
  async getRecommendedIngredients(ingredients: number[]) {
    console.log('추천 재료 API 호출 중...');
    console.log('요청:', { ingredients });
    console.log(
      'API 엔드포인트:',
      `${baseURL}public/data/recommend-ingredient.json`
    );
    const response = await axios.get(
      `${baseURL}public/data/recommend-ingredient.json`
    );
    console.log('응답:', response.data);
    console.log('추천 재료 API 호출 완료');
    return { data: response.data.data.slice(0, 5) };
  }

  async getRecommendedRecipes(ingredients: number[]) {
    console.log('추천 레시피 API 호출 중...');
    console.log('요청:', { ingredients });
    console.log(
      'API 엔드포인트:',
      `${baseURL}public/data/recommend-recipes.json`
    );
    const response = await axios.get(
      `${baseURL}public/data/recommend-recipes.json`
    );
    console.log('응답:', response.data);
    console.log('추천 레시피 API 호출 완료');
    return { data: response.data.data.slice(0, 5) };
  }
}
