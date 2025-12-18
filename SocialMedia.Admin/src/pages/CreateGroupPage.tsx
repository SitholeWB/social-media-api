// pages/CreateGroupPage.tsx
import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PageHeader from '../components/PageHeader';
import { useAppDispatch } from '../store/hooks';
import { createGroup } from '../store/slices/groupsSlice';
import { CreateGroupCommand, GroupType } from '../services/groupsService';
import { FormControl, InputLabel, Select, MenuItem } from '@mui/material';

export default function CreateGroupPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [type, setType] = React.useState<GroupType>(GroupType.Public);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!name.trim()) {
            setError('Name is required');
            return;
        }

        if (!description.trim()) {
            setError('Description is required');
            return;
        }

        try {
            setLoading(true);
            setError(null);

            const command: CreateGroupCommand = {
                name,
                description,
                type,
            };

            await dispatch(createGroup(command)).unwrap();

            navigate('/groups');
        } catch (err: any) {
            setError(err.message || 'Failed to create group');
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = () => {
        navigate('/groups');
    };

    return (
        <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
            <PageHeader
                title="Create Group"
                action={
                    <Button
                        startIcon={<ArrowBackIcon />}
                        onClick={handleCancel}
                        sx={{ textTransform: 'none' }}
                    >
                        Back to Groups
                    </Button>
                }
            />

            <Paper
                elevation={0}
                sx={{
                    p: 3,
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 2,
                }}
            >
                <form onSubmit={handleSubmit}>
                    <Stack spacing={3}>
                        {error && (
                            <Typography color="error" sx={{ p: 2, bgcolor: 'error.lighter', borderRadius: 1 }}>
                                {error}
                            </Typography>
                        )}

                        <TextField
                            autoFocus
                            label="Name"
                            fullWidth
                            variant="standard"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            disabled={loading}
                            required
                        />

                        <TextField
                            label="Description"
                            fullWidth
                            variant="standard"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            disabled={loading}
                            required
                            multiline
                        />

                        <FormControl fullWidth variant="standard">
                            <InputLabel>Group Type</InputLabel>
                            <Select
                                value={type}
                                onChange={(e) => {
                                    const newType = e.target.value as GroupType;
                                    setType(newType);
                                }}
                                disabled={loading}
                            >
                                <MenuItem value={GroupType.Public}>Public</MenuItem>
                                <MenuItem value={GroupType.Private}>Private</MenuItem>
                                <MenuItem value={GroupType.Everyone}>Everyone</MenuItem>
                            </Select>
                        </FormControl>


                        <Stack direction="row" spacing={2} justifyContent="flex-end">
                            <Button
                                onClick={handleCancel}
                                disabled={loading}
                                sx={{ textTransform: 'none' }}
                            >
                                Cancel
                            </Button>
                            <Button
                                type="submit"
                                variant="contained"
                                disabled={loading}
                                sx={{ textTransform: 'none' }}
                            >
                                {loading ? 'Creating...' : 'Create Group'}
                            </Button>
                        </Stack>
                    </Stack>
                </form>
            </Paper>
        </Box>
    );
}