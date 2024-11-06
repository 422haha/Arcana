import { useEffect } from 'react';
import { router } from './AppRouter';
import { Providers } from './providers';
import { queryClient } from '../shared/query/query-client';

const App = () => {
  useEffect(() => {
    // 'dark' 클래스 제거 및 'light' 클래스 추가
    document.documentElement.classList.remove('dark');
    document.documentElement.classList.add('light');
    // 명시적으로 배경색과 텍스트 색상 설정
    document.body.style.backgroundColor = 'white';
    document.body.style.color = 'black';
  }, []);

  return (
    <div className="bg-white text-black">
      <Providers router={router} client={queryClient} />
    </div>
  );
};

export default App;
