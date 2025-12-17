// pages/ViewPollPage.tsx
import * as React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import LinearProgress from '@mui/material/LinearProgress';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import RadioButtonUncheckedIcon from '@mui/icons-material/RadioButtonUnchecked';
import PageHeader from '../components/PageHeader';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchPoll, voteOnPoll, clearCurrentPoll } from '../store/slices/pollsSlice';

export default function ViewPollPage() {
    const { pollId } = useParams<{ pollId: string }>();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const { currentPoll, loading, error } = useAppSelector((state) => state.polls);

    const [selectedOptionId, setSelectedOptionId] = React.useState<string | null>(null);
    const [hasVoted, setHasVoted] = React.useState(false);
    const [voteLoading, setVoteLoading] = React.useState(false);

    React.useEffect(() => {
        if (pollId) {
            dispatch(fetchPoll(pollId));
            
            // Check if user has already voted
            const votedPolls = JSON.parse(localStorage.getItem('votedPolls') || '{}');
            if (votedPolls[pollId]) {
                setHasVoted(true);
                setSelectedOptionId(votedPolls[pollId]);
            }
        }

        // Clean up when component unmounts
        return () => {
            dispatch(clearCurrentPoll());
        };
    }, [dispatch, pollId]);

    // Sort options by voteCount (descending)
    const sortedOptions = React.useMemo(() => {
        if (!currentPoll?.options) return [];
        return [...currentPoll.options].sort((a, b) => b.voteCount - a.voteCount);
    }, [currentPoll]);

    const totalVotes = React.useMemo(() => {
        if (!currentPoll?.options) return 0;
        return currentPoll.options.reduce((sum, option) => sum + option.voteCount, 0);
    }, [currentPoll]);

    const handleOptionSelect = (optionId: string) => {
        if (!hasVoted && currentPoll?.isActive) {
            setSelectedOptionId(optionId);
        }
    };

    const handleVote = async () => {
        if (!selectedOptionId || !pollId || hasVoted || !currentPoll?.isActive) {
            return;
        }

        try {
            setVoteLoading(true);
            
            // Call the vote API
            await dispatch(voteOnPoll({ 
                pollId, 
                optionId: selectedOptionId 
            })).unwrap();
            
            // Mark as voted in localStorage
            const votedPolls = JSON.parse(localStorage.getItem('votedPolls') || '{}');
            votedPolls[pollId] = selectedOptionId;
            localStorage.setItem('votedPolls', JSON.stringify(votedPolls));
            
            setHasVoted(true);
            
            // Refresh poll data to get updated vote counts
            dispatch(fetchPoll(pollId));
        } catch (error) {
            console.error('Failed to vote:', error);
        } finally {
            setVoteLoading(false);
        }
    };

    const handleBack = () => {
        navigate('/polls');
    };

    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return date.toLocaleString('en-US', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
        });
    };

    const getPercentage = (voteCount: number) => {
        if (totalVotes === 0) return 0;
        return Math.round((voteCount / totalVotes) * 100);
    };

    if (loading && !currentPoll) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto', mt: 4 }}>
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
                <Button
                    startIcon={<ArrowBackIcon />}
                    onClick={handleBack}
                    sx={{ textTransform: 'none' }}
                >
                    Back to Polls
                </Button>
            </Box>
        );
    }

    if (!pollId || !currentPoll) {
        return (
            <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto', mt: 4 }}>
                <Alert severity="warning">
                    Poll not found
                </Alert>
                <Button
                    startIcon={<ArrowBackIcon />}
                    onClick={handleBack}
                    sx={{ mt: 2, textTransform: 'none' }}
                >
                    Back to Polls
                </Button>
            </Box>
        );
    }

    const isExpired = currentPoll.expiresAt && new Date(currentPoll.expiresAt) < new Date();
    const canVote = !isExpired && currentPoll.isActive && !hasVoted;

    return (
        <Box sx={{ width: '100%', maxWidth: 800, mx: 'auto' }}>
            <PageHeader
                title="Poll Details"
                action={
                    <Button
                        startIcon={<ArrowBackIcon />}
                        onClick={handleBack}
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
                    mb: 3,
                }}
            >
                <Stack spacing={3}>
                    {/* Poll Question */}
                    <Typography variant="h4" component="h1">
                        {currentPoll.question}
                    </Typography>

                    {/* Poll Status */}
                    <Stack direction="row" spacing={2} alignItems="center" flexWrap="wrap">
                        <Typography 
                            variant="body2" 
                            sx={{ 
                                px: 2, 
                                py: 0.5, 
                                borderRadius: 1,
                                backgroundColor: currentPoll.isActive ? 'success.light' : 'error.light',
                                color: currentPoll.isActive ? 'success.contrastText' : 'error.contrastText',
                            }}
                        >
                            {currentPoll.isActive ? 'Active' : 'Inactive'}
                        </Typography>
                        
                        {currentPoll.expiresAt && (
                            <Typography variant="body2" color="text.secondary">
                                {isExpired ? 'Expired on ' : 'Expires on '}
                                {formatDate(currentPoll.expiresAt)}
                            </Typography>
                        )}
                        
                        <Typography variant="body2" color="text.secondary">
                            Poll ID: {currentPoll.id.substring(0, 8)}...
                        </Typography>
                    </Stack>

                    {/* Total Votes */}
                    <Typography variant="h6" color="primary">
                        Total Votes: {totalVotes}
                    </Typography>

                    {/* Options List */}
                    <Stack spacing={2}>
                        {sortedOptions.map((option, index) => {
                            const isSelected = selectedOptionId === option.id;
                            const percentage = getPercentage(option.voteCount);
                            const isWinning = index === 0 && sortedOptions.length > 1 && totalVotes > 0;
                            
                            return (
                                <Card
                                    key={option.id}
                                    variant="outlined"
                                    sx={{
                                        borderColor: isSelected ? 'primary.main' : 'divider',
                                        borderWidth: isSelected ? 2 : 1,
                                        cursor: canVote ? 'pointer' : 'default',
                                        backgroundColor: isSelected ? 'primary.lighter' : 'background.paper',
                                        transition: 'all 0.2s',
                                        '&:hover': canVote ? {
                                            borderColor: 'primary.main',
                                            backgroundColor: 'action.hover',
                                        } : {},
                                        position: 'relative',
                                    }}
                                    onClick={() => handleOptionSelect(option.id)}
                                >
                                    {isWinning && (
                                        <Box
                                            sx={{
                                                position: 'absolute',
                                                top: 8,
                                                right: 8,
                                                display: 'flex',
                                                alignItems: 'center',
                                                gap: 0.5,
                                                color: 'success.main',
                                            }}
                                        >
                                            <CheckCircleIcon fontSize="small" />
                                            <Typography variant="caption" fontWeight="bold">
                                                Leading
                                            </Typography>
                                        </Box>
                                    )}
                                    
                                    <CardContent>
                                        <Stack spacing={1.5}>
                                            <Stack 
                                                direction="row" 
                                                spacing={1.5} 
                                                alignItems="center"
                                                justifyContent="space-between"
                                            >
                                                <Stack direction="row" spacing={1.5} alignItems="center">
                                                    {canVote ? (
                                                        isSelected ? (
                                                            <RadioButtonUncheckedIcon 
                                                                color="primary" 
                                                                sx={{ 
                                                                    border: '2px solid',
                                                                    borderColor: 'primary.main',
                                                                    borderRadius: '50%',
                                                                    p: 0.25 
                                                                }}
                                                            />
                                                        ) : (
                                                            <RadioButtonUncheckedIcon color="action" />
                                                        )
                                                    ) : (
                                                        <Typography 
                                                            variant="h5" 
                                                            color="primary"
                                                            fontWeight="bold"
                                                        >
                                                            #{index + 1}
                                                        </Typography>
                                                    )}
                                                    
                                                    <Typography variant="h6">
                                                        {option.text}
                                                    </Typography>
                                                </Stack>
                                                
                                                <Stack alignItems="flex-end">
                                                    <Typography variant="h6" color="primary">
                                                        {option.voteCount} {option.voteCount === 1 ? 'vote' : 'votes'}
                                                    </Typography>
                                                    <Typography variant="body2" color="text.secondary">
                                                        {percentage}%
                                                    </Typography>
                                                </Stack>
                                            </Stack>
                                            
                                            {/* Progress Bar */}
                                            <Box sx={{ width: '100%' }}>
                                                <LinearProgress 
                                                    variant="determinate" 
                                                    value={percentage}
                                                    sx={{ 
                                                        height: 8,
                                                        borderRadius: 4,
                                                        backgroundColor: 'action.hover',
                                                        '& .MuiLinearProgress-bar': {
                                                            borderRadius: 4,
                                                        }
                                                    }}
                                                />
                                            </Box>
                                            
                                            {/* Option ID (for debugging) */}
                                            <Typography variant="caption" color="text.disabled">
                                                Option ID: {option.id.substring(0, 8)}...
                                            </Typography>
                                        </Stack>
                                    </CardContent>
                                </Card>
                            );
                        })}
                    </Stack>

                    {/* Vote Button */}
                    {canVote && (
                        <Box sx={{ display: 'flex', justifyContent: 'center', pt: 2 }}>
                            <Button
                                variant="contained"
                                size="large"
                                onClick={handleVote}
                                disabled={!selectedOptionId || voteLoading}
                                sx={{ 
                                    textTransform: 'none',
                                    px: 4,
                                    py: 1.5,
                                    fontSize: '1.1rem',
                                }}
                            >
                                {voteLoading ? 'Submitting Vote...' : 'Submit Vote'}
                            </Button>
                        </Box>
                    )}

                    {/* Already Voted Message */}
                    {hasVoted && (
                        <Paper
                            elevation={0}
                            sx={{
                                p: 2,
                                backgroundColor: 'success.lighter',
                                border: '1px solid',
                                borderColor: 'success.light',
                                borderRadius: 1,
                                textAlign: 'center',
                            }}
                        >
                            <Typography color="success.dark">
                                ✓ You have already voted in this poll
                            </Typography>
                            {selectedOptionId && (
                                <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                                    You voted for: {sortedOptions.find(o => o.id === selectedOptionId)?.text}
                                </Typography>
                            )}
                        </Paper>
                    )}

                    {/* Poll Expired Message */}
                    {isExpired && (
                        <Paper
                            elevation={0}
                            sx={{
                                p: 2,
                                backgroundColor: 'error.lighter',
                                border: '1px solid',
                                borderColor: 'error.light',
                                borderRadius: 1,
                                textAlign: 'center',
                            }}
                        >
                            <Typography color="error.dark">
                                ✗ This poll has expired
                            </Typography>
                        </Paper>
                    )}

                    {/* Poll Inactive Message */}
                    {!currentPoll.isActive && !isExpired && (
                        <Paper
                            elevation={0}
                            sx={{
                                p: 2,
                                backgroundColor: 'warning.lighter',
                                border: '1px solid',
                                borderColor: 'warning.light',
                                borderRadius: 1,
                                textAlign: 'center',
                            }}
                        >
                            <Typography color="warning.dark">
                                ⚠ This poll is currently inactive
                            </Typography>
                        </Paper>
                    )}

                    {/* Refresh Button */}
                    <Box sx={{ display: 'flex', justifyContent: 'center' }}>
                        <Button
                            variant="outlined"
                            onClick={() => dispatch(fetchPoll(pollId))}
                            disabled={loading}
                            sx={{ textTransform: 'none' }}
                        >
                            {loading ? 'Refreshing...' : 'Refresh Results'}
                        </Button>
                    </Box>
                </Stack>
            </Paper>
        </Box>
    );
}