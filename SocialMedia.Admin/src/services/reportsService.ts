// services/reportsService.ts
import { fetchJson } from './api';

export enum ReportStatus {
	Pending = 0,
	Approved = 1,
	Rejected = 2,
	Resolved = 3
}

export interface Report {
	id: string;
	reporterId: string;
	reason: string;
	status: ReportStatus;
	postId: string;
	commentId: string | null;
	createdAt: string;
	// Optional: Post details if needed
	post?: {
		id: string;
		title: string;
		content: string;
		authorId: string;
		authorName?: string;
	};
}

export interface UpdateReportStatusCommand {
	status: ReportStatus;
	moderatorNotes?: string;
}

export interface PagedResult<T> {
	items: T[];
	pageNumber: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
}

export const reportsService = {
	getPendingReports: (pageNumber = 1, pageSize = 10) =>
		fetchJson<PagedResult<Report>>(`/api/v1.0/reports/pending?pageNumber=${pageNumber}&pageSize=${pageSize}`),

	getAllReports: (pageNumber = 1, pageSize = 10, status?: ReportStatus) => {
		const params = new URLSearchParams({
			pageNumber: pageNumber.toString(),
			pageSize: pageSize.toString()
		});
		if (status !== undefined) {
			params.append('status', status.toString());
		}
		return fetchJson<PagedResult<Report>>(`/api/v1.0/reports?${params.toString()}`);
	},

	getReport: (id: string) =>
		fetchJson<Report>(`/api/v1.0/reports/${id}`),

	updateReportStatus: (id: string, command: UpdateReportStatusCommand) =>
		fetchJson<Report>(`/api/v1.0/reports/${id}/status`, {
			method: 'PUT',
			body: JSON.stringify(command),
		}),

	deleteReport: (id: string) =>
		fetchJson<void>(`/api/v1.0/reports/${id}`, {
			method: 'DELETE',
		}),
};