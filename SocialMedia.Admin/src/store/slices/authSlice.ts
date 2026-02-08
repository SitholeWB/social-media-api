import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { authService, LoginRequest, RegisterRequest, AuthResponse, User } from '../../services/authService';

interface AuthState {
	user: User | null;
	token: string | null;
	isAuthenticated: boolean;
	loading: boolean;
	error: string | null;
}

const initialState: AuthState = {
	user: null,
	token: authService.getToken(),
	isAuthenticated: authService.isAuthenticated(),
	loading: false,
	error: null,
};

export const login = createAsyncThunk<AuthResponse, LoginRequest>(
	'auth/login',
	async (request) => {
		return await authService.login(request);
	}
);

export const register = createAsyncThunk<AuthResponse, RegisterRequest>(
	'auth/register',
	async (request) => {
		return await authService.register(request);
	}
);

export const loginWithGoogle = createAsyncThunk<AuthResponse, string>(
	'auth/loginWithGoogle',
	async (idToken, { rejectWithValue }) => {
		try {
			return await authService.loginWithGoogle(idToken);
		} catch (error: any) {
			return rejectWithValue(error.response?.data?.error || 'Google login failed');
		}
	}
);

export const fetchCurrentUser = createAsyncThunk<User | null>(
	'auth/fetchCurrentUser',
	async () => {
		return await authService.getCurrentUser();
	}
);

const authSlice = createSlice({
	name: 'auth',
	initialState,
	reducers: {
		logout: (state) => {
			authService.logout();
			state.user = null;
			state.token = null;
			state.isAuthenticated = false;
			state.error = null;
		},
		setCredentials: (state, action: PayloadAction<{ user: User; token: string }>) => {
			state.user = action.payload.user;
			state.token = action.payload.token;
			state.isAuthenticated = true;
		},
		clearError: (state) => {
			state.error = null;
		},
	},
	extraReducers: (builder) => {
		builder
			// Login
			.addCase(login.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(login.fulfilled, (state, action: PayloadAction<AuthResponse>) => {
				state.loading = false;
				state.token = action.payload.token;
				state.isAuthenticated = true;
				state.user = {
					id: action.payload.userId || action.payload.id,
					email: action.payload.email,
					username: action.payload.username,
				};
			})
			.addCase(login.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Login failed';
			})
			// Google Login
			.addCase(loginWithGoogle.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(loginWithGoogle.fulfilled, (state, action: PayloadAction<AuthResponse>) => {
				state.loading = false;
				state.token = action.payload.token;
				state.isAuthenticated = true;
				state.user = {
					id: action.payload.userId || action.payload.id,
					email: action.payload.email,
					username: action.payload.username,
				};
			})
			.addCase(loginWithGoogle.rejected, (state, action) => {
				state.loading = false;
				state.error = action.payload as string || 'Google login failed';
			})
			// Register
			.addCase(register.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(register.fulfilled, (state, action: PayloadAction<AuthResponse>) => {
				state.loading = false;
				state.token = action.payload.token;
				state.isAuthenticated = true;
				state.user = {
					id: action.payload.userId || action.payload.id,
					email: action.payload.email,
					username: action.payload.username,
				};
			})
			.addCase(register.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Registration failed';
			})
			// Fetch current user
			.addCase(fetchCurrentUser.fulfilled, (state, action) => {
				if (action.payload) {
					state.user = action.payload;
				}
			});
	},
});

export const { logout, setCredentials, clearError } = authSlice.actions;
export default authSlice.reducer;