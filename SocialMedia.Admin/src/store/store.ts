import { configureStore } from '@reduxjs/toolkit';
import groupsReducer from './slices/groupsSlice';
import pollsReducer from './slices/pollsSlice';
import authReducer from './slices/authSlice';
import settingsReducer from './slices/settingsSlice';
import postsReducer from './slices/postsSlice';
import reportsSlice from './slices/reportsSlice';
import dashboardSlice from './slices/dashboardSlice';

export const store = configureStore({
    reducer: {
        groups: groupsReducer,
        polls: pollsReducer,
        auth: authReducer,
        settings: settingsReducer,
        posts: postsReducer,
        reports: reportsSlice,
        dashboard: dashboardSlice,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
