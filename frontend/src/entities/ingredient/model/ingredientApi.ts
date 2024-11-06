import IngredientApi from '../api/ingredient';
import FakeIngredientClient from '../../../shared/api/fakeIngredientClient';
import IngredientClient from '../../../shared/api/ingredientClient';

// localhost이고, Chrome을 쓰지 않을 때만 fakeAPI로 테스트가능
const client = (() => {
  if (window.location.hostname === 'localhost') {
    if (
      navigator.userAgent.includes('Chrome') &&
      !navigator.userAgent.includes('Edg')
    ) {
      return new IngredientClient();
    }
    return new FakeIngredientClient();
  }
  return new IngredientClient();
})();

export const ingredientApi = new IngredientApi(client);
