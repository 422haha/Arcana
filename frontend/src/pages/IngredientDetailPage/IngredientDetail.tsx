import { useParams, useNavigate } from 'react-router-dom';
import MainLayout from '../../app/RoutingLayout/MainLayout';
import RightSideLayout from '../../app/RoutingLayout/RightSideLayout';
import Searchbar from '../../widgets/Searchbar/Searchbar';
import IngredientCard from '../../widgets/IngredientCard/IngredientCard';
import PriceGraphCard from '../../widgets/PriceGraphCard/PriceGraphCard';
import NetworkGraphCard from '../../widgets/NetworkGraphCard/NetworkGraphCard';
import { useIngredientInfo } from '../../features/ingredient/api/useIngredients';

const IngredientDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: ingredient, isLoading, error } = useIngredientInfo(Number(id));

  const handleSearch = (keyword: string) => {
    navigate(`/ingredients?keyword=${encodeURIComponent(keyword)}`);
  };

  const handleItemSelect = (item: { id: number; name: string }) => {
    navigate(`/ingredients/${item.id}`);
  };

  if (isLoading) return <div>로딩 중...</div>;
  if (error) return <div>에러가 발생했습니다.</div>;
  if (!ingredient) return <div>식재료 정보가 없습니다.</div>;

  return (
    <div className="grid grid-cols-10">
      <MainLayout>
        <Searchbar
          type="ingredients"
          onSearch={handleSearch}
          onItemSelect={handleItemSelect}
        />
        <div className="container mx-auto px-4 py-8">
          <h1 className="text-3xl font-bold mb-6">
            {ingredient.name} 상세 정보
          </h1>
          <div className="flex flex-col lg:flex-row gap-8">
            <div className="lg:w-1/4">
              <IngredientCard ingredient={ingredient} />
            </div>
            <div className="lg:w-3/4 flex flex-col gap-8">
              <PriceGraphCard
                graphId={ingredient.ingredientId ?? 0}
                title={`${ingredient.name ?? '알 수 없음'} 가격 추이`}
                width={500}
                height={200}
              />
              <NetworkGraphCard
                graphId={ingredient.ingredientId ?? 0}
                // title={`${ingredient.name ?? '알 수 없음'}`}
                width={700}
                height={700}
              />
            </div>
          </div>
          <div className="mt-8 bg-white shadow-md rounded-lg p-6">
            <h2 className="text-2xl font-semibold mb-4">추가 정보</h2>
            {/* 추가 정보가 있다면 여기에 표시할 수 있습니다 */}
          </div>
        </div>
      </MainLayout>
      <RightSideLayout />
    </div>
  );
};

export default IngredientDetailPage;
