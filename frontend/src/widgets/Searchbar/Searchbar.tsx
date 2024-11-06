import { useState, useEffect, useRef, KeyboardEvent } from 'react';
import { useIngredientAutoComplete } from '../../features/ingredient/api/useIngredients';

interface SearchbarProps {
  type: 'ingredients' | 'recipes';
  onSearch: (keyword: string) => void;
  isSuggestSearch?: boolean;
  onSuggestItemClick?: (item: { id: number; name: string }) => void;
  onItemSelect?: (item: { id: number; name: string }) => void;
}

const Searchbar = ({
  type,
  onSearch,
  isSuggestSearch = false,
  onSuggestItemClick,
  onItemSelect,
}: SearchbarProps) => {
  const [keyword, setKeyword] = useState('');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [selectedIndex, setSelectedIndex] = useState(-1);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const { data: ingredientSuggestions } = useIngredientAutoComplete(
    type === 'ingredients' ? keyword : ''
  );

  const suggestions =
    type === 'ingredients'
      ? ingredientSuggestions
      : console.log('recipeSuggestions');

  useEffect(() => {
    if (keyword.length > 0) {
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
      timeoutRef.current = setTimeout(() => {
        setIsDropdownOpen(true);
      }, 300);
    } else {
      setIsDropdownOpen(false);
    }
    setSelectedIndex(-1);

    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
    };
  }, [keyword]);

  const handleSearch = () => {
    if (isSuggestSearch && suggestions && suggestions.length > 0) {
      const firstSuggestion = suggestions[0];
      onSuggestItemClick?.({
        id: firstSuggestion.ingredientId,
        name: firstSuggestion.name,
      });
    } else {
      onSearch(keyword);
    }
    setIsDropdownOpen(false);
    setKeyword('');
  };

  const handleItemClick = (item: { id: number; name: string }) => {
    if (isSuggestSearch && onSuggestItemClick) {
      onSuggestItemClick(item);
    } else if (onItemSelect) {
      onItemSelect(item);
    } else {
      setKeyword(item.name);
      onSearch(item.name);
    }
    setIsDropdownOpen(false);
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (!suggestions) return;

    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        setSelectedIndex((prevIndex) =>
          prevIndex < suggestions.length - 1 ? prevIndex + 1 : prevIndex
        );
        break;
      case 'ArrowUp':
        e.preventDefault();
        setSelectedIndex((prevIndex) => (prevIndex > 0 ? prevIndex - 1 : -1));
        break;
      case 'Enter':
        e.preventDefault();
        if (selectedIndex >= 0 && selectedIndex < suggestions.length) {
          const selectedItem = suggestions[selectedIndex];
          handleItemClick({
            id: selectedItem.ingredientId,
            name: selectedItem.name,
          });
        } else {
          handleSearch();
        }
        break;
      case 'Escape':
        setIsDropdownOpen(false);
        break;
    }
  };

  return (
    <div className="relative w-full max-w-xl">
      <input
        ref={inputRef}
        type="text"
        value={keyword}
        onChange={(e) => setKeyword(e.target.value)}
        onKeyDown={handleKeyDown}
        className="w-full px-4 py-2 border border-gray-300 rounded-full focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white text-gray-800"
        placeholder={type === 'ingredients' ? '식재료 검색' : '레시피 검색'}
      />
      {isDropdownOpen && suggestions && suggestions.length > 0 && (
        <ul className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-2xl shadow-lg">
          {suggestions.map(
            (item: { ingredientId: number; name: string }, index: number) => (
              <li
                key={item.ingredientId}
                className={`px-4 py-2 cursor-pointer ${
                  index === selectedIndex ? 'bg-blue-100' : 'hover:bg-gray-100'
                }`}
                onClick={() =>
                  handleItemClick({ id: item.ingredientId, name: item.name })
                }
              >
                {item.name}
              </li>
            )
          )}
        </ul>
      )}
    </div>
  );
};

export default Searchbar;
