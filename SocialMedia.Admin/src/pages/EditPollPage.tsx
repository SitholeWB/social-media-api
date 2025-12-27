// pages/EditPollPage.tsx
import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { fetchPoll, updatePoll } from '../store/slices/pollsSlice';
import { fetchGroups } from '../store/slices/groupsSlice';
import { UpdatePollCommand } from '../services/pollsService';
import { FormControl, InputLabel, Select, MenuItem } from '@mui/material';

export default function EditPollPage() {
	const { pollId } = useParams<{ pollId: string }>();
	const navigate = useNavigate();
	const dispatch = useAppDispatch();
	const { currentPoll, loading: reduxLoading } = useAppSelector((state) => state.polls);

	const [question, setQuestion] = React.useState('');
	const [isActive, setIsActive] = React.useState(true);
	const [expiresAt, setExpiresAt] = React.useState<string | undefined>(undefined);
	const [groupId, setGroupId] = React.useState('');
	const [isAnonymous, setIsAnonymous] = React.useState(false);
	const [loading, setLoading] = React.useState(false);
	const [error, setError] = React.useState<string | null>(null);

	const { items: groups } = useAppSelector((state) => state.groups);

	React.useEffect(() => {
		dispatch(fetchPoll(pollId!));
		dispatch(fetchGroups({ pageNumber: 1, pageSize: 100 }));
	}, [dispatch, pollId]);

	React.useEffect(() => {
		if (pollId && currentPoll) {
			setQuestion(currentPoll.question);
			setIsActive(currentPoll.isActive);
			setExpiresAt(currentPoll.expiresAt || undefined);
			setGroupId(currentPoll.groupId || '');
			setIsAnonymous(currentPoll.isAnonymous || false);
		}
	}, [pollId, currentPoll]);

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!question.trim()) {
			setError('Question is required');
			return;
		}

		try {
			setLoading(true);
			setError(null);

			if (pollId) {
				const command: UpdatePollCommand = {
					pollId,
					question,
					isActive,
					expiresAt: expiresAt || undefined,
					groupId,
					isAnonymous,
				};

				await dispatch(updatePoll({
					id: pollId,
					command
				})).unwrap();

				navigate('/polls');
			}
		} catch (err: any) {
			setError(err.message || 'Failed to update poll');
		} finally {
			setLoading(false);
		}
	};

	const handleCancel = () => {
		navigate('/polls');
	};

	if (!pollId) {
		return <Typography>Poll ID is required</Typography>;
	}

	if (reduxLoading && !currentPoll) {
		return <Typography>Loading...</Typography>;
	}

	if (!currentPoll && !reduxLoading) {
		return <Typography>Poll not found</Typography>;
	}

	return (
		<Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
			<PageHeader
				title="Edit Poll"
				action={
					<Button
						startIcon={<ArrowBackIcon />}
						onClick={handleCancel}
						sx={{ textTransform: 'none' }}
					>
						Back to Polls
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
							autoFocus
							label="Question"
							fullWidth
							multiline
							variant="standard"
							value={question}
							onChange={(e) => setQuestion(e.target.value)}
							required
						/>

						<FormControl fullWidth variant="standard">
							<InputLabel>Group</InputLabel>
							<Select
								value={groupId}
								onChange={(e) => setGroupId(e.target.value as string)}
								label="Group"
								disabled={loading || reduxLoading}
							>
								{groups.map((group) => (
									<MenuItem key={group.id} value={group.id}>
										{group.name}
									</MenuItem>
								))}
							</Select>
						</FormControl>

						<FormControlLabel
							control={
								<Checkbox
									checked={isAnonymous}
									onChange={(e) => setIsAnonymous(e.target.checked)}
									disabled={loading || reduxLoading}
								/>
							}
							label="Anonymous Voting"
						/>

						<Paper
							elevation={0}
							sx={{
								p: 2,
								border: '1px solid',
								borderColor: 'divider',
								borderRadius: 1,
							}}
						>
							<FormControlLabel
								control={
									<Checkbox
										checked={isActive}
										onChange={(e) => setIsActive(e.target.checked)}
										disabled={loading || reduxLoading}
									/>
								}
								label="Active Poll"
							/>
						</Paper>

						<TextField
							label="Expires At (Optional)"
							type="datetime-local"
							fullWidth
							value={expiresAt ? new Date(expiresAt).toISOString().slice(0, 16) : ''}
							onChange={(e) => setExpiresAt(e.target.value || undefined)}
							disabled={loading || reduxLoading}
							slotProps={{ inputLabel: { shrink: true } }}
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