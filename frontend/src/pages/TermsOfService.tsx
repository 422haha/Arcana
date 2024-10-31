// src/pages/TermsOfService.tsx
import React from 'react';
import Navbar from '../components/navigation/Navbar';

const TermsOfService: React.FC = () => (
 <div className="relative w-full min-h-screen bg-zinc-900 text-gray-200">
   {/* Background effects */}
   <div className="fixed inset-0 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] 
                 from-blue-500/5 via-zinc-900 to-zinc-900 pointer-events-none" />
   <div className="fixed inset-0 bg-[url('/assets/noise.png')] opacity-[0.015] 
                 pointer-events-none" />
   
   <div id="nav-trigger" className="h-0"></div>
   <Navbar setPressKitOpen={() => {}} />
   
   <main className="relative z-10">
     <div className="container mx-auto px-[20%] py-20">
       <div className="max-w-3xl mx-auto">
         {/* Title Section */}
         <div className="relative mb-16">
           <div className="absolute top-0 left-0 w-8 h-8 border-t-2 border-l-2 
                        border-blue-500/30" />
           <div className="absolute top-0 right-0 w-8 h-8 border-t-2 border-r-2 
                        border-blue-500/30" />
           <h1 className="text-4xl font-pixel text-center
                       text-transparent bg-clip-text 
                       bg-gradient-to-r from-blue-400 to-teal-400">
             이용약관
           </h1>
         </div>

         {/* Content Section */}
         <div className="relative p-8 bg-zinc-800/50 rounded-xl
                      border border-zinc-700/50 group">
           <div className="absolute inset-0 opacity-0 group-hover:opacity-100
                        bg-gradient-to-r from-blue-500/5 to-teal-500/5
                        transition-opacity duration-300 rounded-xl" />
           
           {/* Content paragraphs */}
           <div className="space-y-6">
             <p className="text-gray-300 leading-relaxed">
               여기에 서비스 이용약관의 내용을 작성합니다.
             </p>
             <p className="text-gray-300 leading-relaxed">
               Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio.
             </p>
             <p className="text-gray-300 leading-relaxed">
               Praesent libero. Sed cursus ante dapibus diam.
             </p>
           </div>

           {/* Tech frame corners */}
           <div className="absolute top-0 left-0 w-4 h-4 border-t border-l 
                        border-blue-500/30" />
           <div className="absolute top-0 right-0 w-4 h-4 border-t border-r 
                        border-blue-500/30" />
           <div className="absolute bottom-0 left-0 w-4 h-4 border-b border-l 
                        border-blue-500/30" />
           <div className="absolute bottom-0 right-0 w-4 h-4 border-b border-r 
                        border-blue-500/30" />
         </div>
       </div>
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

export default TermsOfService;