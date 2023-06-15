using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActiveGroupPostService _activeGroupPostService;
        private readonly IGroupPostService _groupPostService;
        private readonly IGroupPostCommentService _groupPostCommentService;
        private readonly IUserReactionService _reactionService;

        public PostUnitOfWork(IActiveGroupPostService activeGroupPostService, IGroupPostService groupPostService, IGroupPostCommentService groupPostCommentService, IUserReactionService reactionService)
        {
            _activeGroupPostService = activeGroupPostService;
            _groupPostService = groupPostService;
            _groupPostCommentService = groupPostCommentService;
            _reactionService = reactionService;
        }

        public IActiveGroupPostService ActiveGroupPostService => _activeGroupPostService;

        public IGroupPostService GroupPostService => _groupPostService;

        public IGroupPostCommentService GroupPostCommentService => _groupPostCommentService;

        public IUserReactionService IReactionService => _reactionService;
    }
}