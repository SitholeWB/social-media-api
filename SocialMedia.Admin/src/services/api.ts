// services/api.ts - Updated version
export const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7168';
export const FILES_API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7221';
const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user'; // Added for user data

// Store refresh token attempt state
let isRefreshing = false;
let refreshSubscribers: Array<(token: string) => void> = [];

// Function to subscribe to token refresh
function subscribeTokenRefresh(callback: (token: string) => void) {
	refreshSubscribers.push(callback);
}

// Function to notify subscribers when token is refreshed
function onTokenRefreshed(token: string) {
	refreshSubscribers.forEach(callback => callback(token));
	refreshSubscribers = [];
}

// Enhanced fetchJson with better auth integration
export async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
	const token = localStorage.getItem(TOKEN_KEY);

	const response = await fetch(`${API_BASE_URL}${url}`, {
		...options,
		headers: {
			'Content-Type': 'application/json',
			...(token && { Authorization: `Bearer ${token}` }),
			...options?.headers,
		},
	});

	// Handle 401 Unauthorized - token expired or invalid
	if (response.status === 401) {
		// Clear auth data
		localStorage.removeItem(TOKEN_KEY);
		localStorage.removeItem(USER_KEY);

		// Redirect to login
		if (window.location.pathname !== '/login') {
			window.location.href = '/login';
		}

		throw new Error('Unauthorized - Please login again');
	}

	// Handle 403 Forbidden - insufficient permissions
	if (response.status === 403) {
		throw new Error('Forbidden - You do not have permission to access this resource');
	}

	if (!response.ok) {
		const errorText = await response.text();
		throw new Error(`API Error (${response.status}): ${errorText || response.statusText}`);
	}

	if (response.status === 204) {
		return {} as T;
	}

	return response.json();
}

// Utility functions for auth data
export const authStorage = {
	// Token management
	setToken(token: string): void {
		localStorage.setItem(TOKEN_KEY, token);
	},

	getToken(): string | null {
		return localStorage.getItem(TOKEN_KEY);
	},

	removeToken(): void {
		localStorage.removeItem(TOKEN_KEY);
	},

	// User data management
	setUser(user: any): void {
		localStorage.setItem(USER_KEY, JSON.stringify(user));
	},

	getUser(): any | null {
		const userStr = localStorage.getItem(USER_KEY);
		if (!userStr) return null;
		try {
			return JSON.parse(userStr);
		} catch {
			localStorage.removeItem(USER_KEY);
			return null;
		}
	},

	removeUser(): void {
		localStorage.removeItem(USER_KEY);
	},

	// Clear all auth data
	clear(): void {
		this.removeToken();
		this.removeUser();
	},

	// Check if authenticated
	isAuthenticated(): boolean {
		const token = this.getToken();
		const user = this.getUser();
		return !!(token && user);
	}
};