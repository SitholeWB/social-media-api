// pages/ReportedPostsPage.tsx
import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Chip from '@mui/material/Chip';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import VisibilityIcon from '@mui/icons-material/Visibility';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid';
import PageHeader from '../components/PageHeader';
import SidePanel from '../components/SidePanel';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { 
    fetchPendingReports, 
    updateReportStatus, 
    deleteReport,
    clearReports,
    setPageNumber,
    setPageSize,
    setStatusFilter
} from '../store/slices/reportsSlice';
import { ReportStatus } from '../services/reportsService';
import { formatDateTime } from '../utils/dateTime';

export default function ReportedPostsPage() {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { 
        items: reports, 
        loading, 
        pagination,
        filters
    } = useAppSelector((state) => state.reports);

    const [openPanel, setOpenPanel] = React.useState(false);
    const [selectedReport, setSelectedReport] = React.useState<any>(null);
    const [actionLoading, setActionLoading] = React.useState(false);

    React.useEffect(() => {
        dispatch(fetchPendingReports({ 
            pageNumber: pagination.pageNumber, 
            pageSize: pagination.pageSize
        }));

        return () => {
            dispatch(clearReports());
        };
    }, [dispatch, pagination.pageNumber, pagination.pageSize, filters.status]);

    const handlePaginationModelChange = (model: { page: number; pageSize: number }) => {
        dispatch(setPageNumber(model.page + 1));
        dispatch(setPageSize(model.pageSize));
    };

    const handleStatusFilterChange = (event: SelectChangeEvent) => {
        const value = event.target.value;
        if (value === 'all') {
            dispatch(setStatusFilter(undefined));
        } else {
            dispatch(setStatusFilter(parseInt(value) as ReportStatus));
        }
    };

    const handleOpenPanel = (report: any) => {
        setSelectedReport(report);
        setOpenPanel(true);
    };

    const handleClosePanel = () => {
        setOpenPanel(false);
        setSelectedReport(null);
    };

    const handleApprove = async (reportId: string) => {
        if (window.confirm('Approve this report? This will mark the report as approved and may take action on the content.')) {
            try {
                setActionLoading(true);
                await dispatch(updateReportStatus({
                    id: reportId,
                    command: { status: ReportStatus.Approved }
                })).unwrap();
                // Refresh the list
                dispatch(fetchPendingReports({ 
                    pageNumber: pagination.pageNumber, 
                    pageSize: pagination.pageSize
                }));
            } catch (error) {
                console.error('Failed to approve report:', error);
            } finally {
                setActionLoading(false);
            }
        }
    };

    const handleReject = async (reportId: string) => {
        if (window.confirm('Reject this report? This will mark the report as rejected.')) {
            try {
                setActionLoading(true);
                await dispatch(updateReportStatus({
                    id: reportId,
                    command: { status: ReportStatus.Rejected }
                })).unwrap();
                dispatch(fetchPendingReports({ 
                    pageNumber: pagination.pageNumber, 
                    pageSize: pagination.pageSize
                }));
            } catch (error) {
                console.error('Failed to reject report:', error);
            } finally {
                setActionLoading(false);
            }
        }
    };

    const handleResolve = async (reportId: string) => {
        if (window.confirm('Mark this report as resolved?')) {
            try {
                setActionLoading(true);
                await dispatch(updateReportStatus({
                    id: reportId,
                    command: { status: ReportStatus.Resolved }
                })).unwrap();
                dispatch(fetchPendingReports({ 
                    pageNumber: pagination.pageNumber, 
                    pageSize: pagination.pageSize
                }));
            } catch (error) {
                console.error('Failed to resolve report:', error);
            } finally {
                setActionLoading(false);
            }
        }
    };

    const handleDelete = async (reportId: string) => {
        if (window.confirm('Are you sure you want to delete this report?')) {
            try {
                setActionLoading(true);
                await dispatch(deleteReport(reportId)).unwrap();
                // No need to refetch, slice handles it
            } catch (error) {
                console.error('Failed to delete report:', error);
            } finally {
                setActionLoading(false);
            }
        }
    };

    const handleViewPost = (postId: string) => {
        // Assuming you have a route to view posts
        navigate(`/posts/${postId}`);
    };

    const getStatusChip = (status: ReportStatus) => {
        switch (status) {
            case ReportStatus.Pending:
                return <Chip label="Pending" color="warning" size="small" />;
            case ReportStatus.Approved:
                return <Chip label="Approved" color="success" size="small" />;
            case ReportStatus.Rejected:
                return <Chip label="Rejected" color="error" size="small" />;
            case ReportStatus.Resolved:
                return <Chip label="Resolved" color="info" size="small" />;
            default:
                return <Chip label="Unknown" size="small" />;
        }
    };

    const getStatusActions = (report: any) => {
        switch (report.status) {
            case ReportStatus.Pending:
                return (
                    <Stack direction="row" spacing={1}>
                        <IconButton 
                            onClick={() => handleApprove(report.id)} 
                            size="small" 
                            color="success"
                            disabled={actionLoading}
                        >
                            <CheckCircleIcon fontSize="small" />
                        </IconButton>
                        <IconButton 
                            onClick={() => handleReject(report.id)} 
                            size="small" 
                            color="error"
                            disabled={actionLoading}
                        >
                            <CancelIcon fontSize="small" />
                        </IconButton>
                    </Stack>
                );
            case ReportStatus.Approved:
                return (
                    <IconButton 
                        onClick={() => handleResolve(report.id)} 
                        size="small" 
                        color="primary"
                        disabled={actionLoading}
                    >
                        <CheckCircleIcon fontSize="small" />
                    </IconButton>
                );
            default:
                return null;
        }
    };

    const columns: GridColDef[] = [
        { 
            field: 'reason', 
            headerName: 'Reason', 
            flex: 1, 
            minWidth: 200,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Typography variant="body2">
                    {params.value}
                </Typography>
            )
        },
        { 
            field: 'status', 
            headerName: 'Status', 
            width: 120,
            renderCell: (params: GridRenderCellParams<any>) => (
                getStatusChip(params.value)
            )
        },
        { 
            field: 'postId', 
            headerName: 'Post', 
            width: 150,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Button
                    variant="outlined"
                    size="small"
                    startIcon={<VisibilityIcon />}
                    onClick={() => handleViewPost(params.value)}
                    disabled={!params.value}
                    sx={{ textTransform: 'none' }}
                >
                    View Post
                </Button>
            )
        },
        { 
            field: 'createdAt', 
            headerName: 'Reported At', 
            width: 180,
            renderCell: (params) => formatDateTime(params.value)
        },
        {
            field: 'actions',
            headerName: 'Actions',
            width: 200,
            sortable: false,
            renderCell: (params: GridRenderCellParams<any>) => (
                <Stack direction="row" spacing={1}>
                    {getStatusActions(params.row)}
                    <IconButton 
                        onClick={() => handleOpenPanel(params.row)} 
                        size="small" 
                        color="primary"
                        disabled={actionLoading}
                    >
                        <MoreVertIcon fontSize="small" />
                    </IconButton>
                    <IconButton 
                        onClick={() => handleDelete(params.row.id)} 
                        size="small" 
                        color="error"
                        disabled={actionLoading}
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
                title="Reported Posts"
                action={
                    <Stack direction="row" spacing={2} alignItems="center">
                        <FormControl size="small" sx={{ minWidth: 150 }}>
                            <InputLabel>Status Filter</InputLabel>
                            <Select
                                value={filters.status?.toString() || 'all'}
                                label="Status Filter"
                                onChange={handleStatusFilterChange}
                                disabled={loading}
                            >
                                <MenuItem value="all">All Reports</MenuItem>
                                <MenuItem value={ReportStatus.Pending.toString()}>Pending</MenuItem>
                                <MenuItem value={ReportStatus.Approved.toString()}>Approved</MenuItem>
                                <MenuItem value={ReportStatus.Rejected.toString()}>Rejected</MenuItem>
                                <MenuItem value={ReportStatus.Resolved.toString()}>Resolved</MenuItem>
                            </Select>
                        </FormControl>
                        <Button
                            variant="outlined"
                            onClick={() => dispatch(fetchPendingReports({ 
                                pageNumber: pagination.pageNumber, 
                                pageSize: pagination.pageSize
                            }))}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            Refresh
                        </Button>
                    </Stack>
                }
            />

            {/* Summary Stats */}
            <Paper
                elevation={0}
                sx={{
                    mb: 3,
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
                    Showing {reports.length} of {pagination.totalCount} reports
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Page {pagination.pageNumber} of {pagination.totalPages}
                </Typography>
            </Paper>

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
                    rows={reports}
                    columns={columns}
                    loading={loading || actionLoading}
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
                        '& .MuiDataGrid-footerContainer': {
                            borderTop: '1px solid',
                            borderColor: 'divider',
                        },
                    }}
                />
            </Paper>

            {/* Empty State */}
            {reports.length === 0 && !loading && (
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
                        No reports found
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {filters.status === undefined 
                            ? 'There are no reports in the system.'
                            : `There are no reports with status "${getStatusChip(filters.status).props.label}".`}
                    </Typography>
                </Paper>
            )}

            {/* Report Details Side Panel */}
            <SidePanel
                open={openPanel}
                onClose={handleClosePanel}
                title="Report Details"
                actions={
                    <>
                        <Button onClick={handleClosePanel} sx={{ textTransform: 'none' }}>
                            Close
                        </Button>
                        {selectedReport?.status === ReportStatus.Pending && (
                            <>
                                <Button 
                                    onClick={() => handleApprove(selectedReport.id)}
                                    color="success"
                                    variant="contained"
                                    sx={{ textTransform: 'none' }}
                                    disabled={actionLoading}
                                >
                                    Approve
                                </Button>
                                <Button 
                                    onClick={() => handleReject(selectedReport.id)}
                                    color="error"
                                    variant="contained"
                                    sx={{ textTransform: 'none' }}
                                    disabled={actionLoading}
                                >
                                    Reject
                                </Button>
                            </>
                        )}
                        {selectedReport?.status === ReportStatus.Approved && (
                            <Button 
                                onClick={() => handleResolve(selectedReport.id)}
                                variant="contained"
                                sx={{ textTransform: 'none' }}
                                disabled={actionLoading}
                            >
                                Mark as Resolved
                            </Button>
                        )}
                    </>
                }
            >
                {selectedReport && (
                    <Stack spacing={3}>
                        {/* Report Info */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Report ID
                            </Typography>
                            <Typography variant="body2" sx={{ fontFamily: 'monospace' }}>
                                {selectedReport.id}
                            </Typography>
                        </Box>

                        {/* Reason */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Reason for Report
                            </Typography>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 2,
                                    bgcolor: 'background.default',
                                    border: '1px solid',
                                    borderColor: 'divider',
                                    borderRadius: 1,
                                }}
                            >
                                <Typography variant="body1">
                                    {selectedReport.reason}
                                </Typography>
                            </Paper>
                        </Box>

                        {/* Status */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Status
                            </Typography>
                            <Box>
                                {getStatusChip(selectedReport.status)}
                            </Box>
                        </Box>

                        {/* Post Info */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Reported Content
                            </Typography>
                            <Stack direction="row" spacing={2} alignItems="center">
                                <Typography variant="body2">
                                    Post ID: {selectedReport.postId}
                                </Typography>
                                <Button
                                    variant="outlined"
                                    size="small"
                                    onClick={() => handleViewPost(selectedReport.postId)}
                                    disabled={!selectedReport.postId}
                                    sx={{ textTransform: 'none' }}
                                >
                                    View Post
                                </Button>
                            </Stack>
                        </Box>

                        {/* Comment Info */}
                        {selectedReport.commentId && (
                            <Box>
                                <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                    Comment
                                </Typography>
                                <Typography variant="body2">
                                    Comment ID: {selectedReport.commentId}
                                </Typography>
                            </Box>
                        )}

                        {/* Reporter Info */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Reporter
                            </Typography>
                            <Typography variant="body2">
                                User ID: {selectedReport.reporterId}
                            </Typography>
                        </Box>

                        {/* Dates */}
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                                Created At
                            </Typography>
                            <Typography variant="body2">
                                {formatDateTime(selectedReport.createdAt)}
                            </Typography>
                        </Box>
                    </Stack>
                )}
            </SidePanel>
        </Box>
    );
}