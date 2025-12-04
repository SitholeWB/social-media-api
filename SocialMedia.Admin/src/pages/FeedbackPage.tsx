import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

export default function FeedbackPage() {
    return (
        <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
            <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
                Feedback
            </Typography>
            <Paper sx={{ p: 3 }}>
                <Typography variant="body1" paragraph>
                    We value your feedback! Please let us know how we can improve the admin panel.
                </Typography>
                <Box component="form" noValidate autoComplete="off">
                    <Stack spacing={2} sx={{ maxWidth: 600 }}>
                        <TextField
                            label="Subject"
                            variant="outlined"
                            fullWidth
                            required
                        />
                        <TextField
                            label="Message"
                            variant="outlined"
                            multiline
                            rows={4}
                            fullWidth
                            required
                        />
                        <Button variant="contained" sx={{ alignSelf: 'flex-start' }}>
                            Submit Feedback
                        </Button>
                    </Stack>
                </Box>
            </Paper>
        </Box>
    );
}
