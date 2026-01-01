import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import DeleteIcon from '@mui/icons-material/Delete';
import IconButton from '@mui/material/IconButton';
import CircularProgress from '@mui/material/CircularProgress';
import PageHeader from '../components/PageHeader';
import { useAppDispatch } from '../store/hooks';
import { createPost } from '../store/slices/postsSlice';
import { fileService } from '../services/fileService';

interface SelectedFile {
	file: File;
	id: string;
	previewUrl: string;
	uploading: boolean;
	uploadedUrl?: string;
	error?: string;
}

export default function CreatePostPage() {
	const { groupId } = useParams<{ groupId: string }>();
	const navigate = useNavigate();
	const dispatch = useAppDispatch();

	const [content, setContent] = React.useState('');
	const [selectedFiles, setSelectedFiles] = React.useState<SelectedFile[]>([]);
	const [loading, setLoading] = React.useState(false);
	const [error, setError] = React.useState<string | null>(null);

	const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
		if (e.target.files) {
			const files = Array.from(e.target.files);
			const newSelectedFiles: SelectedFile[] = files.map(file => ({
				file,
				id: Math.random().toString(36).substring(7),
				previewUrl: URL.createObjectURL(file),
				uploading: false
			}));
			setSelectedFiles(prev => [...prev, ...newSelectedFiles]);
		}
	};

	const removeFile = (id: string) => {
		setSelectedFiles(prev => {
			const fileToRemove = prev.find(f => f.id === id);
			if (fileToRemove) {
				URL.revokeObjectURL(fileToRemove.previewUrl);
			}
			return prev.filter(f => f.id !== id);
		});
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!content.trim()) {
			setError('Content is required');
			return;
		}

		try {
			setLoading(true);
			setError(null);

			// 1. Upload files
			const uploadedMedia = [];
			const updatedFiles = [...selectedFiles];

			for (let i = 0; i < updatedFiles.length; i++) {
				const fileObj = updatedFiles[i];
				if (fileObj.uploadedUrl) {
					uploadedMedia.push({ url: fileObj.uploadedUrl });
					continue;
				}

				try {
					updatedFiles[i] = { ...fileObj, uploading: true, error: undefined };
					setSelectedFiles([...updatedFiles]);

					const response = await fileService.uploadFile(fileObj.file);

					updatedFiles[i] = { ...fileObj, uploading: false, uploadedUrl: response.url };
					setSelectedFiles([...updatedFiles]);
					uploadedMedia.push({ url: response.url });
				} catch (uploadErr: any) {
					updatedFiles[i] = { ...fileObj, uploading: false, error: uploadErr.message || 'Upload failed' };
					setSelectedFiles([...updatedFiles]);
					throw new Error(`Failed to upload ${fileObj.file.name}: ${uploadErr.message}`);
				}
			}

			// 2. Create post
			if (groupId) {
				await dispatch(createPost({
					groupId,
					command: {
						content,
						media: uploadedMedia.length > 0 ? uploadedMedia : undefined
					}
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

	React.useEffect(() => {
		return () => {
			selectedFiles.forEach(f => URL.revokeObjectURL(f.previewUrl));
		};
	}, []);

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
							minRows={3}
						/>

						<Box>
							<Typography variant="subtitle2" gutterBottom fontWeight={600}>
								Media (Images/GIFs)
							</Typography>
							<input
								accept="image/*,image/gif"
								style={{ display: 'none' }}
								id="raised-button-file"
								multiple
								type="file"
								onChange={handleFileSelect}
								disabled={loading}
							/>
							<label htmlFor="raised-button-file">
								<Button
									variant="outlined"
									component="span"
									startIcon={<CloudUploadIcon />}
									disabled={loading}
									sx={{ textTransform: 'none' }}
								>
									Add Media
								</Button>
							</label>

							{selectedFiles.length > 0 && (
								<Stack direction="row" spacing={2} sx={{ mt: 2, flexWrap: 'wrap', gap: 2 }}>
									{selectedFiles.map((fileObj) => (
										<Box
											key={fileObj.id}
											sx={{
												position: 'relative',
												width: 100,
												height: 100,
												borderRadius: 1,
												overflow: 'hidden',
												border: '1px solid',
												borderColor: 'divider'
											}}
										>
											<img
												src={fileObj.previewUrl}
												alt="preview"
												style={{ width: '100%', height: '100%', objectFit: 'cover' }}
											/>
											{fileObj.uploading && (
												<Box
													sx={{
														position: 'absolute',
														top: 0,
														left: 0,
														right: 0,
														bottom: 0,
														bgcolor: 'rgba(255, 255, 255, 0.7)',
														display: 'flex',
														alignItems: 'center',
														justifyContent: 'center'
													}}
												>
													<CircularProgress size={24} />
												</Box>
											)}
											<IconButton
												size="small"
												onClick={() => removeFile(fileObj.id)}
												disabled={loading}
												sx={{
													position: 'absolute',
													top: 2,
													right: 2,
													bgcolor: 'rgba(255, 255, 255, 0.8)',
													'&:hover': { bgcolor: 'rgba(255, 255, 255, 0.9)' }
												}}
											>
												<DeleteIcon fontSize="small" color="error" />
											</IconButton>
											{fileObj.error && (
												<Typography
													variant="caption"
													color="error"
													sx={{
														position: 'absolute',
														bottom: 0,
														left: 0,
														right: 0,
														bgcolor: 'rgba(255, 255, 255, 0.8)',
														textAlign: 'center',
														fontSize: '0.6rem'
													}}
												>
													Error
												</Typography>
											)}
										</Box>
									))}
								</Stack>
							)}
						</Box>

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
								{loading ? 'Processing...' : 'Create Post'}
							</Button>
						</Stack>
					</Stack>
				</form>
			</Paper>
		</Box>
	);
}
