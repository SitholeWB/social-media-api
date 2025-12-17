// store/slices/dashboardSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { dashboardService, DashboardStats, DateRange } from '../../services/dashboardService';

interface DashboardState {
    stats: DashboardStats | null;
    loading: boolean;
    error: string | null;
    dateRange: DateRange;
}

const initialState: DashboardState = {
    stats: null,
    loading: false,
    error: null,
    dateRange: {
        // Default to last 30 days
        startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
        endDate: new Date().toISOString().split('T')[0],
    },
};

export const fetchDashboardStats = createAsyncThunk(
    'dashboard/fetchStats',
    async (dateRange?: DateRange) => {
        const stats = await dashboardService.getDashboardStats(dateRange);
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
        resetDateRange: (state) => {
            const thirtyDaysAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);
            state.dateRange = {
                startDate: thirtyDaysAgo.toISOString().split('T')[0],
                endDate: new Date().toISOString().split('T')[0],
            };
        },
        clearStats: (state) => {
            state.stats = null;
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchDashboardStats.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchDashboardStats.fulfilled, (state, action: PayloadAction<DashboardStats>) => {
                state.loading = false;
                state.stats = action.payload;
            })
            .addCase(fetchDashboardStats.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch dashboard stats';
            });
    },
});

export const { setDateRange, resetDateRange, clearStats } = dashboardSlice.actions;
export default dashboardSlice.reducer;