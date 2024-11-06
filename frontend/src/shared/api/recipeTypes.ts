export interface Recipe {
  recipeId: number;
  name: string;
  kind: string;
  image: string;
}

export interface PaginationInfo {
  total: number;
  page: number;
  perPage: number;
}

export interface RecipeListResponse {
  data: Recipe[];
  pagination: PaginationInfo;
}

export interface RecipeSearchResponse {
  data: Recipe[];
}

export interface RecipeDetail {
  recipeId: number;
  name: string;
  kind: string;
  image: string;
  inbun: string;
  level: string;
  time: string;
  materials: RecipeDetailMaterial[];
  etc: RecipeDetailEtc[];
  steps: RecipeDetailStep[];
}

export interface RecipeDetailMaterial {
  materialId: number;
  name: string;
  capacity: string;
  type: string;
  ingredientId: number;
}

export interface RecipeDetailEtc {
  etcId: number;
  name: string;
  capacity: string;
  type: string;
}

export interface RecipeDetailStep {
  stepId: number;
  content: string;
  image: string;
}

export interface RecipeDetailResponse {
  data: RecipeDetail;
}
