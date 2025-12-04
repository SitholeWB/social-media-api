import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { pollsService, Poll, CreatePollCommand, UpdatePollCommand } from '../../services/pollsService';

interface PollsState {
    items: Poll[];
    loading: boolean;
    error: string | null;
}

const initialState: PollsState = {
    items: [],
    loading: false,
    error: null,
};

export const fetchPolls = createAsyncThunk('polls/fetchPolls', async () => {
    const response = await pollsService.getPolls();
    return response.items;
});

export const createPoll = createAsyncThunk('polls/createPoll', async (command: CreatePollCommand) => {
    await pollsService.createPoll(command);
});

export const updatePoll = createAsyncThunk('polls/updatePoll', async ({ id, command }: { id: string; command: UpdatePollCommand }) => {
    await pollsService.updatePoll(id, command);
});

export const deletePoll = createAsyncThunk('polls/deletePoll', async (id: string) => {
    await pollsService.deletePoll(id);
});

const pollsSlice = createSlice({
    name: 'polls',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchPolls.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPolls.fulfilled, (state, action: PayloadAction<Poll[]>) => {
                state.loading = false;
                state.items = action.payload;
            })
            .addCase(fetchPolls.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || 'Failed to fetch polls';
            });
    },
});

export default pollsSlice.reducer;
