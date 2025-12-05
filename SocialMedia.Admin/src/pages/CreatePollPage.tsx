import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import PageHeader from '../components/PageHeader';
import { useAppDispatch } from '../store/hooks';
import { createPoll } from '../store/slices/pollsSlice';
import { CreatePollCommand } from '../services/pollsService';

export default function CreatePollPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const [formData, setFormData] = React.useState<CreatePollCommand>({
        question: '',
        options: ['', ''],
        expiresAt: undefined,
    });

    const handleOptionChange = (index: number, value: string) => {
        const newOptions = [...formData.options];
        newOptions[index] = value;
        setFormData({ ...formData, options: newOptions });
    };

    const addOption = () => {
        setFormData({ ...formData, options: [...formData.options, ''] });
    };

    const removeOption = (index: number) => {
        const newOptions = formData.options.filter((_, i) => i !== index);
        setFormData({ ...formData, options: newOptions });
    };

    const handleSave = async () => {
        try {
            await dispatch(createPoll(formData)).unwrap();
            navigate('/polls');
        } catch (error) {
            console.error('Failed to create poll', error);
        }
    };

    const handleCancel = () => {
        navigate('/polls');
    };

    return (
        <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
            <PageHeader title="Create Poll" />

            <Paper sx={{ p: 3, mt: 2 }}>
                <Stack spacing={3}>
                    <TextField
                        autoFocus
                        label="Question"
                        fullWidth
                        multiline
                        maxRows={4}
                        variant="outlined"
                        value={formData.question}
                        onChange={(e) => setFormData({ ...formData, question: e.target.value })}
                    />

                    <Box sx={{ p: 2, border: '1px solid', borderColor: 'divider', borderRadius: 1 }}>
                        <Typography variant="subtitle2" sx={{ mb: 2, fontWeight: 600 }}>Options</Typography>
                        <Stack spacing={2}>
                            {formData.options.map((option, index) => (
                                <Stack direction="row" spacing={1} key={index}>
                                    <TextField
                                        fullWidth
                                        size="small"
                                        placeholder={`Option ${index + 1}`}
                                        value={option}
                                        onChange={(e) => handleOptionChange(index, e.target.value)}
                                    />
                                    {formData.options.length > 2 && (
                                        <IconButton size="small" onClick={() => removeOption(index)} color="error">
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    )}
                                </Stack>
                            ))}
                            <Button
                                size="small"
                                startIcon={<AddIcon />}
                                onClick={addOption}
                                sx={{ alignSelf: 'flex-start', textTransform: 'none' }}
                            >
                                Add Option
                            </Button>
                        </Stack>
                    </Box>

                    <TextField
                        label="Expires At"
                        type="datetime-local"
                        fullWidth
                        InputLabelProps={{ shrink: true }}
                        value={formData.expiresAt ? new Date(formData.expiresAt).toISOString().slice(0, 16) : ''}
                        onChange={(e) => setFormData({ ...formData, expiresAt: e.target.value || undefined })}
                    />

                    <Stack direction="row" spacing={2} justifyContent="flex-end">
                        <Button onClick={handleCancel} sx={{ textTransform: 'none' }}>
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSave}
                            variant="contained"
                            sx={{ textTransform: 'none' }}
                            disabled={!formData.question || formData.options.some(o => !o)}
                        >
                            Create Poll
                        </Button>
                    </Stack>
                </Stack>
            </Paper>
        </Box>
    );
}
