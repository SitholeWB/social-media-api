// pages/GroupsPage.tsx - With Server-Side Pagination
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
import { fetchGroups, deleteGroup, setPageNumber, setPageSize } from '../store/slices/groupsSlice';
import { GroupType } from '../services/groupsService';

export default function GroupsPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const {
        items: groups,
        loading,
        pagination
    } = useAppSelector((state) => state.groups);

    // Fetch groups with current pagination
    React.useEffect(() => {
        dispatch(fetchGroups({
            pageNumber: pagination.pageNumber,
            pageSize: pagination.pageSize
        }));
    }, [dispatch, pagination.pageNumber, pagination.pageSize]);

    const handleDelete = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this group?')) {
            try {
                await dispatch(deleteGroup(id)).unwrap();
                // Refetch current page after deletion
                dispatch(fetchGroups({
                    pageNumber: pagination.pageNumber,
                    pageSize: pagination.pageSize
                }));
            } catch (error) {
                console.error('Failed to delete group', error);
            }
        }
    };

    const handlePaginationModelChange = (model: { page: number; pageSize: number }) => {
        // MUI DataGrid uses 0-based page index, convert to 1-based
        dispatch(setPageNumber(model.page + 1));
        dispatch(setPageSize(model.pageSize));
    };

    const columns: GridColDef[] = [
        { field: 'name', headerName: 'Name', flex: 1, minWidth: 150 },
        { field: 'description', headerName: 'Description', flex: 2, minWidth: 200 },
        {
            field: 'type',
            headerName: 'Type',
            width: 120,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography
                    sx={{
                        px: 1,
                        py: 0.5,
                        borderRadius: 1,
                        backgroundColor: params.value === GroupType.Private ? 'error.light' : (params.value === GroupType.Everyone ? 'info.light' : 'success.light'),
                        color: 'white',
                        fontSize: '0.75rem',
                        fontWeight: 500,
                    }}
                >
                    {params.value === GroupType.Private ? 'Private' : (params.value === GroupType.Everyone ? 'Everyone' : 'Public')}
                </Typography>
            )
        },
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
            field: 'polls',
            headerName: 'Polls',
            width: 130,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Button
                    variant="outlined"
                    size="small"
                    onClick={() => navigate(`/groups/${params.row.id}/polls`)}
                    sx={{ textTransform: 'none' }}
                >
                    View Polls
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

            {/* Summary Stats */}
            {groups.length > 0 && (
                <Paper
                    elevation={0}
                    sx={{
                        mb: 2,
                        p: 2,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                    }}
                >
                    <Typography variant="body2" color="text.secondary">
                        Showing {groups.length} of {pagination.totalCount} groups
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Page {pagination.pageNumber} of {pagination.totalPages}
                    </Typography>
                </Paper>
            )}

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
                    paginationMode="server"
                    rowCount={pagination.totalCount}
                    pageSizeOptions={[10, 25, 50]}
                    paginationModel={{
                        page: pagination.pageNumber - 1, // Convert to 0-based
                        pageSize: pagination.pageSize,
                    }}
                    onPaginationModelChange={handlePaginationModelChange}
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
                        '& .MuiDataGrid-footerContainer': {
                            borderTop: '1px solid',
                            borderColor: 'divider',
                        },
                    }}
                />
            </Paper>

            {/* Empty State */}
            {groups.length === 0 && !loading && (
                <Paper
                    elevation={0}
                    sx={{
                        mt: 2,
                        p: 4,
                        border: '1px solid',
                        borderColor: 'divider',
                        borderRadius: 2,
                        textAlign: 'center',
                    }}
                >
                    <Typography variant="h6" color="text.secondary" gutterBottom>
                        No groups found
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                        Create your first group to get started
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => navigate('/groups/create')}
                        sx={{ textTransform: 'none' }}
                    >
                        Create First Group
                    </Button>
                </Paper>
            )}
        </Box>
    );
}