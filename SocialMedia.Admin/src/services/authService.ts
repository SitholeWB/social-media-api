import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7168';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    username: string;
}

export interface AuthResponse {
    token: string;
    userId: string;
    email: string;
    username: string;
}

export interface User {
    id: string;
    email: string;
    username: string;
}

const TOKEN_KEY = 'auth_token';

export const authService = {
    async login(request: LoginRequest): Promise<AuthResponse> {
        const response = await axios.post<AuthResponse>(`${API_BASE_URL}/api/v1.0/auth/login`, request);
        if (response.data.token) {
            localStorage.setItem(TOKEN_KEY, response.data.token);
        }
        return response.data;
    },

    async loginWithGoogle(idToken: string): Promise<AuthResponse> {
        const response = await axios.post<AuthResponse>(`${API_BASE_URL}/api/v1.0/auth/google`, { idToken });
        if (response.data.token) {
            localStorage.setItem(TOKEN_KEY, response.data.token);
        }
        return response.data;
    },

    async register(request: RegisterRequest): Promise<AuthResponse> {
        const response = await axios.post<AuthResponse>(`${API_BASE_URL}/api/v1.0/auth/register`, request);
        if (response.data.token) {
            localStorage.setItem(TOKEN_KEY, response.data.token);
        }
        return response.data;
    },

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);
    },

    getToken(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    },

    isAuthenticated(): boolean {
        return !!this.getToken();
    },

    async getCurrentUser(): Promise<User | null> {
        const token = this.getToken();
        if (!token) return null;

        try {
            const response = await axios.get<User>(`${API_BASE_URL}/api/v1.0/auth/me`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            return response.data;
        } catch (error: any) {
            if (error.response?.status === 401 || error.response?.status === 403) {
                localStorage.removeItem(TOKEN_KEY);
                window.location.href = '/login';
            }
            return null;
        }
    }
};
