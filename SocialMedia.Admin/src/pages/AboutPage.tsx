import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';

export default function AboutPage() {
    return (
        <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
            <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
                About
            </Typography>
            <Paper sx={{ p: 3 }}>
                <Typography variant="body1" paragraph>
                    Social Media Admin Panel v1.0.0
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    This application allows administrators to manage groups, users, polls, and view reports for the Social Media platform.
                </Typography>
            </Paper>
        </Box>
    );
}
