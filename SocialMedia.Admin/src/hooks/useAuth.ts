// hooks/useAuth.ts - Compatible with your fetchJson
import { useState, useEffect, useCallback } from 'react';
import { authService, LoginRequest, RegisterRequest, AuthResponse, User } from '../services/authService';
import { secureStorage } from '../utils/secureStorage'; // Your secure storage utility

interface AuthState {
    user: User | null;
    isLoading: boolean;
    error: string | null;
    isInitialized: boolean;
}

export function useAuth() {
    const [state, setState] = useState<AuthState>({
        user: null,
        isLoading: false,
        error: null,
        isInitialized: false,
    });

    // Load user on mount
    useEffect(() => {
        initializeAuth();
    }, []);

    const initializeAuth = useCallback(async () => {
        try {
            // Try to load from secure storage first
            const secureUser = await secureStorage.getItem<User>('auth_user_secure');
            
            if (secureUser) {
                // Also update localStorage for backward compatibility
                authService.logout(); // Clear any old data
                if (secureUser.token) {
                    localStorage.setItem('auth_token', secureUser.token);
                }
                localStorage.setItem('auth_user', JSON.stringify(secureUser));
                
                setState(prev => ({ ...prev, user: secureUser, isInitialized: true }));
                return;
            }
            
            // Try to load from localStorage (existing users)
            const localUser = authService.getUser();
            if (localUser) {
                // Migrate to secure storage
                await secureStorage.setItem('auth_user_secure', localUser);
                setState(prev => ({ ...prev, user: localUser, isInitialized: true }));
                return;
            }
            
            // Try to fetch current user if token exists
            const token = authService.getToken();
            if (token) {
                const user = await authService.getCurrentUser();
                if (user) {
                    // Save to secure storage
                    await secureStorage.setItem('auth_user_secure', user);
                    setState(prev => ({ ...prev, user, isInitialized: true }));
                    return;
                }
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
    }, []);

    const login = useCallback(async (request: LoginRequest): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));

        try {
            // Call login API via authService
            const authResponse = await authService.login(request);
            
            // Create user object
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };

            // Save to secure storage
            await secureStorage.setItem('auth_user_secure', user);
            
            // Update state
            setState(prev => ({ ...prev, user, isLoading: false }));
            
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Login failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, []);

    const loginWithGoogle = useCallback(async (idToken: string): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));

        try {
            const authResponse = await authService.loginWithGoogle(idToken);
            
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                permissions: authResponse.permissions,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };

            await secureStorage.setItem('auth_user_secure', user);
            setState(prev => ({ ...prev, user, isLoading: false }));
            
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Google login failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, []);

    const register = useCallback(async (request: RegisterRequest): Promise<AuthResponse> => {
        setState(prev => ({ ...prev, isLoading: true, error: null }));

        try {
            const authResponse = await authService.register(request);
            
            const user: User = {
                id: authResponse.id || authResponse.userId,
                email: authResponse.email,
                username: authResponse.username,
                role: authResponse.role,
                permissions: authResponse.permissions,
                token: authResponse.token,
                expiresAt: authResponse.expiresAt,
            };

            await secureStorage.setItem('auth_user_secure', user);
            setState(prev => ({ ...prev, user, isLoading: false }));
            
            return authResponse;
        } catch (error: any) {
            const errorMessage = error.message || 'Registration failed';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, []);

    const logout = useCallback(async (): Promise<void> => {
        setState(prev => ({ ...prev, isLoading: true }));
        
        try {
            // Clear secure storage
            secureStorage.removeItem('auth_user_secure');
            
            // Clear localStorage via authService
            authService.logout();
            
            setState(prev => ({ ...prev, user: null, isLoading: false }));
        } catch (error) {
            console.error('Logout error:', error);
            setState(prev => ({ ...prev, isLoading: false }));
        }
    }, []);

    const updateUser = useCallback(async (updates: Partial<User>): Promise<void> => {
        if (!state.user) {
            throw new Error('No user is logged in');
        }

        setState(prev => ({ ...prev, isLoading: true, error: null }));

        try {
            const updatedUser = { ...state.user, ...updates };
            
            // Update secure storage
            await secureStorage.setItem('auth_user_secure', updatedUser);
            
            // Update localStorage for backward compatibility
            localStorage.setItem('auth_user', JSON.stringify(updatedUser));
            
            setState(prev => ({ ...prev, user: updatedUser, isLoading: false }));
        } catch (error: any) {
            const errorMessage = error.message || 'Failed to update user';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            throw error;
        }
    }, [state.user]);

    const refreshUser = useCallback(async (): Promise<void> => {
        if (!state.user?.token) {
            return;
        }

        setState(prev => ({ ...prev, isLoading: true, error: null }));

        try {
            const user = await authService.getCurrentUser();
            
            if (user) {
                await secureStorage.setItem('auth_user_secure', user);
                setState(prev => ({ ...prev, user, isLoading: false }));
            } else {
                // Token invalid, logout
                await logout();
            }
        } catch (error: any) {
            const errorMessage = error.message || 'Failed to refresh user';
            setState(prev => ({ ...prev, error: errorMessage, isLoading: false }));
            
            // If unauthorized, logout
            if (error.message.includes('Unauthorized')) {
                await logout();
            }
        }
    }, [state.user?.token, logout]);

    const getToken = useCallback((): string | null => {
        return state.user?.token || authService.getToken();
    }, [state.user]);

    const isAuthenticated = useCallback((): boolean => {
        return !!state.user?.token && authService.isAuthenticated();
    }, [state.user]);

    const clearError = useCallback((): void => {
        setState(prev => ({ ...prev, error: null }));
    }, []);

    const hasRole = useCallback((role: string | string[]): boolean => {
        if (!state.user?.role) return false;
        
        if (Array.isArray(role)) {
            return role.includes(state.user.role);
        }
        
        return state.user.role === role;
    }, [state.user]);

    const hasPermission = useCallback((permission: string | string[]): boolean => {
        if (!state.user?.permissions) return false;
        
        if (Array.isArray(permission)) {
            return permission.some(p => state.user!.permissions!.includes(p));
        }
        
        return state.user.permissions.includes(permission);
    }, [state.user]);

    return {
        // State
        user: state.user,
        isLoading: state.isLoading,
        error: state.error,
        isInitialized: state.isInitialized,
        
        // Authentication status
        isAuthenticated: isAuthenticated(),
        
        // Actions
        login,
        loginWithGoogle,
        register,
        logout,
        updateUser,
        refreshUser,
        clearError,
        
        // Utilities
        getToken,
        hasRole,
        hasPermission,
        
        // Quick role checks
        isAdmin: hasRole('admin'),
        isModerator: hasRole(['admin', 'moderator']),
        isUser: hasRole('user'),
    };
}