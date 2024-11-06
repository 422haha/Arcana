import MainLayout from '../../app/RoutingLayout/MainLayout';
import RightSideLayout from '../../app/RoutingLayout/RightSideLayout';
import { useParams } from 'react-router-dom';
import { useRecipeDetail } from '@/features/recipeList/api/useRecipeDetail';
import { Error } from '@/shared/components';
import { createQRCode } from '@/shared/lib/qrcode';
import { useEffect, useState } from 'react';

export const RecipeDetailPage = () => {
  const { id = '' } = useParams<{ id: string }>();
  const { data: recipe, isLoading, error } = useRecipeDetail(id);
  const [qrCode, setQrCode] = useState<string>('');

  useEffect(() => {
    if (recipe) {
      const url = `https://mijung.store/recipes/${recipe.recipeId}`;
      const params = {
        Margin: 2,
        Background: '#ffffff',
        Finder: '#131d87',
        Horizontal: '#dc9c07',
        Vertical: '#d21313',
        Cross: '#131d87',
        'Horizontal thickness': 0.7,
        'Vertical thickness': 0.7,
        'Cross thickness': 0.7,
      };
      createQRCode(url, params).then(setQrCode);
    }
  }, [recipe]);

  if (error) return <Error />;
  if (isLoading) return <div>로딩 중...</div>;

  return (
    <div className="grid grid-cols-10">
      <MainLayout>
        {recipe && (
          <div className="container mx-auto px-4 py-8 relative">
            <div className="flex justify-between items-start mb-6">
              <h1 className="text-3xl font-bold">{recipe.name}</h1>
              <div className="bg-white shadow-md rounded-lg p-2 w-24 h-24 flex-shrink-0 overflow-hidden">
                <div
                  className="w-full h-full flex items-center justify-center"
                  dangerouslySetInnerHTML={{ __html: qrCode }}
                  style={{
                    ['& svg' as string]: {
                      width: '100%',
                      height: '100%',
                      objectFit: 'contain',
                    },
                  }}
                />
              </div>
            </div>
            <div className="bg-white shadow-md rounded-lg p-6">
              <h2 className="text-2xl font-semibold mb-4">재료</h2>
              <ul className="list-disc list-inside text-gray-700 mb-6">
                {recipe.materials?.map((material, index) => (
                  <li key={index}>{material.name}</li>
                ))}
              </ul>

              <h2 className="text-2xl font-semibold mb-4">조리 방법</h2>
              <ol className="list-decimal list-inside text-gray-700 mb-6">
                {recipe.steps?.map((step, index) => (
                  <li key={index}>{step.content}</li>
                ))}
              </ol>

              <div className="flex justify-between text-gray-600">
                <p>조리 시간: {recipe.time}</p>
                <p>난이도: {recipe.level}</p>
              </div>
            </div>
          </div>
        )}
      </MainLayout>
      <RightSideLayout />
    </div>
  );
};
