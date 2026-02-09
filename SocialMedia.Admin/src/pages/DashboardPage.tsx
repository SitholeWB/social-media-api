// pages/DashboardPage.tsx
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { 
  fetchWeeklyStats, 
  fetchMonthlyStats, 
  fetchStatsHistory,
  setViewType, 
  setSelectedDate 
} from '../store/slices/dashboardSlice';
import { useAuth } from '../hooks/useAuth';
import { format, parseISO, isValid } from 'date-fns';

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
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import TodayIcon from '@mui/icons-material/Today';
import DateRangeIcon from '@mui/icons-material/DateRange';
import TimelineIcon from '@mui/icons-material/Timeline';
import PeopleIcon from '@mui/icons-material/People';
import ForumIcon from '@mui/icons-material/Forum';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import CommentIcon from '@mui/icons-material/Comment';
import PostAddIcon from '@mui/icons-material/PostAdd';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import ShowChartIcon from '@mui/icons-material/ShowChart';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import TrendingUp from '@mui/icons-material/TrendingUp';
import TrendingDown from '@mui/icons-material/TrendingDown';
import TrendingFlat from '@mui/icons-material/TrendingFlat';
import InfoIcon from '@mui/icons-material/Info';
import PageHeader from '../components/PageHeader';

// Chart Components
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
  ChartOptions,
  ChartData
} from 'chart.js';
import { Line, Bar, Doughnut } from 'react-chartjs-2';

// Register ChartJS components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
);

// Chart color palette
const CHART_COLORS = {
  primary: '#1976d2',
  secondary: '#dc004e',
  success: '#2e7d32',
  warning: '#ed6c02',
  info: '#0288d1',
  lightBlue: '#42a5f5',
  lightGreen: '#4caf50',
  lightRed: '#ef5350',
  lightOrange: '#ff9800',
  lightPurple: '#9c27b0',
  gray: '#9e9e9e',
  teal: '#009688',
};

interface ComparisonData {
  current: number;
  previous: number;
  change: number;
  changePercentage: number;
}

interface PeriodStats {
  date: string;
  activeUsers: number;
  newPosts: number;
  resultingComments: number;
  resultingReactions: number;
  totalPosts: number;
}

// Safe date formatting function
const safeFormatDate = (dateString: string | null | undefined, dateFormat: string): string => {
  if (!dateString) return 'N/A';
  
  try {
    const date = parseISO(dateString);
    if (!isValid(date)) return 'Invalid Date';
    return format(date, dateFormat);
  } catch (error) {
    console.error('Error formatting date:', error, dateString);
    return 'Date Error';
  }
};

export default function DashboardPage() {
  const dispatch = useAppDispatch();
  const { isAdmin } = useAuth();
  const { 
    statsRecord, 
    statsHistory,
    loading, 
    error, 
    viewType, 
    selectedDate 
  } = useAppSelector((state) => state.dashboard);

  // Track if data is loaded
  const [isInitialized, setIsInitialized] = React.useState(false);

  // Fetch stats on mount and when viewType or selectedDate changes
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        if (viewType === 'weekly') {
          await dispatch(fetchWeeklyStats(selectedDate));
        } else {
          await dispatch(fetchMonthlyStats(selectedDate));
        }
        await dispatch(fetchStatsHistory());
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
      } finally {
        setIsInitialized(true);
      }
    };

    fetchData();
  }, [dispatch, viewType, selectedDate]);

  const handleViewTypeChange = (
    _event: React.MouseEvent<HTMLElement>,
    newViewType: 'weekly' | 'monthly' | null,
  ) => {
    if (newViewType !== null) {
      dispatch(setViewType(newViewType));
    }
  };

  const handleDateChange = (value: string) => {
    dispatch(setSelectedDate(value));
  };

  const handleRefresh = async () => {
    try {
      if (viewType === 'weekly') {
        await dispatch(fetchWeeklyStats(selectedDate));
      } else {
        await dispatch(fetchMonthlyStats(selectedDate));
      }
      await dispatch(fetchStatsHistory());
    } catch (error) {
      console.error('Error refreshing data:', error);
    }
  };

  const handleThisPeriod = () => {
    const today = new Date();
    dispatch(setSelectedDate(format(today, 'yyyy-MM-dd')));
  };

  // Calculate growth percentages
  const calculateGrowth = (total: number, newInPeriod: number) => {
    if (total === 0 || total === newInPeriod) return '0';
    const previous = total - newInPeriod;
    if (previous <= 0) return '100';
    return ((newInPeriod / previous) * 100).toFixed(1);
  };

  // Format numbers with commas
  const formatNumber = (num: number) => {
    return num.toLocaleString('en-US');
  };

  // Format date for display
  const formatChartDate = (dateString: string) => {
    return safeFormatDate(dateString, viewType === 'weekly' ? 'MMM dd' : 'MMM yyyy');
  };

  // Get current period stats
  const getCurrentPeriodStats = (): PeriodStats | null => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    if (!dataPoints || dataPoints.length === 0) return null;
    
    // Sort by date descending (newest first) and get the most recent
    const sortedData = [...dataPoints].sort((a, b) => {
      const dateA = parseISO(a.date);
      const dateB = parseISO(b.date);
      return dateB.getTime() - dateA.getTime();
    });
    
    return sortedData[0] || null;
  };

  // Get previous period stats
  const getPreviousPeriodStats = (): PeriodStats | null => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    if (!dataPoints || dataPoints.length < 2) return null;
    
    // Sort by date descending (newest first) and get the second most recent
    const sortedData = [...dataPoints].sort((a, b) => {
      const dateA = parseISO(a.date);
      const dateB = parseISO(b.date);
      return dateB.getTime() - dateA.getTime();
    });
    
    return sortedData[1] || null;
  };

  // Calculate comparison data between current and previous period
  const calculateComparisonData = () => {
    const current = getCurrentPeriodStats();
    
    if (!current) {
      return {
        activeUsers: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        newPosts: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        comments: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        reactions: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        totalPosts: { current: 0, previous: 0, change: 0, changePercentage: 0 },
      };
    }

    const previous = getPreviousPeriodStats();

    const calculateChange = (currentVal: number, previousVal: number) => {
      const change = currentVal - previousVal;
      const changePercentage = previousVal !== 0 ? (change / previousVal) * 100 : currentVal > 0 ? 100 : 0;
      return { change, changePercentage };
    };

    const activeUsersChange = calculateChange(current.activeUsers, previous?.activeUsers || 0);
    const newPostsChange = calculateChange(current.newPosts, previous?.newPosts || 0);
    const commentsChange = calculateChange(current.resultingComments, previous?.resultingComments || 0);
    const reactionsChange = calculateChange(current.resultingReactions, previous?.resultingReactions || 0);
    const totalPostsChange = calculateChange(current.totalPosts, previous?.totalPosts || 0);

    return {
      activeUsers: {
        current: current.activeUsers,
        previous: previous?.activeUsers || 0,
        ...activeUsersChange,
      },
      newPosts: {
        current: current.newPosts,
        previous: previous?.newPosts || 0,
        ...newPostsChange,
      },
      comments: {
        current: current.resultingComments,
        previous: previous?.resultingComments || 0,
        ...commentsChange,
      },
      reactions: {
        current: current.resultingReactions,
        previous: previous?.resultingReactions || 0,
        ...reactionsChange,
      },
      totalPosts: {
        current: current.totalPosts,
        previous: previous?.totalPosts || 0,
        ...totalPostsChange,
      },
    };
  };

  // Prepare data for full year trend chart (12 months/weeks)
  const prepareYearTrendChartData = (): ChartData<'line'> => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    
    if (!dataPoints || dataPoints.length === 0) {
      return {
        labels: [],
        datasets: []
      };
    }

    // Get all available data points sorted chronologically
    const sortedData = [...dataPoints].sort((a, b) => {
      const dateA = parseISO(a.date);
      const dateB = parseISO(b.date);
      return dateA.getTime() - dateB.getTime();
    });

    const labels = sortedData.map(item => formatChartDate(item.date));

    return {
      labels,
      datasets: [
        {
          label: 'Active Users',
          data: sortedData.map(item => item.activeUsers),
          borderColor: CHART_COLORS.primary,
          backgroundColor: `${CHART_COLORS.primary}20`,
          tension: 0.3,
          fill: false,
          pointRadius: 4,
          pointHoverRadius: 6,
        },
        {
          label: 'New Posts',
          data: sortedData.map(item => item.newPosts),
          borderColor: CHART_COLORS.success,
          backgroundColor: `${CHART_COLORS.success}20`,
          tension: 0.3,
          fill: false,
          pointRadius: 4,
          pointHoverRadius: 6,
        }
      ]
    };
  };

  // Prepare data for period-to-period comparison bar chart
  const prepareComparisonBarChartData = (): ChartData<'bar'> => {
    const current = getCurrentPeriodStats();
    const previous = getPreviousPeriodStats();
    
    if (!current) {
      return {
        labels: [],
        datasets: []
      };
    }

    const currentLabel = formatChartDate(current.date);
    const previousLabel = previous ? formatChartDate(previous.date) : 'Previous';
    
    return {
      labels: [previousLabel, currentLabel],
      datasets: [
        {
          label: 'Active Users',
          data: [previous?.activeUsers || 0, current.activeUsers],
          backgroundColor: CHART_COLORS.primary,
          borderRadius: 6,
        },
        {
          label: 'New Posts',
          data: [previous?.newPosts || 0, current.newPosts],
          backgroundColor: CHART_COLORS.success,
          borderRadius: 6,
        }
      ]
    };
  };

  // Prepare data for cumulative posts growth chart
  const prepareCumulativePostsChartData = (): ChartData<'line'> => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    
    if (!dataPoints || dataPoints.length === 0) {
      return {
        labels: [],
        datasets: []
      };
    }

    const sortedData = [...dataPoints].sort((a, b) => {
      const dateA = parseISO(a.date);
      const dateB = parseISO(b.date);
      return dateA.getTime() - dateB.getTime();
    });

    const labels = sortedData.map(item => formatChartDate(item.date));

    return {
      labels,
      datasets: [
        {
          label: 'Cumulative Posts',
          data: sortedData.map(item => item.totalPosts),
          borderColor: CHART_COLORS.teal,
          backgroundColor: `${CHART_COLORS.teal}20`,
          tension: 0.3,
          fill: true,
          pointRadius: 4,
          pointHoverRadius: 6,
        }
      ]
    };
  };

  // Prepare data for reaction breakdown doughnut chart
  const prepareReactionBreakdownChartData = (): ChartData<'doughnut'> => {
    if (!statsRecord || !statsRecord.reactionBreakdown || statsRecord.reactionBreakdown.length === 0) {
      return {
        labels: [],
        datasets: []
      };
    }

    const emojiColors = [
      CHART_COLORS.primary,
      CHART_COLORS.secondary,
      CHART_COLORS.success,
      CHART_COLORS.warning,
      CHART_COLORS.info,
      CHART_COLORS.lightRed,
      CHART_COLORS.lightOrange,
      CHART_COLORS.lightPurple,
    ];

    return {
      labels: statsRecord.reactionBreakdown.map(item => item.emoji),
      datasets: [
        {
          data: statsRecord.reactionBreakdown.map(item => item.count),
          backgroundColor: statsRecord.reactionBreakdown.map((_, index) => 
            emojiColors[index % emojiColors.length]
          ),
          borderColor: statsRecord.reactionBreakdown.map((_, index) => 
            emojiColors[index % emojiColors.length] + '80'
          ),
          borderWidth: 1,
        }
      ]
    };
  };

  // Calculate averages for the available periods
  const calculateAverages = () => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    
    if (!dataPoints || dataPoints.length === 0) {
      return {
        avgActiveUsers: 0,
        avgNewPosts: 0,
        avgComments: 0,
        avgReactions: 0,
        totalPeriods: 0,
      };
    }

    const sortedData = [...dataPoints].sort((a, b) => {
      const dateA = parseISO(a.date);
      const dateB = parseISO(b.date);
      return dateA.getTime() - dateB.getTime();
    });

    const totalActiveUsers = sortedData.reduce((sum, item) => sum + item.activeUsers, 0);
    const totalNewPosts = sortedData.reduce((sum, item) => sum + item.newPosts, 0);
    const totalComments = sortedData.reduce((sum, item) => sum + item.resultingComments, 0);
    const totalReactions = sortedData.reduce((sum, item) => sum + item.resultingReactions, 0);

    return {
      avgActiveUsers: totalActiveUsers / sortedData.length,
      avgNewPosts: totalNewPosts / sortedData.length,
      avgComments: totalComments / sortedData.length,
      avgReactions: totalReactions / sortedData.length,
      totalPeriods: sortedData.length,
    };
  };

  // Chart options optimized for mobile
  const lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
        labels: {
          padding: 10,
          usePointStyle: true,
          boxWidth: 6,
        }
      },
      tooltip: {
        mode: 'index' as const,
        intersect: false,
        padding: 8,
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        titleColor: '#333',
        bodyColor: '#666',
        borderColor: '#ddd',
        borderWidth: 1,
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: {
          color: 'rgba(0, 0, 0, 0.05)',
        },
        ticks: {
          precision: 0,
          padding: 5,
        }
      },
      x: {
        grid: {
          display: false,
        },
        ticks: {
          padding: 5,
          maxRotation: 45,
          minRotation: 45,
        }
      }
    },
    interaction: {
      intersect: false,
      mode: 'nearest' as const,
    },
  };

  const barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
        labels: {
          padding: 10,
          usePointStyle: true,
          boxWidth: 6,
        }
      },
      tooltip: {
        mode: 'index' as const,
        intersect: false,
        padding: 8,
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        titleColor: '#333',
        bodyColor: '#666',
        borderColor: '#ddd',
        borderWidth: 1,
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: {
          color: 'rgba(0, 0, 0, 0.05)',
        },
        ticks: {
          precision: 0,
          padding: 5,
        }
      },
      x: {
        grid: {
          display: false,
        },
        ticks: {
          padding: 5,
        }
      }
    },
  };

  const doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom' as const,
        labels: {
          padding: 10,
          usePointStyle: true,
          boxWidth: 8,
        }
      },
      tooltip: {
        padding: 8,
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        titleColor: '#333',
        bodyColor: '#666',
        borderColor: '#ddd',
        borderWidth: 1,
        callbacks: {
          label: (context) => {
            const label = context.label || '';
            const value = context.raw as number;
            const total = (context.dataset.data as number[]).reduce((a, b) => a + b, 0);
            const percentage = Math.round((value / total) * 100);
            return `${label}: ${value} (${percentage}%)`;
          }
        }
      }
    }
  };

  // Get trend icon based on change
  const getTrendIcon = (change: number) => {
    if (change > 0) return <TrendingUp color="success" />;
    if (change < 0) return <TrendingDown color="error" />;
    return <TrendingFlat color="disabled" />;
  };

  // Format change text
  const formatChangeText = (change: number, changePercentage: number) => {
    const sign = change > 0 ? '+' : '';
    const percentage = Math.abs(changePercentage).toFixed(1);
    return `${sign}${change} (${sign}${percentage}%)`;
  };

  // Get change color
  const getChangeColor = (change: number) => {
    return change > 0 ? 'success.main' : change < 0 ? 'error.main' : 'text.secondary';
  };

  if (!isAdmin) {
    return (
      <Box sx={{ p: 3, textAlign: 'center' }}>
        <Alert severity="warning" sx={{ mb: 2 }}>
          You need administrator privileges to access the dashboard.
        </Alert>
        <Typography variant="body2" color="text.secondary">
          Contact your administrator for access.
        </Typography>
      </Box>
    );
  }

  const comparisonData = calculateComparisonData();
  const averages = calculateAverages();
  const currentPeriodStats = getCurrentPeriodStats();

  return (
    <Box sx={{ width: '100%', p: { xs: 1, sm: 2, md: 3 } }}>
      <PageHeader
        title="Dashboard"
        subtitle="Platform Statistics & Analytics"
        action={
          <Stack direction="row" spacing={1}>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon />}
              onClick={handleRefresh}
              disabled={loading}
              sx={{ textTransform: 'none' }}
              size="small"
            >
              Refresh
            </Button>
          </Stack>
        }
      />

      {/* Period Selector */}
      <Paper
        elevation={0}
        sx={{
          p: { xs: 2, sm: 3 },
          mb: 3,
          border: '1px solid',
          borderColor: 'divider',
          borderRadius: 2,
        }}
      >
        <Stack spacing={2}>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <DateRangeIcon color="primary" />
              <Typography variant="h6" fontWeight={600} sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                Stats Period
              </Typography>
            </Box>

            <ToggleButtonGroup
              color="primary"
              value={viewType}
              exclusive
              onChange={handleViewTypeChange}
              aria-label="View Type"
              size="small"
              sx={{ flexWrap: 'wrap' }}
            >
              <ToggleButton value="weekly" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>Weekly</ToggleButton>
              <ToggleButton value="monthly" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>Monthly</ToggleButton>
            </ToggleButtonGroup>
          </Box>

          <Grid container spacing={2} alignItems="center">
            <Grid size={{ xs: 12, md: 8 }}>
              <TextField
                label="Select Date in Period"
                type="date"
                fullWidth
                value={selectedDate}
                onChange={(e) => handleDateChange(e.target.value)}
                InputLabelProps={{ shrink: true }}
                disabled={loading}
                helperText={`Showing stats for the ${viewType} containing this date.`}
                size="small"
              />
            </Grid>
            <Grid size={{ xs: 12, md: 4 }}>
              <Button
                variant="outlined"
                onClick={handleThisPeriod}
                disabled={loading}
                fullWidth
                startIcon={<TodayIcon />}
                sx={{ textTransform: 'none', height: '40px' }}
                size="small"
              >
                This Period
              </Button>
            </Grid>
          </Grid>

          {statsHistory && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: 'text.secondary' }}>
              <InfoIcon fontSize="small" />
              <Typography variant="caption">
                Showing {averages.totalPeriods} {viewType === 'weekly' ? 'weeks' : 'months'} of data
              </Typography>
            </Box>
          )}

          {statsRecord && statsRecord.date && (
            <Typography variant="body2" color="text.secondary" sx={{ fontSize: { xs: '0.75rem', sm: '0.875rem' } }}>
              Showing <strong>{viewType}</strong> stats starting from{' '}
              <strong>{safeFormatDate(statsRecord.date, 'PPP')}</strong>
            </Typography>
          )}
        </Stack>
      </Paper>

      {/* Error Display */}
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Loading State */}
      {loading && !statsRecord && (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
          <CircularProgress />
        </Box>
      )}

      {/* Statistics Cards */}
      {statsRecord && currentPeriodStats && isInitialized && (
        <>
          {/* Period Comparison Cards - Mobile Optimized */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <TrendingUp color="primary" />
            <Box component="span" sx={{ fontSize: { xs: '0.875rem', sm: '1rem' } }}>
              {viewType === 'weekly' ? 'Week-over-Week' : 'Month-over-Month'} Comparison
            </Box>
          </Typography>

          <Grid container spacing={2} sx={{ mb: 3 }}>
            {[
              { key: 'activeUsers', label: 'Active Users', icon: <PeopleIcon /> },
              { key: 'newPosts', label: 'New Posts', icon: <PostAddIcon /> },
              { key: 'comments', label: 'Comments', icon: <CommentIcon /> },
              { key: 'reactions', label: 'Reactions', icon: <ThumbUpIcon /> },
              { key: 'totalPosts', label: 'Total Posts', icon: <TimelineIcon /> },
            ].map((metric) => {
              const data = comparisonData[metric.key as keyof typeof comparisonData];
              const avgKey = `avg${metric.label.replace(' ', '')}` as keyof typeof averages;
              const average = averages[avgKey] || 0;
              
              return (
                <Grid size={{ xs: 12, sm: 6, md: 2.4 }} key={metric.key}>
                  <Paper
                    elevation={0}
                    sx={{
                      p: 2,
                      border: '1px solid',
                      borderColor: 'divider',
                      borderRadius: 2,
                      height: '100%',
                      minHeight: '120px',
                    }}
                  >
                    <Stack spacing={0.5}>
                      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                        <Typography variant="caption" color="text.secondary" sx={{ fontSize: '0.7rem' }}>
                          {metric.label}
                        </Typography>
                        <Box sx={{ fontSize: '0.875rem' }}>
                          {metric.icon}
                        </Box>
                      </Box>
                      <Typography variant="h6" fontWeight={700} sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
                        {formatNumber(data.current)}
                      </Typography>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        {getTrendIcon(data.change)}
                        <Typography 
                          variant="caption" 
                          sx={{ 
                            fontSize: '0.7rem',
                            color: getChangeColor(data.change)
                          }}
                        >
                          {formatChangeText(data.change, data.changePercentage)}
                        </Typography>
                      </Box>
                      <Typography variant="caption" color="text.secondary" sx={{ fontSize: '0.65rem' }}>
                        Avg: {average.toFixed(1)}
                      </Typography>
                    </Stack>
                  </Paper>
                </Grid>
              );
            })}
          </Grid>

          {/* Charts Section - Mobile Optimized */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <ShowChartIcon color="primary" />
            <Box component="span" sx={{ fontSize: { xs: '0.875rem', sm: '1rem' } }}>
              Historical Analysis
            </Box>
          </Typography>

          <Grid container spacing={2} sx={{ mb: 3 }}>
            {/* Full Year Trend Chart */}
            <Grid size={{ xs: 12, lg: 8 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: { xs: '300px', sm: '350px', md: '400px' },
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 1, fontSize: { xs: '0.875rem', sm: '1rem' } }}>
                  {viewType === 'weekly' ? 'Weekly' : 'Monthly'} Trend Overview
                </Typography>
                <Box sx={{ height: 'calc(100% - 40px)' }}>
                  {statsHistory && statsHistory.weeks.length > 0 ? (
                    <Line 
                      data={prepareYearTrendChartData()} 
                      options={lineChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary" align="center">
                        Loading trend data...
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Paper>
            </Grid>

            {/* Period Comparison Chart */}
            <Grid size={{ xs: 12, lg: 4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: { xs: '300px', sm: '350px', md: '400px' },
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 1, fontSize: { xs: '0.875rem', sm: '1rem' } }}>
                  Period Comparison
                </Typography>
                <Box sx={{ height: 'calc(100% - 40px)' }}>
                  {statsHistory && statsHistory.weeks.length > 1 ? (
                    <Bar 
                      data={prepareComparisonBarChartData()} 
                      options={barChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary" align="center">
                        Need at least 2 periods
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Paper>
            </Grid>

            {/* Cumulative Posts Growth */}
            <Grid size={{ xs: 12, lg: 6 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: { xs: '300px', sm: '350px' },
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 1, fontSize: { xs: '0.875rem', sm: '1rem' } }}>
                  Cumulative Posts Growth
                </Typography>
                <Box sx={{ height: 'calc(100% - 40px)' }}>
                  {statsHistory && statsHistory.weeks.length > 0 ? (
                    <Line 
                      data={prepareCumulativePostsChartData()} 
                      options={lineChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary">
                        Loading posts data...
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Paper>
            </Grid>

            {/* Reaction Breakdown */}
            <Grid size={{ xs: 12, lg: 6 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: { xs: '300px', sm: '350px' },
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 1, fontSize: { xs: '0.875rem', sm: '1rem' } }}>
                  Reaction Distribution
                </Typography>
                <Box sx={{ height: 'calc(100% - 40px)' }}>
                  {statsRecord.reactionBreakdown && statsRecord.reactionBreakdown.length > 0 ? (
                    <Doughnut 
                      data={prepareReactionBreakdownChartData()} 
                      options={doughnutChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary" align="center">
                        No reaction data available
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Paper>
            </Grid>
          </Grid>

          {/* Current Period Details - Mobile Optimized */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <CalendarMonthIcon color="primary" />
            <Box component="span" sx={{ fontSize: { xs: '0.875rem', sm: '1rem' } }}>
              Current {viewType === 'weekly' ? 'Week' : 'Month'} Details
            </Box>
          </Typography>

          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid size={{ xs: 12, md: 4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Typography variant="subtitle2" color="text.secondary" sx={{ fontSize: '0.75rem' }}>
                    Period Summary
                  </Typography>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Date
                    </Typography>
                    <Typography variant="body2" fontWeight={600}>
                      {safeFormatDate(currentPeriodStats.date, 'PPP')}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Active Users
                    </Typography>
                    <Typography variant="body1" color="primary.main">
                      {formatNumber(currentPeriodStats.activeUsers)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      New Posts
                    </Typography>
                    <Typography variant="body1">
                      {formatNumber(currentPeriodStats.newPosts)}
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Typography variant="subtitle2" color="text.secondary" sx={{ fontSize: '0.75rem' }}>
                    Engagement
                  </Typography>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Comments
                    </Typography>
                    <Typography variant="body1" color="success.main">
                      {formatNumber(currentPeriodStats.resultingComments)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Reactions
                    </Typography>
                    <Typography variant="body1" color="warning.main">
                      {formatNumber(currentPeriodStats.resultingReactions)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Total Engagement
                    </Typography>
                    <Typography variant="body2" fontWeight={600}>
                      {formatNumber(currentPeriodStats.resultingComments + currentPeriodStats.resultingReactions)}
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Typography variant="subtitle2" color="text.secondary" sx={{ fontSize: '0.75rem' }}>
                    Posts Overview
                  </Typography>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      This Period
                    </Typography>
                    <Typography variant="body1" color="info.main">
                      {formatNumber(currentPeriodStats.newPosts)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Cumulative Total
                    </Typography>
                    <Typography variant="body1" color="teal">
                      {formatNumber(currentPeriodStats.totalPosts)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Growth
                    </Typography>
                    <Typography variant="body2" fontWeight={600} color="success.main">
                      +{calculateGrowth(currentPeriodStats.totalPosts, currentPeriodStats.newPosts)}%
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>
          </Grid>
        </>
      )}

      {/* Empty State */}
      {!loading && !statsRecord && !error && isInitialized && (
        <Paper
          elevation={0}
          sx={{
            p: 3,
            border: '1px solid',
            borderColor: 'divider',
            borderRadius: 2,
            textAlign: 'center',
          }}
        >
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No stats found for this {viewType} period
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Try refreshing or selecting a different date
          </Typography>
          <Button
            variant="contained"
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            sx={{ textTransform: 'none' }}
            size="small"
          >
            Refresh Dashboard
          </Button>
        </Paper>
      )}
    </Box>
  );
}