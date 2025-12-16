// pages/EditPostPage.tsx
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
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchPost, updatePost } from '../store/slices/postsSlice';

export default function EditPostPage() {
    const { groupId, postId } = useParams<{ groupId: string; postId: string }>();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { currentPost, loading: reduxLoading } = useAppSelector((state) => state.posts);

    const [content, setContent] = React.useState('');
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState<string | null>(null);

    React.useEffect(() => {
        if (groupId && postId) {
            dispatch(fetchPost({ groupId, postId }));
        }
    }, [dispatch, groupId, postId]);

    React.useEffect(() => {
        if (currentPost && currentPost.id === postId) {
            setContent(currentPost.content);
        }
    }, [currentPost, postId]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!content.trim()) {
            setError('Content is required');
            return;
        }

        try {
            setLoading(true);
            setError(null);

            if (groupId && postId) {
                await dispatch(updatePost({
                    groupId,
                    postId,
                    command: { content }
                })).unwrap();

                navigate(`/groups/${groupId}/posts`);
            }
        } catch (err: any) {
            setError(err.message || 'Failed to update post');
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = () => {
        navigate(`/groups/${groupId}/posts`);
    };

    if (!groupId || !postId) {
        return <Typography>Group ID and Post ID are required </Typography>;
    }

    if (reduxLoading && !currentPost) {
        return <Typography>Loading...</Typography>;
    }

    if (!currentPost && !reduxLoading) {
        return <Typography>Post not found </Typography>;
    }

    return (<Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
        <PageHeader
            title="Edit Post"
            action={
                < Button
                    startIcon={< ArrowBackIcon />}
                    onClick={handleCancel}
                    sx={{ textTransform: 'none' }}
                >
                    Back to Posts
                </Button>
            }
        />

        < Paper
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
                        multiline
                        variant="standard"
                        value={content}
                        onChange={(e) => setContent(e.target.value)}
                        disabled={loading || reduxLoading}
                        required
                    />

                    < Stack direction="row" spacing={2} justifyContent="flex-end" >
                        <Button
                            onClick={handleCancel}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Cancel
                        </Button>
                        < Button
                            type="submit"
                            variant="contained"
                            disabled={loading || reduxLoading}
                            sx={{ textTransform: 'none' }}
                        >
                            {loading ? 'Saving...' : 'Save Changes'}
                        </Button>
                    </Stack>
                </Stack>
            </form>
        </Paper>
    </Box>
    );
}