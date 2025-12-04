import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import FormControlLabel from '@mui/material/FormControlLabel';
import Divider from '@mui/material/Divider';
import Avatar from '@mui/material/Avatar';
import Chip from '@mui/material/Chip';
import LightModeIcon from '@mui/icons-material/LightMode';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LogoutIcon from '@mui/icons-material/Logout';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { logout } from '../store/slices/authSlice';
import { toggleThemeMode } from '../store/slices/settingsSlice';
import { useNavigate } from 'react-router-dom';

export default function SettingsPage() {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { user } = useAppSelector((state) => state.auth);
    const { themeMode } = useAppSelector((state) => state.settings);

    const handleLogout = () => {
        dispatch(logout());
        navigate('/login');
    };

    const handleThemeToggle = () => {
        dispatch(toggleThemeMode());
    };

    return (
        <Box sx={{ width: '100%' }}>
            <PageHeader title="Settings" />

            <Stack spacing={3} sx={{ maxWidth: 800 }}>
                {/* User Profile Section */}
                <Paper
                    elevation={0}
                    sx={{
                        p: 3,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                    }}
                >
                    <Typography variant="h6" fontWeight={600} gutterBottom>
                        Profile
                    </Typography>
                    <Divider sx={{ my: 2 }} />
                    <Stack direction="row" spacing={3} alignItems="center">
                        <Avatar
                            sx={{
                                width: 80,
                                height: 80,
                                bgcolor: 'primary.main',
                                fontSize: '2rem',
                                fontWeight: 700,
                            }}
                        >
                            {user?.username?.charAt(0).toUpperCase() || 'U'}
                        </Avatar>
                        <Box>
                            <Typography variant="h6" fontWeight={600}>
                                {user?.username || 'User'}
                            </Typography>
                            <Typography variant="body2" color="text.secondary" gutterBottom>
                                {user?.email || 'user@example.com'}
                            </Typography>
                            <Chip label="Admin" color="primary" size="small" sx={{ mt: 1 }} />
                        </Box>
                    </Stack>
                </Paper>

                {/* Appearance Section */}
                <Paper
                    elevation={0}
                    sx={{
                        p: 3,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                    }}
                >
                    <Typography variant="h6" fontWeight={600} gutterBottom>
                        Appearance
                    </Typography>
                    <Divider sx={{ my: 2 }} />
                    <Stack spacing={2}>
                        <FormControlLabel
                            control={
                                <Switch
                                    checked={themeMode === 'dark'}
                                    onChange={handleThemeToggle}
                                    icon={<LightModeIcon />}
                                    checkedIcon={<DarkModeIcon />}
                                />
                            }
                            label={
                                <Box>
                                    <Typography variant="body1" fontWeight={500}>
                                        Dark Mode
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        Toggle between light and dark theme
                                    </Typography>
                                </Box>
                            }
                        />
                        <Box
                            sx={{
                                p: 2,
                                border: '1px solid',
                                borderColor: 'divider',
                                borderRadius: 1,
                                bgcolor: 'background.default',
                            }}
                        >
                            <Typography variant="body2" color="text.secondary">
                                Current theme: <strong>{themeMode === 'dark' ? 'Dark' : 'Light'}</strong>
                            </Typography>
                        </Box>
                    </Stack>
                </Paper>

                {/* Account Actions Section */}
                <Paper
                    elevation={0}
                    sx={{
                        p: 3,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                    }}
                >
                    <Typography variant="h6" fontWeight={600} gutterBottom>
                        Account
                    </Typography>
                    <Divider sx={{ my: 2 }} />
                    <Button
                        variant="outlined"
                        color="error"
                        startIcon={<LogoutIcon />}
                        onClick={handleLogout}
                        sx={{
                            textTransform: 'none',
                            fontWeight: 600,
                        }}
                    >
                        Sign Out
                    </Button>
                </Paper>
            </Stack>
        </Box>
    );
}
