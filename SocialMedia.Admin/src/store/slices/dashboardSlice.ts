// store/slices/dashboardSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { dashboardService, StatsHistory, DateRange, StatsRecord } from '../../services/dashboardService';

interface DashboardState {
	statsHistory: StatsHistory | null; // Keep for backward compatibility if needed
	statsRecord: StatsRecord | null;
	loading: boolean;
	error: string | null;
	dateRange: DateRange;
	viewType: 'weekly' | 'monthly';
	selectedDate: string;
}

const initialState: DashboardState = {
	statsHistory: null,
	statsRecord: null,
	loading: false,
	error: null,
	viewType: 'weekly',
	selectedDate: new Date().toISOString().split('T')[0],
	dateRange: {
		// Default to last 30 days
		startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
		endDate: new Date().toISOString().split('T')[0],
	},
};

export const fetchStatsHistory = createAsyncThunk(
	'dashboard/fetchStatsHistory',
	async () => {
		const stats = await dashboardService.getStatsHistory();
		return stats;
	}
);

export const fetchWeeklyStats = createAsyncThunk(
	'dashboard/fetchWeeklyStats',
	async (date?: string) => {
		const stats = await dashboardService.getWeeklyStats(date);
		return stats;
	}
);

export const fetchMonthlyStats = createAsyncThunk(
	'dashboard/fetchMonthlyStats',
	async (date?: string) => {
		const stats = await dashboardService.getMonthlyStats(date);
		return stats;
	}
);

const dashboardSlice = createSlice({
	name: 'dashboard',
	initialState,
	reducers: {
		setDateRange: (state, action: PayloadAction<DateRange>) => {
			state.dateRange = { ...state.dateRange, ...action.payload };
		},
		setViewType: (state, action: PayloadAction<'weekly' | 'monthly'>) => {
			state.viewType = action.payload;
		},
		setSelectedDate: (state, action: PayloadAction<string>) => {
			state.selectedDate = action.payload;
		},
		resetDateRange: (state) => {
			const thirtyDaysAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);
			state.dateRange = {
				startDate: thirtyDaysAgo.toISOString().split('T')[0],
				endDate: new Date().toISOString().split('T')[0],
			};
		},
		clearStats: (state) => {
			state.statsHistory = null;
			state.statsRecord = null;
			state.error = null;
		},
	},
	extraReducers: (builder) => {
		builder
			.addCase(fetchStatsHistory.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchStatsHistory.fulfilled, (state, action: PayloadAction<StatsHistory>) => {
				state.loading = false;
				state.statsHistory = action.payload;
			})
			.addCase(fetchStatsHistory.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch dashboard stats';
			})
			// Weekly Stats
			.addCase(fetchWeeklyStats.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchWeeklyStats.fulfilled, (state, action: PayloadAction<StatsRecord | null>) => {
				state.loading = false;
				state.statsRecord = action.payload;
			})
			.addCase(fetchWeeklyStats.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch weekly stats';
			})
			// Monthly Stats
			.addCase(fetchMonthlyStats.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchMonthlyStats.fulfilled, (state, action: PayloadAction<StatsRecord | null>) => {
				state.loading = false;
				state.statsRecord = action.payload;
			})
			.addCase(fetchMonthlyStats.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch monthly stats';
			});
	},
});

export const { setDateRange, resetDateRange, clearStats, setViewType, setSelectedDate } = dashboardSlice.actions;
export default dashboardSlice.reducer;
