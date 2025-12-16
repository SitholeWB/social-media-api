// services/postsService.ts
import { fetchJson } from './api';

export interface Post {
    id: string;
    groupId: string;
    title?: string;
    content: string;
    createdBy: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreatePostCommand {
    title?: string;
    content: string;
}

export interface UpdatePostCommand {
    title?: string;
    content: string;
}

export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export const postsService = {
    getPostsByGroup: (groupId: string, pageNumber = 1, pageSize = 10) =>
        fetchJson<PagedResult<Post>>(`/api/v1.0/groups/${groupId}/posts?pageNumber=${pageNumber}&pageSize=${pageSize}`),

    getPost: (groupId: string, postId: string) =>
        fetchJson<Post>(`/api/v1.0/posts/${postId}`),

    createPost: (groupId: string, command: CreatePostCommand) =>
        fetchJson<string>(`/api/v1.0/groups/${groupId}/posts`, {
            method: 'POST',
            body: JSON.stringify(command),
        }),

    updatePost: (groupId: string, postId: string, command: UpdatePostCommand) =>
        fetchJson<Post>(`/api/v1.0/posts/${postId}`, {
            method: 'PUT',
            body: JSON.stringify(command),
        }),

    deletePost: (groupId: string, postId: string) =>
        fetchJson<void>(`/api/v1.0/posts/${postId}`, {
            method: 'DELETE',
        }),
};