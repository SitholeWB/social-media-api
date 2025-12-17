// services/dashboardService.ts
import { fetchJson } from './api';

export interface DashboardStats {
    totalUsers: number;
    activeUsers: number;
    totalPosts: number;
    newPostsInPeriod: number;
    totalComments: number;
    newCommentsInPeriod: number;
    totalReactions: number;
    newReactionsInPeriod: number;
}

export interface DateRange {
    startDate?: string; // Format: YYYY-MM-DD
    endDate?: string;   // Format: YYYY-MM-DD
}

export const dashboardService = {
    getDashboardStats: (dateRange?: DateRange): Promise<DashboardStats> => {
        const params = new URLSearchParams();
        
        if (dateRange?.startDate) {
            params.append('startDate', dateRange.startDate);
        }
        
        if (dateRange?.endDate) {
            params.append('endDate', dateRange.endDate);
        }
        
        const queryString = params.toString();
        const url = `/api/v1.0/stats/dashboard${queryString ? `?${queryString}` : ''}`;
        
        return fetchJson<DashboardStats>(url);
    },
};