// pages/CreatePostPage.tsx
import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import PageHeader from '../components/PageHeader';
import { useAppDispatch } from '../store/hooks';
import { createPost } from '../store/slices/postsSlice';

export default function CreatePostPage() {
	const { groupId } = useParams<{ groupId: string }>();
	const navigate = useNavigate();
	const dispatch = useAppDispatch();

	const [content, setContent] = React.useState('');
	const [loading, setLoading] = React.useState(false);
	const [error, setError] = React.useState<string | null>(null);

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!content.trim()) {
			setError('Content is required');
			return;
		}

		try {
			setLoading(true);
			setError(null);

			if (groupId) {
				await dispatch(createPost({
					groupId,
					command: { content }
				})).unwrap();

				navigate(`/groups/${groupId}/posts`);
			}
		} catch (err: any) {
			setError(err.message || 'Failed to create post');
		} finally {
			setLoading(false);
		}
	};

	const handleCancel = () => {
		navigate(`/groups/${groupId}/posts`);
	};

	if (!groupId) {
		return <Typography> Group ID is required </Typography>;
	}

	return (
		<Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
			<PageHeader
				title="Create Post"
				action={
					<Button
						startIcon={<ArrowBackIcon />}
						onClick={handleCancel}
						sx={{ textTransform: 'none' }}
					>
						Back to Posts
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
							label="Content"
							fullWidth
							variant="standard"
							value={content}
							onChange={(e) => setContent(e.target.value)}
							disabled={loading}
							required
							multiline
						/>

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
								disabled={loading}
								sx={{ textTransform: 'none' }}
							>
								{loading ? 'Creating...' : 'Create Post'}
							</Button>
						</Stack>
					</Stack>
				</form>
			</Paper>
		</Box>
	);
}