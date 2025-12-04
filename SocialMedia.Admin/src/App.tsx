import * as React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import CssBaseline from '@mui/material/CssBaseline';
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
import { store } from './store/store';

const xThemeComponents = {
    ...chartsCustomizations,
    ...datePickersCustomizations,
    ...treeViewCustomizations,
};

export default function App(props: { disableCustomTheme?: boolean }) {
    return (
        <Provider store={store}>
            <AppTheme {...props} themeComponents={xThemeComponents}>
                <CssBaseline enableColorScheme />
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<Layout />}>
                            <Route index element={<MainGrid />} />
                            <Route path="groups" element={<GroupsPage />} />
                            <Route path="users" element={<UsersPage />} />
                            <Route path="polls" element={<PollsPage />} />
                            <Route path="reports" element={<ReportsPage />} />
                        </Route>
                    </Routes>
                </BrowserRouter>
            </AppTheme>
        </Provider>
    );
}
