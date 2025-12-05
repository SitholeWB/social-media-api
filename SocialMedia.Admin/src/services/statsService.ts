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

export const getDashboardStats = async (startDate?: string, endDate?: string): Promise<DashboardStats> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return fetchJson<DashboardStats>(`/api/v1.0/Stats/dashboard?${params.toString()}`);
};
