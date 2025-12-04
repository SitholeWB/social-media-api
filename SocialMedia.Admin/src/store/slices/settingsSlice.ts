import { createSlice, PayloadAction } from '@reduxjs/toolkit';

type ThemeMode = 'light' | 'dark';

interface SettingsState {
    themeMode: ThemeMode;
}

const THEME_KEY = 'theme_mode';

const getInitialTheme = (): ThemeMode => {
    const saved = localStorage.getItem(THEME_KEY);
    return (saved === 'light' || saved === 'dark') ? saved : 'light';
};

const initialState: SettingsState = {
    themeMode: getInitialTheme(),
};

const settingsSlice = createSlice({
    name: 'settings',
    initialState,
    reducers: {
        setThemeMode: (state, action: PayloadAction<ThemeMode>) => {
            state.themeMode = action.payload;
            localStorage.setItem(THEME_KEY, action.payload);
        },
        toggleThemeMode: (state) => {
            state.themeMode = state.themeMode === 'light' ? 'dark' : 'light';
            localStorage.setItem(THEME_KEY, state.themeMode);
        },
    },
});

export const { setThemeMode, toggleThemeMode } = settingsSlice.actions;
export default settingsSlice.reducer;
