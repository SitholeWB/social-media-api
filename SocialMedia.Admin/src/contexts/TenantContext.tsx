import React, { createContext, useContext, useEffect, useState } from 'react';
import { tenantStorage, API_BASE_URL } from '../services/api';
import { ThemeProvider, createTheme } from '@mui/material/styles';

export interface TenantTheme {
    primaryColor?: string;
    secondaryColor?: string;
    fontFamily?: string;
    layout?: 'drawer' | 'top-nav' | 'bottom-tabs';
    navigationTabs?: any[];
}

export interface Tenant {
    id: string;
    name: string;
    description?: string;
    themeJson?: string;
}

interface TenantContextType {
    currentTenant: Tenant | null;
    themeConfig: TenantTheme | null;
    setCurrentTenant: (tenantId: string) => void;
}

const TenantContext = createContext<TenantContextType>({
    currentTenant: null,
    themeConfig: null,
    setCurrentTenant: () => {},
});

export const useTenant = () => useContext(TenantContext);

export const TenantProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [currentTenant, setCurrentTenantState] = useState<Tenant | null>(null);
    const [themeConfig, setThemeConfig] = useState<TenantTheme | null>(null);
    const [muiTheme, setMuiTheme] = useState(createTheme());

    const loadTenant = async (id: string) => {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/tenants`);
            if (response.ok) {
                const tenants: Tenant[] = await response.json();
                const tenant = tenants.find(t => t.id === id);
                if (tenant) {
                    setCurrentTenantState(tenant);
                    let config: TenantTheme = {};
                    if (tenant.themeJson) {
                        try {
                            config = JSON.parse(tenant.themeJson);
                        } catch (e) {
                            console.error("Failed to parse themeJson", e);
                        }
                    }
                    setThemeConfig(config);
                    
                    // Generate MUI Theme dynamically
                    const dynamicTheme = createTheme({
                        palette: {
                            primary: {
                                main: config.primaryColor || '#1976d2',
                            },
                            secondary: {
                                main: config.secondaryColor || '#9c27b0',
                            },
                        },
                        typography: {
                            fontFamily: config.fontFamily || '"Roboto", "Helvetica", "Arial", sans-serif',
                        }
                    });
                    setMuiTheme(dynamicTheme);
                }
            }
        } catch (error) {
            console.error('Error fetching tenant', error);
        }
    };

    useEffect(() => {
        const savedTenantId = tenantStorage.getTenantId();
        if (savedTenantId) {
            loadTenant(savedTenantId);
        }
    }, []);

    const setCurrentTenant = (id: string) => {
        tenantStorage.setTenantId(id);
        loadTenant(id);
    };

    return (
        <TenantContext.Provider value={{ currentTenant, themeConfig, setCurrentTenant }}>
            {children}
        </TenantContext.Provider>
    );
};
