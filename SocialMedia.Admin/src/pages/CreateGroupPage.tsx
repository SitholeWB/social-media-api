import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import PageHeader from '../components/PageHeader';
import { useAppDispatch } from '../store/hooks';
import { createGroup } from '../store/slices/groupsSlice';
import { CreateGroupCommand } from '../services/groupsService';

export default function CreateGroupPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [formData, setFormData] = React.useState<CreateGroupCommand>({
        name: '',
        description: '',
        isPublic: true,
        isAutoAdd: false,
    });

    const handleSave = async () => {
        try {
            await dispatch(createGroup(formData)).unwrap();
            navigate('/groups');
        } catch (error) {
            console.error('Failed to create group', error);
        }
    };

    const handleCancel = () => {
        navigate('/groups');
    };

    return (
        <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
            <PageHeader title="Create Group" />

            <Paper sx={{ p: 3, mt: 2 }}>
                <Stack spacing={3}>
                    <TextField
                        autoFocus
                        label="Name"
                        fullWidth
                        variant="outlined"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    />
                    <TextField
                        label="Description"
                        fullWidth
                        multiline
                        rows={8}
                        variant="outlined"
                        value={formData.description}
                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    />
                    <Box sx={{ p: 2, border: '1px solid', borderColor: 'divider', borderRadius: 1 }}>
                        <Stack spacing={1}>
                            <FormControlLabel
                                control={
                                    <Checkbox
                                        checked={formData.isPublic}
                                        onChange={(e) => setFormData({ ...formData, isPublic: e.target.checked })}
                                    />
                                }
                                label="Public Group"
                            />
                            <FormControlLabel
                                control={
                                    <Checkbox
                                        checked={formData.isAutoAdd}
                                        onChange={(e) => setFormData({ ...formData, isAutoAdd: e.target.checked })}
                                    />
                                }
                                label="Auto-add new users"
                            />
                        </Stack>
                    </Box>

                    <Stack direction="row" spacing={2} justifyContent="flex-end">
                        <Button onClick={handleCancel} sx={{ textTransform: 'none' }}>
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSave}
                            variant="contained"
                            sx={{ textTransform: 'none' }}
                            disabled={!formData.name}
                        >
                            Create Group
                        </Button>
                    </Stack>
                </Stack>
            </Paper>
        </Box>
    );
}
