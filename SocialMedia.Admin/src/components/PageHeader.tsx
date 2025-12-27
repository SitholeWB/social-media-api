import * as React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';

interface PageHeaderProps {
	title: string;
	action?: React.ReactNode;
}

export default function PageHeader({ title, action }: PageHeaderProps) {
	return (
		<Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
			<Typography variant="h4" component="h1" fontWeight="bold">
				{title}
			</Typography>
			<Stack direction="row" spacing={2}>
				{action}
			</Stack>
		</Box>
	);
}