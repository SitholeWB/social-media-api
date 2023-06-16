using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActiveGroupPostService _activeGroupPostService;
        private readonly IGroupPostService _groupPostService;
        private readonly IGroupPostCommentService _groupPostCommentService;
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly IPostReactionService _postReactionService;

        public PostUnitOfWork(IActiveGroupPostService activeGroupPostService, IGroupPostService groupPostService, IGroupPostCommentService groupPostCommentService, IEntityDetailsService entityDetailsService, IPostReactionService postReactionService)
        {
            _activeGroupPostService = activeGroupPostService;
            _groupPostService = groupPostService;
            _groupPostCommentService = groupPostCommentService;
            _entityDetailsService = entityDetailsService;
            _postReactionService = postReactionService;
        }

        public IActiveGroupPostService ActiveGroupPostService => _activeGroupPostService;

        public IGroupPostService GroupPostService => _groupPostService;

        public IGroupPostCommentService GroupPostCommentService => _groupPostCommentService;

        public IEntityDetailsService IEntityDetailsService => _entityDetailsService;

        public IPostReactionService IPostReactionService => _postReactionService;
    }
}