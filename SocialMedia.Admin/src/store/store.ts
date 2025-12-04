import { configureStore } from '@reduxjs/toolkit';
import groupsReducer from './slices/groupsSlice';
import pollsReducer from './slices/pollsSlice';

export const store = configureStore({
    reducer: {
        groups: groupsReducer,
        polls: pollsReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
