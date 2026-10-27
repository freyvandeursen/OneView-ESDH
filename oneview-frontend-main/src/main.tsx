import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ThemeProvider } from '@/components/ui/theme-provider';
import Error404 from './pages/Error404.tsx';
import Customers from './pages/Customers.tsx';
import NewCustomer from './pages/NewCustomer.tsx';
import Login from './pages/Login.tsx';
import { ApiProvider } from './context/ApiContext.tsx';
import { Toaster } from './components/ui/sonner.tsx';
import CustomerDetails from './pages/CustomerDetails.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
      <ApiProvider>
        <BrowserRouter>
          <Toaster />
          <Routes>
            <Route path="/" element={<Customers />} />
            <Route
              path="/customers/:customerId"
              element={<CustomerDetails />}
            />
            <Route path="/newcustomer" element={<NewCustomer />} />
            <Route path="*" element={<Error404 />} />
            <Route path="/login" element={<Login />} />
          </Routes>
        </BrowserRouter>
      </ApiProvider>
    </ThemeProvider>
  </StrictMode>
);
