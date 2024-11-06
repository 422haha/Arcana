import { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import MainLayout from '../../app/RoutingLayout/MainLayout';
import RightSideLayout from '../../app/RoutingLayout/RightSideLayout';
import Searchbar from '../../widgets/Searchbar/Searchbar';
import { IngredientList } from '@/features/ingredient/ui/IngredientList';
import { useIngredients } from '@/features/ingredient/api/useIngredients';

const categories = [
  { id: 'all', name: '전체' },
  { id: '100', name: '식량작물' },
  { id: '200', name: '채소류' },
  { id: '300', name: '특용작물' },
  { id: '400', name: '과일류' },
  { id: '500', name: '축산물' },
  { id: '600', name: '수산물' },
];

const IngredientListPage = () => {
  const navigate = useNavigate();
  const [category, setCategory] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [keyword, setKeyword] = useState<string | null>(null);
  const location = useLocation();

  useEffect(() => {
    const searchParams = new URLSearchParams(location.search);
    const keywordParam = searchParams.get('keyword');
    if (keywordParam) {
      setKeyword(keywordParam);
      setCategory('all');
    }
  }, [location.search]);

  const { data, isLoading, error } = useIngredients(
    currentPage,
    10,
    category,
    keyword
  );

  const handleCategoryChange = (newCategory: string) => {
    setCategory(newCategory);
    setCurrentPage(1);
    // 카테고리 변경 시 검색어 초기화
    setKeyword(null);
  };

  const handleSearch = (searchKeyword: string) => {
    setKeyword(searchKeyword);
    setCurrentPage(1);
    // 검색 시 카테고리를 'all'로 설정
    setCategory('all');
  };

  const handleItemSelect = (item: { id: number; name: string }) => {
    navigate(`/ingredients/${item.id}`);
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
  };

  return (
    <div className="grid grid-cols-10">
      <MainLayout>
        <div className="container mx-auto px-4 py-8">
          <h1 className="text-3xl font-bold mb-6">식재료 목록</h1>
          <Searchbar
            type="ingredients"
            onSearch={handleSearch}
            onItemSelect={handleItemSelect}
          />
          <div className="mt-6 mb-8">
            <h2 className="text-xl font-semibold mb-3">카테고리</h2>
            <div className="flex flex-wrap gap-2">
              {categories.map((cat) => (
                <button
                  key={cat.id}
                  className={`px-4 py-2 rounded-full transition-colors duration-300 ${
                    category === cat.id
                      ? 'bg-blue-500 text-white'
                      : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                  }`}
                  onClick={() => handleCategoryChange(cat.id)}
                >
                  {cat.name}
                </button>
              ))}
            </div>
          </div>
          {isLoading ? (
            <div className="flex justify-center items-center h-64">
              <p className="text-xl text-gray-600">로딩 중...</p>
            </div>
          ) : error ? (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative" role="alert">
              <strong className="font-bold">오류 발생!</strong>
              <span className="block sm:inline"> 데이터를 불러오는 데 실패했습니다. 다시 시도해 주세요.</span>
            </div>
          ) : (
            <IngredientList
              ingredients={data?.ingredients || []}
              pagination={data?.pagination}
              onPageChange={handlePageChange}
            />
          )}
        </div>
      </MainLayout>
      <RightSideLayout />
    </div>
  );
};

export default IngredientListPage;
