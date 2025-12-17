// pages/PollsPage.tsx
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
import {
    fetchPolls,
    deletePoll,
    clearPolls,
    setPageNumber,
    setPageSize
} from '../store/slices/pollsSlice';
import { formatDate } from '../utils/dateTime';

export default function PollsPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const {
        items: polls,
        loading,
        pagination
    } = useAppSelector((state) => state.polls);

    React.useEffect(() => {
        dispatch(fetchPolls({
            pageNumber: pagination.pageNumber,
            pageSize: pagination.pageSize
        }));

        return () => {
            dispatch(clearPolls());
        };
    }, [dispatch, pagination.pageNumber, pagination.pageSize]);

    const handleDelete = async (id: string) => {
        if (window.confirm('Are you sure you want to delete this poll?')) {
            try {
                await dispatch(deletePoll(id)).unwrap();
                // No need to refetch here, the slice will handle it
            } catch (error) {
                console.error('Failed to delete poll', error);
            }
        }
    };

    const handlePaginationModelChange = (model: { page: number; pageSize: number }) => {
        // MUI DataGrid uses 0-based page index, but our API uses 1-based
        dispatch(setPageNumber(model.page + 1));
        dispatch(setPageSize(model.pageSize));
    };

    const columns: GridColDef[] = [
        {
            field: 'question',
            headerName: 'Question',
            flex: 2,
            minWidth: 250,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography
                    variant="body1"
                    sx={{
                        cursor: 'pointer',
                        '&:hover': {
                            textDecoration: 'underline',
                            color: 'primary.main'
                        }
                    }}
                    onClick={() => navigate(`/polls/${params.row.id}`)}
                >
                    {params.value}
                </Typography>
            )
        },
        {
            field: 'isActive',
            headerName: 'Active',
            type: 'boolean',
            width: 100,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography
                    sx={{
                        px: 1,
                        py: 0.5,
                        borderRadius: 1,
                        backgroundColor: params.value ? 'success.light' : 'error.light',
                        color: params.value ? 'success.contrastText' : 'error.contrastText',
                        fontSize: '0.75rem',
                        fontWeight: 500,
                    }}
                >
                    {params.value ? 'Active' : 'Inactive'}
                </Typography>
            )
        },
        {
            field: 'options',
            headerName: 'Options',
            width: 100,
            renderCell: (params: GridRenderCellParams<any>) => (
                <>
                    {params.value?.length}
                </>
            )
        },
        {
            field: 'totalVotes',
            headerName: 'Votes',
            width: 100
        },
        {
            field: 'expiresAt',
            headerName: 'Expires At',
            width: 180,
            renderCell: (params: GridRenderCellParams<any>) => (
                <>
                    {formatDate(params.value)}
                </>
            )
        },
        {
            field: 'actions',
            headerName: 'Actions',
            width: 180,
            sortable: false,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Stack direction="row" spacing={1}>
                    <Button
                        variant="outlined"
                        size="small"
                        onClick={() => navigate(`/polls/${params.row.id}`)}
                        sx={{ textTransform: 'none', minWidth: 70 }}
                    >
                        View
                    </Button>
                    <IconButton
                        onClick={() => navigate(`/polls/${params.row.id}/edit`)}
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
                title="Polls"
                action={
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => navigate('/polls/create')}
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

            {/* Summary Statistics */}
            {polls.length > 0 && (
                <Paper
                    elevation={0}
                    sx={{
                        mt: 2,
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
                        Showing {polls.length} of {pagination.totalCount} polls
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        Page {pagination.pageNumber} of {pagination.totalPages}
                    </Typography>
                </Paper>
            )}

            {/* Empty State */}
            {polls.length === 0 && !loading && (
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
                        No polls found
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                        Create your first poll to get started
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => navigate('/polls/create')}
                        sx={{ textTransform: 'none' }}
                    >
                        Create First Poll
                    </Button>
                </Paper>
            )}
        </Box>
    );
}