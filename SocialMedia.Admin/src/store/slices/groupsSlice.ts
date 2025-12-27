// store/slices/groupsSlice.ts - Updated version
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { groupsService, Group, CreateGroupCommand, UpdateGroupCommand } from '../../services/groupsService';

interface GroupsState {
	items: Group[];
	loading: boolean;
	error: string | null;
	currentGroup: Group | null;
	pagination: {
		pageNumber: number;
		pageSize: number;
		totalCount: number;
		totalPages: number;
	};
}

const initialState: GroupsState = {
	items: [],
	loading: false,
	error: null,
	currentGroup: null,
	pagination: {
		pageNumber: 1,
		pageSize: 10,
		totalCount: 0,
		totalPages: 0,
	},
};

export const fetchGroups = createAsyncThunk(
	'groups/fetchGroups',
	async ({ pageNumber = 1, pageSize = 10 }: { pageNumber?: number; pageSize?: number } = {}) => {
		const response = await groupsService.getGroups(pageNumber, pageSize);
		return response;
	}
);

export const fetchGroup = createAsyncThunk('groups/fetchGroup', async (id: string) => {
	const group = await groupsService.getGroup(id);
	return group;
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
	reducers: {
		clearCurrentGroup: (state) => {
			state.currentGroup = null;
		},
		setPageNumber: (state, action: PayloadAction<number>) => {
			state.pagination.pageNumber = action.payload;
		},
		setPageSize: (state, action: PayloadAction<number>) => {
			state.pagination.pageSize = action.payload;
		},
	},
	extraReducers: (builder) => {
		builder
			.addCase(fetchGroups.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchGroups.fulfilled, (state, action: PayloadAction<any>) => {
				state.loading = false;
				state.items = action.payload.items;
				state.pagination = {
					pageNumber: action.payload.pageNumber,
					pageSize: action.payload.pageSize,
					totalCount: action.payload.totalCount,
					totalPages: action.payload.totalPages,
				};
			})
			.addCase(fetchGroups.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch groups';
			})
			.addCase(fetchGroup.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchGroup.fulfilled, (state, action: PayloadAction<Group>) => {
				state.loading = false;
				state.currentGroup = action.payload;
			})
			.addCase(fetchGroup.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch group';
			})
			.addCase(createGroup.fulfilled, (state) => {
				// We could optimistically add the group here
			})
			.addCase(updateGroup.fulfilled, (state) => {
				// Same here
			})
			.addCase(deleteGroup.fulfilled, (state) => {
				// Same here
			});
	},
});

export const { clearCurrentGroup, setPageNumber, setPageSize } = groupsSlice.actions;
export default groupsSlice.reducer;