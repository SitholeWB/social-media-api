import * as React from 'react';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import IconButton from '@mui/material/IconButton';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { Poll, CreatePollCommand, UpdatePollCommand } from '../services/pollsService';
import SidePanel from '../components/SidePanel';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchPolls, createPoll, updatePoll, deletePoll } from '../store/slices/pollsSlice';

export default function PollsPage() {
    const dispatch = useAppDispatch();
    const { items: polls, loading } = useAppSelector((state) => state.polls);

    const [openPanel, setOpenPanel] = React.useState(false);
    const [editingPoll, setEditingPoll] = React.useState<Poll | null>(null);
    const [formData, setFormData] = React.useState<CreatePollCommand>({
        question: '',
        options: ['', ''],
        expiresAt: undefined,
    });
    const [isActive, setIsActive] = React.useState(true);

    React.useEffect(() => {
        dispatch(fetchPolls());
    }, [dispatch]);

    const handleOpenPanel = (poll?: Poll) => {
        if (poll) {
            setEditingPoll(poll);
            setFormData({
                question: poll.question,
                options: [], // Options editing not supported in this simple version
                expiresAt: poll.expiresAt,
            });
            setIsActive(poll.isActive);
        } else {
            setEditingPoll(null);
            setFormData({
                question: '',
                options: ['', ''],
                expiresAt: undefined,
            });
            setIsActive(true);
        }
        setOpenPanel(true);
    };

    const handleClosePanel = () => {
        setOpenPanel(false);
        setEditingPoll(null);
    };

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
            if (editingPoll) {
                const command: UpdatePollCommand = {
                    pollId: editingPoll.id,
                    question: formData.question,
                    isActive: isActive,
                    expiresAt: formData.expiresAt,
                };
                await dispatch(updatePoll({ id: editingPoll.id, command })).unwrap();
            } else {
                await dispatch(createPoll(formData)).unwrap();
            }
            handleClosePanel();
            dispatch(fetchPolls());
        } catch (error) {
            console.error('Failed to save poll', error);
        }
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this poll?')) {
            try {
                await dispatch(deletePoll(id)).unwrap();
                dispatch(fetchPolls());
            } catch (error) {
                console.error('Failed to delete poll', error);
            }
        }
    };

    const columns: GridColDef[] = [
        { field: 'question', headerName: 'Question', flex: 2, minWidth: 250 },
        { field: 'isActive', headerName: 'Active', type: 'boolean', width: 100 },
        {
            field: 'expiresAt',
            headerName: 'Expires At',
            width: 180,
            valueFormatter: (params) => params.value ? new Date(params.value).toLocaleString() : 'Never'
        },
        {
            field: 'actions',
            headerName: 'Actions',
            width: 120,
            sortable: false,
            renderCell: (params: GridRenderCellParams<Poll>) => (
                <Stack direction="row" spacing={1}>
                    <IconButton onClick={() => handleOpenPanel(params.row)} size="small" color="primary">
                        <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton onClick={() => handleDelete(params.row.id)} size="small" color="error">
                        <DeleteIcon fontSize="small" />
                    </IconButton>
                </Stack>
            ),
        },
    ];

    return (
        <Box sx={{ width: '100%' }}>
            <PageHeader
                title="Polls"
                action={
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => handleOpenPanel()}
                        sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600 }}
                    >
                        Create Poll
                    </Button>
                }
            />

            <Paper
                elevation={0}
                sx={{
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 2,
                    overflow: 'hidden'
                }}
            >
                <DataGrid
                    rows={polls}
                    columns={columns}
                    loading={loading}
                    autoHeight
                    initialState={{
                        pagination: {
                            paginationModel: { pageSize: 10 },
                        },
                    }}
                    pageSizeOptions={[10, 25, 50]}
                    disableRowSelectionOnClick
                    sx={{
                        border: 'none',
                        '& .MuiDataGrid-cell:focus': {
                            outline: 'none',
                        },
                        '& .MuiDataGrid-columnHeaders': {
                            backgroundColor: 'background.default',
                            borderBottom: '1px solid',
                            borderColor: 'divider',
                        },
                    }}
                />
            </Paper>

            <SidePanel
                open={openPanel}
                onClose={handleClosePanel}
                title={editingPoll ? 'Edit Poll' : 'Create Poll'}
                actions={
                    <>
                        <Button onClick={handleClosePanel} sx={{ textTransform: 'none' }}>Cancel</Button>
                        <Button onClick={handleSave} variant="contained" sx={{ textTransform: 'none' }}>
                            {editingPoll ? 'Save Changes' : 'Create Poll'}
                        </Button>
                    </>
                }
            >
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

                    {!editingPoll && (
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
                    )}

                    {editingPoll && (
                        <Box sx={{ p: 2, border: '1px solid', borderColor: 'divider', borderRadius: 1 }}>
                            <FormControlLabel
                                control={
                                    <Checkbox
                                        checked={isActive}
                                        onChange={(e) => setIsActive(e.target.checked)}
                                    />
                                }
                                label="Active Poll"
                            />
                        </Box>
                    )}

                    <TextField
                        label="Expires At"
                        type="datetime-local"
                        fullWidth
                        InputLabelProps={{ shrink: true }}
                        value={formData.expiresAt ? new Date(formData.expiresAt).toISOString().slice(0, 16) : ''}
                        onChange={(e) => setFormData({ ...formData, expiresAt: e.target.value || undefined })}
                    />
                </Stack>
            </SidePanel>
        </Box>
    );
}
