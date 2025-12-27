import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';
import Avatar from '@mui/material/Avatar';
import Chip from '@mui/material/Chip';
import Button from '@mui/material/Button';
import EditIcon from '@mui/icons-material/Edit';
import PageHeader from '../components/PageHeader';
import { useAppSelector } from '../store/hooks';

export default function ProfilePage() {
	const { user } = useAppSelector((state) => state.auth);

	return (
		<Box sx={{ width: '100%' }}>
			<PageHeader title="My Profile" />

			<Stack spacing={3} sx={{ maxWidth: 800 }}>
				<Paper
					elevation={0}
					sx={{
						p: 3,
						border: '1px solid',
						borderColor: 'divider',
						borderRadius: 2,
					}}
				>
					<Stack direction={{ xs: 'column', sm: 'row' }} spacing={3} alignItems="center">
						<Avatar
							sx={{
								width: 100,
								height: 100,
								bgcolor: 'primary.main',
								fontSize: '2.5rem',
								fontWeight: 700,
							}}
						>
							{user?.username?.charAt(0).toUpperCase() || 'U'}
						</Avatar>
						<Box sx={{ flexGrow: 1, textAlign: { xs: 'center', sm: 'left' } }}>
							<Typography variant="h5" fontWeight={700}>
								{user?.username || 'User'}
							</Typography>
							<Typography variant="body1" color="text.secondary" gutterBottom>
								{user?.email || 'user@example.com'}
							</Typography>
							<Stack direction="row" spacing={1} justifyContent={{ xs: 'center', sm: 'flex-start' }} sx={{ mt: 1 }}>
								<Chip label="Admin" color="primary" size="small" />
								<Chip label="Active" color="success" size="small" variant="outlined" />
							</Stack>
						</Box>
						<Button
							variant="outlined"
							startIcon={<EditIcon />}
						>
							Edit Profile
						</Button>
					</Stack>
				</Paper>

				<Paper
					elevation={0}
					sx={{
						p: 3,
						border: '1px solid',
						borderColor: 'divider',
						borderRadius: 2,
					}}
				>
					<Typography variant="h6" fontWeight={600} gutterBottom>
						Account Details
					</Typography>
					<Divider sx={{ my: 2 }} />
					<Stack spacing={2}>
						<Box>
							<Typography variant="caption" color="text.secondary">
								User ID
							</Typography>
							<Typography variant="body1">
								{user?.id || 'N/A'}
							</Typography>
						</Box>
						<Box>
							<Typography variant="caption" color="text.secondary">
								Role
							</Typography>
							<Typography variant="body1">
								Administrator
							</Typography>
						</Box>
						<Box>
							<Typography variant="caption" color="text.secondary">
								Joined
							</Typography>
							<Typography variant="body1">
								December 2025
							</Typography>
						</Box>
					</Stack>
				</Paper>
			</Stack>
		</Box>
	);
}