// pages/DashboardPage.tsx
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchDashboardStats, setDateRange, resetDateRange } from '../store/slices/dashboardSlice';
import { useAuth } from '../hooks/useAuth';
import { format, subDays } from 'date-fns';

// Material-UI Components
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Stack from '@mui/material/Stack';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert';
import RefreshIcon from '@mui/icons-material/Refresh';
import TodayIcon from '@mui/icons-material/Today';
import DateRangeIcon from '@mui/icons-material/DateRange';
import TimelineIcon from '@mui/icons-material/Timeline';
import PeopleIcon from '@mui/icons-material/People';
import ForumIcon from '@mui/icons-material/Forum';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import CommentIcon from '@mui/icons-material/Comment';
import PostAddIcon from '@mui/icons-material/PostAdd';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import PageHeader from '../components/PageHeader';

// Chart Components (optional - install recharts if needed)
// import { LineChart, Line, BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default function DashboardPage() {
    const dispatch = useAppDispatch();
    const { user, isAdmin } = useAuth();
    const { stats, loading, error, dateRange } = useAppSelector((state) => state.dashboard);
    
    const [localDateRange, setLocalDateRange] = React.useState(dateRange);

    // Fetch stats on mount and when dateRange changes
    React.useEffect(() => {
        dispatch(fetchDashboardStats(dateRange));
    }, [dispatch, dateRange]);

    const handleDateChange = (field: 'startDate' | 'endDate', value: string) => {
        setLocalDateRange(prev => ({ ...prev, [field]: value }));
    };

    const handleApplyDateRange = () => {
        dispatch(setDateRange(localDateRange));
    };

    const handleResetDateRange = () => {
        const thirtyDaysAgo = format(subDays(new Date(), 30), 'yyyy-MM-dd');
        const today = format(new Date(), 'yyyy-MM-dd');
        
        setLocalDateRange({
            startDate: thirtyDaysAgo,
            endDate: today,
        });
        
        dispatch(resetDateRange());
    };

    const handleRefresh = () => {
        dispatch(fetchDashboardStats(dateRange));
    };

    const handleLast7Days = () => {
        const sevenDaysAgo = format(subDays(new Date(), 7), 'yyyy-MM-dd');
        const today = format(new Date(), 'yyyy-MM-dd');
        
        setLocalDateRange({
            startDate: sevenDaysAgo,
            endDate: today,
        });
        
        dispatch(setDateRange({
            startDate: sevenDaysAgo,
            endDate: today,
        }));
    };

    const handleLast30Days = () => {
        const thirtyDaysAgo = format(subDays(new Date(), 30), 'yyyy-MM-dd');
        const today = format(new Date(), 'yyyy-MM-dd');
        
        setLocalDateRange({
            startDate: thirtyDaysAgo,
            endDate: today,
        });
        
        dispatch(setDateRange({
            startDate: thirtyDaysAgo,
            endDate: today,
        }));
    };

    const handleThisMonth = () => {
        const today = new Date();
        const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        
        setLocalDateRange({
            startDate: format(firstDayOfMonth, 'yyyy-MM-dd'),
            endDate: format(today, 'yyyy-MM-dd'),
        });
        
        dispatch(setDateRange({
            startDate: format(firstDayOfMonth, 'yyyy-MM-dd'),
            endDate: format(today, 'yyyy-MM-dd'),
        }));
    };

    // Calculate growth percentages
    const calculateGrowth = (total: number, newInPeriod: number) => {
        if (total - newInPeriod === 0) return 100; // Avoid division by zero
        return ((newInPeriod / (total - newInPeriod)) * 100).toFixed(1);
    };

    // Format numbers with commas
    const formatNumber = (num: number) => {
        return num.toLocaleString('en-US');
    };
/*
    if (!isAdmin) {
        return (
            <Box sx={{ p: 4, textAlign: 'center' }}>
                <Alert severity="warning" sx={{ mb: 3 }}>
                    You need administrator privileges to access the dashboard.
                </Alert>
                <Typography variant="body2" color="text.secondary">
                    Contact your administrator for access.
                </Typography>
            </Box>
        );
    }
*/

    return (
        <Box sx={{ width: '100%' }}>
            <PageHeader
                title="Dashboard"
                subtitle="Platform Statistics & Analytics"
                action={
                    <Stack direction="row" spacing={2}>
                        <Button
                            variant="outlined"
                            startIcon={<RefreshIcon />}
                            onClick={handleRefresh}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Refresh
                        </Button>
                    </Stack>
                }
            />

            {/* Date Range Selector */}
            <Paper
                elevation={0}
                sx={{
                    p: 3,
                    mb: 3,
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 2,
                }}
            >
                <Stack spacing={3}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <DateRangeIcon color="primary" />
                        <Typography variant="h6" fontWeight={600}>
                            Date Range Filter
                        </Typography>
                    </Box>

                    <Grid container spacing={2} alignItems="center">
                        <Grid size={{ xs: 12, md: 5 }}>
                            <TextField
                                label="Start Date"
                                type="date"
                                fullWidth
                                value={localDateRange.startDate || ''}
                                onChange={(e) => handleDateChange('startDate', e.target.value)}
                                InputLabelProps={{ shrink: true }}
                                disabled={loading}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, md: 5 }}>
                            <TextField
                                label="End Date"
                                type="date"
                                fullWidth
                                value={localDateRange.endDate || ''}
                                onChange={(e) => handleDateChange('endDate', e.target.value)}
                                InputLabelProps={{ shrink: true }}
                                disabled={loading}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, md: 2 }}>
                            <Button
                                variant="contained"
                                onClick={handleApplyDateRange}
                                disabled={loading}
                                fullWidth
                                sx={{ textTransform: 'none', height: '56px' }}
                            >
                                Apply
                            </Button>
                        </Grid>
                    </Grid>

                    {/* Quick Date Range Buttons */}
                    <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={handleLast7Days}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Last 7 Days
                        </Button>
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={handleLast30Days}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Last 30 Days
                        </Button>
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={handleThisMonth}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            This Month
                        </Button>
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={handleResetDateRange}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Reset
                        </Button>
                    </Box>

                    {dateRange.startDate && dateRange.endDate && (
                        <Typography variant="body2" color="text.secondary">
                            Showing data from <strong>{dateRange.startDate}</strong> to <strong>{dateRange.endDate}</strong>
                        </Typography>
                    )}
                </Stack>
            </Paper>

            {/* Error Display */}
            {error && (
                <Alert severity="error" sx={{ mb: 3 }}>
                    {error}
                </Alert>
            )}

            {/* Loading State */}
            {loading && !stats && (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                    <CircularProgress />
                </Box>
            )}

            {/* Statistics Cards */}
            {stats && (
                <>
                    {/* User Statistics */}
                    <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                        <PeopleIcon color="primary" />
                        User Statistics
                    </Typography>
                    
                    <Grid container spacing={3} sx={{ mb: 4 }}>
                        <Grid size={{ xs: 12, md: 6 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 2,
                                    height: '100%',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    '&::before': {
                                        content: '""',
                                        position: 'absolute',
                                        top: 0,
                                        left: 0,
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: 'primary.main',
                                    },
                                }}
                            >
                                <Stack spacing={1}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Typography variant="body2" color="text.secondary">
                                            Total Users
                                        </Typography>
                                        <PeopleIcon color="primary" />
                                    </Box>
                                    <Typography variant="h4" fontWeight={700}>
                                        {formatNumber(stats.totalUsers)}
                                    </Typography>
                                    <Typography variant="body2" color="success.main">
                                        Active Users: {formatNumber(stats.activeUsers)} ({((stats.activeUsers / stats.totalUsers) * 100).toFixed(1)}%)
                                    </Typography>
                                </Stack>
                            </Paper>
                        </Grid>

                        <Grid size={{ xs: 12, md: 6 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 2,
                                    height: '100%',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    '&::before': {
                                        content: '""',
                                        position: 'absolute',
                                        top: 0,
                                        left: 0,
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: 'success.main',
                                    },
                                }}
                            >
                                <Stack spacing={1}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Typography variant="body2" color="text.secondary">
                                            User Activity
                                        </Typography>
                                        <TimelineIcon color="success" />
                                    </Box>
                                    <Typography variant="h4" fontWeight={700}>
                                        {formatNumber(stats.activeUsers)}
                                    </Typography>
                                    <Typography variant="body2" color="primary.main">
                                        {stats.activeUsers === stats.totalUsers ? 'All users active!' : `${stats.totalUsers - stats.activeUsers} inactive users`}
                                    </Typography>
                                </Stack>
                            </Paper>
                        </Grid>
                    </Grid>

                    {/* Content Statistics */}
                    <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                        <ForumIcon color="primary" />
                        Content Statistics
                    </Typography>
                    
                    <Grid container spacing={3} sx={{ mb: 4 }}>
                        {/* Posts */}
                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 2,
                                    height: '100%',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    '&::before': {
                                        content: '""',
                                        position: 'absolute',
                                        top: 0,
                                        left: 0,
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: 'info.main',
                                    },
                                }}
                            >
                                <Stack spacing={1}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Typography variant="body2" color="text.secondary">
                                            Total Posts
                                        </Typography>
                                        <PostAddIcon color="info" />
                                    </Box>
                                    <Typography variant="h4" fontWeight={700}>
                                        {formatNumber(stats.totalPosts)}
                                    </Typography>
                                    <Stack direction="row" spacing={2} alignItems="center">
                                        <Typography variant="body2" color="success.main">
                                            +{formatNumber(stats.newPostsInPeriod)} new
                                        </Typography>
                                        <Typography variant="caption" color="text.secondary">
                                            {calculateGrowth(stats.totalPosts, stats.newPostsInPeriod)}% growth
                                        </Typography>
                                    </Stack>
                                </Stack>
                            </Paper>
                        </Grid>

                        {/* Comments */}
                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 2,
                                    height: '100%',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    '&::before': {
                                        content: '""',
                                        position: 'absolute',
                                        top: 0,
                                        left: 0,
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: 'warning.main',
                                    },
                                }}
                            >
                                <Stack spacing={1}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Typography variant="body2" color="text.secondary">
                                            Total Comments
                                        </Typography>
                                        <CommentIcon color="warning" />
                                    </Box>
                                    <Typography variant="h4" fontWeight={700}>
                                        {formatNumber(stats.totalComments)}
                                    </Typography>
                                    <Stack direction="row" spacing={2} alignItems="center">
                                        <Typography variant="body2" color="success.main">
                                            +{formatNumber(stats.newCommentsInPeriod)} new
                                        </Typography>
                                        <Typography variant="caption" color="text.secondary">
                                            {calculateGrowth(stats.totalComments, stats.newCommentsInPeriod)}% growth
                                        </Typography>
                                    </Stack>
                                </Stack>
                            </Paper>
                        </Grid>

                        {/* Reactions */}
                        <Grid size={{ xs: 12, md: 4 }}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 2,
                                    height: '100%',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    '&::before': {
                                        content: '""',
                                        position: 'absolute',
                                        top: 0,
                                        left: 0,
                                        width: '100%',
                                        height: '4px',
                                        backgroundColor: 'error.main',
                                    },
                                }}
                            >
                                <Stack spacing={1}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                        <Typography variant="body2" color="text.secondary">
                                            Total Reactions
                                        </Typography>
                                        <ThumbUpIcon color="error" />
                                    </Box>
                                    <Typography variant="h4" fontWeight={700}>
                                        {formatNumber(stats.totalReactions)}
                                    </Typography>
                                    <Stack direction="row" spacing={2} alignItems="center">
                                        <Typography variant="body2" color="success.main">
                                            +{formatNumber(stats.newReactionsInPeriod)} new
                                        </Typography>
                                        <Typography variant="caption" color="text.secondary">
                                            {calculateGrowth(stats.totalReactions, stats.newReactionsInPeriod)}% growth
                                        </Typography>
                                    </Stack>
                                </Stack>
                            </Paper>
                        </Grid>
                    </Grid>

                    {/* Summary & Metrics */}
                    <Paper
                        elevation={0}
                        sx={{
                            p: 3,
                            border: '1px solid',
                            borderColor: 'divider',
                            borderRadius: 2,
                            mb: 3,
                        }}
                    >
                        <Typography variant="h6" fontWeight={600} sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
                            <TrendingUpIcon color="primary" />
                            Period Summary
                        </Typography>

                        <Grid container spacing={3}>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <Stack spacing={2}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Typography variant="body1" color="text.secondary">
                                            New Posts
                                        </Typography>
                                        <Typography variant="h6" color="info.main">
                                            {formatNumber(stats.newPostsInPeriod)}
                                        </Typography>
                                    </Box>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Typography variant="body1" color="text.secondary">
                                            New Comments
                                        </Typography>
                                        <Typography variant="h6" color="warning.main">
                                            {formatNumber(stats.newCommentsInPeriod)}
                                        </Typography>
                                    </Box>
                                </Stack>
                            </Grid>
                            <Grid size={{ xs: 12, md: 6 }}>
                                <Stack spacing={2}>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Typography variant="body1" color="text.secondary">
                                            New Reactions
                                        </Typography>
                                        <Typography variant="h6" color="error.main">
                                            {formatNumber(stats.newReactionsInPeriod)}
                                        </Typography>
                                    </Box>
                                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                        <Typography variant="body1" color="text.secondary">
                                            Engagement Rate
                                        </Typography>
                                        <Typography variant="h6" color="success.main">
                                            {stats.totalPosts > 0 
                                                ? ((stats.totalReactions + stats.totalComments) / stats.totalPosts).toFixed(2)
                                                : '0.00'
                                            }
                                        </Typography>
                                    </Box>
                                </Stack>
                            </Grid>
                        </Grid>
                    </Paper>

                    {/* Charts Section (Optional - if you install recharts) */}
                    {/*
                    <Paper
                        elevation={0}
                        sx={{
                            p: 3,
                            border: '1px solid',
                            borderColor: 'divider',
                            borderRadius: 2,
                            mb: 3,
                        }}
                    >
                        <Typography variant="h6" fontWeight={600} sx={{ mb: 3 }}>
                            Activity Trends
                        </Typography>
                        <ResponsiveContainer width="100%" height={300}>
                            <LineChart data={chartData}>
                                <CartesianGrid strokeDasharray="3 3" />
                                <XAxis dataKey="date" />
                                <YAxis />
                                <Tooltip />
                                <Legend />
                                <Line type="monotone" dataKey="posts" stroke="#8884d8" activeDot={{ r: 8 }} />
                                <Line type="monotone" dataKey="comments" stroke="#82ca9d" />
                                <Line type="monotone" dataKey="reactions" stroke="#ffc658" />
                            </LineChart>
                        </ResponsiveContainer>
                    </Paper>
                    */}
                </>
            )}

            {/* Empty State */}
            {!loading && !stats && !error && (
                <Paper
                    elevation={0}
                    sx={{
                        p: 4,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                        textAlign: 'center',
                    }}
                >
                    <Typography variant="h6" color="text.secondary" gutterBottom>
                        No dashboard data available
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                        Try refreshing or adjusting the date range
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<RefreshIcon />}
                        onClick={handleRefresh}
                        sx={{ textTransform: 'none' }}
                    >
                        Refresh Dashboard
                    </Button>
                </Paper>
            )}
        </Box>
    );
}