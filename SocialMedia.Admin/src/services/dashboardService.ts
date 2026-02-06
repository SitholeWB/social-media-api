// services/dashboardService.ts
import { fetchJson } from './api';

export enum StatsType {
	Weekly = 0,
	Monthly = 1
}

export interface ReactionStat {
	emoji: string;
	count: number;
}

export interface StatsRecord {
	id: string;
	statsType: StatsType;
	date: string;
	totalPosts: number;
	activeUsers: number;
	newPosts: number;
	resultingComments: number;
	resultingReactions: number;
	createdAt: string;
	lastModifiedAt?: string;
	reactionBreakdown: ReactionStat[];
}

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

	getWeeklyStats: (date?: string): Promise<StatsRecord | null> => {
		const url = `/api/v1.0/stats/weekly${date ? `?date=${date}` : ''}`;
		return fetchJson<StatsRecord | null>(url);
	},

	getMonthlyStats: (date?: string): Promise<StatsRecord | null> => {
		const url = `/api/v1.0/stats/monthly${date ? `?date=${date}` : ''}`;
		return fetchJson<StatsRecord | null>(url);
	}
};