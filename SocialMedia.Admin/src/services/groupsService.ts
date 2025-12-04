import { fetchJson } from './api';

export interface Group {
    id: string;
    name: string;
    description: string;
    isPublic: boolean;
    isAutoAdd: boolean;
    creatorId: string;
    createdAt: string;
}

export interface CreateGroupCommand {
    name: string;
    description: string;
    isPublic: boolean;
    isAutoAdd: boolean;
}

export interface UpdateGroupCommand {
    groupId: string;
    name: string;
    description: string;
    isPublic: boolean;
    isAutoAdd: boolean;
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

export const groupsService = {
    getGroups: (pageNumber = 1, pageSize = 10) =>
        fetchJson<PagedResult<Group>>(`/groups?pageNumber=${pageNumber}&pageSize=${pageSize}`),

    getGroup: (id: string) =>
        fetchJson<Group>(`/groups/${id}`),

    createGroup: (command: CreateGroupCommand) =>
        fetchJson<string>('/groups', {
            method: 'POST',
            body: JSON.stringify(command),
        }),

    updateGroup: (id: string, command: UpdateGroupCommand) =>
        fetchJson<void>(`/groups/${id}`, {
            method: 'PUT',
            body: JSON.stringify(command),
        }),

    deleteGroup: (id: string) =>
        fetchJson<void>(`/groups/${id}`, {
            method: 'DELETE',
        }),
};
