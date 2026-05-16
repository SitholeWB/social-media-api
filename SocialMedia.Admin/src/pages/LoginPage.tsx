import * as React from 'react';
import Divider from '@mui/material/Divider';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Alert from '@mui/material/Alert';
import Link from '@mui/material/Link';
import GoogleIcon from '@mui/icons-material/Google';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import CircularProgress from '@mui/material/CircularProgress';
import { signInWithPopup, GoogleAuthProvider } from 'firebase/auth';
import { auth, googleProvider } from '../firebaseConfig';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { login, loginWithGoogle, clearError } from '../store/slices/authSlice';
import { tenantStorage, API_BASE_URL } from '../services/api';

export default function LoginPage() {
	const navigate = useNavigate();
	const dispatch = useAppDispatch();
	const { loading, error, isAuthenticated } = useAppSelector((state) => state.auth);

	const [formData, setFormData] = React.useState({
		username: '',
		password: '',
	});
	const [tenants, setTenants] = React.useState<any[]>([]);
	const [selectedTenant, setSelectedTenant] = React.useState<string>('');
	const [loadingTenants, setLoadingTenants] = React.useState(true);

	React.useEffect(() => {
		if (isAuthenticated) {
			navigate('/');
		}
	}, [isAuthenticated, navigate]);

	React.useEffect(() => {
		return () => {
			dispatch(clearError());
		};
	}, [dispatch]);

	React.useEffect(() => {
		const fetchTenants = async () => {
			try {
				const response = await fetch(`${API_BASE_URL}/api/v1/tenants`);
				if (response.ok) {
					const data = await response.json();
					setTenants(data);
					if (data.length > 0) {
						setSelectedTenant(data[0].id);
						tenantStorage.setTenantId(data[0].id);
					}
				}
			} catch (error) {
				console.error('Failed to fetch tenants', error);
			} finally {
				setLoadingTenants(false);
			}
		};
		fetchTenants();
	}, []);

	const handleTenantChange = (event: any) => {
		const newTenant = event.target.value;
		setSelectedTenant(newTenant);
		tenantStorage.setTenantId(newTenant);
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		if (!selectedTenant) {
			alert('Please select a tenant');
			return;
		}
		try {
			await dispatch(login(formData)).unwrap();
		} catch (err) {
			// Error is handled by Redux
		}
	};

	const handleGoogleLogin = async () => {
		try {
			const result = await signInWithPopup(auth, googleProvider);
			// This gives you a Google Access Token. You can use it to access the Google API.
			const credential = GoogleAuthProvider.credentialFromResult(result);
			const token = credential?.idToken;

			if (token) {
				dispatch(loginWithGoogle(token));
			} else {
				// Fallback to Firebase ID Token if google credential token is missing (though our backend expects Google ID Token, verify if this is compatible)
				// For now, let's assume we need the Google ID Token strictly.
				console.error("No Google ID Token found in credential");
			}
		} catch (error: any) {
			console.error(error);
			// Dispatch error to redux if needed or show local alert
		}
	};

	return (
		<Box
			sx={{
				minHeight: '100vh',
				display: 'flex',
				alignItems: 'center',
				justifyContent: 'center',
				background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
				p: 2,
			}}
		>
			<Paper
				elevation={24}
				sx={{
					p: 4,
					maxWidth: 440,
					width: '100%',
					borderRadius: 3,
				}}
			>
				<Stack spacing={3}>
					<Box sx={{ textAlign: 'center' }}>
						<Typography variant="h4" fontWeight={700} gutterBottom>
							Welcome Back
						</Typography>
						<Typography variant="body2" color="text.secondary">
							Sign in to access the admin panel
						</Typography>
					</Box>

					{error && (
						<Alert severity="error" onClose={() => dispatch(clearError())}>
							{error}
						</Alert>
					)}

					<form onSubmit={handleSubmit}>
						<Stack spacing={2.5}>
							{loadingTenants ? (
								<Box display="flex" justifyContent="center">
									<CircularProgress size={24} />
								</Box>
							) : (
								<FormControl fullWidth variant="standard" required>
									<InputLabel id="tenant-select-label">Tenant</InputLabel>
									<Select
										labelId="tenant-select-label"
										value={selectedTenant}
										onChange={handleTenantChange}
										label="Tenant"
									>
										{tenants.map((t) => (
											<MenuItem key={t.id} value={t.id}>{t.name}</MenuItem>
										))}
									</Select>
								</FormControl>
							)}

							<TextField
								label="Username"
								type="text"
								fullWidth
								required
								autoFocus
								variant="standard"
								value={formData.username}
								onChange={(e) => setFormData({ ...formData, username: e.target.value })}
							/>
							<TextField
								label="Password"
								type="password"
								fullWidth
								required
								variant="standard"
								value={formData.password}
								onChange={(e) => setFormData({ ...formData, password: e.target.value })}
							/>
							<Button
								type="submit"
								variant="contained"
								size="large"
								fullWidth
								disabled={loading}
								sx={{
									py: 1.5,
									textTransform: 'none',
									fontWeight: 600,
									fontSize: '1rem',
								}}
							>
								{loading ? 'Signing in...' : 'Sign In'}
							</Button>

							<Divider>OR</Divider>

							<Button
								variant="outlined"
								size="large"
								fullWidth
								startIcon={<GoogleIcon />}
								onClick={handleGoogleLogin}
								sx={{
									py: 1.5,
									textTransform: 'none',
									fontWeight: 600,
									fontSize: '1rem',
									borderColor: 'divider',
									color: 'text.primary',
									'&:hover': {
										borderColor: 'text.primary',
										backgroundColor: 'action.hover',
									},
								}}
							>
								Sign in with Google
							</Button>
						</Stack>
					</form>

					<Box sx={{ textAlign: 'center', pt: 1 }}>
						<Typography variant="body2" color="text.secondary">
							Don't have an account?{' '}
							<Link component={RouterLink} to="/register" fontWeight={600}>
								Sign up
							</Link>
						</Typography>
					</Box>
				</Stack>
			</Paper>
		</Box>
	);
}