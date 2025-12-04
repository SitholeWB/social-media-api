import * as React from 'react';
import { GoogleLogin } from '@react-oauth/google';
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
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { login, loginWithGoogle, clearError } from '../store/slices/authSlice';

export default function LoginPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { loading, error, isAuthenticated } = useAppSelector((state) => state.auth);

    const [formData, setFormData] = React.useState({
        username: '',
        password: '',
    });

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

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await dispatch(login(formData)).unwrap();
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
                            <TextField
                                label="Username"
                                type="text"
                                fullWidth
                                required
                                autoFocus
                                value={formData.username}
                                onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                            />
                            <TextField
                                label="Password"
                                type="password"
                                fullWidth
                                required
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

                            <Box sx={{ display: 'flex', justifyContent: 'center' }}>
                                <GoogleLogin
                                    onSuccess={(credentialResponse) => {
                                        if (credentialResponse.credential) {
                                            dispatch(loginWithGoogle(credentialResponse.credential));
                                        }
                                    }}
                                    onError={() => {
                                        console.log('Login Failed');
                                    }}
                                />
                            </Box>
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
