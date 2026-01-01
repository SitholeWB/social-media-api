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
	setPageNumber,
	setPageSize
} from '../store/slices/postsSlice';
import { formatDateTime } from '../utils/dateTime';
import { useAuth } from '../hooks/useAuth';

export default function PostsPage() {
	const { user } = useAuth();
	const { groupId } = useParams<{ groupId: string }>();
	const navigate = useNavigate();
	const dispatch = useAppDispatch();
	const {
		items: posts,
		loading,
		pagination
	} = useAppSelector((state) => state.posts);

	// Track if we've done the initial fetch
	const hasInitialFetch = React.useRef(false);

	React.useEffect(() => {
		if (!groupId) return;

		// Force initial fetch when groupId is available
		if (!hasInitialFetch.current) {
			dispatch(fetchPostsByGroup({
				groupId,
				pageNumber: pagination.pageNumber,
				pageSize: pagination.pageSize
			}));
			hasInitialFetch.current = true;
			return;
		}

		// Store previous values to detect changes
		const hasPageNumberChanged = pagination.pageNumber !== 1; // Compare with initial
		const hasPageSizeChanged = pagination.pageSize !== 10; // Compare with initial

		// Only fetch if something relevant changed
		if (hasPageNumberChanged || hasPageSizeChanged) {
			dispatch(fetchPostsByGroup({
				groupId,
				pageNumber: pagination.pageNumber,
				pageSize: pagination.pageSize
			}));
		}

		// Cleanup: only clear posts when groupId changes
		return () => {
			// Reset initial fetch flag when groupId changes
			hasInitialFetch.current = false;
		};
	}, [dispatch, groupId, pagination.pageNumber, pagination.pageSize]);

	// Alternative simpler approach - just fetch on mount and when dependencies change
	// React.useEffect(() => {
	//     if (groupId) {
	//         dispatch(fetchPostsByGroup({
	//             groupId,
	//             pageNumber: pagination.pageNumber,
	//             pageSize: pagination.pageSize
	//         }));
	//     }
	//
	//     return () => {
	//         if (groupId) {
	//             dispatch(clearPosts());
	//         }
	//     };
	// }, [dispatch, groupId, pagination.pageNumber, pagination.pageSize]);

	const handleDelete = async (postId: string) => {
		if (window.confirm('Are you sure you want to delete this post?')) {
			try {
				if (groupId) {
					await dispatch(deletePost({ groupId, postId })).unwrap();
					// Refetch to update the list
					dispatch(fetchPostsByGroup({
						groupId,
						pageNumber: pagination.pageNumber,
						pageSize: pagination.pageSize
					}));
				}
			} catch (error) {
				console.error('Failed to delete post', error);
			}
		}
	};

	const handlePaginationModelChange = React.useCallback((model: { page: number; pageSize: number }) => {
		const newPageNumber = model.page + 1;
		const newPageSize = model.pageSize;

		// Only dispatch if values actually changed
		if (newPageNumber !== pagination.pageNumber) {
			dispatch(setPageNumber(newPageNumber));
		}
		if (newPageSize !== pagination.pageSize) {
			dispatch(setPageSize(newPageSize));
		}
	}, [dispatch, pagination.pageNumber, pagination.pageSize]);

	// Memoize columns to prevent unnecessary re-renders
	const columns = React.useMemo<GridColDef[]>(() => [
		{
			field: 'content',
			headerName: 'Content',
			flex: 2,
			minWidth: 300,
			renderCell: (params: GridRenderCellParams<any>) => (
				<Typography
					variant="body2"
					sx={{
						cursor: 'pointer',
						'&:hover': {
							textDecoration: 'underline',
							color: 'primary.main'
						}
					}}
					onClick={() => navigate(`/groups/${groupId}/posts/${params.row.id}`)}
				>
					{params.value?.length > 100 ? `${params.value.substring(0, 100)}...` : params.value}
				</Typography>
			)
		},
		{
			field: 'commentCount',
			headerName: 'Comments',
			width: 110,
			renderCell: (params: GridRenderCellParams<any>) => (
				<Typography variant="body2" align="center">
					{params.value || 0}
				</Typography>
			)
		},
		{
			field: 'authorName',
			headerName: 'Author',
			width: 150,
		},
		{
			field: 'likeCount',
			headerName: 'Reactions',
			width: 100,
			renderCell: (params: GridRenderCellParams<any>) => (
				<Typography variant="body2" align="center">
					{params.value || 0}
				</Typography>
			)
		},
		{
			field: 'reactions',
			headerName: 'Reaction Details',
			width: 200,
			renderCell: (params: GridRenderCellParams<any>) => (
				<Stack direction="row" spacing={0.5} flexWrap="wrap" useFlexGap>
					{params?.value?.map((r: any) => (
						<Box key={r.emoji} sx={{ display: 'flex', alignItems: 'center', mr: 1 }}>
							<Typography variant="body2">{r.emoji}</Typography>
							<Typography variant="caption" color="text.secondary" sx={{ ml: 0.5 }}>
								({r.count})
							</Typography>
						</Box>
					)) || (
							<Typography variant="caption" color="text.secondary">
								No reactions
							</Typography>
						)}
				</Stack>
			)
		},
		{
			field: 'media',
			headerName: 'Media',
			width: 150,
			renderCell: (params: GridRenderCellParams<any>) => {
				const media = params.value as any[];
				if (!media || media.length === 0) return <Typography variant="caption" color="text.secondary">None</Typography>;

				return (
					<Stack direction="row" spacing={0.5} sx={{ py: 1 }}>
						{media.slice(0, 3).map((m, i) => (
							<Box
								key={i}
								sx={{
									width: 32,
									height: 32,
									borderRadius: 0.5,
									overflow: 'hidden',
									border: '1px solid',
									borderColor: 'divider'
								}}
							>
								<img
									src={m.url}
									alt="media"
									style={{ width: '100%', height: '100%', objectFit: 'cover' }}
								/>
							</Box>
						))}
						{media.length > 3 && (
							<Typography variant="caption" sx={{ alignSelf: 'center', ml: 0.5 }}>
								+{media.length - 3}
							</Typography>
						)}
					</Stack>
				);
			}
		},
		{
			field: 'createdAt',
			headerName: 'Created At',
			width: 180,
			renderCell: (params) => formatDateTime(params.value)
		},
		{
			field: 'actions',
			headerName: 'Actions',
			width: 120,
			sortable: false,
			renderCell: (params: GridRenderCellParams<any>) => (
				<Stack direction="row" spacing={1}>
					{params.row.authorId === user?.id && (
						<>
							<IconButton
								onClick={() => navigate(`/groups/${groupId}/posts/${params.row.id}/edit`)}
								size="small"
								color="primary"
								title="Edit post"
							>
								<EditIcon fontSize="small" />
							</IconButton>
							<IconButton
								onClick={() => handleDelete(params.row.id)}
								size="small"
								color="error"
								title="Delete post"
							>
								<DeleteIcon fontSize="small" />
							</IconButton>
						</>
					)}
					{params.row.authorId !== user?.id && (
						<Typography variant="caption" color="text.secondary">
							Not your post
						</Typography>
					)}
				</Stack>
			),
		},
	], [groupId, user?.id, navigate, handleDelete]); // Added handleDelete to dependencies

	// Handle navigation to create post
	const handleCreatePost = React.useCallback(() => {
		navigate(`/groups/${groupId}/posts/create`);
	}, [groupId, navigate]);

	// Handle back to groups
	const handleBackToGroups = React.useCallback(() => {
		navigate('/groups');
	}, [navigate]);

	if (!groupId) {
		return (
			<Box sx={{ p: 4, textAlign: 'center' }}>
				<Typography variant="h6" color="error" gutterBottom>
					Group ID is required
				</Typography>
				<Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
					Please select a group to view posts
				</Typography>
				<Button
					onClick={handleBackToGroups}
					variant="contained"
					sx={{ textTransform: 'none' }}
				>
					Back to Groups
				</Button>
			</Box>
		);
	}

	return (
		<Box sx={{ width: '100%', maxWidth: 1400, mx: 'auto', p: { xs: 1, sm: 2, md: 3 } }}>
			<PageHeader
				title="Posts"
				action={
					<Stack direction="row" spacing={2} alignItems="center">
						<Button
							onClick={handleBackToGroups}
							variant="outlined"
							sx={{ textTransform: 'none' }}
						>
							Back to Groups
						</Button>
						<Button
							variant="contained"
							startIcon={<AddIcon />}
							onClick={handleCreatePost}
							sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600 }}
							disabled={loading}
						>
							Create Post
						</Button>
					</Stack>
				}
			/>

			{/* Summary Stats */}
			{posts.length > 0 && (
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
						flexWrap: 'wrap',
						gap: 2,
					}}
				>
					<Typography variant="body2" color="text.secondary">
						Showing <strong>{posts.length}</strong> of <strong>{pagination.totalCount}</strong> posts
					</Typography>
					<Typography variant="body2" color="text.secondary">
						Page <strong>{pagination.pageNumber}</strong> of <strong>{pagination.totalPages}</strong>
					</Typography>
					{user && (
						<Typography variant="body2" color="text.secondary">
							Your posts: <strong>{posts.filter(p => p.authorId === user.id).length}</strong>
						</Typography>
					)}
				</Paper>
			)}

			<Typography variant="body2" color="text.secondary" sx={{ mb: 2, fontStyle: 'italic' }}>
				💡 Note: New posts may take a few moments to appear. Click on post content to view details.
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
					getRowHeight={() => 'auto'}
					sx={{
						border: 'none',
						'& .MuiDataGrid-cell': {
							py: 1.5,
							'&:focus': {
								outline: 'none',
							},
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
						'& .MuiDataGrid-row': {
							'&:hover': {
								backgroundColor: 'action.hover',
							},
						},
					}}
				/>
			</Paper>

			{/* Empty State */}
			{posts.length === 0 && !loading && (
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
						No posts found in this group
					</Typography>
					<Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
						Be the first to create a post in this group
					</Typography>
					<Button
						variant="contained"
						startIcon={<AddIcon />}
						onClick={handleCreatePost}
						sx={{ textTransform: 'none', mr: 2 }}
					>
						Create First Post
					</Button>
					<Button
						variant="outlined"
						onClick={handleBackToGroups}
						sx={{ textTransform: 'none' }}
					>
						Back to Groups
					</Button>
				</Paper>
			)}

			{/* Loading State */}
			{loading && posts.length === 0 && (
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
					<Typography variant="body2" color="text.secondary">
						Loading posts...
					</Typography>
				</Paper>
			)}
		</Box>
	);
}