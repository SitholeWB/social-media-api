// services/pollsService.ts
import { fetchJson } from './api';

export interface PollOption {
	id: string;
	text: string;
	voteCount: number;
}

export interface Poll {
	id: string;
	question: string;
	isActive: boolean;
	expiresAt?: string;
	creatorId: string;
	groupId: string;
	groupName: string;
	isAnonymous: boolean;
	options: PollOption[];
}

export interface CreatePollCommand {
	question: string;
	options: string[];
	expiresAt?: string;
	groupId: string;
	isAnonymous: boolean;
}

export interface UpdatePollCommand {
	pollId: string;
	question: string;
	isActive: boolean;
	expiresAt?: string;
	groupId: string;
	isAnonymous: boolean;
}

export interface PagedResult<T> {
	items: T[];
	pageNumber: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
}

export const pollsService = {
	getPolls: (pageNumber = 1, pageSize = 10, groupId?: string) => {
		const url = new URL(`${window.location.origin}/api/v1.0/polls/active`);
		url.searchParams.append('pageNumber', pageNumber.toString());
		url.searchParams.append('pageSize', pageSize.toString());
		if (groupId) url.searchParams.append('groupId', groupId);
		return fetchJson<PagedResult<Poll>>(url.pathname + url.search);
	},

	getPoll: (id: string) =>
		fetchJson<Poll>(`/api/v1.0/polls/${id}`),

	createPoll: (command: CreatePollCommand) =>
		fetchJson<Poll>('/api/v1.0/polls', {
			method: 'POST',
			body: JSON.stringify(command),
		}),

	updatePoll: (id: string, command: UpdatePollCommand) =>
		fetchJson<Poll>(`/api/v1.0/polls/${id}`, {
			method: 'PUT',
			body: JSON.stringify(command),
		}),

	deletePoll: (id: string) =>
		fetchJson<void>(`/api/v1.0/polls/${id}`, {
			method: 'DELETE',
		}),

	voteOnPoll: (pollId: string, optionId: string) =>
		fetchJson<void>(`/api/v1.0/polls/${pollId}/vote`, {
			method: 'POST',
			body: JSON.stringify({ optionId }),
		}),
};