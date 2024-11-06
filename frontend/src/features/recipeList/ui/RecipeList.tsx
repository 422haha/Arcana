import { useRecipeList } from '../api/useRecipeList';
import { useEffect, useState } from 'react';
import { Error } from '@/shared/components';
import { RecipeCardStack } from '@/widgets/RecipeCard/RecipeCardStack';
import { RxReload } from 'react-icons/rx';
import { motion } from 'framer-motion';

interface Props {
  keyword: string;
}

export const RecipeList = ({ keyword }: Props) => {
  const [page, setPage] = useState<number>(1);
  const [rotation, setRotation] = useState<number>(0);
  const { data, isFetching, error } = useRecipeList({ page, keyword });

  // keyword 변경 시 페이지 1로 초기화
  useEffect(() => {
    setPage(1);
  }, [keyword]);

  if (error) return <Error />;

  const handleNextPage = () => {
    if (data?.pagination) {
      const lastPage = Math.ceil(
        data.pagination.total / data.pagination.perPage
      );
      setPage((prePage) => (prePage % lastPage) + 1);
      setRotation((rotation + 360) % 360000);
    }
  };

  return (
    <div className="relative w-full h-3/4">
      <motion.button
        onClick={handleNextPage}
        className="absolute z-10 top-24 end-6 text-4xl"
        animate={{ rotate: rotation }}
        transition={{ duration: 0.5, ease: 'easeInOut' }}
      >
        <RxReload />
      </motion.button>
      {data?.recipes.length === 0 ? (
        <div>데이터가 없습니다.</div>
      ) : (
        <>{data?.recipes && <RecipeCardStack recipes={data.recipes} />}</>
      )}
      {isFetching ? <span>Loading...</span> : null}
    </div>
  );
};
