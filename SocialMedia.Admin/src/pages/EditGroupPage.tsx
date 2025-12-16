// pages/EditGroupPage.tsx
import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchGroups, updateGroup } from '../store/slices/groupsSlice';
import { UpdateGroupCommand } from '../services/groupsService';

export default function EditGroupPage() {
    const { groupId } = useParams<{ groupId: string }>();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { items: groups, loading: reduxLoading } = useAppSelector((state) => state.groups);

    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [isPublic, setIsPublic] = React.useState(true);
    const [isAutoAdd, setIsAutoAdd] = React.useState(false);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState<string | null>(null);

    React.useEffect(() => {
        dispatch(fetchGroups());
    }, [dispatch]);

    React.useEffect(() => {
        if (groupId && groups.length > 0) {
            const group = groups.find(g => g.id === groupId);
            if (group) {
                setName(group.name);
                setDescription(group.description);
                setIsPublic(group.isPublic);
                setIsAutoAdd(group.isAutoAdd);
            }
        }
    }, [groupId, groups]);

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

            if (groupId) {
                const command: UpdateGroupCommand = {
                    groupId,
                    name,
                    description,
                    isPublic,
                    isAutoAdd,
                };
                
                await dispatch(updateGroup({ 
                    id: groupId, 
                    command 
                })).unwrap();
                
                navigate('/groups');
            }
        } catch (err: any) {
            setError(err.message || 'Failed to update group');
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = () => {
        navigate('/groups');
    };

    const currentGroup = groups.find(g => g.id === groupId);

    if (!groupId) {
        return <Typography>Group ID is required</Typography>;
    }

    if (reduxLoading && !currentGroup) {
        return <Typography>Loading...</Typography>;
    }

    if (!currentGroup && !reduxLoading) {
        return <Typography>Group not found</Typography>;
    }

    return (
        <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
            <PageHeader
                title="Edit Group"
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
                            disabled={loading || reduxLoading}
                            required
                        />

                        <TextField
                            label="Description"
                            fullWidth
                            multiline
                            variant="standard"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            disabled={loading || reduxLoading}
                            required
                        />

                        <Paper
                            elevation={0}
                            sx={{
                                p: 2,
                                border: '1px solid',
                                borderColor: 'divider',
                                borderRadius: 1,
                            }}
                        >
                            <Stack spacing={1}>
                                <FormControlLabel
                                    control={
                                        <Checkbox
                                            checked={isPublic}
                                            onChange={(e) => setIsPublic(e.target.checked)}
                                            disabled={loading || reduxLoading}
                                        />
                                    }
                                    label="Public Group"
                                />
                                <FormControlLabel
                                    control={
                                        <Checkbox
                                            checked={isAutoAdd}
                                            onChange={(e) => setIsAutoAdd(e.target.checked)}
                                            disabled={loading || reduxLoading}
                                        />
                                    }
                                    label="Auto-add new users"
                                />
                            </Stack>
                        </Paper>

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
                                disabled={loading || reduxLoading}
                                sx={{ textTransform: 'none' }}
                            >
                                {loading ? 'Saving...' : 'Save Changes'}
                            </Button>
                        </Stack>
                    </Stack>
                </form>
            </Paper>
        </Box>
    );
}