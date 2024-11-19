// // src/components/common/Layout.tsx
// import React from 'react';
// import Navbar from '../../components/navigation/Navbar';
// import Footer from '../../components/navigation/Footer';

// interface LayoutProps {
//   children: React.ReactNode;
//   excludePadding?: boolean;
// }

// const Layout: React.FC<LayoutProps> = ({ children, excludePadding = false }) => {
//   return (
//     <div className="min-h-screen flex flex-col">
//       <Navbar />
//       <main className={`flex-grow ${!excludePadding ? 'px-[20%]' : ''}`}>
//         {children}
//       </main>
//       <Footer />
//     </div>
//   );
// };

// export default Layout;