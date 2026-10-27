import React, { createContext, useContext } from 'react';
import type { ReactNode } from 'react';
import type { AxiosInstance } from 'axios';
import api from '../api/api';

const ApiContext = createContext<AxiosInstance>(api);

interface ApiProviderProps {
  children: ReactNode;
}

export const ApiProvider: React.FC<ApiProviderProps> = ({ children }) => {
  return <ApiContext.Provider value={api}>{children}</ApiContext.Provider>;
};

export const useApi = (): AxiosInstance => useContext(ApiContext);
