import React, { useState, ChangeEvent, FormEvent } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

const Footer = () => {
  const [email, setEmail] = useState('');

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      const response = await axios.post('/api/v1/subscribe', { email });
      console.log('Success:', response.data);
      setEmail('');
    } catch (error) {
      console.error('Error:', error);
    }
  };

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    setEmail(e.target.value);
  };

  return (
    <footer className="relative bg-zinc-900/95 text-gray-300 py-16 px-8 border-t border-zinc-800">
      <div className="absolute top-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      <div className="absolute bottom-0 left-0 w-full h-px bg-gradient-to-r from-transparent via-blue-500/30 to-transparent" />
      
      <div className="container mx-auto flex flex-col md:flex-row items-center justify-between space-y-12 md:space-y-0">
        {/* SNS Links */}
        <div className="flex flex-col items-center space-y-8">
          <div className="flex space-x-4">
            {['youtube', 'instagram', 'facebook', 'twitter', 'discord'].map((platform) => (
              <a
                key={platform}
                href="#"
                className="group relative p-2"
              >
                <img 
                  src={`/assets/icons/${platform}.png`}
                  alt={platform}
                  className="h-6 w-6"
                />
              </a>
            ))}
          </div>
        </div>

        {/* Logo */}
        <div className="flex items-center">
          <Link to="/" className="relative">
            <img
              src="/assets/images/arcana-logo.png"
              alt="Arcana Logo"
              className="h-16"
            />
          </Link>
        </div>

        {/* Newsletter */}
        <div className="flex flex-col items-center space-y-4">
          <h3 className="text-2xl font-pixel text-gray-200">Join our newsletter!</h3>
          <form onSubmit={handleSubmit} className="flex items-center space-x-2">
            <div className="relative">
              <input
                type="email"
                value={email}
                onChange={handleChange}
                placeholder="enter your email"
                className="p-3 w-64 rounded-lg bg-zinc-800/50 
                          border border-zinc-700 hover:border-zinc-600
                          text-gray-200 placeholder-gray-500
                          focus:outline-none focus:border-blue-500/50
                          font-pixel transition-all duration-300"
                style={{ zIndex: 10 }}
              />
            </div>
            <button
              type="submit"
              className="relative p-3 rounded-lg bg-zinc-800/50 
                       border border-zinc-700 hover:border-zinc-600
                       transition-all duration-300"
              style={{ zIndex: 10 }}
            >
              <img
                src="/assets/icons/email.png"
                alt="Subscribe"
                className="w-6 h-6"
              />
            </button>
          </form>
        </div>
      </div>
    </footer>
  );
};

export default Footer;