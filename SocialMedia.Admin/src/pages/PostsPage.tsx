// pages/PostsPage.tsx
import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
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
    fetchPostsByGroup,
    deletePost,
    clearPosts,
    setPageNumber,
    setPageSize
} from '../store/slices/postsSlice';
import { formatDateTime } from '../utils/dateTime';

export default function PostsPage() {
    const { user } = useAppSelector((state) => state.auth);
    const { groupId } = useParams<{ groupId: string }>();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const {
        items: posts,
        loading,
        pagination
    } = useAppSelector((state) => state.posts);

    React.useEffect(() => {
        if (groupId) {
            dispatch(fetchPostsByGroup({
                groupId,
                pageNumber: pagination.pageNumber,
                pageSize: pagination.pageSize
            }));
        }
        return () => {
            dispatch(clearPosts());
        };
    }, [dispatch, groupId, pagination.pageNumber, pagination.pageSize]);

    const handleDelete = async (postId: string) => {
        if (window.confirm('Are you sure you want to delete this post?')) {
            try {
                if (groupId) {
                    await dispatch(deletePost({ groupId, postId })).unwrap();
                }
            } catch (error) {
                console.error('Failed to delete post', error);
            }
        }
    };

    const handlePaginationModelChange = (model: { page: number; pageSize: number }) => {
        dispatch(setPageNumber(model.page + 1));
        dispatch(setPageSize(model.pageSize));
    };

    const columns: GridColDef[] = [
        {
            field: 'content',
            headerName: 'Content',
            flex: 2,
            minWidth: 300,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography variant="body2" noWrap>
                    {params.value?.length > 100 ? `${params.value.substring(0, 100)}...` : params.value}
                </Typography>
            )
        },
        {
            field: 'createdBy',
            headerName: 'Author',
            minWidth: 150,
        },
        {
            field: 'createdAt',
            headerName: 'Created At',
            minWidth: 250,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography variant="body2" noWrap>
                    {formatDateTime(params?.value)}
                </Typography>
            )
        },
        {
            field: 'actions',
            headerName: 'Actions',
            minWidth: 200,
            sortable: false,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Stack direction="row" spacing={1}>
                    {params.row.authorId === user?.id &&
                        <>
                        
                            <IconButton
                                onClick={() => navigate(`/groups/${groupId}/posts/${params.row.id}/edit`)}
                                size="small"
                                color="primary"
                            >
                                <EditIcon fontSize="small" />
                            </IconButton>
                            <IconButton onClick={() => handleDelete(params.row.id)} size="small" color="error">
                                <DeleteIcon fontSize="small" />
                            </IconButton>
                        </>
                    }
                </Stack>
            ),
        },
    ];
    
    if (!groupId) {
        return <Typography>Group ID is required</Typography>;
    }

    return (
        <Box sx={{ width: '100%' }}>
            <PageHeader
                title="Posts"
                action={
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={() => navigate(`/groups/${groupId}/posts/create`)}
                        sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600 }}
                    >
                        Create Post
                    </Button>
                }
            />

            <Typography variant="body2" color="text.secondary">
                NOTE: New added posts may take a few moments to appear in the list.
            </Typography>

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
                    rows={posts}
                    columns={columns}
                    loading={loading}
                    autoHeight
                    paginationMode="server"
                    rowCount={pagination.totalCount}
                    pageSizeOptions={[10, 25, 50]}
                    paginationModel={{
                        page: pagination.pageNumber - 1,
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
                    }}
                />
            </Paper>
        </Box>
    );
}