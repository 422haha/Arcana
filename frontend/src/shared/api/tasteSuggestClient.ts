import axios from 'axios';

const baseURL = import.meta.env.VITE_PROD_FAST_API_URL;

export default class TasteSuggestClient {
  async getRecommendedIngredients(ingredients: number[]) {
    console.log('추천 재료 API 호출 중...');
    console.log('요청:', { ingredients, count: 5 });
    const params = new URLSearchParams();
    ingredients.forEach((id) => params.append('ingredients', id.toString()));
    params.append('count', '5');
    const response = await axios.get(
      `${baseURL}/carts/recommends/ingredients?${params.toString()}`
    );
    console.log('응답:', response.data);
    console.log('추천 재료 API 호출 완료');
    return response;
  }

  async getRecommendedRecipes(ingredients: number[]) {
    console.log('추천 레시피 API 호출 중...');
    console.log('요청:', { ingredients, count: 5 });
    const params = new URLSearchParams();
    ingredients.forEach((id) => params.append('ingredients', id.toString()));
    params.append('count', '5');
    const response = await axios.get(
      `${baseURL}/carts/recommends/recipes?${params.toString()}`
    );
    console.log('응답:', response.data);
    console.log('추천 레시피 API 호출 완료');
    return response;
  }
}
