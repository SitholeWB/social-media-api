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
import { format, parseISO, isSameMonth, isSameWeek, startOfMonth, endOfMonth } from 'date-fns';

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

  // Fetch stats on mount and when viewType or selectedDate changes
  React.useEffect(() => {
    if (viewType === 'weekly') {
      dispatch(fetchWeeklyStats(selectedDate));
    } else {
      dispatch(fetchMonthlyStats(selectedDate));
    }
    // Fetch history data for charts
    dispatch(fetchStatsHistory());
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

  const handleRefresh = () => {
    if (viewType === 'weekly') {
      dispatch(fetchWeeklyStats(selectedDate));
    } else {
      dispatch(fetchMonthlyStats(selectedDate));
    }
    dispatch(fetchStatsHistory());
  };

  const handleThisPeriod = () => {
    dispatch(setSelectedDate(format(new Date(), 'yyyy-MM-dd')));
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
    const date = parseISO(dateString);
    return viewType === 'weekly' 
      ? format(date, 'MMM dd')
      : format(date, 'MMM yyyy');
  };

  // Get current period stats
  const getCurrentPeriodStats = (): PeriodStats | null => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    if (!dataPoints || dataPoints.length === 0) return null;
    
    // Sort by date descending (newest first) and get the most recent
    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(b.date).getTime() - new Date(a.date).getTime()
    );
    
    return {
      date: sortedData[0].date,
      activeUsers: sortedData[0].activeUsers,
      newPosts: sortedData[0].newPosts,
      resultingComments: sortedData[0].resultingComments,
      resultingReactions: sortedData[0].resultingReactions,
      totalPosts: sortedData[0].totalPosts,
    };
  };

  // Get previous period stats
  const getPreviousPeriodStats = (): PeriodStats | null => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    if (!dataPoints || dataPoints.length < 2) return null;
    
    // Sort by date descending (newest first) and get the second most recent
    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(b.date).getTime() - new Date(a.date).getTime()
    );
    
    return {
      date: sortedData[1].date,
      activeUsers: sortedData[1].activeUsers,
      newPosts: sortedData[1].newPosts,
      resultingComments: sortedData[1].resultingComments,
      resultingReactions: sortedData[1].resultingReactions,
      totalPosts: sortedData[1].totalPosts,
    };
  };

  // Calculate comparison data between current and previous period
  const calculateComparisonData = () => {
    const current = getCurrentPeriodStats();
    const previous = getPreviousPeriodStats();
    
    if (!current) {
      return {
        activeUsers: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        newPosts: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        comments: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        reactions: { current: 0, previous: 0, change: 0, changePercentage: 0 },
        totalPosts: { current: 0, previous: 0, change: 0, changePercentage: 0 },
      };
    }

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

    // Get all available data points (up to 12) sorted chronologically
    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(a.date).getTime() - new Date(b.date).getTime()
    );

    const labels = sortedData.map(item => {
      const date = parseISO(item.date);
      if (viewType === 'weekly') {
        return format(date, 'MMM dd');
      } else {
        return format(date, 'MMM yyyy');
      }
    });

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
        },
        {
          label: 'Comments',
          data: sortedData.map(item => item.resultingComments),
          borderColor: CHART_COLORS.warning,
          backgroundColor: `${CHART_COLORS.warning}20`,
          tension: 0.3,
          fill: false,
          pointRadius: 4,
          pointHoverRadius: 6,
        },
        {
          label: 'Reactions',
          data: sortedData.map(item => item.resultingReactions),
          borderColor: CHART_COLORS.secondary,
          backgroundColor: `${CHART_COLORS.secondary}20`,
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
    const previousLabel = previous ? formatChartDate(previous.date) : 'Previous Period';
    
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
        },
        {
          label: 'Comments',
          data: [previous?.resultingComments || 0, current.resultingComments],
          backgroundColor: CHART_COLORS.warning,
          borderRadius: 6,
        },
        {
          label: 'Reactions',
          data: [previous?.resultingReactions || 0, current.resultingReactions],
          backgroundColor: CHART_COLORS.secondary,
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

    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(a.date).getTime() - new Date(b.date).getTime()
    );

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

  // Prepare data for engagement rate chart (Comments + Reactions per Post)
  const prepareEngagementRateChartData = (): ChartData<'bar'> => {
    const dataPoints = viewType === 'weekly' ? statsHistory?.weeks : statsHistory?.months;
    
    if (!dataPoints || dataPoints.length === 0) {
      return {
        labels: [],
        datasets: []
      };
    }

    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(a.date).getTime() - new Date(b.date).getTime()
    );

    const engagementRates = sortedData.map(item => {
      if (item.newPosts === 0) return 0;
      return (item.resultingComments + item.resultingReactions) / item.newPosts;
    });

    const labels = sortedData.map(item => formatChartDate(item.date));

    return {
      labels,
      datasets: [
        {
          label: 'Engagement Rate',
          data: engagementRates,
          backgroundColor: engagementRates.map(rate => 
            rate > 5 ? CHART_COLORS.success :
            rate > 2 ? CHART_COLORS.primary :
            rate > 0 ? CHART_COLORS.warning :
            CHART_COLORS.gray
          ),
          borderRadius: 4,
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

  // Calculate averages for the last 12 periods
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

    const sortedData = [...dataPoints].sort((a, b) => 
      new Date(a.date).getTime() - new Date(b.date).getTime()
    );

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

  // Chart options
  const lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
        labels: {
          padding: 15,
          usePointStyle: true,
        }
      },
      tooltip: {
        mode: 'index' as const,
        intersect: false,
        padding: 10,
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
          padding: 8,
        }
      },
      x: {
        grid: {
          display: false,
        },
        ticks: {
          padding: 8,
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
          padding: 15,
          usePointStyle: true,
        }
      },
      tooltip: {
        mode: 'index' as const,
        intersect: false,
        padding: 10,
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
          padding: 8,
        }
      },
      x: {
        grid: {
          display: false,
        },
        ticks: {
          padding: 8,
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
          padding: 15,
          usePointStyle: true,
          boxWidth: 10,
        }
      },
      tooltip: {
        padding: 10,
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

  const comparisonData = calculateComparisonData();
  const averages = calculateAverages();
  const currentPeriodStats = getCurrentPeriodStats();

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

      {/* Period Selector */}
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
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <DateRangeIcon color="primary" />
              <Typography variant="h6" fontWeight={600}>
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
            >
              <ToggleButton value="weekly">Weekly</ToggleButton>
              <ToggleButton value="monthly">Monthly</ToggleButton>
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
              />
            </Grid>
            <Grid size={{ xs: 12, md: 4 }}>
              <Button
                variant="outlined"
                onClick={handleThisPeriod}
                disabled={loading}
                fullWidth
                startIcon={<TodayIcon />}
                sx={{ textTransform: 'none', height: '56px' }}
              >
                Today / This Period
              </Button>
            </Grid>
          </Grid>

          {statsHistory && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: 'text.secondary' }}>
              <InfoIcon fontSize="small" />
              <Typography variant="caption">
                Showing {averages.totalPeriods} {viewType === 'weekly' ? 'weeks' : 'months'} of data (max 12 records)
              </Typography>
            </Box>
          )}

          {statsRecord && (
            <Typography variant="body2" color="text.secondary">
              Showing <strong>{viewType}</strong> stats starting from <strong>{format(new Date(statsRecord.date), 'PPP')}</strong>
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
      {loading && !statsRecord && (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      )}

      {/* Statistics Cards */}
      {statsRecord && currentPeriodStats && (
        <>
          {/* Period Comparison Cards */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
            <TrendingUp color="primary" />
            {viewType === 'weekly' ? 'Week-over-Week' : 'Month-over-Month'} Comparison
          </Typography>

          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2.5,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Active Users
                    </Typography>
                    {getTrendIcon(comparisonData.activeUsers.change)}
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatNumber(comparisonData.activeUsers.current)}
                  </Typography>
                  <Typography variant="body2" color={getChangeColor(comparisonData.activeUsers.change)}>
                    {formatChangeText(comparisonData.activeUsers.change, comparisonData.activeUsers.changePercentage)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Avg: {averages.avgActiveUsers.toFixed(1)}
                  </Typography>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2.5,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      New Posts
                    </Typography>
                    {getTrendIcon(comparisonData.newPosts.change)}
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatNumber(comparisonData.newPosts.current)}
                  </Typography>
                  <Typography variant="body2" color={getChangeColor(comparisonData.newPosts.change)}>
                    {formatChangeText(comparisonData.newPosts.change, comparisonData.newPosts.changePercentage)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Avg: {averages.avgNewPosts.toFixed(1)}
                  </Typography>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2.5,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Comments
                    </Typography>
                    {getTrendIcon(comparisonData.comments.change)}
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatNumber(comparisonData.comments.current)}
                  </Typography>
                  <Typography variant="body2" color={getChangeColor(comparisonData.comments.change)}>
                    {formatChangeText(comparisonData.comments.change, comparisonData.comments.changePercentage)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Avg: {averages.avgComments.toFixed(1)}
                  </Typography>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2.5,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Reactions
                    </Typography>
                    {getTrendIcon(comparisonData.reactions.change)}
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatNumber(comparisonData.reactions.current)}
                  </Typography>
                  <Typography variant="body2" color={getChangeColor(comparisonData.reactions.change)}>
                    {formatChangeText(comparisonData.reactions.change, comparisonData.reactions.changePercentage)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Avg: {averages.avgReactions.toFixed(1)}
                  </Typography>
                </Stack>
              </Paper>
            </Grid>

            <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 2.5,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '100%',
                }}
              >
                <Stack spacing={1}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Posts
                    </Typography>
                    <TimelineIcon color="info" />
                  </Box>
                  <Typography variant="h5" fontWeight={700}>
                    {formatNumber(comparisonData.totalPosts.current)}
                  </Typography>
                  <Typography variant="body2" color="info.main">
                    +{comparisonData.totalPosts.change} this period
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    Cumulative total
                  </Typography>
                </Stack>
              </Paper>
            </Grid>
          </Grid>

          {/* Charts Section */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
            <ShowChartIcon color="primary" />
            Last {averages.totalPeriods} {viewType === 'weekly' ? 'Weeks' : 'Months'} Analysis
          </Typography>

          <Grid container spacing={3} sx={{ mb: 4 }}>
            {/* Full Year Trend Chart */}
            <Grid size={{ xs: 12, lg: 8 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 3,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '450px',
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                  <TimelineIcon fontSize="small" />
                  {viewType === 'weekly' ? '12-Week' : '12-Month'} Trend Overview
                </Typography>
                <Box sx={{ height: 'calc(450px - 60px)' }}>
                  {statsHistory && statsHistory.weeks.length > 0 ? (
                    <Line 
                      data={prepareYearTrendChartData()} 
                      options={lineChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary">
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
                  p: 3,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '450px',
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                  <TrendingUp fontSize="small" />
                  Period Comparison
                </Typography>
                <Box sx={{ height: 'calc(450px - 60px)' }}>
                  {statsHistory && statsHistory.weeks.length > 1 ? (
                    <Bar 
                      data={prepareComparisonBarChartData()} 
                      options={barChartOptions}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary" align="center">
                        Need at least 2 periods<br />for comparison
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
                  p: 3,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '400px',
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                  <PostAddIcon fontSize="small" />
                  Posts Growth & Cumulative Total
                </Typography>
                <Box sx={{ height: 'calc(400px - 60px)' }}>
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

            {/* Engagement Rate Chart */}
            <Grid size={{ xs: 12, lg: 6 }}>
              <Paper
                elevation={0}
                sx={{
                  p: 3,
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  height: '400px',
                }}
              >
                <Typography variant="subtitle1" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
                  <ThumbUpIcon fontSize="small" />
                  Engagement Rate (Comments + Reactions per Post)
                </Typography>
                <Box sx={{ height: 'calc(400px - 60px)' }}>
                  {statsHistory && statsHistory.weeks.length > 0 ? (
                    <Bar 
                      data={prepareEngagementRateChartData()} 
                      options={{
                        ...barChartOptions,
                        plugins: {
                          ...barChartOptions.plugins,
                          tooltip: {
                            ...barChartOptions.plugins?.tooltip,
                            callbacks: {
                              label: (context) => {
                                return `Engagement Rate: ${context.raw.toFixed(2)}`;
                              }
                            }
                          }
                        }
                      }}
                    />
                  ) : (
                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                      <Typography color="text.secondary">
                        Loading engagement data...
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Paper>
            </Grid>
          </Grid>

          {/* Current Period Detailed Stats */}
          <Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
            <CalendarMonthIcon color="primary" />
            Current {viewType === 'weekly' ? 'Week' : 'Month'} Details
          </Typography>

          <Grid container spacing={3} sx={{ mb: 4 }}>
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
                    backgroundColor: 'primary.main',
                  },
                }}
              >
                <Stack spacing={1.5}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Period Summary
                    </Typography>
                    <InfoIcon color="primary" />
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Period Date
                    </Typography>
                    <Typography variant="body1" fontWeight={600}>
                      {currentPeriodStats.date ? format(parseISO(currentPeriodStats.date), 'PPP') : 'N/A'}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Active Users
                    </Typography>
                    <Typography variant="h6" color="primary.main">
                      {formatNumber(currentPeriodStats.activeUsers)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      New Content Created
                    </Typography>
                    <Typography variant="body1">
                      {formatNumber(currentPeriodStats.newPosts)} posts
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>

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
                    backgroundColor: 'success.main',
                  },
                }}
              >
                <Stack spacing={1.5}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Engagement Metrics
                    </Typography>
                    <CommentIcon color="success" />
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Comments Generated
                    </Typography>
                    <Typography variant="h6" color="success.main">
                      {formatNumber(currentPeriodStats.resultingComments)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Reactions Received
                    </Typography>
                    <Typography variant="h6" color="warning.main">
                      {formatNumber(currentPeriodStats.resultingReactions)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Total Engagement
                    </Typography>
                    <Typography variant="body1" fontWeight={600}>
                      {formatNumber(currentPeriodStats.resultingComments + currentPeriodStats.resultingReactions)} interactions
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>

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
                <Stack spacing={1.5}>
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="body2" color="text.secondary">
                      Posts Overview
                    </Typography>
                    <TimelineIcon color="info" />
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Current Period Posts
                    </Typography>
                    <Typography variant="h6" color="info.main">
                      {formatNumber(currentPeriodStats.newPosts)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Cumulative Total
                    </Typography>
                    <Typography variant="h6" color="teal">
                      {formatNumber(currentPeriodStats.totalPosts)}
                    </Typography>
                  </Box>
                  <Box>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Growth This Period
                    </Typography>
                    <Typography variant="body1" fontWeight={600} color="success.main">
                      +{calculateGrowth(currentPeriodStats.totalPosts, currentPeriodStats.newPosts)}%
                    </Typography>
                  </Box>
                </Stack>
              </Paper>
            </Grid>
          </Grid>

          {/* Reaction Breakdown */}
          {statsRecord.reactionBreakdown && statsRecord.reactionBreakdown.length > 0 && (
            <Grid container spacing={3} sx={{ mb: 3 }}>
              <Grid size={{ xs: 12, md: 6 }}>
                <Paper
                  elevation={0}
                  sx={{
                    p: 3,
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 2,
                  }}
                >
                  <Typography variant="h6" fontWeight={600} sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
                    <ThumbUpIcon color="primary" />
                    Reaction Breakdown
                  </Typography>

                  <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
                    {statsRecord.reactionBreakdown.map((item, index) => (
                      <Paper
                        key={index}
                        elevation={0}
                        sx={{
                          p: 2,
                          minWidth: '80px',
                          textAlign: 'center',
                          border: '1px solid',
                          borderColor: 'divider',
                          borderRadius: 2,
                          backgroundColor: 'action.hover'
                        }}
                      >
                        <Typography variant="h4" sx={{ mb: 1 }}>
                          {item.emoji}
                        </Typography>
                        <Typography variant="h6" fontWeight={700}>
                          {formatNumber(item.count)}
                        </Typography>
                      </Paper>
                    ))}
                  </Box>
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
                  }}
                >
                  <Typography variant="h6" fontWeight={600} sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
                    <CalendarMonthIcon color="primary" />
                    Reaction Distribution
                  </Typography>
                  <Box sx={{ height: '250px' }}>
                    <Doughnut 
                      data={prepareReactionBreakdownChartData()} 
                      options={doughnutChartOptions}
                    />
                  </Box>
                </Paper>
              </Grid>
            </Grid>
          )}
        </>
      )}

      {/* Empty State */}
      {!loading && !statsRecord && !error && (
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
            No stats found for this {viewType} period
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Try refreshing or selecting a different date
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