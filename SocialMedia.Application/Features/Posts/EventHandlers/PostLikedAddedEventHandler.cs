namespace SocialMedia.Application;

public class PostLikedAddedEventHandler :
    IEventHandler<PostLikeAddedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly IPostRankService _postRankService;

    public PostLikedAddedEventHandler(
        IPostReadRepository readRepository, IPostRankService postRankService)
    {
        _readRepository = readRepository;
        _postRankService = postRankService;
    }

    public async Task Handle(PostLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Like == null)
        {
            throw new ArgumentNullException(nameof(notification.Like), "Like in LikeAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        if (notification.Like.PostId.HasValue)
        {
            var post = await _readRepository.GetByIdAsync(notification.Like.PostId.Value, cancellationToken);
            if (post != null)
            {
                var reaction = post.Reactions.FirstOrDefault(r => r.Emoji == notification.Like.Emoji || r.Emoji == notification.OldEmoji);
                if (reaction != null)
                {
                    if (notification.ToggleLikeType == ToggleLikeType.Removed)
                    {
                        reaction.Count--;
                        post.ReactionCount--;
                    }
                    else if (notification.ToggleLikeType == ToggleLikeType.Updated)
                    {
                        var oldReaction = post.Reactions.FirstOrDefault(r => r.Emoji == notification.OldEmoji);
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
                        post.ReactionCount++;
                    }
                }
                else
                {
                    post.Reactions.Add(new ReactionReadDto
                    {
                        Emoji = notification.Like.Emoji,
                        Count = 1
                    });
                    post.ReactionCount++;
                }
                post.Reactions = post.Reactions.Where(r => r.Count > 0).ToList();
                await _readRepository.UpdateAsync(post, cancellationToken);
                await _postRankService.UpdatePostRankAsync(post.Id, cancellationToken);
            }
        }
    }
}