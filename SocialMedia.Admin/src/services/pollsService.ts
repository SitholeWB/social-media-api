import { fetchJson } from './api';

export interface Poll {
    id: string;
    question: string;
    isActive: boolean;
    expiresAt?: string;
    creatorId: string;
    options: PollOption[];
}

export interface PollOption {
    id: string;
    text: string;
    voteCount: number;
}

export interface CreatePollCommand {
    question: string;
    options: string[];
    expiresAt?: string;
}

export interface UpdatePollCommand {
    pollId: string;
    question: string;
    isActive: boolean;
    expiresAt?: string;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export const pollsService = {
    getPolls: (pageNumber = 1, pageSize = 10) =>
        fetchJson<PagedResult<Poll>>(`/api/v1.0/polls/active?pageNumber=${pageNumber}&pageSize=${pageSize}`),

    getPoll: (id: string) =>
        fetchJson<Poll>(`/api/v1.0/polls/${id}`),

    createPoll: (command: CreatePollCommand) =>
        fetchJson<string>('/api/v1.0/polls', {
            method: 'POST',
            body: JSON.stringify(command),
        }),

    updatePoll: (id: string, command: UpdatePollCommand) =>
        fetchJson<void>(`/api/v1.0/polls/${id}`, {
            method: 'PUT',
            body: JSON.stringify(command),
        }),

    deletePoll: (id: string) =>
        fetchJson<void>(`/api/v1.0/polls/${id}`, {
            method: 'DELETE',
        }),
};
