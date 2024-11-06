import { Link, useLocation } from 'react-router-dom';
import { useState } from 'react';
import { FaSearch, FaBook, FaHeart, FaBars } from 'react-icons/fa';

const Navbar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const location = useLocation();

  const isActive = (path: string) => location.pathname === path;

  const getLinkClass = (path: string) =>
    `py-4 px-2 ${
      isActive(path)
        ? 'text-blue-500 font-semibold'
        : 'text-gray-600 hover:text-blue-500 transition duration-300'
    }`;

  return (
    <nav className="bg-white shadow-sm fixed top-0 left-0 right-0 z-50">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex justify-between">
          <div className="flex space-x-7">
            <div>
              <Link to="/" className="flex items-center py-4 px-2">
                <img src="/icons/logo.svg" alt="로고" className="h-8 w-8" />
                <span className="font-semibold text-gray-500 text-lg ml-2">
                  미정(味定)
                </span>
              </Link>
            </div>
          </div>
          {/* 데스크톱 메뉴 */}
          <div className="hidden md:flex items-center space-x-1">
            <Link to="/ingredients" className={getLinkClass('/ingredients')}>
              <FaSearch className="inline-block mr-1" />
              식재료 찾기
            </Link>
            <Link to="/recipes" className={getLinkClass('/recipes')}>
              <FaBook className="inline-block mr-1" />
              레시피 찾기
            </Link>
            <Link to="/recipes/jjim" className={getLinkClass('/recipes/jjim')}>
              <FaHeart className="inline-block mr-1" />
              찜한 레시피
            </Link>
          </div>
          {/* 모바일 메뉴 버튼 */}
          <div className="md:hidden flex items-center">
            <button onClick={() => setIsOpen(!isOpen)} className="text-gray-500">
              <FaBars className="w-6 h-6" />
            </button>
          </div>
        </div>
      </div>
      {/* 모바일 메뉴 */}
      {isOpen && (
        <div className="md:hidden">
          <Link
            to="/ingredients"
            className="block py-2 px-4 text-sm text-gray-500 hover:bg-gray-100"
          >
            <FaSearch className="inline-block mr-2" />
            식재료 찾기
          </Link>
          <Link
            to="/recipes"
            className="block py-2 px-4 text-sm text-gray-500 hover:bg-gray-100"
          >
            <FaBook className="inline-block mr-2" />
            레시피 찾기
          </Link>
          <Link
            to="/recipes/jjim"
            className="block py-2 px-4 text-sm text-gray-500 hover:bg-gray-100"
          >
            <FaHeart className="inline-block mr-2" />
            찜한 레시피
          </Link>
        </div>
      )}
    </nav>
  );
};

export default Navbar;
