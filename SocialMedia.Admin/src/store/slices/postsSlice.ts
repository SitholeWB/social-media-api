// store/slices/postsSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { postsService, Post, CreatePostCommand, UpdatePostCommand, PagedResult } from '../../services/postsService';

interface PostsState {
    items: Post[];
    loading: boolean;
    error: string | null;
    currentPost: Post | null;
    pagination: {
        pageNumber: number;
        pageSize: number;
        totalCount: number;
        totalPages: number;
    };
}

const initialState: PostsState = {
    items: [],
    loading: false,
    error: null,
    currentPost: null,
    pagination: {
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
    },
};

export const fetchPostsByGroup = createAsyncThunk(
    'posts/fetchPostsByGroup',
    async ({ groupId, pageNumber = 1, pageSize = 10 }: {
        groupId: string;
        pageNumber?: number;
        pageSize?: number;
    }) => {
        const response = await postsService.getPostsByGroup(groupId, pageNumber, pageSize);
        return response;
    }
);

export const fetchPost = createAsyncThunk(
    'posts/fetchPost',
    async ({ groupId, postId }: { groupId: string; postId: string }) => {
        const post = await postsService.getPost(groupId, postId);
        return post;
    }
);
/*
export const createPost = createAsyncThunk(
    'posts/createPost',
    async ({ groupId, command }: { groupId: string; command: CreatePostCommand }) => {
        const postId = await postsService.createPost(groupId, command);
        const newPost: Post = {
            id: postId,
            content: command.content,
            title: command.title,
            groupId,
            createdAt: new Date().toISOString(),
            createdBy: 'current-user', // Replace with actual current user
            updatedAt: new Date().toISOString(),
        };
        return newPost;
    }
);*/

export const createPost = createAsyncThunk<
    Post, 
    { groupId: string; command: CreatePostCommand } // ✅ argument type
>(
    'posts/createPost',
    async ({ groupId, command }) => {
        const postId = await postsService.createPost(groupId, command);
        return {
            id: postId,
            content: command.content,
            title: command.title,
            groupId,
            createdAt: new Date().toISOString(),
            createdBy: 'current-user',
            updatedAt: new Date().toISOString(),
        } as Post;
    }
);

export const updatePost = createAsyncThunk(
    'posts/updatePost',
    async ({ groupId, postId, command }: {
        groupId: string;
        postId: string;
        command: UpdatePostCommand
    }) => {
        const post = await postsService.updatePost(groupId, postId, command);
        return post;
    }
);

export const deletePost = createAsyncThunk(
    'posts/deletePost',
    async ({ groupId, postId }: { groupId: string; postId: string }) => {
        await postsService.deletePost(groupId, postId);
        return postId;
    }
);

const postsSlice = createSlice({
    name: 'posts',
    initialState,
    reducers: {
        clearCurrentPost: (state) => {
            state.currentPost = null;
        },
        clearPosts: (state) => {
            state.items = [];
            state.currentPost = null;
            state.pagination = initialState.pagination;
        },
        setPageNumber: (state, action: PayloadAction<number>) => {
            state.pagination.pageNumber = action.payload;
        },
        setPageSize: (state, action: PayloadAction<number>) => {
            state.pagination.pageSize = action.payload;
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchPostsByGroup.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPostsByGroup.fulfilled, (state, action: PayloadAction<PagedResult<Post>>) => {
                state.loading = false;
                state.items = action.payload.items;
                state.pagination = {
                    pageNumber: action.payload.pageNumber,
                    pageSize: action.payload.pageSize,
                    totalCount: action.payload.totalCount,
                    totalPages: action.payload.totalPages,
                };
            })
            .addCase(fetchPostsByGroup.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch posts';
            })
            .addCase(fetchPost.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPost.fulfilled, (state, action: PayloadAction<Post>) => {
                state.loading = false;
                state.currentPost = action.payload;
            })
            .addCase(fetchPost.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch post';
            })
            .addCase(createPost.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(createPost.fulfilled, (state, action: PayloadAction<Post>) => {
                state.loading = false;
                state.items.unshift(action.payload);
                state.pagination.totalCount += 1;
            })
            .addCase(createPost.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to create post';
            })
            .addCase(updatePost.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(updatePost.fulfilled, (state, action: PayloadAction<Post>) => {
                state.loading = false;
                const index = state.items.findIndex(post => post.id === action.payload.id);
                if (index !== -1) {
                    state.items[index] = action.payload;
                }
                if (state.currentPost?.id === action.payload.id) {
                    state.currentPost = action.payload;
                }
            })
            .addCase(updatePost.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to update post';
            })
            .addCase(deletePost.fulfilled, (state, action: PayloadAction<string>) => {
                state.items = state.items.filter(post => post.id !== action.payload);
                if (state.currentPost?.id === action.payload) {
                    state.currentPost = null;
                }
                state.pagination.totalCount -= 1;
            });
    },
});

export const { clearCurrentPost, clearPosts, setPageNumber, setPageSize } = postsSlice.actions;
export default postsSlice.reducer;