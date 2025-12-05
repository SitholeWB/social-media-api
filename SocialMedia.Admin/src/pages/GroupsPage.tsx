import * as React from 'react';
import { useNavigate } from 'react-router-dom';
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
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import { Group, CreateGroupCommand, UpdateGroupCommand } from '../services/groupsService';
import SidePanel from '../components/SidePanel';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchGroups, createGroup, updateGroup, deleteGroup } from '../store/slices/groupsSlice';

export default function GroupsPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { items: groups, loading } = useAppSelector((state) => state.groups);

    const [openPanel, setOpenPanel] = React.useState(false);
    const [editingGroup, setEditingGroup] = React.useState<Group | null>(null);
    const [formData, setFormData] = React.useState<CreateGroupCommand>({
        name: '',
        description: '',
        isPublic: true,
        isAutoAdd: false,
    });

    React.useEffect(() => {
        dispatch(fetchGroups());
    }, [dispatch]);

    const handleOpenPanel = (group: Group) => {
        setEditingGroup(group);
        setFormData({
            name: group.name,
            description: group.description,
            isPublic: group.isPublic,
            isAutoAdd: group.isAutoAdd,
        });
        setOpenPanel(true);
    };

    const handleClosePanel = () => {
        setOpenPanel(false);
        setEditingGroup(null);
    };

    const handleSave = async () => {
        try {
            if (editingGroup) {
                const command: UpdateGroupCommand = {
                    groupId: editingGroup.id,
                    ...formData,
                };
                await dispatch(updateGroup({ id: editingGroup.id, command })).unwrap();
            }
            handleClosePanel();
            dispatch(fetchGroups());
        } catch (error) {
            console.error('Failed to save group', error);
        }
    };

    const handleDelete = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this group?')) {
            try {
                await dispatch(deleteGroup(id)).unwrap();
                dispatch(fetchGroups());
            } catch (error) {
                console.error('Failed to delete group', error);
            }
        }
    };

    const columns: GridColDef[] = [
        { field: 'name', headerName: 'Name', flex: 1, minWidth: 150 },
        { field: 'description', headerName: 'Description', flex: 2, minWidth: 200 },
        { field: 'isPublic', headerName: 'Public', type: 'boolean', width: 100 },
        { field: 'isAutoAdd', headerName: 'Auto Add', type: 'boolean', width: 100 },
        {
            field: 'actions',
            headerName: 'Actions',
            width: 120,
            sortable: false,
            renderCell: (params: GridRenderCellParams<Group>) => (
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
                title="Groups"
                action={
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => navigate('/groups/create')}
                        sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600 }}
                    >
                        Create Group
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
                    rows={groups}
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
                title="Edit Group"
                actions={
                    <>
                        <Button onClick={handleClosePanel} sx={{ textTransform: 'none' }}>Cancel</Button>
                        <Button onClick={handleSave} variant="contained" sx={{ textTransform: 'none' }}>
                            Save Changes
                        </Button>
                    </>
                }
            >
                <Stack spacing={3}>
                    <TextField
                        autoFocus
                        label="Name"
                        fullWidth
                        multiline
                        maxRows={4}
                        variant="outlined"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    />
                    <TextField
                        label="Description"
                        fullWidth
                        multiline
                        rows={4}
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
                </Stack>
            </SidePanel>
        </Box>
    );
}
