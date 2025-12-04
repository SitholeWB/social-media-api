import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { groupsService, Group, CreateGroupCommand, UpdateGroupCommand } from '../../services/groupsService';

interface GroupsState {
    items: Group[];
    loading: boolean;
    error: string | null;
}

const initialState: GroupsState = {
    items: [],
    loading: false,
    error: null,
};

export const fetchGroups = createAsyncThunk('groups/fetchGroups', async () => {
    const response = await groupsService.getGroups();
    return response.items;
});

export const createGroup = createAsyncThunk('groups/createGroup', async (command: CreateGroupCommand) => {
    await groupsService.createGroup(command);
});

export const updateGroup = createAsyncThunk('groups/updateGroup', async ({ id, command }: { id: string; command: UpdateGroupCommand }) => {
    await groupsService.updateGroup(id, command);
});

export const deleteGroup = createAsyncThunk('groups/deleteGroup', async (id: string) => {
    await groupsService.deleteGroup(id);
});

const groupsSlice = createSlice({
    name: 'groups',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchGroups.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchGroups.fulfilled, (state, action: PayloadAction<Group[]>) => {
                state.loading = false;
                state.items = action.payload;
            })
            .addCase(fetchGroups.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch groups';
            })
            .addCase(createGroup.fulfilled, (state) => {
                // We could optimistically add the group here, but for now we'll just re-fetch
            })
            .addCase(updateGroup.fulfilled, (state) => {
                // Same here
            })
            .addCase(deleteGroup.fulfilled, (state) => {
                // Same here
            });
    },
});

export default groupsSlice.reducer;
