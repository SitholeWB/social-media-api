using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;

namespace SocialMediaApi.Domain.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActivePostService _activePostService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly IPostReactionService _postReactionService;
        private readonly ICommentReactionService _commentReactionService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly IUserPostService _userPostService;
        private readonly IUserGroupService _userGroupService;

        public PostUnitOfWork(IActivePostService activePostService, IPostService groupPostService, ICommentService commentService, IEntityDetailsService entityDetailsService, IPostReactionService postReactionService, ICommentReactionService commentReactionService, IUserDetailsService userDetailsService, IUserPostService userPostService, IUserGroupService userGroupService)
        {
            _activePostService = activePostService;
            _postService = groupPostService;
            _commentService = commentService;
            _entityDetailsService = entityDetailsService;
            _postReactionService = postReactionService;
            _commentReactionService = commentReactionService;
            _userDetailsService = userDetailsService;
            _userPostService = userPostService;
            _userGroupService = userGroupService;
        }

        public IActivePostService ActivePostService => _activePostService;

        public IPostService PostService => _postService;

        public ICommentService CommentService => _commentService;

        public IEntityDetailsService EntityDetailsService => _entityDetailsService;

        public IPostReactionService PostReactionService => _postReactionService;

        public ICommentReactionService CommentReactionService => _commentReactionService;

        public IUserDetailsService UserDetailsService => _userDetailsService;

        public IUserPostService UserPostService => _userPostService;

        public IUserGroupService UserGroupService => _userGroupService;
    }
}