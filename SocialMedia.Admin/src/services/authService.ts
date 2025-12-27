// services/authService.ts - Updated to work with your fetchJson
import { fetchJson, authStorage, API_BASE_URL } from './api';

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
	id: string;
	role?: string;
	permissions?: string[];
	expiresAt?: string;
}

export interface User {
	id: string;
	email: string;
	username: string;
	role?: string;
	permissions?: string[];
	token?: string;
	expiresAt?: string;
}

export const authService = {
	async login(request: LoginRequest): Promise<AuthResponse> {
		const response = await fetchJson<AuthResponse>('/api/v1.0/auth/login', {
			method: 'POST',
			body: JSON.stringify(request),
		});

		if (response.token) {
			// Save token
			authStorage.setToken(response.token);

			// Create and save user object
			const user: User = {
				id: response.id || response.userId,
				email: response.email,
				username: response.username,
				role: response.role,
				permissions: response.permissions,
				token: response.token,
				expiresAt: response.expiresAt,
			};
			authStorage.setUser(user);
		}

		return response;
	},

	async loginWithGoogle(idToken: string): Promise<AuthResponse> {
		const response = await fetchJson<AuthResponse>('/api/v1.0/auth/google', {
			method: 'POST',
			body: JSON.stringify({ idToken }),
		});

		if (response.token) {
			authStorage.setToken(response.token);

			const user: User = {
				id: response.id || response.userId,
				email: response.email,
				username: response.username,
				role: response.role,
				permissions: response.permissions,
				token: response.token,
				expiresAt: response.expiresAt,
			};
			authStorage.setUser(user);
		}

		return response;
	},

	async register(request: RegisterRequest): Promise<AuthResponse> {
		const response = await fetchJson<AuthResponse>('/api/v1.0/auth/register', {
			method: 'POST',
			body: JSON.stringify(request),
		});

		if (response.token) {
			authStorage.setToken(response.token);

			const user: User = {
				id: response.id || response.userId,
				email: response.email,
				username: response.username,
				role: response.role,
				permissions: response.permissions,
				token: response.token,
				expiresAt: response.expiresAt,
			};
			authStorage.setUser(user);
		}

		return response;
	},

	logout(): void {
		authStorage.clear();
		// Optionally call logout API if you have one
		// fetchJson('/api/v1.0/auth/logout', { method: 'POST' }).catch(console.error);
	},

	getToken(): string | null {
		return authStorage.getToken();
	},

	getUser(): User | null {
		return authStorage.getUser();
	},

	isAuthenticated(): boolean {
		return authStorage.isAuthenticated();
	},

	async getCurrentUser(): Promise<User | null> {
		const token = authStorage.getToken();
		const existingUser = authStorage.getUser();

		if (!token) {
			authStorage.clear();
			return null;
		}

		try {
			// If we already have user data, return it
			if (existingUser) {
				return existingUser;
			}

			// Otherwise fetch from API
			const response = await fetchJson<User>('/api/v1.0/auth/me');

			// Add token to user object
			const userWithToken = {
				...response,
				token: token
			};

			// Save to storage
			authStorage.setUser(userWithToken);

			return userWithToken;
		} catch (error: any) {
			if (error.message.includes('Unauthorized') || error.message.includes('401')) {
				authStorage.clear();
				if (window.location.pathname !== '/login') {
					window.location.href = '/login';
				}
			}
			return null;
		}
	},

	// Optional: Token refresh function
	async refreshToken(): Promise<AuthResponse | null> {
		try {
			const response = await fetchJson<AuthResponse>('/api/v1.0/auth/refresh', {
				method: 'POST',
			});

			if (response.token) {
				authStorage.setToken(response.token);

				// Update user with new token
				const user = authStorage.getUser();
				if (user) {
					user.token = response.token;
					if (response.expiresAt) {
						user.expiresAt = response.expiresAt;
					}
					authStorage.setUser(user);
				}

				return response;
			}

			return null;
		} catch (error) {
			console.error('Failed to refresh token:', error);
			authStorage.clear();
			throw error;
		}
	},

	// Check if token is expired
	isTokenExpired(): boolean {
		const user = authStorage.getUser();
		if (!user?.expiresAt) return false;

		const expiresAt = new Date(user.expiresAt);
		const now = new Date();
		return now >= expiresAt;
	},

	// Get remaining token lifetime in milliseconds
	getTokenLifetime(): number | null {
		const user = authStorage.getUser();
		if (!user?.expiresAt) return null;

		const expiresAt = new Date(user.expiresAt);
		const now = new Date();
		return expiresAt.getTime() - now.getTime();
	}
};