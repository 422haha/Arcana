import TasteSuggestApi from '../api/tasteSuggest';
import FakeTasteSuggestClient from '@/shared/api/fakeTasteSuggestClient';
import TasteSuggestClient from '@/shared/api/tasteSuggestClient';

// localhost이고, Chrome을 쓰지 않을 때만 fakeAPI로 테스트가능
const client = (() => {
  if (window.location.hostname === 'localhost') {
    if (
      navigator.userAgent.includes('Chrome') &&
      !navigator.userAgent.includes('Edg')
    ) {
      return new TasteSuggestClient();
    }
    return new FakeTasteSuggestClient();
  }
  return new TasteSuggestClient();
})();

export const tasteSuggestApi = new TasteSuggestApi(client);
