using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActivePostService _activePostService;
        private readonly IPostService _groupPostService;
        private readonly IPostCommentService _postCommentService;
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly IPostReactionService _postReactionService;
        private readonly IPostCommentReactionService _postCommentReactionService;

        public PostUnitOfWork(IActivePostService activePostService, IPostService groupPostService, IPostCommentService postCommentService, IEntityDetailsService entityDetailsService, IPostReactionService postReactionService, IPostCommentReactionService postCommentReactionService)
        {
            _activePostService = activePostService;
            _groupPostService = groupPostService;
            _postCommentService = postCommentService;
            _entityDetailsService = entityDetailsService;
            _postReactionService = postReactionService;
            _postCommentReactionService = postCommentReactionService;
        }

        public IActivePostService ActivePostService => _activePostService;

        public IPostService PostService => _groupPostService;

        public IPostCommentService PostCommentService => _postCommentService;

        public IEntityDetailsService EntityDetailsService => _entityDetailsService;

        public IPostReactionService PostReactionService => _postReactionService;

        public IPostCommentReactionService PostCommentReactionService => _postCommentReactionService;
    }
}