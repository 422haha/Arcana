/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{js,ts,jsx,tsx}', './index.html'],
  theme: {
    extend: {
      // 기존 색상 유지하면서 레트로 퓨처리즘 색상 추가
      colors: {
        neonGreen: '#00FFA3',
        cyberPink: '#FF007A',
        darkBlue: '#02010A',
        // 새로운 색상 추가
        'retro': {
          blue: {
            light: '#60a5fa',
            DEFAULT: '#3b82f6',
            dark: '#1d4ed8'
          },
          teal: {
            light: '#2dd4bf',
            DEFAULT: '#14b8a6',
            dark: '#0d9488'
          }
        }
      },

      // 폰트 패밀리 확장
      fontFamily: {
        pixel: ['NeoDunggeunmo', 'DungGeunMo', 'monospace'],
        retro: ['"Press Start 2P"', 'cursive'],
        sans: ['Pretendard', 'MonoplexKR', 'sans-serif'],
        mono: ['MonoplexKR', 'JetBrains Mono', 'monospace'],
        content: ['ChosunGu', 'Pretendard', 'sans-serif'],  // 긴 설명문용
        gui: ['Pretendard', 'ChosunGu', 'sans-serif'],      // UI 요소용
        code: ['MonoplexKR', 'JetBrains Mono', 'monospace'], // 코드/숫자용
        description: ['GmarketSans', 'ChosunGu', 'Pretendard', 'sans-serif'], // 게임 설명문용
      },

      // 기존 네온 그림자 효과 유지 및 확장
      boxShadow: {
        neon: '0 0 15px 2px rgba(0, 255, 163, 0.8)',
        'neon-blue': '0 0 15px 2px rgba(59, 130, 246, 0.5)',
        'neon-teal': '0 0 15px 2px rgba(45, 212, 191, 0.5)',
      },

      // 애니메이션 추가
      animation: {
        'scan': 'scan 8s linear infinite',
        'glitch': 'glitch 4s linear infinite',
        'pulse-slow': 'pulse 4s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },

      // 키프레임 추가
      keyframes: {
        scan: {
          '0%': { transform: 'translateY(0)' },
          '100%': { transform: 'translateY(100%)' }
        },
        glitch: {
          '0%, 100%': { transform: 'none' },
          '33%': { transform: 'skewX(2deg)' },
          '66%': { transform: 'skewX(-2deg)' }
        },
        marqueeRight: {
          '0%': { transform: 'translateX(-100%)' },
          '100%': { transform: 'translateX(0)' },
        },
        marqueeLeft: {
          '0%': { transform: 'translateX(0)' },
          '100%': { transform: 'translateX(-100%)' },
        },
        cursor: {
          '0%, 100%': { opacity: 1 },
          '50%': { opacity: 0 },
      },
    },
      animation: {
        'marquee-right': 'marqueeRight 30s linear infinite',
        'marquee-left': 'marqueeLeft 30s linear infinite',
         'cursor-blink': 'cursor 1s steps(2) infinite',
      },
        

      // 배경 이미지 패턴
      backgroundImage: {
        'noise': "url('/assets/noise.png')",
        'scanlines': "url('/assets/scanlines.png')",
      },

      // 트랜지션
      transitionDuration: {
        '400': '400ms',
      },

      // 블러 효과
      backdropBlur: {
        xs: '2px',
      },
      
    },
  },
  plugins: [],
};