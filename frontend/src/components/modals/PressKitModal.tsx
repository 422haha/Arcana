// src/pages/PressKitModal.tsx
interface PressKitModalProps {
  isOpen: boolean;
  onClose: () => void;
 }
 
 const PressKitModal: React.FC<PressKitModalProps> = ({ isOpen, onClose }) => {
  const categories = [
    { name: 'Logos', url: '/assets/press/logos.zip' },
    { name: 'Art & Visuals', url: '/assets/press/art.zip' },
    { name: 'Screenshots', url: '/assets/press/screenshots.zip' },
  ];
 
  return (
    <div
      className={`fixed inset-0 z-50 flex items-center justify-center transition-opacity duration-300
                ${isOpen ? 'opacity-100 visible' : 'opacity-0 invisible'}`}
    >
      {/* Backdrop with blur */}
      <div
        className="absolute inset-0 bg-zinc-900/90 backdrop-blur-sm transition-opacity"
        onClick={onClose}
      />
 
      {/* Modal Content */}
      <div className="relative bg-zinc-900/95 border border-zinc-800/50 rounded-xl 
                    max-w-3xl w-[90%] p-8 animate-fade-in overflow-hidden">
        {/* Background effects */}
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] 
                      from-blue-500/5 via-transparent to-transparent pointer-events-none" />
        
        {/* Corner decorations */}
        <div className="absolute top-0 left-0 w-16 h-16 border-l-2 border-t-2 
                     border-blue-500/20 pointer-events-none" />
        <div className="absolute top-0 right-0 w-16 h-16 border-r-2 border-t-2 
                     border-blue-500/20 pointer-events-none" />
        <div className="absolute bottom-0 left-0 w-16 h-16 border-l-2 border-b-2 
                     border-blue-500/20 pointer-events-none" />
        <div className="absolute bottom-0 right-0 w-16 h-16 border-r-2 border-b-2 
                     border-blue-500/20 pointer-events-none" />
 
        {/* Title */}
        <div className="relative mb-12">
          <div className="absolute top-0 left-0 w-8 h-8 border-t-2 border-l-2 
                       border-blue-500/30" />
          <div className="absolute top-0 right-0 w-8 h-8 border-t-2 border-r-2 
                       border-blue-500/30" />
          <h2 className="text-4xl font-pixel text-center
                      text-transparent bg-clip-text 
                      bg-gradient-to-r from-blue-400 to-teal-400">
            Press Kit
          </h2>
        </div>
 
        {/* Categories */}
        <div className="space-y-6 relative">
          {categories.map((category) => (
            <div
              key={category.name}
              className="relative p-6 bg-zinc-800/50 rounded-xl
                      border border-zinc-700/50 group"
            >
              <div className="flex justify-between items-center">
                <h3 className="text-xl font-pixel text-transparent bg-clip-text 
                            bg-gradient-to-r from-blue-400 to-teal-400">
                  {category.name}
                </h3>
                <button
                  onClick={() => window.open(category.url)}
                  className="px-6 py-2 rounded-lg font-pixel
                          bg-zinc-800/80 hover:bg-zinc-800
                          border border-zinc-700/50 hover:border-zinc-600
                          text-gray-400 hover:text-gray-200
                          transition-all duration-300 relative group"
                >
                  <span className="relative z-10">Download ZIP</span>
                  <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                              bg-gradient-to-r from-blue-500/10 to-teal-500/10
                              rounded-lg blur-sm transition-opacity duration-300" />
                </button>
              </div>
              
              {/* Hover effect */}
              <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                          bg-gradient-to-r from-blue-500/5 to-teal-500/5
                          transition-opacity duration-300 rounded-xl" />
            </div>
          ))}
        </div>
 
        {/* Close button */}
        <button
          onClick={onClose}
          className="absolute top-4 right-4 w-8 h-8
                  text-gray-400 hover:text-gray-200
                  flex items-center justify-center
                  rounded-lg group transition-colors duration-300"
        >
          {/* <span className="relative z-10">✕</span> */}
          <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                       bg-gradient-to-r from-blue-500/10 to-teal-500/10
                       rounded-lg blur-sm transition-opacity duration-300" />
        </button>
 
        {/* Border effects */}
        <div className="absolute -top-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
        <div className="absolute -bottom-px left-0 w-full h-px 
                     bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      </div>
    </div>
  );
 };
 
 export default PressKitModal;