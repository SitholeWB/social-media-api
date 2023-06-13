using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActiveGroupPostService _activeGroupPostService;
        private readonly IGroupPostService _groupPostService;
        private readonly IGroupPostCommentService _groupPostCommentService;

        public PostUnitOfWork(IActiveGroupPostService activeGroupPostService, IGroupPostService groupPostService, IGroupPostCommentService groupPostCommentService)
        {
            _activeGroupPostService = activeGroupPostService;
            _groupPostService = groupPostService;
            _groupPostCommentService = groupPostCommentService;
        }

        public IActiveGroupPostService ActiveGroupPostService => _activeGroupPostService;

        public IGroupPostService GroupPostService => _groupPostService;

        public IGroupPostCommentService GroupPostCommentService => _groupPostCommentService;
    }
}