import * as React from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Alert from '@mui/material/Alert';
import Link from '@mui/material/Link';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import CircularProgress from '@mui/material/CircularProgress';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { register, clearError } from '../store/slices/authSlice';
import { tenantStorage, API_BASE_URL } from '../services/api';
import { register, clearError } from '../store/slices/authSlice';

export default function RegisterPage() {
	const navigate = useNavigate();
	const dispatch = useAppDispatch();
	const { loading, error, isAuthenticated } = useAppSelector((state) => state.auth);

	const [formData, setFormData] = React.useState({
		username: '',
		email: '',
		password: '',
		confirmPassword: '',
	});
	const [tenants, setTenants] = React.useState<any[]>([]);
	const [selectedTenant, setSelectedTenant] = React.useState<string>('');
	const [loadingTenants, setLoadingTenants] = React.useState(true);

	const [validationError, setValidationError] = React.useState<string | null>(null);

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
		setValidationError(null);

		if (!selectedTenant) {
			setValidationError('Please select a tenant');
			return;
		}

		if (formData.password !== formData.confirmPassword) {
			setValidationError('Passwords do not match');
			return;
		}

		if (formData.password.length < 6) {
			setValidationError('Password must be at least 6 characters');
			return;
		}

		try {
			await dispatch(register({
				username: formData.username,
				email: formData.email,
				password: formData.password,
			})).unwrap();
		} catch (err) {
			// Error is handled by Redux
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
							Create Account
						</Typography>
						<Typography variant="body2" color="text.secondary">
							Sign up to get started with the admin panel
						</Typography>
					</Box>

					{(error || validationError) && (
						<Alert
							severity="error"
							onClose={() => {
								dispatch(clearError());
								setValidationError(null);
							}}
						>
							{validationError || error}
						</Alert>
					)}

					<form onSubmit={handleSubmit}>
						<Stack spacing={2.5}>
							{loadingTenants ? (
								<Box display="flex" justifyContent="center">
									<CircularProgress size={24} />
								</Box>
							) : (
								<FormControl fullWidth required>
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
								fullWidth
								required
								autoFocus
								value={formData.username}
								onChange={(e) => setFormData({ ...formData, username: e.target.value })}
							/>
							<TextField
								label="Email"
								type="email"
								fullWidth
								required
								value={formData.email}
								onChange={(e) => setFormData({ ...formData, email: e.target.value })}
							/>
							<TextField
								label="Password"
								type="password"
								fullWidth
								required
								value={formData.password}
								onChange={(e) => setFormData({ ...formData, password: e.target.value })}
							/>
							<TextField
								label="Confirm Password"
								type="password"
								fullWidth
								required
								value={formData.confirmPassword}
								onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
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
								{loading ? 'Creating account...' : 'Sign Up'}
							</Button>
						</Stack>
					</form>

					<Box sx={{ textAlign: 'center', pt: 1 }}>
						<Typography variant="body2" color="text.secondary">
							Already have an account?{' '}
							<Link component={RouterLink} to="/login" fontWeight={600}>
								Sign in
							</Link>
						</Typography>
					</Box>
				</Stack>
			</Paper>
		</Box>
	);
}