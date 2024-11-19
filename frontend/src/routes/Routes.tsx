import { Routes, Route, Navigate } from 'react-router-dom';  // Navigate 추가
import { Suspense, lazy } from 'react';  // 코드 스플리팅을 위한 import
import MainPage from '../pages/MainPage';
import DownloadPage from '../pages/DownloadPage';
import SupportPage from '../pages/SupportPage';

// 자주 접근하지 않는 페이지는 lazy loading 적용
const TermsOfService = lazy(() => import('../pages/TermsOfService'));
const PrivacyPolicy = lazy(() => import('../pages/PrivacyPolicy'));
const CookieSettings = lazy(() => import('../pages/CookieSettings'));

interface AppRoutesProps {
  setPressKitOpen: (isOpen: boolean) => void;
}

const AppRoutes: React.FC<AppRoutesProps> = ({ setPressKitOpen }) => {
  return (
    <Routes>
      <Route path="/" element={<MainPage setPressKitOpen={setPressKitOpen} />} />
      <Route path="/download" element={<DownloadPage setPressKitOpen={setPressKitOpen} />} />
      <Route path="/support" element={<SupportPage setPressKitOpen={setPressKitOpen} />} />
      
      {/* 자주 접근하지 않는 페이지는 Suspense로 감싸기 */}
      <Route path="/terms-of-service" element={
        <Suspense fallback={<div>Loading...</div>}>
          <TermsOfService />
        </Suspense>
      } />
      <Route path="/privacy-policy" element={
        <Suspense fallback={<div>Loading...</div>}>
          <PrivacyPolicy />
        </Suspense>
      } />
      <Route path="/cookie-settings" element={
        <Suspense fallback={<div>Loading...</div>}>
          <CookieSettings />
        </Suspense>
      } />

      {/* 404 페이지 처리 */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;