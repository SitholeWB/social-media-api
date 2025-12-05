import axios from '../utils/axios';

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

    const response = await axios.get<DashboardStats>(`/stats/dashboard?${params.toString()}`);
    return response.data;
};
