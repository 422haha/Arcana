export interface IngredientRecommendation {
  ingredientId: number;
  name: string;
}

export interface RecipeRecommendation {
  recipeId: number;
  name: string;
  kind: string;
  image: string;
}
