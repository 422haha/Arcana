import { useState, useCallback, useRef, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Recipe } from '@/shared/api/recipeTypes';
import { defaultRecipeImg } from '@/shared/url/defualtImage';
import { useNavigate } from 'react-router-dom';

interface RecipeCardProps {
  recipe: Recipe;
  position: { x: number; y: number; rotate: number };
  isActive: boolean;
  onClick: () => void;
}

const RecipeCard = ({
  recipe,
  position,
  isActive,
  onClick,
}: RecipeCardProps) => {
  const navigate = useNavigate();
  return (
    <motion.div
      className={`
      absolute w-40 h-56 bg-white rounded-lg shadow-2xl overflow-hidden cursor-pointer
      ${isActive ? 'z-50' : 'z-10'}`}
      initial={position}
      animate={
        isActive
          ? {
              x: 0,
              y: -100,
              rotate: 0,
              scale: 1.2,
              transition: { type: 'spring', stiffness: 300, damping: 20 },
            }
          : {
              ...position,
              scale: 1,
              transition: { type: 'spring', stiffness: 300, damping: 20 },
            }
      }
      onClick={(e) => {
        e.stopPropagation();
        if (isActive) {
          navigate(`/recipes/${recipe.recipeId}`);
        } else {
          onClick();
        }
      }}
    >
      <img
        src={recipe.image || defaultRecipeImg}
        alt={recipe.name}
        className="w-full h-2/3 object-cover"
      />
      <div className="p-2">
        <h3 className="text-sm font-semibold">{recipe.name}</h3>
        <p className="text-xs text-gray-600">{recipe.kind}</p>
      </div>
    </motion.div>
  );
};

interface Props {
  recipes: Recipe[];
}

export const RecipeCardStack = (props: Props) => {
  const { recipes } = props;
  const [activeIndex, setActiveIndex] = useState<number | null>(null);
  const [containerWidth, setContainerWidth] = useState<number>(0);
  const containerRef = useRef<HTMLDivElement>(null);
  const totalCards = recipes.length;
  const fanAngle = 60; // Total angle of the fan shape

  // 화면 넓이 추적
  useEffect(() => {
    const updateWidth = () => {
      if (containerRef.current) {
        setContainerWidth(containerRef.current.offsetWidth);
      }
    };

    updateWidth();
    window.addEventListener('resize', updateWidth);
    return () => window.removeEventListener('resize', updateWidth);
  }, []);

  const handleContainerClick = useCallback(() => {
    setActiveIndex(null);
  }, []);

  const calculateCardPosition = (index: number) => {
    const angle =
      (index - (totalCards - 1) / 2) * (fanAngle / (totalCards - 1));
    const radian = (angle * Math.PI) / 180;
    // Adjust this factor to change the overall spread
    const radius = containerWidth * 0.6;
    const x = radius * Math.sin(radian);
    const y = radius * (1 - Math.cos(radian));
    return { x, y, rotate: angle };
  };

  return (
    <div
      ref={containerRef}
      className="relative w-full h-full"
      onClick={handleContainerClick}
    >
      <div className="absolute top-40 left-1/2 transform -translate-x-20">
        {recipes.map((recipe, index) => (
          <RecipeCard
            key={recipe.recipeId}
            recipe={recipe}
            position={calculateCardPosition(index)}
            isActive={activeIndex === index}
            onClick={() =>
              setActiveIndex((prevIndex) =>
                prevIndex === index ? null : index
              )
            }
          />
        ))}
      </div>
    </div>
  );
};
