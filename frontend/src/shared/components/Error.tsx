import { useRouteError } from 'react-router-dom';
import { FaExclamationTriangle, FaRedo } from 'react-icons/fa';

export const Error = () => {
  const error = useRouteError() as any;

  const handleReload = () => {
    window.location.reload();
  };

  return (
    <div className="flex flex-col justify-center items-center min-h-screen bg-gray-50 px-4 py-10 font-sans text-gray-800">
      <h1 className="text-4xl font-bold text-blue-600 mb-6">미정<span className="text-2xl text-gray-600">味定</span></h1>
      <FaExclamationTriangle className="text-6xl text-yellow-500 mb-6" />
      <h2 className="text-3xl font-bold mb-4">오류가 발생했습니다</h2>
      <p className="text-xl text-center mb-8 max-w-2xl">
        {error?.message || '알 수 없는 오류가 발생했습니다. 다시 시도해 주세요.'}
      </p>
      <button 
        className="flex items-center bg-blue-500 text-white font-bold py-3 px-6 rounded-lg hover:bg-blue-600 transition duration-300"
        onClick={handleReload}
      >
        <FaRedo className="mr-2" />
        다시 시도
      </button>
      {error?.stack && (
        <div className="w-full max-w-4xl mt-12 bg-white rounded-lg shadow-md p-6">
          <h3 className="text-2xl font-bold mb-4">개발자를 위한 오류 정보:</h3>
          <div className="mb-6">
            <p><strong>오류 유형:</strong> {error.name}</p>
            <p><strong>오류 메시지:</strong> {error.message}</p>
            {error.code && <p><strong>오류 코드:</strong> {error.code}</p>}
            {error.status && <p><strong>HTTP 상태:</strong> {error.status}</p>}
          </div>
          <div className="bg-gray-100 p-4 rounded-lg">
            <h4 className="text-lg font-semibold mb-2">스택 트레이스:</h4>
            <pre className="text-sm text-gray-700 whitespace-pre-wrap break-words">
              {error.stack}
            </pre>
          </div>
        </div>
      )}
    </div>
  );
};
