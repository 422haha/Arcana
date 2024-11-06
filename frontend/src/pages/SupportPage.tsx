// src/pages/SupportPage.tsx
import React from 'react';
import { useState, useRef } from 'react';
import Navbar from '../components/navigation/Navbar';
import axios from 'axios';
import toast from 'react-hot-toast';

type SupportCategory = '문제도움' | '버그신고' | '피드백제출';

interface SupportPageProps {
  setPressKitOpen: (isOpen: boolean) => void;
}

interface SupportForm {
  category: SupportCategory;
  email: string;
  title: string;
  description: string;
  screenshots: File[];
}

const SupportPage: React.FC<SupportPageProps> = ({ setPressKitOpen }) => {
  const [form, setForm] = useState<SupportForm>({
    category: '문제도움',
    email: '',
    title: '',
    description: '',
    screenshots: []
  });
  const [isDragging, setIsDragging] = useState(false);
  const [error] = useState<string | null>(null);
  const [successMessage] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const MAX_FILES = 5;
  const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/gif', 'image/jpg'];
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validateEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  // const validateFile = (file: File): string | null => {
  //   const sizeInMB = file.size / (1024 * 1024); // Convert to MB
  
  //   if (!ALLOWED_TYPES.includes(file.type)) {
  //     return 'JPG, PNG, GIF 형식의 이미지만 업로드 가능합니다.';
  //   }
  //   if (sizeInMB > 1) {
  //     return `파일 크기가 너무 큽니다: ${file.name} (${sizeInMB.toFixed(1)}MB)`;
  //   }
  //   return null;
  // };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    // 필수 입력값 검증 확장
    if (!form.category || !form.email || !form.title || !form.description) {
      toast.error(
        <div className="flex items-center space-x-2">
          <span>{!form.description ? '설명을 입력해주세요' : '필수 항목을 모두 입력해주세요'}</span>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 5000,
          className: 'font-pixel'
        }
      );
      setIsSubmitting(false);
      return;
    }
    if (!validateEmail(form.email)) {
      toast.error(
        <div className="flex items-center space-x-2">
          {/* <AlertCircle className="w-5 h-5" /> */}
          <span>올바른 이메일 형식이 아닙니다</span>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 5000,
          className: 'font-pixel'
        }
      );
      setIsSubmitting(false);
      return;
    }
  
    try {
      const validFiles = form.screenshots.filter(file => {
        const sizeInMB = file.size / (1024 * 1024);
        return ALLOWED_TYPES.includes(file.type) && sizeInMB <= 1;
      });
  
      if (validFiles.length !== form.screenshots.length) {
        toast.error(
          <div className="flex items-center space-x-2">
            <span>파일 크기나 형식이 맞지 않는 파일이 제외되었습니다.</span>
          </div>,
          {
            style: {
              background: '#18181b',
              color: '#ef4444',
              border: '1px solid #3f3f46',
              padding: '16px',
            },
            duration: 3000,
            className: 'font-pixel'
          }
        );
        setIsSubmitting(false);
        return;
      }
  
      const formData = new FormData();
      formData.append('category', form.category);
      formData.append('email', form.email);
      formData.append('title', form.title);
      formData.append('description', form.description);
      
      // 검증된 파일들만 추가
      validFiles.forEach((file) => {
        formData.append('screenshots', file);
      });

      const response = await axios.post('/support', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        }
      });
  
      if (response.data) {
        toast.success(
          <div className="flex items-center space-x-2">
            <div className="flex flex-col">
              <span className="font-medium">접수 완료</span>
              <span className="text-sm text-gray-200">
                검토 후 이메일로 답변드리겠습니다.
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
            duration: 5000,
            className: 'font-pixel'
          }
        );
  
        // 폼 초기화
        setForm({
          category: '문제도움',
          email: '',
          title: '',
          description: '',
          screenshots: []
        });
      }
    } catch (error) {
      let errorMessage = '서버 연결에 실패했습니다. 잠시 후 다시 시도해주세요.';
      
      if (axios.isAxiosError(error) && error.response) {
        errorMessage = error.response.data?.message || '문의 제출 중 오류가 발생했습니다.';
      }
  
      toast.error(
        <div className="flex items-center space-x-2">
          {/* <X className="w-5 h-5" /> */}
          <div className="flex flex-col">
            <span className="font-medium">제출 실패</span>
            <span className="text-sm text-gray-200">{errorMessage}</span>
          </div>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 5000,
          className: 'font-pixel'
        }
      );
      console.error('Support submission error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFileDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
  
    const files = Array.from(e.dataTransfer.files).filter(file =>
      file.type.startsWith('image/')
    );
  
    // 파일 개수 검증
    if (form.screenshots.length + files.length > MAX_FILES) {
      toast.error(
        <div className="flex items-center space-x-2">
          {/* <AlertCircle className="w-5 h-5" /> */}
          <span>최대 {MAX_FILES}개의 파일만 업로드할 수 있습니다</span>
        </div>,
        {
          style: {
            background: '#18181b',
            color: '#ef4444',
            border: '1px solid #3f3f46',
            padding: '16px',
          },
          duration: 3000,
          className: 'font-pixel'
        }
      );
      return;
    }
  
    // 각 파일 크기 및 형식 검증
    for (const file of files) {
      const sizeInMB = file.size / (1024 * 1024);
      if (!ALLOWED_TYPES.includes(file.type)) {
        toast.error(
          <div className="flex items-center space-x-2">
            {/* <AlertCircle className="w-5 h-5" /> */}
            <span>JPG, PNG, GIF 형식의 이미지만 업로드 가능합니다.</span>
          </div>,
          {
            style: {
              background: '#18181b',
              color: '#ef4444',
              border: '1px solid #3f3f46',
              padding: '16px',
            },
            duration: 3000,
            className: 'font-pixel'
          }
        );
        return;
      }
      if (sizeInMB > 1) {
        toast.error(
          <div className="flex items-center space-x-2">
            {/* <AlertCircle className="w-5 h-5" /> */}
            <span>
              파일 크기 제한 초과: {file.name} ({sizeInMB.toFixed(1)}MB)
            </span>
          </div>,
          {
            style: {
              background: '#18181b',
              color: '#ef4444',
              border: '1px solid #3f3f46',
              padding: '16px',
            },
            duration: 3000,
            className: 'font-pixel'
          }
        );
        return;
      }
    }
  
    // 모든 검증을 통과한 경우 파일 추가
    setForm(prev => ({
      ...prev,
      screenshots: [...prev.screenshots, ...files]
    }));
  };

  

  const handleFileInput = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const files = Array.from(e.target.files);
  
      // 파일 개수 검증
      if (form.screenshots.length + files.length > MAX_FILES) {
        toast.error(
          <div className="flex items-center space-x-2">
            {/* <AlertCircle className="w-5 h-5" /> */}
            <span>최대 {MAX_FILES}개의 파일만 업로드할 수 있습니다.</span>
          </div>,
          {
            style: {
              background: '#18181b',
              color: '#ef4444',
              border: '1px solid #3f3f46',
              padding: '16px',
            },
            duration: 3000,
            className: 'font-pixel'
          }
        );
        if (fileInputRef.current) {
          fileInputRef.current.value = '';  // 입력 초기화
        }
        return;
      }
  
      // 각 파일 크기 및 형식 검증
      for (const file of files) {
        const sizeInMB = file.size / (1024 * 1024);
        if (!ALLOWED_TYPES.includes(file.type)) {
          toast.error(
            <div className="flex items-center space-x-2">
              {/* <AlertCircle className="w-5 h-5" /> */}
              <span>JPG, PNG, GIF 형식의 이미지만 업로드 가능합니다.</span>
            </div>,
            {
              style: {
                background: '#18181b',
                color: '#ef4444',
                border: '1px solid #3f3f46',
                padding: '16px',
              },
              duration: 3000,
              className: 'font-pixel'
            }
          );
          if (fileInputRef.current) {
            fileInputRef.current.value = '';  // 입력 초기화
          }
          return;
        }
        if (sizeInMB > 1) {
          toast.error(
            <div className="flex items-center space-x-2">
              {/* <AlertCircle className="w-5 h-5" /> */}
              <span>
                파일 크기 제한 초과: {file.name} ({sizeInMB.toFixed(1)}MB)
              </span>
            </div>,
            {
              style: {
                background: '#18181b',
                color: '#ef4444',
                border: '1px solid #3f3f46',
                padding: '16px',
              },
              duration: 3000,
              className: 'font-pixel'
            }
          );
          if (fileInputRef.current) {
            fileInputRef.current.value = '';  // 입력 초기화
          }
          return;
        }
      }
  
      // 모든 검증을 통과한 경우 파일 추가
      setForm(prev => ({
        ...prev,
        screenshots: [...prev.screenshots, ...files]
      }));
    }

    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  return (
    <div className="relative w-full min-h-screen bg-zinc-900 text-gray-200">
      {/* Background effects */}
      <div className="fixed inset-0 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] 
                    from-blue-500/5 via-zinc-900 to-zinc-900 pointer-events-none" />
      <div className="fixed inset-0 bg-[url('/assets/noise.png')] opacity-[0.015] 
                    pointer-events-none" />
 
      <div id="nav-trigger" className="h-0"></div>
      <Navbar setPressKitOpen={setPressKitOpen} />
      
      <main className="relative z-10">
        <div className="container mx-auto px-[20%] py-20">
          {/* Title Section */}
          <div className="relative mb-16">
            <div className="absolute top-0 left-0 w-8 h-8 border-t-2 border-l-2 
                         border-blue-500/30" />
            <div className="absolute top-0 right-0 w-8 h-8 border-t-2 border-r-2 
                         border-blue-500/30" />
            <h1 className="text-4xl font-pixel text-center
                        text-transparent bg-clip-text 
                        bg-gradient-to-r from-blue-400 to-teal-400">
              고객 지원
            </h1>
          </div>
 
          {/* Form Section */}
          <form onSubmit={handleSubmit} className="max-w-2xl mx-auto space-y-8">
            {/* Category Select */}
            <div className="space-y-2">
            <div className="text-sm text-gray-400 text-right mb-2">
              * 표시된 항목은 필수 입력사항입니다
            </div>
              <label className="block font-pixel text-gray-300">문의 유형</label>
              <select
                value={form.category}
                onChange={e => setForm({ ...form, category: e.target.value as SupportCategory })}
                className="w-full p-3 rounded-lg 
                        bg-zinc-800/80 backdrop-blur-sm
                        border border-zinc-700/50 
                        text-gray-300 font-pixel
                        focus:border-blue-500/50 focus:outline-none
                        transition-colors duration-300"
              >
                <option value="문제도움">게임 플레이 도움</option>
                <option value="버그신고">버그 신고</option>
                <option value="피드백제출">피드백 제출</option>
              </select>
            </div>
 
            {/* Email Input */}
            <div className="space-y-2">
              <label className="block font-pixel text-gray-300">이메일 *</label>
              <input
                type="email"
                required
                value={form.email}
                onChange={e => setForm({ ...form, email: e.target.value })}
                className="w-full p-3 rounded-lg 
                        bg-zinc-800/80 backdrop-blur-sm
                        border border-zinc-700/50 
                        text-gray-300
                        focus:border-blue-500/50 focus:outline-none
                        transition-colors duration-300"
              />
            </div>
 
            {/* Title Input */}
            <div className="space-y-2">
              <label className="block font-pixel text-gray-300">제목 *</label>
              <input
                type="text"
                required
                value={form.title}
                onChange={e => setForm({ ...form, title: e.target.value })}
                className="w-full p-3 rounded-lg 
                        bg-zinc-800/80 backdrop-blur-sm
                        border border-zinc-700/50 
                        text-gray-300
                        focus:border-blue-500/50 focus:outline-none
                        transition-colors duration-300"
              />
            </div>
 
            {/* Description Textarea */}
            <div className="space-y-2">
              <label className="block font-pixel text-gray-300">
                설명 * {/* 별표 추가 */}
              </label>
              <textarea
                required  // required 속성 추가
                value={form.description}
                onChange={e => setForm({ ...form, description: e.target.value })}
                placeholder="문의하실 내용을 자세히 작성해주세요"  // 플레이스홀더 추가
                className="w-full p-3 rounded-lg h-40
                        bg-zinc-800/80 backdrop-blur-sm
                        border border-zinc-700/50 
                        text-gray-300
                        focus:border-blue-500/50 focus:outline-none
                        transition-colors duration-300
                        resize-none"
              />
            </div>
 
            {/* File Upload Section */}
            <div className="space-y-2">
            <div className="flex justify-between items-center">
              <label className="block font-pixel text-gray-300">스크린샷</label>
              <div className="text-sm text-gray-400">
                {form.screenshots.length}/{MAX_FILES} 파일
              </div>
              </div>
              <div className="text-sm text-gray-400 space-y-1 mb-2">
                <p>• 최대 {MAX_FILES}개 파일까지 업로드 가능</p>
                <p>• 파일당 최대 1MB</p>
                <p>• JPG, PNG, GIF 형식만 허용</p>
              </div>
              <div
                onDragOver={e => {
                  e.preventDefault();
                  setIsDragging(true);
                }}
                onDragLeave={() => setIsDragging(false)}
                onDrop={handleFileDrop}
                className={`relative border-2 border-dashed rounded-lg p-8 text-center
                        transition-colors duration-300 group
                        ${isDragging 
                          ? 'border-blue-500/50 bg-blue-500/10' 
                          : 'border-zinc-700/50 hover:border-zinc-600/80'}`}
              >
                <input
                  type="file"
                  ref={fileInputRef}
                  onChange={handleFileInput}
                  multiple
                  accept="image/*"
                  className="hidden"
                />
                <div className="space-y-2">
                  <p className="text-gray-400">이미지를 드래그하여 업로드하거나</p>
                  <button
                    type="button"
                    onClick={() => fileInputRef.current?.click()}
                    className="font-pixel text-transparent bg-clip-text 
                            bg-gradient-to-r from-blue-400 to-teal-400
                            hover:from-blue-300 hover:to-teal-300
                            transition-all duration-300"
                  >
                    파일 선택
                  </button>
                </div>
                {/* <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                            bg-gradient-to-r from-blue-500/5 to-teal-500/5
                            rounded-lg transition-opacity duration-300" /> */}
              </div>
              {/* Screenshot Preview */}
              {form.screenshots.length > 0 && (
                <div className="mt-4 flex flex-wrap gap-2">
                  {form.screenshots.map((file, index) => (
                    <div key={index} className="relative group">
                      <div className="absolute -inset-1 bg-gradient-to-r from-blue-500/20 to-teal-500/20 
                                  rounded-lg blur-lg opacity-0 group-hover:opacity-75 transition-opacity duration-500" />
                      <img
                        src={URL.createObjectURL(file)}
                        alt={`Screenshot ${index + 1}`}
                        className="relative w-20 h-20 object-cover rounded-lg
                                border border-zinc-700/50 group-hover:border-zinc-600/80
                                transition-all duration-300"
                      />
                      <button
                        type="button"
                        onClick={() =>
                          setForm(prev => ({
                            ...prev,
                            screenshots: prev.screenshots.filter((_, i) => i !== index)
                          }))
                        }
                        className="absolute -top-2 -right-2 w-6 h-6
                                bg-zinc-800/80 hover:bg-zinc-800
                                border border-zinc-700/50 hover:border-zinc-600
                                rounded-full flex items-center justify-center
                                text-gray-400 hover:text-gray-200
                                transition-all duration-300"
                      >
                        ×
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
 
            {/* Submit Button */}
            <button
              type="submit"
              disabled={isSubmitting}
              className="relative w-full group px-6 py-3 rounded-lg
                      bg-zinc-800/80 hover:bg-zinc-800
                      border border-zinc-700/50 hover:border-zinc-600
                      transition-all duration-300
                      disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isSubmitting ? (
                <div className="flex items-center justify-center space-x-2">
                  <div className="w-5 h-5 border-2 border-gray-300 border-t-blue-500 rounded-full animate-spin" />
                  <span className="font-pixel text-gray-300">처리중...</span>
                </div>
              ) : (
                <span className="relative z-10 font-pixel
                              text-transparent bg-clip-text 
                              bg-gradient-to-r from-blue-400 to-teal-400">
                  문의하기
                </span>
              )}
              <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                          bg-gradient-to-r from-blue-500/10 to-teal-500/10
                          rounded-lg blur-sm transition-opacity duration-300" />
            </button>
 
            {/* Status Messages */}
            {successMessage && (
              <p className="text-teal-400 font-pixel text-center">{successMessage}</p>
            )}
            {error && (
              <p className="text-red-400 font-pixel text-center">오류: {error}</p>
            )}
          </form>
        </div>
      </main>
 
      {/* Decorative corner elements */}
      <div className="fixed top-0 left-0 w-16 h-16 border-l-2 border-t-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed top-0 right-0 w-16 h-16 border-r-2 border-t-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed bottom-0 left-0 w-16 h-16 border-l-2 border-b-2 
                   border-blue-500/20 pointer-events-none" />
      <div className="fixed bottom-0 right-0 w-16 h-16 border-r-2 border-b-2 
                   border-blue-500/20 pointer-events-none" />
    </div>
  );
 };
 
 export default SupportPage;