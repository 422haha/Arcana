import RecipeApi from '../api/recipe';
import FakeRecipeClient from '@/shared/api/fakeRecipeClient';
import RecipeClient from '@/shared/api/recipeClient';

// localhost이고, Chrome을 쓰지 않을 때만 fakeAPI로 테스트가능
const client = (() => {
  if (window.location.hostname === 'localhost') {
    if (
      navigator.userAgent.includes('Chrome') &&
      !navigator.userAgent.includes('Edg')
    ) {
      return new RecipeClient();
    }
    return new FakeRecipeClient();
  }
  return new RecipeClient();
})();

export const recipeApi = new RecipeApi(client);
