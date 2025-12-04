import * as React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Provider, useSelector } from 'react-redux';
import CssBaseline from '@mui/material/CssBaseline';
import { useColorScheme } from '@mui/material/styles';
import AppTheme from './theme/AppTheme';
import {
    chartsCustomizations,
    datePickersCustomizations,
    treeViewCustomizations,
} from './theme/customizations';
import Layout from './Layout';
import MainGrid from './components/MainGrid';
import GroupsPage from './pages/GroupsPage';
import UsersPage from './pages/UsersPage';
import PollsPage from './pages/PollsPage';
import ReportsPage from './pages/ReportsPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import SettingsPage from './pages/SettingsPage';
import ProtectedRoute from './components/ProtectedRoute';
import { store, RootState } from './store/store';

const xThemeComponents = {
    ...chartsCustomizations,
    ...datePickersCustomizations,
    ...treeViewCustomizations,
};

function ThemeSync() {
    const { setMode } = useColorScheme();
    const themeMode = useSelector((state: RootState) => state.settings.themeMode);

    React.useEffect(() => {
        setMode(themeMode);
    }, [themeMode, setMode]);

    return null;
}

function AppContent(props: { disableCustomTheme?: boolean }) {
    return (
        <AppTheme {...props} themeComponents={xThemeComponents}>
            <CssBaseline enableColorScheme />
            <ThemeSync />
            <BrowserRouter>
                <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route
                        path="/"
                        element={
                            <ProtectedRoute>
                                <Layout />
                            </ProtectedRoute>
                        }
                    >
                        <Route index element={<MainGrid />} />
                        <Route path="groups" element={<GroupsPage />} />
                        <Route path="users" element={<UsersPage />} />
                        <Route path="polls" element={<PollsPage />} />
                        <Route path="reports" element={<ReportsPage />} />
                        <Route path="settings" element={<SettingsPage />} />
                    </Route>
                </Routes>
            </BrowserRouter>
        </AppTheme>
    );
}

export default function App(props: { disableCustomTheme?: boolean }) {
    return (
        <Provider store={store}>
            <AppContent {...props} />
        </Provider>
    );
}
