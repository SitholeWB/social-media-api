import React, { createContext, useState, useEffect, useCallback, ReactNode } from 'react';
import { authService, User, LoginRequest, RegisterRequest, AuthResponse } from '../services/authService';
import { secureStorage } from '../utils/secureStorage';
import { jwtDecode } from "jwt-decode";
import { useDispatch } from 'react-redux';
import { setCredentials, logout as logoutAction } from '../store/slices/authSlice';

interface AuthState {
    user: User | null;
    isLoading: boolean;
    error: string | null;
    isInitialized: boolean;
}

interface JwtPayload {
    role?: string;
}

export interface AuthContextType {
    user: User | null;
    isLoading: boolean;
    error: string | null;
    isInitialized: boolean;
    isAuthenticated: boolean;
    isAdmin: boolean;
    isModerator: boolean;
    isUser: boolean;
    login: (request: LoginRequest) => Promise<AuthResponse>;
    loginWithGoogle: (idToken: string) => Promise<AuthResponse>;
    register: (request: RegisterRequest) => Promise<AuthResponse>;
    logout: () => Promise<void>;
    updateUser: (updates: Partial<User>) => Promise<void>;
    refreshUser: () => Promise<void>;
    clearError: () => void;
    getToken: () => string | null;
    hasRole: (role: string | string[]) => boolean;
    hasPermission: (permission: string | string[]) => boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [state, setState] = useState<AuthState>({
        user: null,
        isLoading: false,
        error: null,
        isInitialized: false,
    });

    const dispatch = useDispatch();

    const initializeAuth = useCallback(async () => {
        try {
            // Try to load from secure storage first
            const secureUser = await secureStorage.getItem<User>('auth_user_secure');

            if (secureUser) {
                // Ensure localStorage matches for backward compatibility and API calls
                if (secureUser.token && localStorage.getItem('auth_token') !== secureUser.token) {
                    localStorage.setItem('auth_token', secureUser.token);
                }

                const localUserStr = localStorage.getItem('auth_user');
                const secureUserStr = JSON.stringify(secureUser);
                if (localUserStr !== secureUserStr) {
                    localStorage.setItem('auth_user', secureUserStr);
                }

                // Sync with Redux
                if (secureUser.token) {
                    dispatch(setCredentials({ user: secureUser, token: secureUser.token }));
                }

                setState(prev => ({ ...prev, user: secureUser, isInitialized: true }));
                return;
            }

            // Try to load from localStorage (migration case)
            const localUser = authService.getUser();
            if (localUser) {
                await secureStorage.setItem('auth_user_secure', localUser);
                const token = authService.getToken();
                if (token) {
                    dispatch(setCredentials({ user: localUser, token }));
                }
                setState(prev => ({ ...prev, user: localUser, isInitialized: true }));
                return;
            }

            setState(prev => ({ ...prev, isInitialized: true }));
        } catch (error) {
            console.error('Failed to initialize auth:', error);
            setState(prev => ({
                ...prev,
                error: 'Failed to initialize authentication',
                isInitialized: true
            }));
        }
    }, [dispatch]);

    useEffect(() => {
        initializeAuth();
    }, [initializeAuth]);

    const login = useCallback(async (request: LoginRequest): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));
        try {
            const authResponse = await authService.login(request);
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };
            await secureStorage.setItem('auth_user_secure', user);
            dispatch(setCredentials({ user, token: authResponse.token }));
            setState(prev => ({ ...prev, user, isLoading: false }));
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Login failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, [dispatch]);

    const loginWithGoogle = useCallback(async (idToken: string): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));
        try {
            const authResponse = await authService.loginWithGoogle(idToken);
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };
            await secureStorage.setItem('auth_user_secure', user);
            dispatch(setCredentials({ user, token: authResponse.token }));
            setState(prev => ({ ...prev, user, isLoading: false }));
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Google login failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, [dispatch]);

    const register = useCallback(async (request: RegisterRequest): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));
        try {
            const authResponse = await authService.register(request);
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };
            await secureStorage.setItem('auth_user_secure', user);
            dispatch(setCredentials({ user, token: authResponse.token }));
            setState(prev => ({ ...prev, user, isLoading: false }));
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Registration failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, [dispatch]);

    const logout = useCallback(async (): Promise<void> => {
        setState(prev => ({ ...prev, isLoading: true }));
        try {
            secureStorage.removeItem('auth_user_secure');
            authService.logout();
            dispatch(logoutAction());
            setState(prev => ({ ...prev, user: null, isLoading: false }));
        } catch (error) {
            console.error('Logout error:', error);
            setState(prev => ({ ...prev, isLoading: false }));
        }
    }, [dispatch]);

    const updateUser = useCallback(async (updates: Partial<User>): Promise<void> => {
        if (!state.user) throw new Error('No user is logged in');
        setState(prev => ({ ...prev, isLoading: true, error: null }));
        try {
            const updatedUser = { ...state.user, ...updates };
            await secureStorage.setItem('auth_user_secure', updatedUser);
            localStorage.setItem('auth_user', JSON.stringify(updatedUser)); // Keep in sync
            setState(prev => ({ ...prev, user: updatedUser, isLoading: false }));
        } catch (error: any) {
            const errorMessage = error.message || 'Failed to update user';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, [state.user]);

    const refreshUser = useCallback(async (): Promise<void> => {
        if (!state.user?.token) return;
        setState(prev => ({ ...prev, isLoading: true, error: null }));
        try {
            const user = await authService.getCurrentUser();
            if (user) {
                await secureStorage.setItem('auth_user_secure', user);
                setState(prev => ({ ...prev, user, isLoading: false }));
            } else {
                await logout();
            }
        } catch (error: any) {
            setState(prev => ({ ...prev, isLoading: false }));
            if (error.message?.includes('Unauthorized')) {
                await logout();
            }
        }
    }, [state.user?.token, logout]);

    const hasRole = useCallback((role: string | string[]): boolean => {
        if (!state.user?.token) return false;
        try {
            const decoded: JwtPayload = jwtDecode(state.user.token);
            if (!decoded.role) return false;
            const userRole = decoded.role.toLowerCase();
            if (Array.isArray(role)) {
                return role.some(r => r.toLowerCase() === userRole);
            }
            return userRole === role.toLowerCase();
        } catch (error) {
            return false;
        }
    }, [state.user]);

    const hasPermission = useCallback((permission: string | string[]): boolean => {
        if (!state.user?.permissions) return false;
        if (Array.isArray(permission)) {
            return permission.some(p => state.user!.permissions!.includes(p));
        }
        return state.user.permissions.includes(permission);
    }, [state.user]);

    const authValue: AuthContextType = {
        user: state.user,
        isLoading: state.isLoading,
        error: state.error,
        isInitialized: state.isInitialized,
        isAuthenticated: !!state.user?.token && authService.isAuthenticated(),
        isAdmin: hasRole('admin'),
        isModerator: hasRole(['admin', 'moderator']),
        isUser: hasRole('user'),
        login,
        loginWithGoogle,
        register,
        logout,
        updateUser,
        refreshUser,
        clearError: () => setState(prev => ({ ...prev, error: null })),
        getToken: () => state.user?.token || authService.getToken(),
        hasRole,
        hasPermission,
    };

    return <AuthContext.Provider value={authValue}>{children}</AuthContext.Provider>;
};
