// pages/GroupsPage.tsx - Updated version
import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchGroups, deleteGroup } from '../store/slices/groupsSlice';

export default function GroupsPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { items: groups, loading } = useAppSelector((state) => state.groups);

    React.useEffect(() => {
        dispatch(fetchGroups());
    }, [dispatch]);

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
            field: 'posts',
            headerName: 'Posts',
            width: 130,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Button
                    variant="outlined"
                    size="small"
                    onClick={() => navigate(`/groups/${params.row.id}/posts`)}
                    sx={{ textTransform: 'none' }}
                >
                    View Posts
                </Button>
            ),
        },
        {
            field: 'actions',
            headerName: 'Actions',
            width: 120,
            sortable: false,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Stack direction="row" spacing={1}>
                    <IconButton
                        onClick={() => navigate(`/groups/${params.row.id}/edit`)}
                        size="small"
                        color="primary"
                    >
                        <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                        onClick={() => handleDelete(params.row.id)}
                        size="small"
                        color="error"
                    >
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
        </Box>
    );
}