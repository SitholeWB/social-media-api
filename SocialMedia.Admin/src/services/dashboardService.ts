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

export interface StatsHistory {
	weeks: any[];
	months: any[];
}

export interface DateRange {
	startDate?: string; // Format: YYYY-MM-DD
	endDate?: string;   // Format: YYYY-MM-DD
}

export const dashboardService = {
	getStatsHistory: (): Promise<StatsHistory> => {
		const params = new URLSearchParams();
		params.append('count', '12');
		const queryString = params.toString();
		const url = `/api/v1.0/stats/history${queryString ? `?${queryString}` : ''}`;

		return fetchJson<StatsHistory>(url);
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