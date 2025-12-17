// store/slices/reportsSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { 
    reportsService, 
    Report, 
    UpdateReportStatusCommand,
    ReportStatus,
    PagedResult 
} from '../../services/reportsService';

interface ReportsState {
    items: Report[];
    currentReport: Report | null;
    loading: boolean;
    error: string | null;
    pagination: {
        pageNumber: number;
        pageSize: number;
        totalCount: number;
        totalPages: number;
    };
    filters: {
        status?: ReportStatus;
    };
}

const initialState: ReportsState = {
    items: [],
    currentReport: null,
    loading: false,
    error: null,
    pagination: {
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
    },
    filters: {
        status: ReportStatus.Pending, // Default to pending reports
    },
};

// Fetch pending reports
export const fetchPendingReports = createAsyncThunk(
    'reports/fetchPendingReports',
    async ({ pageNumber = 1, pageSize = 10 }: { pageNumber?: number; pageSize?: number } = {}) => {
        const response = await reportsService.getPendingReports(pageNumber, pageSize);
        return response;
    }
);

// Fetch all reports with optional status filter
export const fetchReports = createAsyncThunk(
    'reports/fetchReports',
    async ({ 
        pageNumber = 1, 
        pageSize = 10, 
        status 
    }: { 
        pageNumber?: number; 
        pageSize?: number; 
        status?: ReportStatus 
    } = {}) => {
        const response = await reportsService.getAllReports(pageNumber, pageSize, status);
        return response;
    }
);

// Fetch single report
export const fetchReport = createAsyncThunk(
    'reports/fetchReport',
    async (id: string) => {
        const report = await reportsService.getReport(id);
        return report;
    }
);

// Update report status
export const updateReportStatus = createAsyncThunk(
    'reports/updateReportStatus',
    async ({ id, command }: { id: string; command: UpdateReportStatusCommand }) => {
        const report = await reportsService.updateReportStatus(id, command);
        return report;
    }
);

// Delete report
export const deleteReport = createAsyncThunk(
    'reports/deleteReport',
    async (id: string) => {
        await reportsService.deleteReport(id);
        return id;
    }
);

const reportsSlice = createSlice({
    name: 'reports',
    initialState,
    reducers: {
        clearCurrentReport: (state) => {
            state.currentReport = null;
        },
        clearReports: (state) => {
            state.items = [];
            state.currentReport = null;
            state.pagination = initialState.pagination;
        },
        setPageNumber: (state, action: PayloadAction<number>) => {
            state.pagination.pageNumber = action.payload;
        },
        setPageSize: (state, action: PayloadAction<number>) => {
            state.pagination.pageSize = action.payload;
        },
        setStatusFilter: (state, action: PayloadAction<ReportStatus | undefined>) => {
            state.filters.status = action.payload;
            state.pagination.pageNumber = 1; // Reset to first page when filter changes
        },
    },
    extraReducers: (builder) => {
        builder
            // Fetch pending reports
            .addCase(fetchPendingReports.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPendingReports.fulfilled, (state, action: PayloadAction<PagedResult<Report>>) => {
                state.loading = false;
                state.items = action.payload.items;
                state.pagination = {
                    pageNumber: action.payload.pageNumber,
                    pageSize: action.payload.pageSize,
                    totalCount: action.payload.totalCount,
                    totalPages: action.payload.totalPages,
                };
            })
            .addCase(fetchPendingReports.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch pending reports';
            })
            
            // Fetch all reports
            .addCase(fetchReports.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchReports.fulfilled, (state, action: PayloadAction<PagedResult<Report>>) => {
                state.loading = false;
                state.items = action.payload.items;
                state.pagination = {
                    pageNumber: action.payload.pageNumber,
                    pageSize: action.payload.pageSize,
                    totalCount: action.payload.totalCount,
                    totalPages: action.payload.totalPages,
                };
            })
            .addCase(fetchReports.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch reports';
            })
            
            // Fetch single report
            .addCase(fetchReport.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchReport.fulfilled, (state, action: PayloadAction<Report>) => {
                state.loading = false;
                state.currentReport = action.payload;
                
                // Update the report in items array if it exists there
                const index = state.items.findIndex(report => report.id === action.payload.id);
                if (index !== -1) {
                    state.items[index] = action.payload;
                }
            })
            .addCase(fetchReport.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch report';
            })
            
            // Update report status
            .addCase(updateReportStatus.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(updateReportStatus.fulfilled, (state, action: PayloadAction<Report>) => {
                state.loading = false;
                
                // Update in items array
                const index = state.items.findIndex(report => report.id === action.payload.id);
                if (index !== -1) {
                    state.items[index] = action.payload;
                }
                
                // Update current report if it's the one being updated
                if (state.currentReport?.id === action.payload.id) {
                    state.currentReport = action.payload;
                }
            })
            .addCase(updateReportStatus.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to update report status';
            })
            
            // Delete report
            .addCase(deleteReport.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(deleteReport.fulfilled, (state, action: PayloadAction<string>) => {
                state.loading = false;
                // Remove from items array
                state.items = state.items.filter(report => report.id !== action.payload);
                
                // Clear current report if it's the one being deleted
                if (state.currentReport?.id === action.payload) {
                    state.currentReport = null;
                }
                
                // Update pagination count
                state.pagination.totalCount = Math.max(0, state.pagination.totalCount - 1);
            })
            .addCase(deleteReport.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to delete report';
            });
    },
});

export const { 
    clearCurrentReport, 
    clearReports, 
    setPageNumber, 
    setPageSize,
    setStatusFilter
} = reportsSlice.actions;

export default reportsSlice.reducer;