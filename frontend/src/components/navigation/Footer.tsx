import { useState, FormEvent } from 'react';
import axios from 'axios';
import { Link, useLocation } from 'react-router-dom';
import toast from 'react-hot-toast';
import '../../styles/components/FooterLogo.css';
interface SubscribeResponse {
  success: boolean;
  message: string;
}

const Footer: React.FC = () => {
  const location = useLocation();
  const isHome = location.pathname === '/';
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleLogoClick = () => {
    if (isHome) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
      // window.location.reload();
    }
  };

  const validateEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const handleEmailSubmit = async (e: FormEvent) => {
    e.preventDefault();
  
    if (!email) {
      toast.error(
        <div className="flex items-center space-x-2">
          <span>이메일을 입력해주세요</span>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 3000,
          className: 'font-pixel',
          position: 'top-center',
          id: 'email-validation'
        }
      );
      return;
    }
    
    if (!validateEmail(email)) {
      toast.dismiss();
      toast.error(
        <div className="flex items-center space-x-2">
          <span>올바른 이메일 형식이 아닙니다</span>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 3000,
          className: 'font-pixel',
          position: 'top-center',
          id: 'email-format'
        }
      );
      return;
    }
  
    setIsLoading(true);
    toast.dismiss();

    try {
      const response = await axios.post<SubscribeResponse>(
        '/subscribe',
        { subscribe: email },
        {
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );
    
      if (response.data?.success === false) {
        if (response.data.message?.includes('중복')) {  
          throw new Error('이미 구독 중인 이메일입니다');
        }
        throw new Error(response.data?.message || '구독 처리 중 오류가 발생했습니다');
      }
    
      toast.success(
        <div className="flex items-center space-x-2">
          <div className="flex flex-col">
            <span className="font-medium">구독 완료!</span>
            <span className="text-sm text-gray-200">
              뉴스레터 구독이 완료되었습니다
            </span>
          </div>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#22c55e',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 4000,
          className: 'font-pixel'
        }
      );
      setEmail('');
    
    } catch (error) {
      let errorMessage = '서버 연결에 실패했습니다. 잠시 후 다시 시도해주세요.';
      
      if (axios.isAxiosError(error) && error.response) {
        if (error.response.data.message === '이미 구독된 이메일입니다.') {
          errorMessage = '이미 구독 중인 이메일입니다';
        } else if (error.response.data.message) {
          errorMessage = error.response.data.message;
        }
      }
    
      toast.error(
        <div className="flex items-center space-x-2">
          <div className="flex flex-col">
            <span className="font-medium">구독 실패</span>
            <span className="text-sm text-gray-200">
              {errorMessage}
            </span>
          </div>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 4000,
          className: 'font-pixel'
        }
      );
    } finally {
      setIsLoading(false);
    }
  };

  const handleKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      await handleEmailSubmit(e as unknown as FormEvent);
    }
  };

  return (
    <footer className="relative bg-zinc-900/95 text-gray-300 py-16 px-8 border-t border-zinc-800">
      <div className="absolute top-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      <div className="absolute bottom-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      
      <div className="container mx-auto grid grid-cols-1 md:grid-cols-3 gap-12 items-center">
        {/* Left Section - Social Media & Links */}
        <div className="flex flex-col items-center justify-center space-y-8">
          <div className="w-[264px] flex justify-center space-x-4"> {/* Match newsletter input width */}
            {[
              { platform: 'youtube', url: 'https://www.youtube.com/' },
              { platform: 'instagram', url: 'https://www.instagram.com/' },
              { platform: 'twitter', url: 'https://twitter.com/' },
              { platform: 'discord', url: 'https://discord.com/invite/2egDbgqFNP' }
            ].map(({ platform, url }) => (
              <a key={platform} href={url} target="_blank" rel="noopener noreferrer" aria-label={platform} className="group relative">
                <div className="absolute inset-0 bg-blue-500/20 rounded-lg blur-md opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                <div className="relative p-2 rounded-lg bg-zinc-800/50 border border-zinc-700 group-hover:border-zinc-600 group-hover:bg-zinc-800 transition-all duration-300">
                  <img 
                    src={`/assets/icons/${platform}.png`}
                    alt={platform}
                    className="h-6 w-6 object-contain group-hover:scale-110 transition-transform saturate-50 group-hover:saturate-100"
                  />
                </div>
              </a>
            ))}
          </div>
  
          <div className="text-sm space-x-6 text-gray-400 font-pixel">
            {[
              { to: '/terms-of-service', text: '이용약관' },
              { to: '/privacy-policy', text: '개인정보 처리방침' }
            ].map((link) => (
              <Link key={link.to} to={link.to} className="relative group">
                <span className="group-hover:text-gray-200 transition-colors duration-300">
                  {link.text}
                </span>
                <div className="absolute bottom-0 left-0 w-full h-px scale-x-0 
                              group-hover:scale-x-100 transition-transform duration-300
                              bg-gradient-to-r from-blue-500/50 to-teal-500/50" />
              </Link>
            ))}
          </div>
        </div>
  
        <div className="flex items-center justify-center">
          {isHome ? (
            <button 
              onClick={handleLogoClick} 
              aria-label="Scroll to top" 
              className="shine-effect"
            >
              <img
                src="/logo-name.png"
                alt="Arcana Logo"
                className="h-20 object-contain transition-transform duration-300 hover:brightness-150"
              />
            </button>
          ) : (
            <Link to="/" aria-label="Home" className="shine-effect">
              <img
                src="/logo-name.png"
                alt="Arcana Logo"
                className="h-20 object-contain transition-transform duration-300 hover:brightness-150"
              />
            </Link>
          )}
        </div>

  
        <div className="flex flex-col items-center space-y-4">
          <h3 className="text-2xl font-pixel text-gray-200">Join our newsletter!</h3>
          <form onSubmit={handleEmailSubmit} className="relative z-10 flex items-center space-x-2">
            <div className="relative group">
              <input
                type="email"
                placeholder={email ? '' : 'enter your email'}
                value={email}
                onFocus={(e) => e.target.placeholder = 'example@gmail.com'}
                onBlur={(e) => e.target.placeholder = email ? '' : 'enter your email'}
                onChange={(e) => setEmail(e.target.value)}
                onKeyDown={handleKeyDown}
                disabled={isLoading}
                className="relative z-20 p-3 w-64 rounded-lg bg-zinc-800/50
                          border border-zinc-700 
                          text-gray-200 placeholder-gray-500
                          focus:outline-none focus:border-blue-500/50
                          font-pixel transition-colors duration-300
                          hover:border-zinc-600 hover:bg-zinc-800/60
                          disabled:opacity-50 disabled:cursor-not-allowed"
              />
            </div>
            <button
              type="submit"
              disabled={isLoading}
              className="relative z-20 group p-3 rounded-lg bg-zinc-800/50 border border-zinc-700 
                        hover:border-zinc-600 hover:bg-zinc-800
                        transition-colors duration-300
                        disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <div className="w-6 h-6 border-2 border-gray-300 border-t-blue-500 rounded-full animate-spin" />
              ) : (
                <img
                  src="/assets/icons/email.png"
                  alt="Subscribe"
                  className="w-6 h-6 object-contain
                            group-hover:scale-110 transition-transform
                            saturate-50 group-hover:saturate-100"
                />
              )}
            </button>
          </form>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
