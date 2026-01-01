// store/slices/pollsSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import {
	pollsService,
	Poll,
	CreatePollCommand,
	UpdatePollCommand,
	PagedResult
} from '../../services/pollsService';

interface PollsState {
	items: Poll[];
	currentPoll: Poll | null;
	loading: boolean;
	error: string | null;
	pagination: {
		pageNumber: number;
		pageSize: number;
		totalCount: number;
		totalPages: number;
	};
}

const initialState: PollsState = {
	items: [],
	currentPoll: null,
	loading: false,
	error: null,
	pagination: {
		pageNumber: 1,
		pageSize: 10,
		totalCount: 0,
		totalPages: 0,
	},
};

// Fetch all polls with pagination
export const fetchPolls = createAsyncThunk(
	'polls/fetchPolls',
	async ({ pageNumber = 1, pageSize = 10, groupId }: { pageNumber?: number; pageSize?: number; groupId?: string } = {}) => {
		const response = await pollsService.getPolls(groupId ?? 'not-set', pageNumber, pageSize);
		return response;
	}
);

// Fetch single poll by ID
export const fetchPoll = createAsyncThunk(
	'polls/fetchPoll',
	async (id: string) => {
		const poll = await pollsService.getPoll(id);
		return poll;
	}
);

// Create new poll
export const createPoll = createAsyncThunk(
	'polls/createPoll',
	async (command: CreatePollCommand) => {
		const poll = await pollsService.createPoll(command);
		return poll;
	}
);

// Update existing poll
export const updatePoll = createAsyncThunk(
	'polls/updatePoll',
	async ({ id, command }: { id: string; command: UpdatePollCommand }) => {
		const poll = await pollsService.updatePoll(id, command);
		return poll;
	}
);

// Delete poll
export const deletePoll = createAsyncThunk(
	'polls/deletePoll',
	async (id: string) => {
		await pollsService.deletePoll(id);
		return id;
	}
);

// Vote on a poll
export const voteOnPoll = createAsyncThunk(
	'polls/voteOnPoll',
	async ({ pollId, optionId }: { pollId: string; optionId: string }) => {
		await pollsService.voteOnPoll(pollId, optionId);
		return { pollId, optionId };
	}
);

const pollsSlice = createSlice({
	name: 'polls',
	initialState,
	reducers: {
		clearCurrentPoll: (state) => {
			state.currentPoll = null;
		},
		clearPolls: (state) => {
			state.items = [];
			state.currentPoll = null;
			state.pagination = initialState.pagination;
		},
		setPageNumber: (state, action: PayloadAction<number>) => {
			state.pagination.pageNumber = action.payload;
		},
		setPageSize: (state, action: PayloadAction<number>) => {
			state.pagination.pageSize = action.payload;
		},
		updatePollInList: (state, action: PayloadAction<Poll>) => {
			const index = state.items.findIndex(poll => poll.id === action.payload.id);
			if (index !== -1) {
				state.items[index] = action.payload;
			}
		},
		removePollFromList: (state, action: PayloadAction<string>) => {
			state.items = state.items.filter(poll => poll.id !== action.payload);
		}
	},
	extraReducers: (builder) => {
		builder
			// Fetch polls (list)
			.addCase(fetchPolls.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchPolls.fulfilled, (state, action: PayloadAction<PagedResult<Poll>>) => {
				state.loading = false;
				state.items = action.payload.items;
				state.pagination = {
					pageNumber: action.payload.pageNumber,
					pageSize: action.payload.pageSize,
					totalCount: action.payload.totalCount,
					totalPages: action.payload.totalPages,
				};
			})
			.addCase(fetchPolls.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch polls';
			})

			// Fetch single poll
			.addCase(fetchPoll.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(fetchPoll.fulfilled, (state, action: PayloadAction<Poll>) => {
				state.loading = false;
				state.currentPoll = action.payload;

				// Update the poll in items array if it exists there
				const index = state.items.findIndex(poll => poll.id === action.payload.id);
				if (index !== -1) {
					state.items[index] = action.payload;
				}
			})
			.addCase(fetchPoll.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to fetch poll';
			})

			// Create poll
			.addCase(createPoll.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(createPoll.fulfilled, (state, action: PayloadAction<Poll>) => {
				state.loading = false;
				// Add new poll to the beginning of the list
				state.items.unshift(action.payload);
				state.pagination.totalCount += 1;
			})
			.addCase(createPoll.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to create poll';
			})

			// Update poll
			.addCase(updatePoll.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(updatePoll.fulfilled, (state, action: PayloadAction<Poll>) => {
				state.loading = false;

				// Update in items array
				const index = state.items.findIndex(poll => poll.id === action.payload.id);
				if (index !== -1) {
					state.items[index] = action.payload;
				}

				// Update current poll if it's the one being edited
				if (state.currentPoll?.id === action.payload.id) {
					state.currentPoll = action.payload;
				}
			})
			.addCase(updatePoll.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to update poll';
			})

			// Delete poll
			.addCase(deletePoll.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(deletePoll.fulfilled, (state, action: PayloadAction<string>) => {
				state.loading = false;
				// Remove from items array
				state.items = state.items.filter(poll => poll.id !== action.payload);

				// Clear current poll if it's the one being deleted
				if (state.currentPoll?.id === action.payload) {
					state.currentPoll = null;
				}

				// Update pagination count
				state.pagination.totalCount = Math.max(0, state.pagination.totalCount - 1);
			})
			.addCase(deletePoll.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to delete poll';
			})

			// Vote on poll
			.addCase(voteOnPoll.pending, (state) => {
				state.loading = true;
				state.error = null;
			})
			.addCase(voteOnPoll.fulfilled, (state) => {
				state.loading = false;
				// The vote has been recorded, but we don't update the poll here
				// The component should refetch the poll to get updated vote counts
			})
			.addCase(voteOnPoll.rejected, (state, action) => {
				state.loading = false;
				state.error = action.error.message || 'Failed to vote on poll';
			});
	},
});

export const {
	clearCurrentPoll,
	clearPolls,
	setPageNumber,
	setPageSize,
	updatePollInList,
	removePollFromList
} = pollsSlice.actions;

export default pollsSlice.reducer;