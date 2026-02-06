// pages/DashboardPage.tsx
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchWeeklyStats, fetchMonthlyStats, setViewType, setSelectedDate } from '../store/slices/dashboardSlice';
import { useAuth } from '../hooks/useAuth';
import { format } from 'date-fns';

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
import PageHeader from '../components/PageHeader';

export default function DashboardPage() {
	const dispatch = useAppDispatch();
	const { isAdmin } = useAuth();
	const { statsRecord, loading, error, viewType, selectedDate } = useAppSelector((state) => state.dashboard);

	// Fetch stats on mount and when viewType or selectedDate changes
	React.useEffect(() => {
		if (viewType === 'weekly') {
			dispatch(fetchWeeklyStats(selectedDate));
		} else {
			dispatch(fetchMonthlyStats(selectedDate));
		}
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
			{statsRecord && (
				<>
					{/* Activity Statistics */}
					<Typography variant="h6" fontWeight={600} sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
						<PeopleIcon color="primary" />
						User Activity for this {viewType === 'weekly' ? 'Week' : 'Month'}
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
											Active Users
										</Typography>
										<PeopleIcon color="primary" />
									</Box>
									<Typography variant="h4" fontWeight={700}>
										{formatNumber(statsRecord.activeUsers)}
									</Typography>
									<Typography variant="body2" color="text.secondary">
										Users performing actions during this period
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
											New Posts
										</Typography>
										<PostAddIcon color="success" />
									</Box>
									<Typography variant="h4" fontWeight={700}>
										{formatNumber(statsRecord.newPosts)}
									</Typography>
									<Typography variant="body2" color="success.main">
										{calculateGrowth(statsRecord.totalPosts, statsRecord.newPosts)}% growth
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
						{/* Global Posts */}
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
											Total Posts (Cumulative)
										</Typography>
										<TimelineIcon color="info" />
									</Box>
									<Typography variant="h4" fontWeight={700}>
										{formatNumber(statsRecord.totalPosts)}
									</Typography>
									<Typography variant="body2" color="info.main">
										Global post count at period end
									</Typography>
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
											New Comments
										</Typography>
										<CommentIcon color="warning" />
									</Box>
									<Typography variant="h4" fontWeight={700}>
										{formatNumber(statsRecord.resultingComments)}
									</Typography>
									<Typography variant="body2" color="warning.main">
										In response to posts this period
									</Typography>
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
											New Reactions
										</Typography>
										<ThumbUpIcon color="error" />
									</Box>
									<Typography variant="h4" fontWeight={700}>
										{formatNumber(statsRecord.resultingReactions)}
									</Typography>
									<Typography variant="body2" color="error.main">
										Likes & reactions across the platform
									</Typography>
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
											{formatNumber(statsRecord.newPosts)}
										</Typography>
									</Box>
									<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
										<Typography variant="body1" color="text.secondary">
											New Comments
										</Typography>
										<Typography variant="h6" color="warning.main">
											{formatNumber(statsRecord.resultingComments)}
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
											{formatNumber(statsRecord.resultingReactions)}
										</Typography>
									</Box>
									<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
										<Typography variant="body1" color="text.secondary">
											Engagement Rate
										</Typography>
										<Typography variant="h6" color="success.main">
											{statsRecord.newPosts > 0
												? ((statsRecord.resultingReactions + statsRecord.resultingComments) / statsRecord.newPosts).toFixed(2)
												: '0.00'
											}
										</Typography>
									</Box>
								</Stack>
							</Grid>
						</Grid>
					</Paper>

					{/* Reaction Breakdown */}
					{statsRecord.reactionBreakdown && statsRecord.reactionBreakdown.length > 0 && (
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