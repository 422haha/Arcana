// ingredient 데이터 타입 정의

export interface Ingredient {
  ingredientId: number;
  name: string;
  retailUnit: string;
  retailUnitsize: string;
  image: string;
  price: string;
  changeRate: number;
  changePrice: number;
}

export interface PaginationInfo {
  total: number;
  page: number;
  perPage: number;
}

export interface IngredientResponse {
  data: Ingredient[];
  pagination: PaginationInfo;
}

export interface IngredientSiseRequest {
  period: 'year' | 'month' | 'week';
  change: 'positive' | 'negative';
  count: number;
}

export interface IngredientSise {
  ingredientId: number;
  name: string;
  retailUnit: string;
  retailUnitsize: string;
  image: string;
  price: string;
  changeRate: number;
  changePrice: number;
}

export interface IngredientSiseResponse {
  data: IngredientSise[];
}

export interface IngredientInfo {
  ingredientId: number;
  name: string;
  retailUnit: string;
  retailUnitsize: string;
  image: string;
  price: string;
  changeRate: number;
  changePrice: number;
}

export interface IngredientCosineResponse {
  ingredientId: number;
  itemName: string;
  cosine: number; // 코사인 유사도
}
