// pages/SettingsPage.tsx
import React from 'react';
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
import EditIcon from '@mui/icons-material/Edit';
import SecurityIcon from '@mui/icons-material/Security';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import PersonIcon from '@mui/icons-material/Person';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { toggleThemeMode } from '../store/slices/settingsSlice';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { CircularProgress } from '@mui/material';

export default function SettingsPage() {
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { themeMode } = useAppSelector((state) => state.settings);
    
    // Use the new useAuth hook
    const { 
        user, 
        logout, 
        isLoading, 
        isAdmin, 
        isModerator, 
        isUser,
        hasRole,
        hasPermission 
    } = useAuth();

    const handleLogout = async () => {
        try {
            await logout();
            navigate('/login');
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    const handleThemeToggle = () => {
        dispatch(toggleThemeMode());
    };

    const handleEditProfile = () => {
        navigate('/profile/edit');
    };

    const handleChangePassword = () => {
        navigate('/change-password');
    };

    // Show loading state while auth is initializing
    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    // If not authenticated, redirect to login
    if (!user) {
        navigate('/login');
        return null;
    }

    // Get role display name
    const getRoleDisplay = () => {
        if (isAdmin) return 'Administrator';
        if (isModerator) return 'Moderator';
        if (isUser) return 'User';
        return 'Member';
    };

    // Get role color
    const getRoleColor = () => {
        if (isAdmin) return 'error';
        if (isModerator) return 'warning';
        if (isUser) return 'primary';
        return 'default';
    };

    // Get role icon
    const getRoleIcon = () => {
        if (isAdmin) return <AdminPanelSettingsIcon />;
        if (isModerator) return <SecurityIcon />;
        return <PersonIcon />;
    };

    return (
        <Box sx={{ width: '100%' }}>
            <PageHeader title="Settings" />

            <Stack spacing={3} sx={{ maxWidth: 800, mx: 'auto' }}>
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
                    <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
                        <Typography variant="h6" fontWeight={600}>
                            Profile
                        </Typography>
                        <Button
                            variant="outlined"
                            size="small"
                            startIcon={<EditIcon />}
                            onClick={handleEditProfile}
                            sx={{ textTransform: 'none' }}
                        >
                            Edit Profile
                        </Button>
                    </Stack>
                    <Divider sx={{ mb: 3 }} />
                    
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
                        <Box sx={{ flex: 1 }}>
                            <Stack spacing={1}>
                                <Typography variant="h6" fontWeight={600}>
                                    {user?.username || 'User'}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    {user?.email || 'No email provided'}
                                </Typography>
                                <Stack direction="row" spacing={1} alignItems="center">
                                    <Chip 
                                        label={getRoleDisplay()} 
                                        color={getRoleColor() as any} 
                                        size="small" 
                                        icon={getRoleIcon()}
                                    />
                                    {user?.permissions && user.permissions.length > 0 && (
                                        <Typography variant="caption" color="text.secondary">
                                            • {user.permissions.length} permission{user.permissions.length !== 1 ? 's' : ''}
                                        </Typography>
                                    )}
                                </Stack>
                            </Stack>
                        </Box>
                    </Stack>
                    
                    {/* Additional user info */}
                    <Box sx={{ mt: 3, p: 2, bgcolor: 'action.hover', borderRadius: 1 }}>
                        <Stack spacing={1}>
                            <Typography variant="body2" color="text.secondary">
                                <strong>User ID:</strong> {user.id}
                            </Typography>
                            {user.role && (
                                <Typography variant="body2" color="text.secondary">
                                    <strong>Role:</strong> {user.role}
                                </Typography>
                            )}
                        </Stack>
                    </Box>
                </Paper>

                {/* Account Security Section */}
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
                        Security
                    </Typography>
                    <Divider sx={{ my: 2 }} />
                    <Stack spacing={2}>
                        <Button
                            variant="outlined"
                            startIcon={<SecurityIcon />}
                            onClick={handleChangePassword}
                            sx={{ 
                                textTransform: 'none',
                                justifyContent: 'flex-start',
                                width: 'fit-content'
                            }}
                        >
                            Change Password
                        </Button>
                        <Typography variant="body2" color="text.secondary">
                            Last updated: {new Date().toLocaleDateString()}
                        </Typography>
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

                {/* Permissions Section (Visible to admins/moderators) */}
                {(isAdmin || isModerator) && user?.permissions && user.permissions.length > 0 && (
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
                            Permissions
                        </Typography>
                        <Divider sx={{ my: 2 }} />
                        <Stack spacing={1}>
                            {user.permissions.map((permission, index) => (
                                <Chip
                                    key={index}
                                    label={permission}
                                    size="small"
                                    variant="outlined"
                                    sx={{ mr: 1, mb: 1 }}
                                />
                            ))}
                        </Stack>
                    </Paper>
                )}

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
                        Account Actions
                    </Typography>
                    <Divider sx={{ my: 2 }} />
                    <Stack spacing={2}>
                        <Button
                            variant="outlined"
                            color="error"
                            startIcon={<LogoutIcon />}
                            onClick={handleLogout}
                            sx={{
                                textTransform: 'none',
                                fontWeight: 600,
                                width: 'fit-content'
                            }}
                        >
                            Sign Out
                        </Button>
                        <Typography variant="body2" color="text.secondary">
                            Signing out will clear your session and require you to login again.
                        </Typography>
                    </Stack>
                </Paper>

                {/* Session Info Section */}
                <Paper
                    elevation={0}
                    sx={{
                        p: 3,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                        bgcolor: 'action.hover',
                    }}
                >
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Session Information
                    </Typography>
                    <Stack spacing={1}>
                        <Typography variant="body2" color="text.secondary">
                            <strong>Logged in as:</strong> {user.username}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            <strong>Session started:</strong> {new Date().toLocaleString()}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            <strong>Authentication method:</strong> Email/Password
                        </Typography>
                    </Stack>
                </Paper>
            </Stack>
        </Box>
    );
}