import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Copyright from '../internals/components/Copyright';

export default function UsersPage() {
    return (
        <Grid container spacing={2} columns={12} sx={{ width: '100%' }}>
            <Grid item xs={12}>
                <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
                    Users
                </Typography>
            </Grid>
            <Grid item xs={12}>
                {/* Placeholder for DataGrid */}
                <Typography>Users Management Content Here</Typography>
            </Grid>
            <Copyright sx={{ my: 4 }} />
        </Grid>
    );
}
