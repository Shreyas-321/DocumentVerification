import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

import { Toaster } from 'react-hot-toast';
import { AuthProvider, useAuth } from './Services/AuthService';
import NavBar from './Components/Layout/NavBar';
import AppRoutes from './Routes/AppRoutes';
import './App.css';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

// Enhanced Loading component with beautiful animations
const LoadingScreen = () => (
  <div className="min-vh-100 d-flex align-items-center justify-content-center bg-gradient-primary">
    <div className="text-center text-white">
      <div className="spinner-border text-white mb-3" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      <h2 className="mb-3">Risk Management System</h2>
      <p className="text-white-50">Loading your dashboard...</p>
    </div>
  </div>
);

// App content component that uses auth context
const AppContent = () => {
  const { loading } = useAuth();
  
  if (loading) {
    return <LoadingScreen />;
  }
  
  return (
    <div className="min-vh-100">
      <NavBar />
      <main>
        <AppRoutes />
      </main>
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 4000,
          style: {
            background: '#363636',
            color: '#fff',
            borderRadius: '10px',
            boxShadow: '0 10px 40px rgba(0, 0, 0, 0.1)',
          },
          success: {
            iconTheme: {
              primary: '#22c55e',
              secondary: '#fff',
            },
          },
          error: {
            iconTheme: {
              primary: '#ef4444',
              secondary: '#fff',
            },
          },
        }}
      />
    </div>
  );
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Router>
          <AppContent />
        </Router>
      </AuthProvider>
    </QueryClientProvider>
  );
}

export default App;
