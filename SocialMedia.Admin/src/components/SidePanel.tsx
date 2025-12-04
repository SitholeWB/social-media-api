import * as React from 'react';
import Drawer from '@mui/material/Drawer';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';

interface SidePanelProps {
    open: boolean;
    onClose: () => void;
    title: string;
    children: React.ReactNode;
    actions?: React.ReactNode;
}

export default function SidePanel({ open, onClose, title, children, actions }: SidePanelProps) {
    return (
        <Drawer
            anchor="right"
            open={open}
            onClose={onClose}
            PaperProps={{
                sx: { width: { xs: '100%', sm: 400, md: 500 } },
            }}
        >
            <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
                <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="h6" component="div">
                        {title}
                    </Typography>
                    <IconButton onClick={onClose}>
                        <CloseIcon />
                    </IconButton>
                </Box>
                <Divider />
                <Box sx={{ p: 3, flexGrow: 1, overflowY: 'auto' }}>
                    {children}
                </Box>
                {actions && (
                    <>
                        <Divider />
                        <Box sx={{ p: 2, bgcolor: 'background.default' }}>
                            <Stack direction="row" spacing={2} justifyContent="flex-end">
                                {actions}
                            </Stack>
                        </Box>
                    </>
                )}
            </Box>
        </Drawer>
    );
}
