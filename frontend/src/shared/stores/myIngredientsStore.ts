import create from 'zustand';
import { persist } from 'zustand/middleware';

interface Ingredient {
  id: number;
  name: string;
}

interface MyIngredientsState {
  ingredients: Ingredient[];
  addIngredient: (id: number, name: string) => void;
  removeIngredient: (id: number) => void;
  clearIngredients: () => void;
}

export const useMyIngredientsStore = create<MyIngredientsState>()(
  persist(
    (set) => ({
      ingredients: [],
      addIngredient: (id: number, name: string) =>
        set((state) => ({
          ingredients: state.ingredients.some((i) => i.id === id)
            ? state.ingredients
            : [...state.ingredients, { id, name }],
        })),
      removeIngredient: (id: number) =>
        set((state) => ({
          ingredients: state.ingredients.filter((i) => i.id !== id),
        })),
      clearIngredients: () => set({ ingredients: [] }),
    }),
    {
      name: 'my-ingredients-storage',
    }
  )
);
