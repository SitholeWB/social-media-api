namespace SocialMedia.Application;

public class CommentLikeAddedEventHandler :
    IEventHandler<CommentLikeAddedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;
    private readonly IPostRankService _postRankService;

    public CommentLikeAddedEventHandler(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository,
        IPostRankService postRankService)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
        _postRankService = postRankService;
    }

    public async Task HandleAsync(CommentLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Like == null)
        {
            throw new ArgumentNullException(nameof(notification.Like), "Like in LikeAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        if (notification.Like.CommentId.HasValue)
        {
            var comment = await _commentReadRepository.GetByIdAsync(notification.Like.CommentId.Value, cancellationToken);
            if (comment != null)
            {
                var reaction = comment.Reactions.FirstOrDefault(r => r.Emoji == notification.Like.Emoji || r.Emoji == notification.OldEmoji);
                if (reaction != null)
                {
                    if (notification.ToggleLikeType == ToggleLikeType.Removed)
                    {
                        reaction.Count--;
                        comment.Stats.LikeCount--;
                    }
                    else if (notification.ToggleLikeType == ToggleLikeType.Updated)
                    {
                        var oldReaction = comment.Reactions.FirstOrDefault(r => r.Emoji == notification.OldEmoji);
                        if (oldReaction != null)
                        {
                            oldReaction.Count--;
                        }
                        reaction.Count++;
                        reaction.Emoji = notification.Like.Emoji;
                    }
                    else if (notification.ToggleLikeType == ToggleLikeType.Added)
                    {
                        reaction.Count++;
                        comment.Stats.LikeCount++;
                    }
                }
                else
                {
                    comment.Reactions.Add(new ReactionReadDto
                    {
                        Emoji = notification.Like.Emoji,
                        Count = 1
                    });
                    comment.Stats.LikeCount++;
                }
                comment.Reactions = comment.Reactions.Where(r => r.Count > 0).ToList();
                await _commentReadRepository.UpdateAsync(comment, cancellationToken);

                // Update TopComments in PostReadModel if present
                var post = await _readRepository.GetByIdAsync(comment.PostId, cancellationToken);
                if (post != null)
                {
                    var topComment = post.TopComments?.FirstOrDefault(c => c.CommentId == comment.Id);
                    if (topComment != null)
                    {
                        topComment.LikeCount = comment.Stats.LikeCount;
                        topComment.Reactions = comment.Reactions;
                        await _readRepository.UpdateAsync(post, cancellationToken);
                        await _postRankService.UpdatePostRankAsync(post.Id, cancellationToken);
                    }
                }
            }
        }
    }
}