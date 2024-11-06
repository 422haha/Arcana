import { useNavigate, useLocation } from 'react-router-dom';
import { FaSearch, FaBook, FaHeart, FaBars, FaHome } from 'react-icons/fa';

interface MobileNavbarProps {
  onToggleTasteSuggest: () => void;
  isTasteSuggestOpen: boolean;
}

const MobileNavbar = ({
  onToggleTasteSuggest,
  isTasteSuggestOpen,
}: MobileNavbarProps) => {
  const navigate = useNavigate();
  const location = useLocation();

  const handleNavigation = (path: string) => {
    navigate(path);
    if (isTasteSuggestOpen) {
      onToggleTasteSuggest();
    }
  };

  const isActive = (path: string) =>
    !isTasteSuggestOpen && location.pathname === path;

  const getButtonClass = (path: string) => `
    flex flex-col items-center
    ${isActive(path) ? 'text-blue-500' : 'text-gray-500'}
  `;

  return (
    <nav
      className={`bg-white border-t border-gray-200 fixed bottom-0 left-0 right-0 z-50 ${
        isTasteSuggestOpen ? 'bg-opacity-90' : ''
      }`}
    >
      <div className="flex justify-around items-center h-16">
        <button
          onClick={() => handleNavigation('/ingredients')}
          className={getButtonClass('/ingredients')}
        >
          <FaSearch className="text-2xl" />
          <span className="text-xs mt-1">식재료</span>
        </button>
        <button
          onClick={() => handleNavigation('/recipes')}
          className={getButtonClass('/recipes')}
        >
          <FaBook className="text-2xl" />
          <span className="text-xs mt-1">레시피</span>
        </button>
        <button
          onClick={() => handleNavigation('/')}
          className={getButtonClass('/')}
        >
          <FaHome className="text-2xl" />
          <span className="text-xs mt-1">홈</span>
        </button>
        <button
          onClick={() => handleNavigation('/recipes/jjim')}
          className={getButtonClass('/recipes/jjim')}
        >
          <FaHeart className="text-2xl" />
          <span className="text-xs mt-1">찜한 레시피</span>
        </button>
        <button
          onClick={onToggleTasteSuggest}
          className={`flex flex-col items-center ${
            isTasteSuggestOpen ? 'text-blue-500' : 'text-gray-500'
          }`}
        >
          <FaBars className="text-2xl" />
          <span className="text-xs mt-1">추천</span>
        </button>
      </div>
    </nav>
  );
};

export default MobileNavbar;
