using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.UnitOfWork
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly IActiveGroupPostService _activeGroupPostService;
        private readonly IGroupPostService _groupPostService;

        public PostUnitOfWork(IActiveGroupPostService activeGroupPostService, IGroupPostService groupPostService)
        {
            _activeGroupPostService = activeGroupPostService;
            _groupPostService = groupPostService;
        }

        public IActiveGroupPostService ActiveGroupPostService => _activeGroupPostService;

        public IGroupPostService GroupPostService => _groupPostService;
    }
}